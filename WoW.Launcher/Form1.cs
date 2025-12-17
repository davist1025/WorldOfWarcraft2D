using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Json;
using WoW.Client.Shared;

namespace WoW.Launcher
{
    public partial class Form1 : Form
    {
        private bool _isLoginState = false;

        public static string GameVersion = "";
        public static string LauncherVersion = "";

        public Form1()
        {
            InitializeComponent();
        }

        private void labelGameVer_Click(object sender, EventArgs e)
        {

        }

        private void linkLabelGoToLogin_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!_isLoginState)
            {
                label3.Hide();
                textBoxEmail.Hide();
                buttonRegister.Text = "Login";
                linkLabelGoToLogin.Text = "Need an account?";
                _isLoginState = true;
            }
            else
            {
                label3.Show();
                textBoxEmail.Show();
                buttonRegister.Text = "Register";
                linkLabelGoToLogin.Text = "Have an account?";
                _isLoginState = false;
            }
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            // fetch the current game, launcher version.
            var value = await HttpService.GetVersionAsync();
            dynamic dynamicResponse = JsonConvert.DeserializeObject(value); // this is icky.

            GameVersion = dynamicResponse["client"];
            LauncherVersion = dynamicResponse["launcher"];

            labelGameVer.Text = string.Format(labelGameVer.Text, GameVersion);
            labelLauncherVer.Text = string.Format(labelLauncherVer.Text, LauncherVersion);
        }

        private async void buttonRegister_Click(object sender, EventArgs e)
        {
            string accountName = textBoxAccountName.Text.Trim();
            string hashedPassword = Utils.ToSHA256(textBoxPassword.Text.Trim());


            if (_isLoginState)
            {
                // hack: debug code to test http services.
                var logonResponse = await HttpService.Login(accountName, hashedPassword);
                MessageBox.Show(logonResponse);
            }
            else
            {
                var registerResponse = await HttpService.Register(accountName, hashedPassword);
                MessageBox.Show(registerResponse);
            }
        }

        private void linkLabelNews_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            MessageBox.Show("Coming soon!\n\nRefer to the GitHub, Trello or BlueSky pages for more information.", "WoW Pixel Project", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void OnBskyClick(object sender, EventArgs e) => Process.Start(new ProcessStartInfo("https://bsky.app/profile/mrkokiri.bsky.social") { UseShellExecute = true });

        private void OnTrelloClick(object sender, EventArgs e) => Process.Start(new ProcessStartInfo("https://trello.com/b/4BRTSQXK/wow2d") { UseShellExecute = true });

        private void OnGitClick(object sender, EventArgs e) => Process.Start(new ProcessStartInfo("https://github.com/davist1025/WorldOfWarcraft2D") { UseShellExecute = true });
    }
}
