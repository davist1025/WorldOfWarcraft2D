using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WoW.Launcher
{
    public partial class Options : Form
    {
        public Options()
        {
            InitializeComponent();
        }

        private void OnDisplay(object sender, EventArgs e)
        {
            checkBoxConsoleOutput.Checked = LauncherApp.Config.IsConsoleDisplayed;
            textBoxClientInstallation.Text = LauncherApp.Config.ClientInstallPath;
            textBoxLauncherInstallation.Text = LauncherApp.Config.LauncherInstallPath;
        }
    }
}
