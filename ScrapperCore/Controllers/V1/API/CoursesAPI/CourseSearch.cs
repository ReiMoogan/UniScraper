using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using ScrapperCore.Models.V1;
using Swashbuckle.AspNetCore.Annotations;

namespace ScrapperCore.Controllers.V1.API.CoursesAPI;

public partial class Courses
{
    [HttpGet]
    [Route("course-search")]
    [SwaggerOperation(
        Summary = "Fetch course descriptions that are similar to the query."
    )]
    [ProducesResponseType(typeof(Paginate<ClassDescription>), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public async Task<IActionResult> GetCourseSearch(
        [FromQuery(Name = "course")] string? course = null,
        [FromQuery(Name = "term")] string? term = null
    )
    {
        if (term == null || course == null)
        {
            return BadRequest(Error.GetError(102));
        }

        if (!int.TryParse(term, out var termNumber))
        {
            return BadRequest(Error.GetError(2));
        }

        var parameters = new
        {
            ordinal = 1, simple_name = course, term = termNumber
        };

        var sqlPredicates = new List<string>();
        var queryParams = new DynamicParameters();

        foreach (var parameter in parameters.GetType().GetProperties())
        {
            var value = parameter.GetValue(parameters);
            if (value == null)
                continue;

            var name = parameter.Name;

            switch (value)
            {
                case int i:
                    sqlPredicates.Add($"{name} = @{name}");
                    queryParams.Add(name, i, DbType.Int32);
                    break;
                case string s:
                    sqlPredicates.Add($"{name} LIKE '%' + @{name} + '%'");
                    queryParams.Add(name, s, DbType.String);
                    break;
                default:
                    throw new Exception($"Unable to parse parameter query of {value.GetType()}");
            }
        }

        // Subquery is used by `parameters` with `ordinal = 1`.
        // It's easier to keep it outside since it will automatically get ANDed in the SQL string.
        var sql = $@"
            SELECT simple_name, course_name FROM
                (SELECT *, ROW_NUMBER() OVER (PARTITION BY simple_name ORDER BY crn DESC) AS ordinal FROM [UCM].[v1api]) reimu
                WHERE {string.Join(" AND ", sqlPredicates)};
        ";
        await using var connection = new SqlConnection(_config.SqlConnection);
        var classes = await connection.QueryAsync<ClassDescription>(sql, queryParams);
        return Ok(new Paginate<ClassDescription>(classes));
    }
}