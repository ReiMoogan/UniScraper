using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FastMember;
using FetchUCM;
using FetchUCM.Models;

namespace StandaloneTester;

public static class UpdateUCMClasses
{
    public static async Task UpdateClasses(SqlConnection connection)
    {
        // Drop if a previous session crashed mid-way.
        await DropTemporaryTables(connection);
        
        var catalog = new UCMCatalog();
        var terms = await catalog.GetAllTerms();
        foreach (var term in terms)
        {
            // Skip read/view-only tables, no need to update static tables.
            if (term.Description.ToLower().Contains("only")) continue;
            
            Console.WriteLine("Creating temporary databases...");
            await CreateTemporaryTables(connection);
            Console.WriteLine($"Reading from {term.Description}");
            await UpdateTerm(connection, catalog, term.Code);
            Console.WriteLine("Dropping temporary databases...");
            await DropTemporaryTables(connection);
        }
    }

    private static async Task UpdateTerm(SqlConnection connection, UCMCatalog catalog, int term)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
            
        var everything = (await catalog.GetAllClasses(term)).ToList();

        Console.WriteLine($"Fetched data in {stopwatch.Elapsed.Seconds}s...");
        stopwatch.Restart();

        var classes = everything.Select(o => (IDBClass)o);
        var professors = everything.SelectMany(o => o.Faculty).Select(o => (IDBProfessor)o).GroupBy(o => o.Id)
            .Select(o => o.First());
        var faculty = everything.SelectMany(o =>
            o.Faculty.Select(p => new Faculty { ProfessorEmail = p.Email, ClassId = o.Id }));
        var meetings = everything
            .SelectMany(o => o.MeetingsFaculty.Select(p => new DBMeetingTime(o.CourseReferenceNumber, p.Time)))
            .GroupBy(o => new { o.ClassId, o.MeetingType }).Select(o => o.First());
        var classTable = new DataTable();
        var professorTable = new DataTable();
        var facultyTable = new DataTable();
        var meetingTable = new DataTable();

        await using (var reader = ObjectReader.Create(classes))
        {
            classTable.Load(reader);
        }

        await using (var reader = ObjectReader.Create(professors))
        {
            professorTable.Load(reader);
        }

        await using (var reader = ObjectReader.Create(faculty))
        {
            facultyTable.Load(reader);
        }

        await using (var reader = ObjectReader.Create(meetings))
        {
            meetingTable.Load(reader);
        }

        Console.WriteLine($"Prepared data in {stopwatch.Elapsed.Seconds}s...");
        stopwatch.Restart();

        using (var copier = new SqlBulkCopy(connection))
        {
            copier.DestinationTableName = "#class";
            Utilities.MapSql(classTable, copier);
            await copier.WriteToServerAsync(classTable);
            copier.DestinationTableName = "#professor";
            Utilities.MapSql(professorTable, copier);
            await copier.WriteToServerAsync(professorTable);
            copier.DestinationTableName = "#faculty";
            Utilities.MapSql(facultyTable, copier);
            await copier.WriteToServerAsync(facultyTable);
            copier.DestinationTableName = "#meeting";
            Utilities.MapSql(meetingTable, copier);
            await copier.WriteToServerAsync(meetingTable);
        }

        Console.WriteLine($"Copied data in {stopwatch.Elapsed.Seconds}s...");
        stopwatch.Restart();

        try
        {
            await connection.ExecuteAsync(
                "EXEC [UniScraper].[UCM].[MergeUpload];");
            Console.WriteLine($"Merged data in {stopwatch.Elapsed.Seconds}s...");
            stopwatch.Restart();

            await connection.ExecuteAsync(
                "UPDATE UniScraper.UCM.stats SET last_update = SYSDATETIME() WHERE table_name = 'class';");
            Console.WriteLine(
                $"For {term}, updated {classTable.Rows.Count} classes, {professorTable.Rows.Count} professors, and {meetingTable.Rows.Count} meetings!");
        }
        catch (SqlException ex)
        {
            Console.WriteLine("Failed to merge data! Might be temporal?");
            Console.WriteLine(ex);
        }
    }
    
    private static async Task CreateTemporaryTables(IDbConnection connection)
    {
        await connection.ExecuteAsync(
            @"SELECT TOP 0 * INTO #class FROM [UniScraper].[UCM].[class];
                CREATE TABLE #faculty
                (
	                class_id int NOT NULL,
	                professor_email varchar(256) NOT NULL
                );
                SELECT TOP 0 * INTO #meeting FROM [UniScraper].[UCM].[meeting];
                CREATE TABLE #professor
                (
                    id int not null constraint id_temp_pk primary key nonclustered,
                    last_name nvarchar(64) NOT NULL,
                    first_name nvarchar(64) NOT NULL,
                    email varchar(64) NOT NULL,
                    -- For compatibility with the IDBProfessor object
                    num_ratings int DEFAULT 0 NOT NULL,
                    rating real DEFAULT 0.0 NOT NULL
                );
                ");
    }

    private static async Task DropTemporaryTables(IDbConnection connection)
    {
        try
        {
            await connection.ExecuteAsync(
                "DROP TABLE IF EXISTS #class; DROP TABLE IF EXISTS #faculty; DROP TABLE IF EXISTS #meeting; DROP TABLE IF EXISTS #professor;");
        }
        catch (Exception)
        {
            // ignored
        }
    }
}