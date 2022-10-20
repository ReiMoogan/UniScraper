using Microsoft.AspNetCore.Mvc;
using ScrapperCore.Utilities;

namespace ScrapperCore.Controllers.V1.API.CoursesAPI;

[ApiController]
[Route("v1/api/courses")]
public partial class Courses : ControllerBase
{
    private readonly ScrapperConfig _config;

    public Courses(ScrapperConfig config)
    {
        _config = config;
    }
}