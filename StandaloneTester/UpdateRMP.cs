using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using FastMember;
using FetchRateMyProf;

namespace StandaloneTester;

public static class UpdateRMP
{
    public static async Task UpdateProfessors(SqlConnection connection)
    {
        const int ucmsid = 4767;
        var uni = RateMyProfessor.GetSchool(ucmsid);

        var professors = await uni.GetAllProfessors().ToListAsync();
        var noDuplicates = professors.GroupBy(o => new { FirstName = o.FirstName.Replace("-", "").Replace(" ", ""), LastName = o.LastName.Replace("-", "").Replace(" ", "") }).Select(o =>
        {
            if (o.Count() > 1)
                Console.WriteLine($"Duplicate between {string.Join(", ", o)}");
            return o.Aggregate((a, b) => a.NumRatings > b.NumRatings ? a : b);
        }).ToList();
        var professorTable = new DataTable();
        
        await using (var reader = ObjectReader.Create(noDuplicates, "Id", "FirstName", "LastName", "MiddleName", "Department", "NumRatings", "OverallRating")) {
            professorTable.Load(reader);
        }

        await CreateRMPTemporaryTables(connection);
        using (var copier = new SqlBulkCopy(connection))
        {
            copier.DestinationTableName = "#professor_rmp";
            copier.ColumnMappings.Add("Id", "rmp_id");
            copier.ColumnMappings.Add("FirstName", "first_name");
            copier.ColumnMappings.Add("LastName", "last_name");
            copier.ColumnMappings.Add("MiddleName", "middle_name");
            copier.ColumnMappings.Add("Department", "department");
            copier.ColumnMappings.Add("NumRatings", "num_ratings");
            copier.ColumnMappings.Add("OverallRating", "rating");
            await copier.WriteToServerAsync(professorTable);
        }

        await connection.ExecuteAsync(
            "EXEC [UniScraper].[UCM].[MergeRMP];");
        await DropRMPTemporaryTables(connection);
        Console.WriteLine($"Updated {noDuplicates.Count} professors!");
    }
    
    private static async Task CreateRMPTemporaryTables(IDbConnection connection)
    {
        // Drop if a previous session crashed mid-way.
        await DropRMPTemporaryTables(connection);
        await connection.ExecuteAsync(
            @"CREATE TABLE #professor_rmp
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
    
    private static async Task DropRMPTemporaryTables(IDbConnection connection)
    {
        try
        {
            await connection.ExecuteAsync("DROP TABLE IF EXISTS #professor_rmp;");
        }
        catch (Exception)
        {
            // ignored
        }
    }
}