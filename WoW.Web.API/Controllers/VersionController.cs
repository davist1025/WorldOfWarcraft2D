using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace WoW.Web.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VersionController : ControllerBase
    {
        [HttpGet]
        public ActionResult<string> Get()
        {
            return new OkObjectResult(JsonConvert.SerializeObject(
                new Dictionary<string, string>()
                {
                    { "client", "0.1.0" },
                    { "launcher", "0.2.0" }
                }));
        }
    }
}
