namespace WoW.Launcher
{
    partial class Options
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            groupBox1 = new GroupBox();
            checkBoxConsoleOutput = new CheckBox();
            label1 = new Label();
            textBoxClientInstallation = new TextBox();
            buttonBrowseClientDir = new Button();
            buttonBrowseLauncherDir = new Button();
            textBoxLauncherInstallation = new TextBox();
            label2 = new Label();
            buttonApply = new Button();
            buttonRestoreDefaults = new Button();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(checkBoxConsoleOutput);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(309, 125);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Client Options";
            // 
            // checkBoxConsoleOutput
            // 
            checkBoxConsoleOutput.AutoSize = true;
            checkBoxConsoleOutput.Location = new Point(6, 26);
            checkBoxConsoleOutput.Name = "checkBoxConsoleOutput";
            checkBoxConsoleOutput.Size = new Size(180, 24);
            checkBoxConsoleOutput.TabIndex = 0;
            checkBoxConsoleOutput.Text = "Run w/ console output";
            checkBoxConsoleOutput.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 149);
            label1.Name = "label1";
            label1.Size = new Size(156, 20);
            label1.TabIndex = 1;
            label1.Text = "Client Installation Path";
            // 
            // textBoxClientInstallation
            // 
            textBoxClientInstallation.Location = new Point(12, 172);
            textBoxClientInstallation.Name = "textBoxClientInstallation";
            textBoxClientInstallation.ReadOnly = true;
            textBoxClientInstallation.Size = new Size(275, 27);
            textBoxClientInstallation.TabIndex = 2;
            // 
            // buttonBrowseClientDir
            // 
            buttonBrowseClientDir.Location = new Point(293, 171);
            buttonBrowseClientDir.Name = "buttonBrowseClientDir";
            buttonBrowseClientDir.Size = new Size(28, 29);
            buttonBrowseClientDir.TabIndex = 3;
            buttonBrowseClientDir.Text = "...";
            buttonBrowseClientDir.UseVisualStyleBackColor = true;
            // 
            // buttonBrowseLauncherDir
            // 
            buttonBrowseLauncherDir.Location = new Point(293, 224);
            buttonBrowseLauncherDir.Name = "buttonBrowseLauncherDir";
            buttonBrowseLauncherDir.Size = new Size(28, 29);
            buttonBrowseLauncherDir.TabIndex = 6;
            buttonBrowseLauncherDir.Text = "...";
            buttonBrowseLauncherDir.UseVisualStyleBackColor = true;
            // 
            // textBoxLauncherInstallation
            // 
            textBoxLauncherInstallation.Location = new Point(12, 225);
            textBoxLauncherInstallation.Name = "textBoxLauncherInstallation";
            textBoxLauncherInstallation.ReadOnly = true;
            textBoxLauncherInstallation.Size = new Size(275, 27);
            textBoxLauncherInstallation.TabIndex = 5;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 202);
            label2.Name = "label2";
            label2.Size = new Size(100, 20);
            label2.TabIndex = 4;
            label2.Text = "Launcher Path";
            // 
            // buttonApply
            // 
            buttonApply.Location = new Point(145, 258);
            buttonApply.Name = "buttonApply";
            buttonApply.Size = new Size(94, 29);
            buttonApply.TabIndex = 7;
            buttonApply.Text = "Apply";
            buttonApply.UseVisualStyleBackColor = true;
            // 
            // buttonRestoreDefaults
            // 
            buttonRestoreDefaults.Location = new Point(12, 258);
            buttonRestoreDefaults.Name = "buttonRestoreDefaults";
            buttonRestoreDefaults.Size = new Size(127, 29);
            buttonRestoreDefaults.TabIndex = 8;
            buttonRestoreDefaults.Text = "Restore Defaults";
            buttonRestoreDefaults.UseVisualStyleBackColor = true;
            // 
            // Options
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(333, 295);
            Controls.Add(buttonRestoreDefaults);
            Controls.Add(buttonApply);
            Controls.Add(buttonBrowseLauncherDir);
            Controls.Add(textBoxLauncherInstallation);
            Controls.Add(label2);
            Controls.Add(buttonBrowseClientDir);
            Controls.Add(textBoxClientInstallation);
            Controls.Add(label1);
            Controls.Add(groupBox1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Options";
            Text = "Options";
            Load += OnDisplay;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private GroupBox groupBox1;
        private CheckBox checkBoxConsoleOutput;
        private Label label1;
        private TextBox textBoxClientInstallation;
        private Button buttonBrowseClientDir;
        private Button buttonBrowseLauncherDir;
        private TextBox textBoxLauncherInstallation;
        private Label label2;
        private Button buttonApply;
        private Button buttonRestoreDefaults;
    }
}