using System.Data.SqlClient;
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
    public async Task<CoursePagination> Get(
        [FromQuery] int? crn = null,
        [FromQuery] string subject = null,
        [FromQuery(Name = "course_id")] string courseId = null,
        [FromQuery(Name = "course_name")] string courseName = null,
        [FromQuery(Name = "units")] int? units = null,
        [FromQuery(Name = "type")] string type = null,
        [FromQuery(Name = "days")] string days = null,
        [FromQuery(Name = "hours")] string hours = null,
        [FromQuery(Name = "room")] string room = null,
        [FromQuery(Name = "dates")] string dates = null,
        [FromQuery(Name = "instructor")] string instructor = null,
        [FromQuery(Name = "lecture")] string lecture = null,
        [FromQuery(Name = "lecture_crn")] int? lectureCRN = null,
        [FromQuery(Name = "discussion")] string discussion = null,
        [FromQuery(Name = "attached_crn")] int? attachedCRN = null,
        [FromQuery(Name = "term")] int? term = null,
        [FromQuery(Name = "capacity")] int? capacity = null,
        [FromQuery(Name = "enrolled")] int? enrolled = null,
        [FromQuery(Name = "available")] int? available = null,
        [FromQuery(Name = "final_type")] string finalType = null,
        [FromQuery(Name = "final_days")] string finalDays = null,
        [FromQuery(Name = "final_hours")] string finalHours = null,
        [FromQuery(Name = "final_room")] string finalRoom = null,
        [FromQuery(Name = "final_dates")] string finalDates = null,
        [FromQuery(Name = "simple_name")] string simpleName = null,
        [FromQuery(Name = "linked_courses")] string linkedCourses = null
    )
    {
        await using var connection = new SqlConnection(_config.SqlConnection);
        var classes = await connection.QueryAsync<Class>(@"
            DROP TABLE IF EXISTS #crn;
            SELECT course_reference_number AS crn INTO #crn FROM [UCM].[class] WHERE term = 202230;
            EXEC [UCM].[GetCourses];
        ");
        return new CoursePagination(classes);
    }
}