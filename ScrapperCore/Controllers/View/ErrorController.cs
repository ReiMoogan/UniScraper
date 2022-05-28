using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ScrapperCore.Utilities;

namespace ScrapperCore.Controllers.View;

[ApiController]
[ApiExplorerSettings(IgnoreApi = true)]
[Route("/Error/{code}")]
public class ErrorController : HTMLController
{
    public ErrorController(ILogger<ErrorController> logger, ScrapperConfig config) : base(logger, config)
    {
    }
    
    protected override void SetupRouter()
    {
        Router.Clear();
        AddFolderToRouter("", "StaticViews/views/error");
    }
    
    [HttpGet]
    public override async Task<IActionResult> Get(string code)
    {
        var file = $"StaticViews/views/error/{code}.html";
        if (!System.IO.File.Exists(file))
            return await base.Get("500.html");
        return await base.Get($"{code}.html");
    }
}