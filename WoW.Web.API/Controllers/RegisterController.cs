using Isopoh.Cryptography.Argon2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WoW.Database.Models;
using WoW.Database.Models.Auth;
using static WoW.Framework.Utils;

namespace WoW.Web.API.Controllers
{
    [ApiController]
    [Route("api/{controller}")]
    public class RegisterController : ControllerBase
    {
        private AuthContext _auth = null;

        public RegisterController(AuthContext authContext)
            => _auth = authContext;

        [HttpPost]
        public async Task<ActionResult> TryRegister([FromBody] Dictionary<string, string> loginData)
        {
            string accountName = loginData["accountName"].ToUpper();
            string hashedPw = loginData["hashedPw"];

            try
            {
                bool userExists = await _auth.Accounts.AnyAsync(a => a.Username.Equals(accountName));

                if (userExists)
                    return new ConflictObjectResult("An account with the given username already exists!");
                else
                {
                    // users password should already be hashed coming in.
                    string argonPassword = Argon2.Hash(hashedPw);

                    await _auth.Accounts.AddAsync(new Account()
                    {
                        Username = accountName.ToUpper(),
                        HashedPassword = argonPassword,
                        SecurityLevel = (int)AccountSecurityType.Player
                    });
                    await _auth.SaveChangesAsync();

                    return new OkObjectResult("Account created!");
                }
            }
            catch (Exception ex)
            {
                return new OkObjectResult($"Failed to register a new account --\n\n{ex.Message}");
            }
        }
    }
}
