using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoW.Launcher
{
    /// <summary>
    /// The configuration data for the Launcher; the majority of these can be found in Options.cs.
    /// </summary>
    internal class LauncherConfiguration
    {
        public bool IsConsoleDisplayed = false;
        public string ClientInstallPath = "";
        public string LauncherInstallPath = "";
    }
}
