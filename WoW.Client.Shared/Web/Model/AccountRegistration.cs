using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Client.Shared.Web.Model
{
    /// <summary>
    /// C -> Web.
    /// A new account for game registration.
    /// </summary>
    public class AccountRegistration
    {
        public string AccountName { get; set; }

        /// <summary>
        /// Passwords are hashed with SHA256, sent, and then again using Argon2 server-side.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// The user's email. Currently not used.
        /// </summary>
        [Obsolete("Not used in production (10/13/25).")]
        public string EmailAddress { get; set; }

        /// <summary>
        /// The user's geographical location and language (enUS, enUK, enAU, etc).
        /// </summary>
        public string Locale { get; set; }
    }
}
