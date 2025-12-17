namespace WoW.Launcher
{
    partial class FormOptions
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
            label1 = new Label();
            textBoxRootPath = new TextBox();
            buttonBrowsePath = new Button();
            groupBox2 = new GroupBox();
            checkBox1 = new CheckBox();
            button1 = new Button();
            button2 = new Button();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(buttonBrowsePath);
            groupBox1.Controls.Add(textBoxRootPath);
            groupBox1.Controls.Add(label1);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(376, 99);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Directory";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 34);
            label1.Name = "label1";
            label1.Size = new Size(142, 20);
            label1.TabIndex = 0;
            label1.Text = "WoW2D Root Path -";
            // 
            // textBoxRootPath
            // 
            textBoxRootPath.Location = new Point(6, 57);
            textBoxRootPath.Name = "textBoxRootPath";
            textBoxRootPath.Size = new Size(322, 27);
            textBoxRootPath.TabIndex = 1;
            // 
            // buttonBrowsePath
            // 
            buttonBrowsePath.Location = new Point(334, 55);
            buttonBrowsePath.Name = "buttonBrowsePath";
            buttonBrowsePath.Size = new Size(32, 29);
            buttonBrowsePath.TabIndex = 2;
            buttonBrowsePath.Text = "...";
            buttonBrowsePath.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(checkBox1);
            groupBox2.Location = new Point(12, 117);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(376, 64);
            groupBox2.TabIndex = 3;
            groupBox2.TabStop = false;
            groupBox2.Text = "Game";
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(6, 26);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(121, 24);
            checkBox1.TabIndex = 0;
            checkBox1.Text = "Offline-mode";
            checkBox1.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Location = new Point(294, 187);
            button1.Name = "button1";
            button1.Size = new Size(94, 29);
            button1.TabIndex = 4;
            button1.Text = "Save";
            button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            button2.Location = new Point(194, 187);
            button2.Name = "button2";
            button2.Size = new Size(94, 29);
            button2.TabIndex = 5;
            button2.Text = "Cancel";
            button2.UseVisualStyleBackColor = true;
            // 
            // FormOptions
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(400, 227);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "FormOptions";
            Text = "Options";
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private Label label1;
        private TextBox textBoxRootPath;
        private Button buttonBrowsePath;
        private GroupBox groupBox2;
        private CheckBox checkBox1;
        private Button button1;
        private Button button2;
    }
}