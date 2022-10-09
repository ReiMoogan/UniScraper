using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using ScrapperCore.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace ScrapperCore.Controllers.V1.API.Courses;

[ApiController]
[Route("v1/api/courses")]
public class GetTerms : ControllerBase
{
    private readonly ScrapperConfig _config;

    public GetTerms(ScrapperConfig config)
    {
        _config = config;
    }

    [HttpGet]
    [Route("get-terms")]
    [SwaggerOperation(
        Summary = "Fetch all available terms."
    )]
    public async Task<IEnumerable<string>> Get()
    {
        await using var connection = new SqlConnection(_config.SqlConnection);

        const string sql = "SELECT DISTINCT term FROM [UCM].[class]";
        var terms = await connection.QueryAsync<string>(sql);
        return terms;
    }
}