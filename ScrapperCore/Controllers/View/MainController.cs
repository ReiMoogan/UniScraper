using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ScrapperCore.Controllers.View;

[ApiController]
// ReSharper disable once RouteTemplates.ControllerRouteParameterIsNotPassedToMethods
[Route("/{**page}")]
public class MainController : HTMLController
{
    public MainController(ILogger<MainController> logger) : base(logger)
    {
            
    }

    protected override void SetupRouter()
    {
        Router.Clear();
            
        // HTML Routes
        Router.Add("", "StaticViews/main/index.html");
        Router.Add("home", "StaticViews/main/index.html");

        // literally everything else
        AddFolderToRouter("", "StaticViews/main");
    }
}