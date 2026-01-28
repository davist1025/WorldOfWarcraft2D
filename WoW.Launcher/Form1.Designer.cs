namespace WoW.Launcher
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            pictureBox1 = new PictureBox();
            labelGameVer = new Label();
            labelLauncherVer = new Label();
            linkLabelNews = new LinkLabel();
            pictureBoxGit = new PictureBox();
            pictureBoxTrello = new PictureBox();
            pictureBoxBsky = new PictureBox();
            menuStrip1 = new MenuStrip();
            gameToolStripMenuItem = new ToolStripMenuItem();
            installToolStripMenuItem = new ToolStripMenuItem();
            optionsToolStripMenuItem = new ToolStripMenuItem();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            splitContainer1 = new SplitContainer();
            linkLabelGoToLogin = new LinkLabel();
            buttonRegister = new Button();
            textBoxEmail = new TextBox();
            label3 = new Label();
            textBoxPassword = new TextBox();
            label2 = new Label();
            textBoxAccountName = new TextBox();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxGit).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxTrello).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxBsky).BeginInit();
            menuStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(148, 27);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(66, 67);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // labelGameVer
            // 
            labelGameVer.AutoSize = true;
            labelGameVer.Location = new Point(0, 150);
            labelGameVer.Name = "labelGameVer";
            labelGameVer.Size = new Size(77, 20);
            labelGameVer.TabIndex = 2;
            labelGameVer.Text = "Game v{0}";
            labelGameVer.Click += labelGameVer_Click;
            // 
            // labelLauncherVer
            // 
            labelLauncherVer.AutoSize = true;
            labelLauncherVer.Location = new Point(0, 170);
            labelLauncherVer.Name = "labelLauncherVer";
            labelLauncherVer.Size = new Size(97, 20);
            labelLauncherVer.TabIndex = 3;
            labelLauncherVer.Text = "Launcher v{0}";
            // 
            // linkLabelNews
            // 
            linkLabelNews.AutoSize = true;
            linkLabelNews.Location = new Point(319, 169);
            linkLabelNews.Name = "linkLabelNews";
            linkLabelNews.Size = new Size(45, 20);
            linkLabelNews.TabIndex = 5;
            linkLabelNews.TabStop = true;
            linkLabelNews.Text = "News";
            linkLabelNews.LinkClicked += linkLabelNews_LinkClicked;
            // 
            // pictureBoxGit
            // 
            pictureBoxGit.Cursor = Cursors.Hand;
            pictureBoxGit.Image = (Image)resources.GetObject("pictureBoxGit.Image");
            pictureBoxGit.Location = new Point(124, 100);
            pictureBoxGit.Name = "pictureBoxGit";
            pictureBoxGit.Size = new Size(34, 36);
            pictureBoxGit.TabIndex = 6;
            pictureBoxGit.TabStop = false;
            pictureBoxGit.Click += OnGitClick;
            // 
            // pictureBoxTrello
            // 
            pictureBoxTrello.Cursor = Cursors.Hand;
            pictureBoxTrello.Image = (Image)resources.GetObject("pictureBoxTrello.Image");
            pictureBoxTrello.Location = new Point(164, 100);
            pictureBoxTrello.Name = "pictureBoxTrello";
            pictureBoxTrello.Size = new Size(34, 36);
            pictureBoxTrello.TabIndex = 7;
            pictureBoxTrello.TabStop = false;
            pictureBoxTrello.Click += OnTrelloClick;
            // 
            // pictureBoxBsky
            // 
            pictureBoxBsky.Cursor = Cursors.Hand;
            pictureBoxBsky.Image = (Image)resources.GetObject("pictureBoxBsky.Image");
            pictureBoxBsky.Location = new Point(204, 100);
            pictureBoxBsky.Name = "pictureBoxBsky";
            pictureBoxBsky.Size = new Size(34, 36);
            pictureBoxBsky.TabIndex = 8;
            pictureBoxBsky.TabStop = false;
            pictureBoxBsky.Click += OnBskyClick;
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { gameToolStripMenuItem, optionsToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(364, 28);
            menuStrip1.TabIndex = 9;
            menuStrip1.Text = "menuStrip1";
            // 
            // gameToolStripMenuItem
            // 
            gameToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { installToolStripMenuItem });
            gameToolStripMenuItem.Name = "gameToolStripMenuItem";
            gameToolStripMenuItem.Size = new Size(62, 24);
            gameToolStripMenuItem.Text = "Game";
            // 
            // installToolStripMenuItem
            // 
            installToolStripMenuItem.Name = "installToolStripMenuItem";
            installToolStripMenuItem.Size = new Size(140, 26);
            installToolStripMenuItem.Text = "Install...";
            // 
            // optionsToolStripMenuItem
            // 
            optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            optionsToolStripMenuItem.Size = new Size(75, 24);
            optionsToolStripMenuItem.Text = "Options";
            optionsToolStripMenuItem.Click += optionsToolStripMenuItem_Click;
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new Size(20, 20);
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1 });
            statusStrip1.Location = new Point(0, 401);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(364, 26);
            statusStrip1.TabIndex = 10;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(54, 20);
            toolStripStatusLabel1.Text = "Ready!";
            // 
            // splitContainer1
            // 
            splitContainer1.BorderStyle = BorderStyle.Fixed3D;
            splitContainer1.Dock = DockStyle.Bottom;
            splitContainer1.Location = new Point(0, 193);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(linkLabelGoToLogin);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(buttonRegister);
            splitContainer1.Panel2.Controls.Add(textBoxEmail);
            splitContainer1.Panel2.Controls.Add(label3);
            splitContainer1.Panel2.Controls.Add(textBoxPassword);
            splitContainer1.Panel2.Controls.Add(label2);
            splitContainer1.Panel2.Controls.Add(textBoxAccountName);
            splitContainer1.Panel2.Controls.Add(label1);
            splitContainer1.Size = new Size(364, 208);
            splitContainer1.SplitterDistance = 163;
            splitContainer1.TabIndex = 11;
            // 
            // linkLabelGoToLogin
            // 
            linkLabelGoToLogin.AutoSize = true;
            linkLabelGoToLogin.Location = new Point(20, 86);
            linkLabelGoToLogin.Name = "linkLabelGoToLogin";
            linkLabelGoToLogin.Size = new Size(126, 20);
            linkLabelGoToLogin.TabIndex = 0;
            linkLabelGoToLogin.TabStop = true;
            linkLabelGoToLogin.Text = "Have an account?";
            linkLabelGoToLogin.LinkClicked += linkLabelGoToLogin_LinkClicked;
            // 
            // buttonRegister
            // 
            buttonRegister.Location = new Point(91, 172);
            buttonRegister.Name = "buttonRegister";
            buttonRegister.Size = new Size(94, 29);
            buttonRegister.TabIndex = 6;
            buttonRegister.Text = "Register";
            buttonRegister.UseVisualStyleBackColor = true;
            // 
            // textBoxEmail
            // 
            textBoxEmail.Location = new Point(14, 139);
            textBoxEmail.Name = "textBoxEmail";
            textBoxEmail.PlaceholderText = "Optional";
            textBoxEmail.Size = new Size(171, 27);
            textBoxEmail.TabIndex = 5;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(14, 116);
            label3.Name = "label3";
            label3.Size = new Size(46, 20);
            label3.TabIndex = 4;
            label3.Text = "Email";
            // 
            // textBoxPassword
            // 
            textBoxPassword.Location = new Point(14, 86);
            textBoxPassword.Name = "textBoxPassword";
            textBoxPassword.PasswordChar = '*';
            textBoxPassword.Size = new Size(171, 27);
            textBoxPassword.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(14, 63);
            label2.Name = "label2";
            label2.Size = new Size(70, 20);
            label2.TabIndex = 2;
            label2.Text = "Password";
            // 
            // textBoxAccountName
            // 
            textBoxAccountName.Location = new Point(14, 33);
            textBoxAccountName.Name = "textBoxAccountName";
            textBoxAccountName.Size = new Size(171, 27);
            textBoxAccountName.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(14, 10);
            label1.Name = "label1";
            label1.Size = new Size(107, 20);
            label1.TabIndex = 0;
            label1.Text = "Account Name";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(364, 427);
            Controls.Add(splitContainer1);
            Controls.Add(statusStrip1);
            Controls.Add(pictureBoxBsky);
            Controls.Add(pictureBoxTrello);
            Controls.Add(pictureBoxGit);
            Controls.Add(linkLabelNews);
            Controls.Add(labelLauncherVer);
            Controls.Add(labelGameVer);
            Controls.Add(pictureBox1);
            Controls.Add(menuStrip1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MainMenuStrip = menuStrip1;
            MaximizeBox = false;
            Name = "Form1";
            Text = "WPP Launcher";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxGit).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxTrello).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBoxBsky).EndInit();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private Label labelGameVer;
        private Label labelLauncherVer;
        private LinkLabel linkLabelNews;
        private PictureBox pictureBoxGit;
        private PictureBox pictureBoxTrello;
        private PictureBox pictureBoxBsky;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem gameToolStripMenuItem;
        private ToolStripMenuItem installToolStripMenuItem;
        private ToolStripMenuItem optionsToolStripMenuItem;
        private StatusStrip statusStrip1;
        private SplitContainer splitContainer1;
        private LinkLabel linkLabelGoToLogin;
        private Button buttonRegister;
        private TextBox textBoxEmail;
        private Label label3;
        private TextBox textBoxPassword;
        private Label label2;
        private TextBox textBoxAccountName;
        private Label label1;
        private ToolStripStatusLabel toolStripStatusLabel1;
    }
}
