using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using ScrapperCore.Models.V1;
using Swashbuckle.AspNetCore.Annotations;

namespace ScrapperCore.Controllers.V1.API.CoursesAPI;

public partial class Courses
{
    [HttpPost]
    [Route("course-match")]
    [SwaggerOperation(
        Summary = "Get all matching courses given course number prefixes and term."
    )]
    [ProducesResponseType(typeof(Dictionary<string, IEnumerable<Class>>), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public async Task<IActionResult> Post([FromBody] CourseMatchQuery query)
    {
        // Unlike Bobcat Courses, we store terms as an integer.
        if (!int.TryParse(query.Term, out _))
        {
            Response.StatusCode = 400;
            return BadRequest(Error.GetError(2));
        }

        await using var connection = new SqlConnection(_config.SqlConnection);

        // We combine multiple queries together, giving us multiple result sets (separate lists per prefix in the query)
        // Arbitrary limit of 50 to reduce matches
        const string sql = "SELECT TOP 50 * FROM [UCM].[v1api] WHERE term = @Term AND course_id LIKE @{0} + '%';";

        // Another arbitrary limit of 16 to reduce how many results we return
        var courseList = query.CourseList.Distinct().Take(16).ToList();
        var queries = "";
        var queryParams = new DynamicParameters();
        queryParams.Add("Term", query.Term);

        // Use dynamic parameters to handle adding multiple queries
        // To prevent malicious SQL injections, we still use named parameters and let Dapper handle the parameter injection
        var index = 1;
        foreach (var prefix in courseList)
        {
            var parameterName = $"P{index++}";
            queries += string.Format(sql, parameterName);
            queryParams.Add(parameterName, prefix);
        }

        if (queries.Length == 0)
        {
            return Ok(new Dictionary<string, IEnumerable<Class>>());
        }

        // Copy back the results into a dictionary
        using var reader = await connection.QueryMultipleAsync(queries, queryParams);
        var output = new Dictionary<string, IEnumerable<Class>>();

        foreach (var prefix in courseList)
        {
            if (reader.IsConsumed) break;
            var courses = await reader.ReadAsync<Class>();
            output.Add(prefix, courses);
        }

        return Ok(output);
    }
}