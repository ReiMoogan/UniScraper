using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using ScrapperCore.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace ScrapperCore.Controllers.V1.API;

[ApiController]
[Route("v1/api")]
public class Statistics : ControllerBase
{
    private readonly ScrapperConfig _config;
    
    public Statistics(ScrapperConfig config)
    {
        _config = config;
    }
    
    [HttpGet]
    [Route("statistics")]
    [SwaggerOperation(
        Summary = "Get information about the UCMScraper database.",
        Description = "May contain zeroes for backwards-compatibility/stubbing."
    )]
    public async Task<Models.V1.Statistics> Get()
    {
        await using var connection = new SqlConnection(_config.SqlConnection);
        return await connection.QuerySingleAsync<Models.V1.Statistics>(@"
            SELECT
	            (SELECT COUNT(*) FROM [UCM].[class]) AS total_classes,	
	            (SELECT COUNT(*) FROM [UCM].[professor]) AS total_professors,	
	            (SELECT COUNT(*) FROM [UCM].[meeting]) AS total_meetings,
	            (SELECT TOP 1 last_update FROM [UCM].[stats] ORDER BY last_update DESC) AS last_update;
        ");
    }
}