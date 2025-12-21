using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WoW.Server.Shared.Database.Model;

namespace WoW.Web.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VersionController : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<string>> Get()
        {
            return new OkObjectResult(JsonConvert.SerializeObject(
                new Dictionary<string, string>()
                {
                    { "client", "-infdev-rev1" },
                    { "launcher", "-infdev-rev1" }
                }));
        }
    }
}
