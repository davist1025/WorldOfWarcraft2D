using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using WoW.Client.Shared.Web.Model;

namespace WoW.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        /// <summary>
        /// Invoked when a client is attempting to sign-up with a new account.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Post([FromBody] object value)
        {
            var request = Request;


            // todo: asynchrnous request processing.
            // todo: request validation? (content-type, headers, etc).

            return Ok("test");
        }
    }
}
