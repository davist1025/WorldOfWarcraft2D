using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WoW.DevKit
{
    public partial class KitApp : Form
    {
        public KitApp()
        {
            InitializeComponent();
        }

        public void BuildDatabaseView(string[] dbNames)
        {
            for (int i = 0; i < dbNames.Length; i++)
            {
                var name = dbNames[i];
                if (name.ToLower().StartsWith("wpp"))
                    listBox_DbList.Items.Add(name);
            }
            Show();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            MySQLManager.Closeout();
        }
    }
}
