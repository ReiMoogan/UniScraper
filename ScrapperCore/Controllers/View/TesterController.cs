using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ScrapperCore.Utilities;

namespace ScrapperCore.Controllers.View
{
    [ApiController]
    [Route("/Tester/{**page}")]
    public class TesterController : HTMLController
    {
        public TesterController(ILogger<HTMLController> logger, ScrapperConfig config) : base(logger, config)
        {
            
        }

        protected override void SetupRouter()
        {
            Router.Clear();
            
            // HTML Routes
            Router.Add("student-test", "StaticViews/views/student-test.html");
        }
        
        [HttpGet]
        public override async Task<IActionResult> Get(string page)
        {
            if(Router.Count == 0)
                SetupRouter();

            return await base.Get(page);
        }
    }
}