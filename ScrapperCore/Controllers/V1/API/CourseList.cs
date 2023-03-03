using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using ScrapperCore.Models.V1;
using ScrapperCore.Utilities;
using Swashbuckle.AspNetCore.Annotations;

namespace ScrapperCore.Controllers.V1.API;

[ApiController]
[Route("v1/api")]
public class CourseList : ControllerBase
{
    private readonly ScrapperConfig _config;

    public CourseList(ScrapperConfig config)
    {
        _config = config;
    }

    [HttpGet]
    [Route("courses-list")]
    [SwaggerOperation(
        Summary = "Fetch all of the courses for the current term (by default).",
        Description = "May contain zeroes for backwards-compatibility/stubbing."
    )]
    public async Task<Paginate<Class>> Get(
        [FromQuery(Name = "crn")] int? crn = null,
        [FromQuery(Name = "subject")] string? subject = null,
        [FromQuery(Name = "course_id")] string? courseId = null,
        [FromQuery(Name = "course_name")] string? courseName = null,
        [FromQuery(Name = "units")] int? units = null,
        [FromQuery(Name = "type")] string? type = null,
        [FromQuery(Name = "days")] string? days = null,
        [FromQuery(Name = "hours")] string? hours = null,
        [FromQuery(Name = "room")] string? room = null,
        [FromQuery(Name = "dates")] string? dates = null,
        [FromQuery(Name = "instructor")] string? instructor = null,
        [FromQuery(Name = "lecture")] string? lecture = null,
        [FromQuery(Name = "lecture_crn")] int? lectureCRN = null,
        [FromQuery(Name = "discussion")] string? discussion = null,
        [FromQuery(Name = "attached_crn")] int? attachedCRN = null,
        [FromQuery(Name = "term")] int? term = null,
        [FromQuery(Name = "capacity")] int? capacity = null,
        [FromQuery(Name = "enrolled")] int? enrolled = null,
        [FromQuery(Name = "available")] int? available = null,
        [FromQuery(Name = "final_type")] string? finalType = null,
        [FromQuery(Name = "final_days")] string? finalDays = null,
        [FromQuery(Name = "final_hours")] string? finalHours = null,
        [FromQuery(Name = "final_room")] string? finalRoom = null,
        [FromQuery(Name = "final_dates")] string? finalDates = null,
        [FromQuery(Name = "simple_name")] string? simpleName = null,
        [FromQuery(Name = "linked_courses")] string? linkedCourses = null
    )
    {
        await using var connection = new SqlConnection(_config.SqlConnection);

        term ??= V1Utilities.DefaultTerm(); // At least filter *some* records.

        var parameters = new
        {
            crn, subject, courseId, courseName, units, type, days, hours, room, dates, instructor,
            lecture, lecture_crn = lectureCRN, discussion, attached_crn = attachedCRN, term, capacity, enrolled,
            available, final_type = finalType, final_days = finalDays, final_hours = finalHours, final_room = finalRoom,
            final_dates = finalDates, simple_name = simpleName, linked_courses = linkedCourses
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

        var sql = $@"
            SELECT * FROM [UCM].[v1api] {(sqlPredicates.Count == 0 ? "" : "WHERE")} {string.Join(" AND ", sqlPredicates)};
        ";
        var classes = await connection.QueryAsync<Class>(sql, queryParams);
        return new Paginate<Class>(classes);
    }
}