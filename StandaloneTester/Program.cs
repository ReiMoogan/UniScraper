using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;

namespace StandaloneTester;

internal static class Program
{
    private static async Task Main()
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();
        var config = Config.Load().Verify();
        await using var connection = new SqlConnection(config.SqlConnection);
        Console.WriteLine("Opening database connection...");
        await connection.OpenAsync();
        Console.WriteLine("Updating classes...");
        var classTask = UpdateUCMClasses.UpdateClasses(connection);
        Console.WriteLine("Updating professors...");
        var profTask = UpdateRMP.UpdateProfessors(connection);
        Console.WriteLine("Waiting for previous two tasks to finish...");
        await Task.WhenAll(classTask, profTask);
        Console.WriteLine("Updating class descriptions...");
        await UpdateUCMDescriptions.UpdateDescriptions(connection);
        Console.WriteLine("Closing database connection...");
        await connection.CloseAsync();
        Console.WriteLine($"Finished in {stopwatch.Elapsed.TotalSeconds}s!");
    }
}