using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Serialization;
using ScrapperCore.Controllers.V2;
using ScrapperCore.Models.V2;
using ScrapperCore.Models.V2.GraphQL;
using ScrapperCore.Models.V2.SQL;
using ScrapperCore.Utilities;

namespace ScrapperCore;

public class Startup
{
    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers().AddNewtonsoftJson(o =>
        {
            o.SerializerSettings.ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new SnakeCaseNamingStrategy()
            };
        });
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "UCMScraper API",
                Version = "v1",
                Description = "A backwards-compatible API, based upon the CSE120 course planner."
            });
            c.EnableAnnotations();
        });
        services.AddSwaggerGenNewtonsoftSupport();
        services.AddHttpClient();

        var config = ScrapperConfig.Load();
        services.AddSingleton(config);
        
        // For GraphQL
        services.AddDbContextFactory<UniScraperContext>(options => options.UseSqlServer(config.SqlConnection));
        services.AddScoped<IClassRepository, ClassRepository>();
        services
            .AddGraphQLServer()
            .AddType<ClassType>()
            .AddQueryType<Query>()
            .AddSubscriptionType<Subscription>()
            .AddInMemorySubscriptions();
        
        // Honestly don't know a better place to put this
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseStatusCodePagesWithReExecute("/Error", "?code={0}");
        }

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapSwagger();
            endpoints.MapGraphQL("/v2/graphql");
        });

        app.UseSwagger(c =>
        {
            c.RouteTemplate = "v1/api/{documentName}/swagger.json";
        });
        
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("v1/swagger.json", "UCMScraper API v1");
            c.RoutePrefix = "v1/api";
            c.DocumentTitle = "UniScraper API Docs";
        });

        app.UseWebSockets();
    }
}