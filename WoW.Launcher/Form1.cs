namespace WoW.Launcher
{
    public partial class Form1 : Form
    {
        private bool _isLoginState = false;

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
                checkBoxOfflineMode.Show();
                buttonRegister.Text = "Login";
                linkLabelGoToLogin.Text = "Need an account?";
                _isLoginState = true;
            }
            else
            {
                label3.Show();
                textBoxEmail.Show();
                checkBoxOfflineMode.Hide();
                buttonRegister.Text = "Register";
                linkLabelGoToLogin.Text = "Have an account?";
                _isLoginState = false;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            checkBoxOfflineMode.Hide();
        }

        private void OnOfflineModeChanged(object sender, EventArgs e)
        {
            textBoxAccountName.Enabled = !checkBoxOfflineMode.Checked;
            textBoxPassword.Enabled = !checkBoxOfflineMode.Checked;
        }
    }
}
