using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using FetchRateMyProf;
using ScrapperCore.Utilities;

namespace StandaloneTester
{
    class Program
    {
        private static async Task Main()
        {
            var config = ScrapperConfig.Load().Verify();
            await using var connection = new SqlConnection(config.SqlConnection);
            await connection.OpenAsync();
            const int ucmsid = 4767;
            var uni = RateMyProfessor.GetSchool(ucmsid);
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
            }
            await connection.CloseAsync();
        }
    }
}