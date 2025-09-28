using System.Diagnostics;
using WoW.DevKit;

namespace WoW.DatabaseTool
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnMySQLConnect_Click(object sender, EventArgs e)
        {
            string uid = !string.IsNullOrEmpty(textBox_Uname.Text) ? textBox_Uname.Text : "admin";
            string pwd = !string.IsNullOrEmpty(textBox_Pass.Text) ? textBox_Pass.Text : "1111";
            string host = !string.IsNullOrEmpty(textBox_Host.Text) ? textBox_Host.Text : "127.0.0.1";
            // skip port usage for now.

            string[] dbList = null;

            if (MySQLManager.IsValid(host, uid, pwd))
                dbList = MySQLManager.QueryDatabases();

            if (dbList != null)
            {
                KitApp app = new KitApp();
                app.BuildDatabaseView(dbList);
            }
        }
    }
}
