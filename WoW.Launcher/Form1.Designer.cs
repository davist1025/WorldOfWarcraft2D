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
            panelRegister = new Panel();
            textBox3 = new TextBox();
            label5 = new Label();
            button2 = new Button();
            button1 = new Button();
            textBox2 = new TextBox();
            label2 = new Label();
            textBox1 = new TextBox();
            label1 = new Label();
            labelGameVer = new Label();
            labelLauncherVer = new Label();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            panelRegister.SuspendLayout();
            SuspendLayout();
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(120, 22);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(66, 67);
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // panelRegister
            // 
            panelRegister.BorderStyle = BorderStyle.Fixed3D;
            panelRegister.Controls.Add(textBox3);
            panelRegister.Controls.Add(label5);
            panelRegister.Controls.Add(button2);
            panelRegister.Controls.Add(button1);
            panelRegister.Controls.Add(textBox2);
            panelRegister.Controls.Add(label2);
            panelRegister.Controls.Add(textBox1);
            panelRegister.Controls.Add(label1);
            panelRegister.Dock = DockStyle.Bottom;
            panelRegister.Location = new Point(0, 163);
            panelRegister.Name = "panelRegister";
            panelRegister.Size = new Size(307, 170);
            panelRegister.TabIndex = 1;
            // 
            // textBox3
            // 
            textBox3.Location = new Point(126, 82);
            textBox3.Name = "textBox3";
            textBox3.PlaceholderText = "Optional";
            textBox3.Size = new Size(167, 27);
            textBox3.TabIndex = 7;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(10, 85);
            label5.Name = "label5";
            label5.Size = new Size(49, 20);
            label5.TabIndex = 6;
            label5.Text = "Email:";
            // 
            // button2
            // 
            button2.Location = new Point(10, 127);
            button2.Name = "button2";
            button2.Size = new Size(94, 29);
            button2.TabIndex = 5;
            button2.Text = "<- Back";
            button2.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Location = new Point(199, 127);
            button1.Name = "button1";
            button1.Size = new Size(94, 29);
            button1.TabIndex = 4;
            button1.Text = "Register";
            button1.UseVisualStyleBackColor = true;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(126, 49);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(167, 27);
            textBox2.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(10, 52);
            label2.Name = "label2";
            label2.Size = new Size(73, 20);
            label2.TabIndex = 2;
            label2.Text = "Password:";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(126, 16);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(167, 27);
            textBox1.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(10, 19);
            label1.Name = "label1";
            label1.Size = new Size(110, 20);
            label1.TabIndex = 0;
            label1.Text = "Account Name:";
            // 
            // labelGameVer
            // 
            labelGameVer.AutoSize = true;
            labelGameVer.Location = new Point(0, 120);
            labelGameVer.Name = "labelGameVer";
            labelGameVer.Size = new Size(77, 20);
            labelGameVer.TabIndex = 2;
            labelGameVer.Text = "Game v{0}";
            // 
            // labelLauncherVer
            // 
            labelLauncherVer.AutoSize = true;
            labelLauncherVer.Location = new Point(0, 140);
            labelLauncherVer.Name = "labelLauncherVer";
            labelLauncherVer.Size = new Size(97, 20);
            labelLauncherVer.TabIndex = 3;
            labelLauncherVer.Text = "Launcher v{0}";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(307, 333);
            Controls.Add(labelLauncherVer);
            Controls.Add(labelGameVer);
            Controls.Add(panelRegister);
            Controls.Add(pictureBox1);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            panelRegister.ResumeLayout(false);
            panelRegister.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox pictureBox1;
        private Panel panelRegister;
        private Label label1;
        private Button button1;
        private TextBox textBox2;
        private Label label2;
        private TextBox textBox1;
        private Button button2;
        private Label labelGameVer;
        private Label labelLauncherVer;
        private TextBox textBox3;
        private Label label5;
    }
}
