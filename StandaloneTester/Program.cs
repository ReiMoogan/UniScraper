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
            var catalog = new UCMCatalog();
            var term = (await catalog.GetAllTerms())[0].Code;
            Console.WriteLine($"Reading from term {term}");

            var everything = await catalog.GetAllClasses(term).ToListAsync();
            var classes = everything.Select(o => (IDBClass) o);
            var professors = everything.SelectMany(o => o.Faculty).Select(o => (IDBProfessor) o);
            var faculty = everything.SelectMany(o =>
                o.Faculty.Select(p => new Faculty { ProfessorId = p.BannerId, ClassId = o.Id }));
            var meetings = everything.SelectMany(o => o.MeetingsFaculty.Select(p => new DBMeetingTime(o.Id, p.Time)));
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

            /*
            await foreach (var item in catalog.GetAllClasses(term))
            {
                await connection.ExecuteAsync(
                    "MERGE INTO UniScraper.UCM.class WITH (HOLDLOCK) AS Target " +
                    "USING (Values(@Term, @CourseReferenceNumber, @CourseNumber, @CampusDescription, @CourseTitle, @CreditHours, @MaximumEnrollment, @Enrollment, @SeatsAvailable, @WaitCapacity, @WaitAvailable)) AS SOURCE(term, course_reference_number, course_number, campus_description, course_title, credit_hours, maximum_enrollment, enrollment, seats_available, wait_capacity, wait_available) " +
                    "ON Target.id = @Id WHEN MATCHED THEN " +
                    "UPDATE SET term = SOURCE.term, course_reference_number = SOURCE.course_reference_Number, course_number = SOURCE.course_number, campus_description = SOURCE.campus_description, course_title = SOURCE.course_title, credit_hours = SOURCE.credit_hours, maximum_enrollment = SOURCE.maximum_enrollment, enrollment = SOURCE.enrollment, seats_available = SOURCE.seats_available, wait_capacity = SOURCE.wait_capacity, wait_available = SOURCE.wait_available " +
                    "WHEN NOT MATCHED THEN " +
                    "INSERT (id, term, course_reference_number, course_number, campus_description, course_title, credit_hours, maximum_enrollment, enrollment, seats_available, wait_capacity, wait_available) VALUES (@Id, @Term, @CourseReferenceNumber, @CourseNumber, @CampusDescription, @CourseTitle, @CreditHours, @MaximumEnrollment, @Enrollment, @SeatsAvailable, @WaitCapacity, @WaitAvailable);",
                    new { item.Id, item.Term, item.CourseReferenceNumber, item.CourseNumber, item.CampusDescription, item.CourseTitle, item.CreditHours, item.MaximumEnrollment, item.Enrollment, item.SeatsAvailable, item.WaitCapacity, item.WaitAvailable });
                
                foreach (var professor in item.Faculty)
                {
                    var nameSplit = professor.DisplayName.Split(',');
                    var firstName = nameSplit[^1].Trim();
                    var lastName = nameSplit[0].Trim();
                    await connection.ExecuteAsync(
                        "MERGE INTO UniScraper.UCM.professor WITH (HOLDLOCK) AS Target " +
                        "USING (Values(@LastName, @FirstName, @Email)) AS SOURCE(last_name, first_name, email) " +
                        "ON Target.id = @Id WHEN MATCHED THEN " +
                        "UPDATE SET last_name = SOURCE.last_name, first_name = SOURCE.first_name, email = SOURCE.email " +
                        "WHEN NOT MATCHED THEN " +
                        "INSERT (id, last_name, first_name, email) VALUES (@Id, @LastName, @FirstName, @Email);",
                        new { Id = professor.BannerId, FirstName = firstName, LastName = lastName, professor.Email });

                    await connection.ExecuteAsync(
                        "MERGE INTO UniScraper.UCM.faculty WITH (HOLDLOCK) AS Target " +
                        "USING (VALUES(@ClassId, @ProfessorId)) AS SOURCE(class_id, professor_id) " +
                        "ON Target.class_id = @ClassId AND Target.professor_id = @ProfessorId " +
                        "WHEN NOT MATCHED THEN INSERT (class_id, professor_id) VALUES (@ClassId, @ProfessorId);",
                        new { ClassId = item.Id, ProfessorId = professor.BannerId });
                    
                    ++countProfessors;
                }

                foreach (var meeting in item.MeetingsFaculty)
                {
                    var time = meeting.Time;
                    await connection.ExecuteAsync(
                        "MERGE INTO UniScraper.UCM.meeting WITH (HOLDLOCK) AS Target " +
                        "USING (Values(@BeginTime, @EndTime, @BeginDate, @EndDate, @Building, @BuildingDescription, @Campus, @CampusDescription, @Room, @CreditHourSession, @HoursPerWeek, @InSession, @Type)) AS SOURCE(begin_time, end_time, begin_date, end_date, building, building_description, campus, campus_description, room, credit_hour_session, hours_per_week, in_session, meeting_type) " +
                        "ON Target.class_id = @Id AND Target.meeting_type = @Type WHEN MATCHED THEN " +
                        "UPDATE SET begin_time = SOURCE.begin_time, end_time = SOURCE.end_time, begin_date = SOURCE.begin_date, end_date = SOURCE.end_date, building = SOURCE.building, building_description = SOURCE.building_description, campus = SOURCE.campus, campus_description = SOURCE.campus_description, room = SOURCE.room, credit_hour_session = SOURCE.credit_hour_session, hours_per_week = SOURCE.hours_per_week, in_session = SOURCE.in_session, meeting_type = SOURCE.meeting_type " +
                        "WHEN NOT MATCHED THEN " +
                        "INSERT (class_id, begin_time, end_time, begin_date, end_date, building, building_description, campus, campus_description, room, credit_hour_session, hours_per_week, in_session, meeting_type) VALUES (@Id, @BeginTime, @EndTime, @BeginDate, @EndDate, @Building, @BuildingDescription, @Campus, @CampusDescription, @Room, @CreditHourSession, @HoursPerWeek, @InSession, @Type);",
                        new { item.Id, time.BeginTime, time.EndTime, time.BeginDate, time.EndDate, time.Building, 
                            time.BuildingDescription, time.Campus, time.CampusDescription, time.Room, time.CreditHourSession,
                            time.HoursPerWeek, time.InSession, time.Type });
                    ++countMeetings;
                }
                
                ++countClasses;
            }*/

            await connection.ExecuteAsync(
                "UPDATE UniScraper.UCM.stats SET last_update = SYSDATETIME() WHERE table_name = 'class';");
            Console.WriteLine($"Updated {classTable.Rows.Count} classes, {professorTable.Rows.Count} professors, and {meetingTable.Rows.Count} meetings!");
        }
    }
}