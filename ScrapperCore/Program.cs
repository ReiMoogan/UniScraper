using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using ScrapperCore.Utilities;

namespace ScrapperCore
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    var configPort = ScrapperConfig.Load().Port;
                    webBuilder.UseStartup<Startup>().UseUrls($"https://*:{configPort}");
                });
    }
}