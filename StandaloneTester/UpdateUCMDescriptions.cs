using System;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FastMember;
using FetchUCM;
using FetchUCM.Models;

namespace StandaloneTester;

public static class UpdateUCMDescriptions
{
    // Remember to update the DB if this is changed!
    private const int MaxDescriptionLength = 1024;
    
    public static async Task UpdateDescriptions(SqlConnection connection)
    {
        // Only run every so often, so we don't effectively DoS the school.
        if (!await TimeToRun(connection))
        {
            Console.WriteLine("Skipping attribute fetching.");
            return;
        }
        
        // Drop if a previous session crashed mid-way.
        await DropTemporaryTable(connection);
        
        var catalog = new UCMCatalog();
        var terms = await catalog.GetAllTerms();
        var descriptions = await catalog.GetAllDescriptions(terms.First().Code);
        var reduced = descriptions.Select(o => (IDBDescription) o);
        var descriptionTable = new DataTable();
        
        await using (var reader = ObjectReader.Create(reduced)) {
            descriptionTable.Load(reader);
        }

        await CreateTemporaryTable(connection);
        using (var copier = new SqlBulkCopy(connection))
        {
            copier.DestinationTableName = "#description";
            Utilities.MapSql(descriptionTable, copier);
            await copier.WriteToServerAsync(descriptionTable);
        }

        var crnPairs = await connection.QueryAsync<(string, int, int)>(
            "EXEC [UniScraper].[UCM].[MergeDescription];");

        var fetchedPairs = crnPairs.AsParallel().Select(o =>
        {
            var (courseNumber, courseReferenceNumber, term) = o;
            var description = catalog.GetCourseDescription(term, courseReferenceNumber).GetAwaiter().GetResult();
            return new CourseExtendedAttributes { CourseNumber = courseNumber, CourseDescription = description };
        }).ToList();

        // Re-populate with full values.
        descriptionTable = new DataTable();
        
        await using (var reader = ObjectReader.Create(fetchedPairs)) {
            descriptionTable.Load(reader);
        }

        await PostCreateTemporaryTable(connection);
        using (var copier = new SqlBulkCopy(connection))
        {
            copier.DestinationTableName = "#description";
            Utilities.MapSql(descriptionTable, copier);
            await copier.WriteToServerAsync(descriptionTable);
        }
        
        await connection.ExecuteAsync("EXEC [UniScraper].[UCM].[PostMergeDescription];");
        await DropTemporaryTable(connection);
        Console.WriteLine($"Updated {descriptionTable.Rows.Count} descriptions!");
    }

    private static async Task<bool> TimeToRun(SqlConnection connection)
    {
        var time = await connection.QueryFirstOrDefaultAsync<int>(
            "SELECT DATEDIFF(SECOND, last_update, SYSDATETIME()) FROM [UCM].[stats] WHERE table_name = 'description';");
        return time > 24 * 60 * 60; // Wait every day.
    }
    
    private static async Task CreateTemporaryTable(SqlConnection connection)
    {
        // Drop if a previous session crashed mid-way.
        await DropTemporaryTable(connection);
        await connection.ExecuteAsync(
            @"SELECT TOP 0 * INTO #description FROM [UniScraper].[UCM].[description];");
    }
    
    private static async Task PostCreateTemporaryTable(SqlConnection connection)
    {
        await DropTemporaryTable(connection);
        await connection.ExecuteAsync(
            $@"CREATE TABLE #description
                (
	                course_number varchar(16) NOT NULL,
	                course_description varchar({MaxDescriptionLength})
                );");
    }
    
    private static async Task DropTemporaryTable(SqlConnection connection)
    {
        try
        {
            await connection.ExecuteAsync("DROP TABLE IF EXISTS #description;");
        }
        catch (Exception)
        {
            // ignored
        }
    }
}