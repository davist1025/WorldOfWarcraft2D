using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Database
{
    /// <summary>
    /// Basically a global object to hold and access data.
    /// </summary>
    public static class EnvironmentContext
    {
        /// <summary>
        /// A copy of ConfigurationManager.AppSettings, often shared from an external library. In the case, WoW.Realmserver.
        /// </summary>
        public static NameValueCollection AppSettings;

        public static string GetContextConnectionString()
            => $"server={AppSettings["hostname"]};uid={AppSettings["uid"]};pwd={AppSettings["password"]};database={AppSettings["database"]}";
    }
}
