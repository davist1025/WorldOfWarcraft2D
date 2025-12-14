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
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
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
            ClientSize = new Size(364, 380);
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
    }
}
