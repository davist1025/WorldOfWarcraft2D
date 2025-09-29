using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WoW.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VersionController : ControllerBase
    {
        /// <summary>
        /// Returns the current launcher version, all game versions and which are still supported by the game servers.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string[] Get()
        {
            return new string[]
            {
                "0.1.0",
                "0.1.1"
            };
        }
    }
}
