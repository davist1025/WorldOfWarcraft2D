using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Server.Shared
{
    public class Vocab
    {
        public enum SecurityLevel
        {
            Player = 1, // has access to ALL commands and server functionality.
            Gamemaster,
            Administrator
        }
    }

    // hack: 'Utils' now exists in multiple projects (Client and server shared projects). this needs to be cleaned, if possible.
    public class Utils
    {
        public static string ToSha256(string input)
        {
            string shaHash = "";

            using (var hash = SHA256.Create())
            {
                var byteArray = hash.ComputeHash(Encoding.UTF8.GetBytes(input));
                shaHash = Convert.ToHexString(byteArray).ToLower();
            }

            return shaHash;
        }
    }
}
