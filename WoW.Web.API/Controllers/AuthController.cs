using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Diagnostics;
using WoW.Server.Shared.Database.Model;
using WoW.Server.Shared.Database.Model.Auth;

namespace WoW.Web.API.Controllers
{
    [ApiController]
    [Route("api/{controller}")]
    public class AuthController : ControllerBase
    {
        private AuthContext _auth = null;

        public AuthController(AuthContext authContext)
            => _auth = authContext;

        [HttpPost]
        public async Task<ActionResult> TryLogon([FromBody] Dictionary<string, string> loginData)
        {
            try
            {
                Account user = _auth.Accounts.Single(a => a.Username.Equals(loginData["accountName"]));

                // todo: start auth process.
                // here, the client should display some kind of reactive ui (progress bar, etc) while the server works.

                return new OkObjectResult("Logged in!");
            }
            catch
            {
                return new OkObjectResult($"'{loginData["accountName"]}' was not found!");
            }
        }
    }
}
