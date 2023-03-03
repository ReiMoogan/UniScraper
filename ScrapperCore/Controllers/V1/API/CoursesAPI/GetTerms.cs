using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ScrapperCore.Controllers.V1.API.CoursesAPI;

public partial class Courses
{
    [HttpGet]
    [Route("get-terms")]
    [SwaggerOperation(
        Summary = "Fetch all available terms."
    )]
    public async Task<IEnumerable<string>> GetTerms()
    {
        await using var connection = new SqlConnection(_config.SqlConnection);

        const string sql = "SELECT DISTINCT term FROM [UCM].[class]";
        var terms = await connection.QueryAsync<string>(sql);
        return terms;
    }
}