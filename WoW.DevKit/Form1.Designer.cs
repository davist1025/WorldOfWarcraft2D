namespace WoW.DatabaseTool
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            label2 = new Label();
            textBox_Uname = new TextBox();
            textBox_Pass = new TextBox();
            btnMySQLConnect = new Button();
            linkAbout = new LinkLabel();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            textBox_Host = new TextBox();
            label3 = new Label();
            textBox_Port = new TextBox();
            label4 = new Label();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(45, 47);
            label1.Name = "label1";
            label1.Size = new Size(125, 20);
            label1.TabIndex = 0;
            label1.Text = "MySQL Username";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(45, 77);
            label2.Name = "label2";
            label2.Size = new Size(70, 20);
            label2.TabIndex = 1;
            label2.Text = "Password";
            // 
            // textBox_Uname
            // 
            textBox_Uname.Location = new Point(173, 44);
            textBox_Uname.Name = "textBox_Uname";
            textBox_Uname.Size = new Size(178, 27);
            textBox_Uname.TabIndex = 2;
            // 
            // textBox_Pass
            // 
            textBox_Pass.Location = new Point(173, 74);
            textBox_Pass.Name = "textBox_Pass";
            textBox_Pass.Size = new Size(178, 27);
            textBox_Pass.TabIndex = 3;
            textBox_Pass.UseSystemPasswordChar = true;
            // 
            // btnMySQLConnect
            // 
            btnMySQLConnect.Location = new Point(173, 172);
            btnMySQLConnect.Name = "btnMySQLConnect";
            btnMySQLConnect.Size = new Size(94, 29);
            btnMySQLConnect.TabIndex = 4;
            btnMySQLConnect.Text = "Connect";
            btnMySQLConnect.UseVisualStyleBackColor = true;
            btnMySQLConnect.Click += btnMySQLConnect_Click;
            // 
            // linkAbout
            // 
            linkAbout.AutoSize = true;
            linkAbout.Location = new Point(301, 176);
            linkAbout.Name = "linkAbout";
            linkAbout.Size = new Size(50, 20);
            linkAbout.TabIndex = 5;
            linkAbout.TabStop = true;
            linkAbout.Text = "About";
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new Size(20, 20);
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1 });
            statusStrip1.Location = new Point(0, 239);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(410, 26);
            statusStrip1.TabIndex = 6;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(62, 20);
            toolStripStatusLabel1.Text = "WPP 0.1";
            // 
            // textBox_Host
            // 
            textBox_Host.Location = new Point(173, 107);
            textBox_Host.Name = "textBox_Host";
            textBox_Host.Size = new Size(178, 27);
            textBox_Host.TabIndex = 8;
            textBox_Host.UseSystemPasswordChar = true;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(45, 110);
            label3.Name = "label3";
            label3.Size = new Size(40, 20);
            label3.TabIndex = 7;
            label3.Text = "Host";
            // 
            // textBox_Port
            // 
            textBox_Port.Location = new Point(173, 139);
            textBox_Port.Name = "textBox_Port";
            textBox_Port.PlaceholderText = "3306";
            textBox_Port.Size = new Size(64, 27);
            textBox_Port.TabIndex = 10;
            textBox_Port.UseSystemPasswordChar = true;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(45, 142);
            label4.Name = "label4";
            label4.Size = new Size(35, 20);
            label4.TabIndex = 9;
            label4.Text = "Port";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(410, 265);
            Controls.Add(textBox_Port);
            Controls.Add(label4);
            Controls.Add(textBox_Host);
            Controls.Add(label3);
            Controls.Add(statusStrip1);
            Controls.Add(linkAbout);
            Controls.Add(btnMySQLConnect);
            Controls.Add(textBox_Pass);
            Controls.Add(textBox_Uname);
            Controls.Add(label2);
            Controls.Add(label1);
            Name = "Form1";
            Text = "WoW2D - Development Kit";
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private Label label2;
        private TextBox textBox_Uname;
        private TextBox textBox_Pass;
        private Button btnMySQLConnect;
        private LinkLabel linkAbout;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private TextBox textBox_Host;
        private Label label3;
        private TextBox textBox_Port;
        private Label label4;
    }
}
