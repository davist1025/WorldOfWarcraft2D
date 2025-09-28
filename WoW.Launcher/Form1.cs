namespace WoW.Launcher
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void OnUseLatestCheckChanged(object sender, EventArgs e)
        {
            comboBox1.Enabled = !checkBox1.Checked;
        }
    }
}
