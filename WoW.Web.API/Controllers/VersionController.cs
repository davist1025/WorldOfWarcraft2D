using Microsoft.AspNetCore.Mvc;

namespace WoW.Web.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VersionController : ControllerBase
    {
        [HttpGet]
        public ActionResult<string> Get([FromQuery] string application)
        {
            application = application.Trim().ToLower();

            switch (application)
            {
                case "client": return new OkObjectResult("0.1.0");
                case "launcher": return new OkObjectResult("0.2.0"); ;
            }

            return new OkObjectResult(application);
        }
    }
}
