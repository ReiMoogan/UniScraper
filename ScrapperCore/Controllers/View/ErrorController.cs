using Microsoft.AspNetCore.Mvc;

namespace ScrapperCore.Controllers.View;

[ApiController]
[Route("/Error/{id:int}")]
public class ErrorController : ControllerBase
{
    [HttpGet]
    public ContentResult Get(int code)
    {
        var file = $"StaticViews/views/error/{code}.html";
        if (!System.IO.File.Exists(file))
            file = "StaticViews/views/error/500.html";
        var data = System.IO.File.ReadAllText(file);
        return base.Content(data, "text/html");
    }
}