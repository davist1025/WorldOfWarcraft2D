using Newtonsoft.Json;

namespace WoW.Launcher
{
    public partial class LauncherApp : Form
    {
        public const string CLIENT_INSTALLATION_PATH_DEFAULT = "C:/WPP";
        public const string LAUNCHER_INSTALLATION_PATH_DEFAULT = "C:/WPP/Launcher";
        public const string LAUNCHER_CONFIG_FILE = $"{LAUNCHER_INSTALLATION_PATH_DEFAULT}/config.json";

        internal static LauncherConfiguration Config;

        public LauncherApp()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Fetch all game versions and news.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnAppStartup(object sender, EventArgs e)
        {
            // todo: see summary.
            if (!Directory.Exists(CLIENT_INSTALLATION_PATH_DEFAULT))
                Directory.CreateDirectory(CLIENT_INSTALLATION_PATH_DEFAULT);

            if (!Directory.Exists(LAUNCHER_INSTALLATION_PATH_DEFAULT))
                Directory.CreateDirectory(LAUNCHER_INSTALLATION_PATH_DEFAULT);

            if (!File.Exists(LAUNCHER_CONFIG_FILE))
                CreateConfiguration();
            else
            {
                try
                {
                    string configData = File.ReadAllText(LAUNCHER_CONFIG_FILE);
                    Config = JsonConvert.DeserializeObject<LauncherConfiguration>(configData);
                    MessageBox.Show("Configuration file was loaded successfully!", "WoW Pixel Project", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"There was a problem deserializing the configuration file found at: {LAUNCHER_CONFIG_FILE}\n({ex.Message}).\n\nPress OK to create a new one.", "WoW Pixel Project", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    CreateConfiguration();
                }
            }
        }

        private void CreateConfiguration()
        {
            Config = new LauncherConfiguration()
            {
                IsConsoleDisplayed = false,
                ClientInstallPath = CLIENT_INSTALLATION_PATH_DEFAULT,
                LauncherInstallPath = LAUNCHER_INSTALLATION_PATH_DEFAULT
            };

            string configData = JsonConvert.SerializeObject(Config, Formatting.Indented);
            File.WriteAllText(LAUNCHER_CONFIG_FILE, configData);

            MessageBox.Show("Configuration file created.", "WoW Pixel Project", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OnUseLatestCheckChanged(object sender, EventArgs e)
        {
            comboBox1.Enabled = !checkBox1.Checked;
        }

        private void OnOptionsClick(object sender, EventArgs e)
        {
            Options newOptions = new Options();
            newOptions.ShowDialog(this);
        }
    }
}
