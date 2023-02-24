using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ScrapperCore.Models.V1;
using Swashbuckle.AspNetCore.Annotations;

namespace ScrapperCore.Controllers.V1.API.CoursesAPI;

public partial class Courses
{
    [HttpPost]
    [Route("schedule-search")]
    [SwaggerOperation(
        Summary = "Find all courses which match the restrictions provided."
    )]
    [ProducesResponseType(typeof(Paginate<ClassDescription>), 200)]
    [ProducesResponseType(typeof(Error), 400)]
    public async Task<IActionResult> PostScheduleSearch([FromBody] ScheduleSearchQuery query)
    {
        if (query.Term == null)
        {
            return BadRequest(Error.GetError(101));
        }

        if (!int.TryParse(query.Term, out _))
        {
            return BadRequest(Error.GetError(2));
        }

        await using var connection = new SqlConnection(_config.SqlConnection);
        throw new NotImplementedException();
        // var classes = await connection.QueryAsync<ClassDescription>(sql, queryParams);
        // return Ok(new Paginate<ClassDescription>(classes));
    }
}