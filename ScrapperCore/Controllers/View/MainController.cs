using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ScrapperCore.Utilities;

namespace ScrapperCore.Controllers.View;

[ApiController]
[Route("/{**page}")]
public class MainController : HTMLController
{
    public MainController(ILogger<MainController> logger, ScrapperConfig config) : base(logger, config)
    {
            
    }

    protected override void SetupRouter()
    {
        // So why not serve static? Because I need a way to serve my own routes.
        // Even though ASP.NET Core can do that too. Oh well.
        Router.Clear();
            
        // HTML Routes
        Router.Add("", "StaticViews/views/index.html");
        Router.Add("home", "StaticViews/views/index.html");

        // Holy crap icon stuff
        AddFolderToRouter("", "StaticViews/img");

        // All other files
        // Do not include StaticViews/views as that picks up student-test
        AddFolderToRouter("img", "StaticViews/img");
        AddFolderToRouter("js", "StaticViews/js");
        AddFolderToRouter("css", "StaticViews/css");
    }
}