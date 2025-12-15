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
            splitContainer1 = new SplitContainer();
            linkLabelGoToLogin = new LinkLabel();
            checkBoxOfflineMode = new CheckBox();
            buttonRegister = new Button();
            textBoxEmail = new TextBox();
            label3 = new Label();
            textBoxPassword = new TextBox();
            label2 = new Label();
            textBoxAccountName = new TextBox();
            label1 = new Label();
            linkLabelNews = new LinkLabel();
            pictureBox2 = new PictureBox();
            pictureBox3 = new PictureBox();
            pictureBox4 = new PictureBox();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(152, 12);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(66, 67);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // labelGameVer
            // 
            labelGameVer.AutoSize = true;
            labelGameVer.Location = new Point(0, 129);
            labelGameVer.Name = "labelGameVer";
            labelGameVer.Size = new Size(77, 20);
            labelGameVer.TabIndex = 2;
            labelGameVer.Text = "Game v{0}";
            labelGameVer.Click += labelGameVer_Click;
            // 
            // labelLauncherVer
            // 
            labelLauncherVer.AutoSize = true;
            labelLauncherVer.Location = new Point(0, 149);
            labelLauncherVer.Name = "labelLauncherVer";
            labelLauncherVer.Size = new Size(97, 20);
            labelLauncherVer.TabIndex = 3;
            labelLauncherVer.Text = "Launcher v{0}";
            // 
            // splitContainer1
            // 
            splitContainer1.BorderStyle = BorderStyle.Fixed3D;
            splitContainer1.Dock = DockStyle.Bottom;
            splitContainer1.Location = new Point(0, 172);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(linkLabelGoToLogin);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(checkBoxOfflineMode);
            splitContainer1.Panel2.Controls.Add(buttonRegister);
            splitContainer1.Panel2.Controls.Add(textBoxEmail);
            splitContainer1.Panel2.Controls.Add(label3);
            splitContainer1.Panel2.Controls.Add(textBoxPassword);
            splitContainer1.Panel2.Controls.Add(label2);
            splitContainer1.Panel2.Controls.Add(textBoxAccountName);
            splitContainer1.Panel2.Controls.Add(label1);
            splitContainer1.Size = new Size(364, 208);
            splitContainer1.SplitterDistance = 163;
            splitContainer1.TabIndex = 4;
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
            // checkBoxOfflineMode
            // 
            checkBoxOfflineMode.AutoSize = true;
            checkBoxOfflineMode.Location = new Point(14, 119);
            checkBoxOfflineMode.Name = "checkBoxOfflineMode";
            checkBoxOfflineMode.Size = new Size(121, 24);
            checkBoxOfflineMode.TabIndex = 7;
            checkBoxOfflineMode.Text = "Offline-mode";
            checkBoxOfflineMode.UseVisualStyleBackColor = true;
            checkBoxOfflineMode.CheckedChanged += OnOfflineModeChanged;
            // 
            // buttonRegister
            // 
            buttonRegister.Location = new Point(91, 172);
            buttonRegister.Name = "buttonRegister";
            buttonRegister.Size = new Size(94, 29);
            buttonRegister.TabIndex = 6;
            buttonRegister.Text = "Register";
            buttonRegister.UseVisualStyleBackColor = true;
            buttonRegister.Click += buttonRegister_Click;
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
            // linkLabelNews
            // 
            linkLabelNews.AutoSize = true;
            linkLabelNews.Location = new Point(319, 149);
            linkLabelNews.Name = "linkLabelNews";
            linkLabelNews.Size = new Size(45, 20);
            linkLabelNews.TabIndex = 5;
            linkLabelNews.TabStop = true;
            linkLabelNews.Text = "News";
            linkLabelNews.LinkClicked += linkLabelNews_LinkClicked;
            // 
            // pictureBox2
            // 
            pictureBox2.Cursor = Cursors.Hand;
            pictureBox2.Image = (Image)resources.GetObject("pictureBox2.Image");
            pictureBox2.Location = new Point(128, 85);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(34, 36);
            pictureBox2.TabIndex = 6;
            pictureBox2.TabStop = false;
            // 
            // pictureBox3
            // 
            pictureBox3.Cursor = Cursors.Hand;
            pictureBox3.Image = (Image)resources.GetObject("pictureBox3.Image");
            pictureBox3.Location = new Point(168, 85);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(34, 36);
            pictureBox3.TabIndex = 7;
            pictureBox3.TabStop = false;
            // 
            // pictureBox4
            // 
            pictureBox4.Cursor = Cursors.Hand;
            pictureBox4.Image = (Image)resources.GetObject("pictureBox4.Image");
            pictureBox4.Location = new Point(208, 85);
            pictureBox4.Name = "pictureBox4";
            pictureBox4.Size = new Size(34, 36);
            pictureBox4.TabIndex = 8;
            pictureBox4.TabStop = false;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(364, 380);
            Controls.Add(pictureBox4);
            Controls.Add(pictureBox3);
            Controls.Add(pictureBox2);
            Controls.Add(linkLabelNews);
            Controls.Add(splitContainer1);
            Controls.Add(labelLauncherVer);
            Controls.Add(labelGameVer);
            Controls.Add(pictureBox1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Form1";
            Text = "WPP Launcher";
            Load += Form1_Load;
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private Label labelGameVer;
        private Label labelLauncherVer;
        private SplitContainer splitContainer1;
        private Label label1;
        private TextBox textBoxEmail;
        private Label label3;
        private TextBox textBoxPassword;
        private Label label2;
        private TextBox textBoxAccountName;
        private Button buttonRegister;
        private LinkLabel linkLabelGoToLogin;
        private CheckBox checkBoxOfflineMode;
        private LinkLabel linkLabelNews;
        private PictureBox pictureBox2;
        private PictureBox pictureBox3;
        private PictureBox pictureBox4;
    }
}
