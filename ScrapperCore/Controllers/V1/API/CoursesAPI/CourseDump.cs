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
    [Route("course-dump")]
    [SwaggerOperation(
        Summary = "Fetch all course descriptions."
    )]
    public async Task<Paginate<SubjectCourse>> GetCourseDump(
        [FromQuery(Name = "course_name")] string? courseName = null,
        [FromQuery(Name = "term")] int? term = null,
        [FromQuery(Name = "course_description")] string? courseDescription = null,
        [FromQuery(Name = "course_subject")] string? courseSubject = null
    )
    {
        var parameters = new
        {
            ordinal = 1, subject = courseSubject, course_name = courseDescription,
            term = term ?? V1Utilities.DefaultTerm(), simple_name = courseName
        };

        var sqlPredicates = new List<string>();
        var queryParams = new DynamicParameters();

        foreach (var parameter in parameters.GetType().GetProperties())
        {
            var value = parameter.GetValue(parameters);
            if (value == null)
                continue;

            var name = parameter.Name;
            sqlPredicates.Add($"{name} = @{name}");

            switch (value)
            {
                case int i:
                    queryParams.Add(name, i, DbType.Int32);
                    break;
                case string s:
                    queryParams.Add(name, s, DbType.String);
                    break;
                default:
                    throw new Exception($"Unable to parse parameter query of {value.GetType()}");
            }
        }

        // Subquery is used by `parameters` with `ordinal = 1`.
        // It's easier to keep it outside since it will automatically get ANDed in the SQL string.
        var sql = $@"
            SELECT crn, simple_name, term, course_name, subject FROM
                (SELECT *, ROW_NUMBER() OVER (PARTITION BY simple_name ORDER BY crn DESC) AS ordinal FROM [UCM].[v1api]) reimu
                WHERE {string.Join(" AND ", sqlPredicates)};
        ";
        await using var connection = new SqlConnection(_config.SqlConnection);
        var classes = await connection.QueryAsync<SubjectCourse>(sql, queryParams);
        return new Paginate<SubjectCourse>(classes);
    }
}