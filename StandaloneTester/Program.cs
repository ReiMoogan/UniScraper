using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FastMember;
using FetchRateMyProf;
using FetchUCM;
using FetchUCM.Models;
using ScrapperCore.Utilities;

namespace StandaloneTester
{
    internal static class Program
    {
        private static async Task Main()
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var config = ScrapperConfig.Load().Verify();
            await using var connection = new SqlConnection(config.SqlConnection);
            Console.WriteLine("Opening database connection...");
            await connection.OpenAsync();
            Console.WriteLine("Creating temporary databases...");
            await CreateTemporaryTables(connection);
            Console.WriteLine("Updating classes...");
            var classTask = UpdateClasses(connection);
            Console.WriteLine("Updating professors...");
            var profTask = UpdateProfessors(connection);
            Console.WriteLine("Waiting for both to finish...");
            await Task.WhenAll(classTask, profTask);
            Console.WriteLine("Freeing temporary databases...");
            await DeleteTemporaryTables(connection);
            Console.WriteLine("Closing database connection...");
            await connection.CloseAsync();
            Console.WriteLine($"Finished in {stopwatch.Elapsed.TotalSeconds}s!");
        }

        private static async Task UpdateProfessors(SqlConnection connection)
        {
            const int ucmsid = 4767;
            var uni = RateMyProfessor.GetSchool(ucmsid);

            var professors = await uni.GetAllProfessors().ToListAsync();
            var professorTable = new DataTable();
            
            await using (var reader = ObjectReader.Create(professors, "Id", "FirstName", "LastName", "MiddleName", "Department", "NumRatings", "OverallRating")) {
                professorTable.Load(reader);
            }
            
            using (var copier = new SqlBulkCopy(connection))
            {
                copier.DestinationTableName = "#professor_rmp";
                copier.ColumnMappings.Add("Id", "rmp_id");
                copier.ColumnMappings.Add("FirstName", "first_name");
                copier.ColumnMappings.Add("LastName", "last_name");
                copier.ColumnMappings.Add("MiddleName", "middle_name");
                copier.ColumnMappings.Add("Department", "department");
                copier.ColumnMappings.Add("OverallRating", "rating");
                await copier.WriteToServerAsync(professorTable);
            }
            
            /*
            try
            {
                var result = await connection.ExecuteAsync(
                    "UPDATE UniScraper.UCM.professor SET rmp_id = @RMPID, middle_name = @MiddleName, department = @Department, num_ratings = @NumRatings, rating = @OverallRating WHERE REPLACE(REPLACE(first_name, ' ', ''), '-', '') LIKE REPLACE(REPLACE(@FirstName, ' ', ''), '-', '') AND REPLACE(REPLACE(last_name, ' ', ''), '-', '') LIKE REPLACE(REPLACE(@LastName, ' ', ''), '-', '');",
                    new
                    {
                        RMPID = professor.Id, FirstName = $"%{professor.FirstName}%",
                        LastName = $"%{professor.LastName}%", professor.MiddleName, professor.Department,
                        professor.NumRatings, professor.OverallRating
                    });
                
                if (result == 0)
                    Console.WriteLine($"Could not find {professor.FirstName}, {professor.MiddleName}, {professor.LastName} in the database...");
                else
                    ++count;
            }
            catch (Exception)
            {
                Console.WriteLine($"Failed query for {professor.FirstName} {professor.LastName}");
                throw;
            }*/

            await connection.ExecuteAsync(
                "MERGE INTO UniScraper.UCM.professor WITH (HOLDLOCK) AS Target " +
                "USING (SELECT rmp_id, first_name, middle_name, last_name, department, rating FROM #professor_rmp) AS SOURCE(rmp_id, first_name, middle_name, last_name, department, rating) " +
                "ON Target.rmp_id = SOURCE.rmp_id WHEN MATCHED THEN " +
                "UPDATE SET first_name = SOURCE.first_name, middle_name = SOURCE.middle_name, last_name = SOURCE.last_name, department = SOURCE.department, rating = SOURCE.rating;");

            await connection.ExecuteAsync(
                "UPDATE UniScraper.UCM.stats SET last_update = SYSDATETIME() WHERE table_name = 'professor';");
            Console.WriteLine($"Updated {professors.Count} professors!");
        }

