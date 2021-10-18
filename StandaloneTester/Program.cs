using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using FetchRateMyProf;
using FetchUCM;
using ScrapperCore.Utilities;

namespace StandaloneTester
{
    internal static class Program
    {
        private static async Task Main()
        {
            var config = ScrapperConfig.Load().Verify();
            await using var connection = new SqlConnection(config.SqlConnection);
            Console.WriteLine("Opening database connection...");
            await connection.OpenAsync();
            // await UpdateProfessors(connection);
            await UpdateClasses(connection);
            Console.WriteLine("Closing database connection...");
            await connection.CloseAsync();
            Console.WriteLine("Finished!");
        }

        private static async Task UpdateProfessors(IDbConnection connection)
        {
            Console.WriteLine("Updating professors...");
            const int ucmsid = 4767;
            var uni = RateMyProfessor.GetSchool(ucmsid);
            var count = 0;
            await foreach (var professor in uni.GetAllProfessors())
            {
                await connection.ExecuteAsync(
                    "MERGE INTO UniScraper.UCM.professor WITH (HOLDLOCK) AS Target " +
                    "USING (Values(@LastName, @FirstName, @MiddleName, @Department, @NumRatings, @OverallRating)) AS SOURCE(last_name, first_name, middle_name, department, num_ratings, rating) " +
                    "ON Target.id = @Id WHEN MATCHED THEN " +
                    "UPDATE SET last_name = SOURCE.last_name, first_name = SOURCE.first_name, middle_name = SOURCE.middle_name, department = SOURCE.department, num_ratings = SOURCE.num_ratings, rating = SOURCE.rating " +
                    "WHEN NOT MATCHED THEN " +
                    "INSERT (id, last_name, first_name, middle_name, department, num_ratings, rating) VALUES (@Id, @LastName, @FirstName, @MiddleName, @Department, @NumRatings, @OverallRating);",
                    new { professor.Id, professor.LastName, professor.FirstName, professor.MiddleName, professor.Department, professor.NumRatings, professor.OverallRating });
                ++count;
            }

            await connection.ExecuteAsync(
                "UPDATE UniScraper.UCM.stats SET last_update = SYSDATETIME() WHERE table_name = 'professor';");
            Console.WriteLine($"Updated {count} professors!");
        }
        
        private static async Task UpdateClasses(IDbConnection connection)
        {
            const string jsessionid = "FFEA532A79CF472D54118F1DFBED9892";
            Console.WriteLine("Updating classes...");
            var catalog = new UCMCatalog();
            var term = (await catalog.GetAllTerms())[0].Code;
            Console.WriteLine($"Reading from term {term}");
            var count = 0;
            
            await foreach (var item in catalog.GetAllClasses(term, jsessionid))
            {
                
                ++count;
            }

            await connection.ExecuteAsync(
                "UPDATE UniScraper.UCM.stats SET last_update = SYSDATETIME() WHERE table_name = 'class';");
            Console.WriteLine($"Updated {count} classes!");
        }
    }
}