using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
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

        var updateLinkedCourses = await TimeToRun(connection);
        if (!updateLinkedCourses)
        {
            Console.WriteLine("Skipping linked courses.");
        }
        
        foreach (var term in terms)
        {
            // Skip read/view-only tables, no need to update static tables.
            if (term.Description.ToLower().Contains("only")) continue;
            catalog.ClearCookies();
            Console.WriteLine("Creating temporary databases...");
            await CreateTemporaryTables(connection);
            Console.WriteLine($"Reading from {term.Description}");
            await UpdateTerm(connection, catalog, term.Code, updateLinkedCourses);
            Console.WriteLine("Dropping temporary databases...");
            await DropTemporaryTables(connection);
        }
    }

    private static async Task UpdateTerm(SqlConnection connection, UCMCatalog catalog, int term, bool updateLinkedCourses = false)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
            
        var everything = (await catalog.GetAllClasses(term)).ToList();

        Console.WriteLine($"Fetched data in {stopwatch.Elapsed.Seconds}s...");
        stopwatch.Restart();

        var classes = everything.Select(o => (IDBClass)o);
        var professors = everything.SelectMany(o => o.Faculty).Select(o => (IDBProfessor)o).GroupBy(o => o.Email)
            .Select(o => o.First());
        var faculty = everything.SelectMany(o =>
            o.Faculty.Select(p => new Faculty { ProfessorEmail = p.Email, ClassId = o.Id }));
        var meetings = everything
            .SelectMany(o => o.MeetingsFaculty.Select(p => new DBMeetingTime(o.Id, p.Time)))
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
            
            Console.WriteLine(
                $"For {term}, updated {classTable.Rows.Count} classes, {professorTable.Rows.Count} professors, and {meetingTable.Rows.Count} meetings!");
        }
        catch (SqlException ex)
        {
            Console.WriteLine($"Failed to merge data! Might be temporal? Line {ex.LineNumber}");
            Console.WriteLine(ex);
        }

        if (updateLinkedCourses)
            await UpdateLinkedCourses(connection, catalog, everything);
    }

    private static async Task UpdateLinkedCourses(SqlConnection connection, UCMCatalog catalog, List<Class> everything)
    {
        try
        {
            Console.WriteLine("Updating linked courses... (please watch warmly)");

            await DropTemporaryTables(connection);
            await CreateTemporaryTables(connection);

            var lockObject = new object();
            var linkedTable = new DataTable();
            linkedTable.Columns.Add(new DataColumn("parent", typeof(int)));
            linkedTable.Columns.Add(new DataColumn("child", typeof(int)));

            var numProcessed = 0;
            everything.AsParallel().WithDegreeOfParallelism(16).ForAll(o =>
            {
                var linkedSections = catalog.GetLinkedSections(o.Term, o.CourseReferenceNumber).GetAwaiter().GetResult();
                lock (lockObject) // DataTables are not thread-safe, so we load HTTP results in different threads and store synchronously.
                {
                    foreach (var linked in linkedSections)
                    {
                        var row = linkedTable.NewRow();
                        row["parent"] = o.Id;
                        row["child"] = o.Term * 10000 + linked;
                        linkedTable.Rows.Add(row);
                    }
                }

                ++numProcessed;
                if (numProcessed % 100 == 0)
                    Console.WriteLine($"{numProcessed}/{everything.Count} done...");
            });

            using (var copier = new SqlBulkCopy(connection))
            {
                copier.DestinationTableName = "#linked_section";
                copier.ColumnMappings.Add("parent", "parent");
                copier.ColumnMappings.Add("child", "child");
                await copier.WriteToServerAsync(linkedTable);
            }

            await connection.ExecuteAsync("EXEC [UniScraper].[UCM].[MergeLinkedCourses];");
            Console.WriteLine($"Created {linkedTable.Rows.Count} links/edges!");
        }
        catch (SqlException ex)
        {
            Console.WriteLine("Failed to merge data (again)! Might be temporal?");
            Console.WriteLine(ex);
        }
    }

    private static async Task<bool> TimeToRun(IDbConnection connection)
    {
        var time = await connection.QueryFirstOrDefaultAsync<int>(
            "SELECT DATEDIFF(SECOND, last_update, SYSDATETIME()) FROM [UCM].[stats] WHERE table_name = 'linked_section';");
        return time > 24 * 60 * 60; // Wait every day.
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
                    email varchar(64) NOT NULL CONSTRAINT email_temp_pk PRIMARY KEY CLUSTERED,
                    last_name nvarchar(64) NOT NULL,
                    first_name nvarchar(64) NOT NULL,
                    -- For compatibility with the IDBProfessor object
                    num_ratings int DEFAULT 0 NOT NULL,
                    rating real DEFAULT 0.0 NOT NULL,
                    difficulty real DEFAULT 0.0 NOT NULL
                );
                SELECT TOP 0 * INTO #linked_section FROM [UniScraper].[UCM].[linked_section];
                ");
    }

    private static async Task DropTemporaryTables(IDbConnection connection)
    {
        try
        {
            await connection.ExecuteAsync(
                "DROP TABLE IF EXISTS #class; DROP TABLE IF EXISTS #faculty; DROP TABLE IF EXISTS #meeting; DROP TABLE IF EXISTS #professor; DROP TABLE IF EXISTS #linked_section;");
        }
        catch (Exception)
        {
            // ignored
        }
    }
}