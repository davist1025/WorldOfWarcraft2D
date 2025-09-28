using Microsoft.AspNetCore.Mvc;

namespace WoW.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VersionController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            return "This is a test";
        }
    }
}