        private static async Task CreateTemporaryTables(IDbConnection connection)
        {
            await connection.ExecuteAsync(
                @"SELECT TOP 0 * INTO #class FROM [UniScraper].[UCM].[class];
                SELECT TOP 0 * INTO #faculty FROM [UniScraper].[UCM].[faculty];
                SELECT TOP 0 * INTO #meeting FROM [UniScraper].[UCM].[meeting];
                SELECT TOP 0 * INTO #professor FROM [UniScraper].[UCM].[professor];
                CREATE TABLE #professor_rmp
                (
                    rmp_id int not null constraint rmp_id_pk primary key nonclustered,
                    last_name nvarchar(64) NOT NULL,
                    first_name nvarchar(64) NOT NULL,
                    middle_name nvarchar(64),
                    department varchar(64),
                    num_ratings int constraint DF_num DEFAULT 0 NOT NULL,
                    rating real constraint DF_rate DEFAULT 0.0 NOT NULL
                );

                CREATE UNIQUE INDEX pp_id ON #professor_rmp (rmp_id);
                ");
        }
        
        private static async Task DeleteTemporaryTables(IDbConnection connection)
        {
            await connection.ExecuteAsync(
                "DROP TABLE #class; DROP TABLE #faculty; DROP TABLE #meeting; DROP TABLE #professor; DROP TABLE #professor_rmp;");
        }

        private static void MapSql(DataTable table, SqlBulkCopy copier)
        {
            copier.ColumnMappings.Clear();
            foreach (var col in table.Columns)
            {
                var name = ((DataColumn) col).ColumnName;
                var snake = string.Concat(name.Select((x, i) =>
                    i > 0 && char.IsUpper(x) && !char.IsUpper(name[i - 1]) ? $"_{x}" : x.ToString())).ToLower();
                copier.ColumnMappings.Add(name, snake);
            }
        }
        
        private static async Task UpdateClasses(SqlConnection connection)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            
            var catalog = new UCMCatalog();
            var term = (await catalog.GetAllTerms())[0].Code;
            Console.WriteLine($"Reading from term {term}");
            var everything = await catalog.GetAllClasses(term);
            
            Console.WriteLine($"Fetched data in {stopwatch.Elapsed.Seconds}s...");
            stopwatch.Restart();
            
            var classes = everything.Select(o => (IDBClass) o);
            var professors = everything.SelectMany(o => o.Faculty).Select(o => (IDBProfessor) o).GroupBy(o => o.Id).Select(o => o.First());
            var faculty = everything.SelectMany(o =>
                o.Faculty.Select(p => new Faculty { ProfessorId = p.BannerId, ClassId = o.Id }));
            var meetings = everything.SelectMany(o => o.MeetingsFaculty.Select(p => new DBMeetingTime(o.Id, p.Time))).GroupBy(o => new {o.ClassId, o.MeetingType}).Select(o => o.First());
            var classTable = new DataTable();
            var professorTable = new DataTable();
            var facultyTable = new DataTable();
            var meetingTable = new DataTable();
            
            await using (var reader = ObjectReader.Create(classes)) {
                classTable.Load(reader);
            }
            await using (var reader = ObjectReader.Create(professors)) {
                professorTable.Load(reader);
            }
            await using (var reader = ObjectReader.Create(faculty)) {
                facultyTable.Load(reader);
            }
            await using (var reader = ObjectReader.Create(meetings)) {
                meetingTable.Load(reader);
            }
            
            Console.WriteLine($"Prepared data in {stopwatch.Elapsed.Seconds}s...");
            stopwatch.Restart();

            using (var copier = new SqlBulkCopy(connection))
            {
                copier.DestinationTableName = "#class";
                MapSql(classTable, copier);
                await copier.WriteToServerAsync(classTable);
                copier.DestinationTableName = "#professor";
                MapSql(professorTable, copier);
                await copier.WriteToServerAsync(professorTable);
                copier.DestinationTableName = "#faculty";
                MapSql(facultyTable, copier);
                await copier.WriteToServerAsync(facultyTable);
                copier.DestinationTableName = "#meeting";
                MapSql(meetingTable, copier);
                await copier.WriteToServerAsync(meetingTable);
            }
            
            Console.WriteLine($"Copied data in {stopwatch.Elapsed.Seconds}s...");
            stopwatch.Restart();
            
            await connection.ExecuteAsync(
                "EXEC [UniScraper].[UCM].[MergeUpload];");

            Console.WriteLine($"Merged data in {stopwatch.Elapsed.Seconds}s...");
            stopwatch.Restart();
            
            await connection.ExecuteAsync(
                "UPDATE UniScraper.UCM.stats SET last_update = SYSDATETIME() WHERE table_name = 'class';");
            Console.WriteLine($"Updated {classTable.Rows.Count} classes, {professorTable.Rows.Count} professors, and {meetingTable.Rows.Count} meetings!");
        }
    }
}