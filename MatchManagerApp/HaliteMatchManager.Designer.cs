namespace Halite3MatchManager
{
    partial class HaliteMatchManager
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
            if (disposing && (components != null)) {
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cBot0 = new System.Windows.Forms.TextBox();
            this.cBot1 = new System.Windows.Forms.TextBox();
            this.cResults = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.cGame100 = new System.Windows.Forms.RadioButton();
            this.cGame1 = new System.Windows.Forms.RadioButton();
            this.cGame1000 = new System.Windows.Forms.RadioButton();
            this.cGame10 = new System.Windows.Forms.RadioButton();
            this.panel1 = new System.Windows.Forms.Panel();
            this.cMS48 = new System.Windows.Forms.RadioButton();
            this.cMS32 = new System.Windows.Forms.RadioButton();
            this.cMS40 = new System.Windows.Forms.RadioButton();
            this.cMS56 = new System.Windows.Forms.RadioButton();
            this.cMS64 = new System.Windows.Forms.RadioButton();
            this.cMSMix = new System.Windows.Forms.RadioButton();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(19, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "New(bot0)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(19, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Old (bot1)";
            // 
            // cBot0
            // 
            this.cBot0.Location = new System.Drawing.Point(95, 19);
            this.cBot0.Name = "cBot0";
            this.cBot0.Size = new System.Drawing.Size(797, 20);
            this.cBot0.TabIndex = 2;
            // 
            // cBot1
            // 
            this.cBot1.Location = new System.Drawing.Point(95, 49);
            this.cBot1.Name = "cBot1";
            this.cBot1.Size = new System.Drawing.Size(797, 20);
            this.cBot1.TabIndex = 3;
            // 
            // cResults
            // 
            this.cResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cResults.Font = new System.Drawing.Font("Courier New", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cResults.Location = new System.Drawing.Point(0, 79);
            this.cResults.Multiline = true;
            this.cResults.Name = "cResults";
            this.cResults.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.cResults.Size = new System.Drawing.Size(1205, 333);
            this.cResults.TabIndex = 4;
            this.cResults.WordWrap = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(8, 71);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "Play";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cBot0);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.cBot1);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(1205, 79);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Bots";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.panel2);
            this.groupBox2.Controls.Add(this.panel1);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.groupBox2.Location = new System.Drawing.Point(0, 412);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(1205, 101);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Options";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.cGame100);
            this.panel2.Controls.Add(this.cGame1);
            this.panel2.Controls.Add(this.cGame1000);
            this.panel2.Controls.Add(this.cGame10);
            this.panel2.Location = new System.Drawing.Point(65, 41);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(324, 23);
            this.panel2.TabIndex = 13;
            // 
            // cGame100
            // 
            this.cGame100.AutoSize = true;
            this.cGame100.Location = new System.Drawing.Point(123, 3);
            this.cGame100.Name = "cGame100";
            this.cGame100.Size = new System.Drawing.Size(43, 17);
            this.cGame100.TabIndex = 11;
            this.cGame100.Text = "100";
            this.cGame100.UseVisualStyleBackColor = true;
            // 
            // cGame1
            // 
            this.cGame1.AutoSize = true;
            this.cGame1.Checked = true;
            this.cGame1.Location = new System.Drawing.Point(3, 3);
            this.cGame1.Name = "cGame1";
            this.cGame1.Size = new System.Drawing.Size(31, 17);
            this.cGame1.TabIndex = 11;
            this.cGame1.TabStop = true;
            this.cGame1.Text = "1";
            this.cGame1.UseVisualStyleBackColor = true;
            // 
            // cGame1000
            // 
            this.cGame1000.AutoSize = true;
            this.cGame1000.Location = new System.Drawing.Point(192, 3);
            this.cGame1000.Name = "cGame1000";
            this.cGame1000.Size = new System.Drawing.Size(49, 17);
            this.cGame1000.TabIndex = 11;
            this.cGame1000.Text = "1000";
            this.cGame1000.UseVisualStyleBackColor = true;
            // 
            // cGame10
            // 
            this.cGame10.AutoSize = true;
            this.cGame10.Location = new System.Drawing.Point(60, 3);
            this.cGame10.Name = "cGame10";
            this.cGame10.Size = new System.Drawing.Size(37, 17);
            this.cGame10.TabIndex = 11;
            this.cGame10.Text = "10";
            this.cGame10.UseVisualStyleBackColor = true;
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.cMS48);
            this.panel1.Controls.Add(this.cMS32);
            this.panel1.Controls.Add(this.cMS40);
            this.panel1.Controls.Add(this.cMS56);
            this.panel1.Controls.Add(this.cMS64);
            this.panel1.Controls.Add(this.cMSMix);
            this.panel1.Location = new System.Drawing.Point(65, 12);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(275, 23);
            this.panel1.TabIndex = 12;
            // 
            // cMS48
            // 
            this.cMS48.AutoSize = true;
            this.cMS48.Location = new System.Drawing.Point(89, 3);
            this.cMS48.Name = "cMS48";
            this.cMS48.Size = new System.Drawing.Size(37, 17);
            this.cMS48.TabIndex = 9;
            this.cMS48.Text = "48";
            this.cMS48.UseVisualStyleBackColor = true;
            // 
            // cMS32
            // 
            this.cMS32.AutoSize = true;
            this.cMS32.Checked = true;
            this.cMS32.Location = new System.Drawing.Point(3, 3);
            this.cMS32.Name = "cMS32";
            this.cMS32.Size = new System.Drawing.Size(37, 17);
            this.cMS32.TabIndex = 9;
            this.cMS32.TabStop = true;
            this.cMS32.Text = "32";
            this.cMS32.UseVisualStyleBackColor = true;
            // 
            // cMS40
            // 
            this.cMS40.AutoSize = true;
            this.cMS40.Location = new System.Drawing.Point(46, 3);
            this.cMS40.Name = "cMS40";
            this.cMS40.Size = new System.Drawing.Size(37, 17);
            this.cMS40.TabIndex = 9;
            this.cMS40.Text = "40";
            this.cMS40.UseVisualStyleBackColor = true;
            // 
            // cMS56
            // 
            this.cMS56.AutoSize = true;
            this.cMS56.Location = new System.Drawing.Point(132, 3);
            this.cMS56.Name = "cMS56";
            this.cMS56.Size = new System.Drawing.Size(37, 17);
            this.cMS56.TabIndex = 9;
            this.cMS56.Text = "56";
            this.cMS56.UseVisualStyleBackColor = true;
            // 
            // cMS64
            // 
            this.cMS64.AutoSize = true;
            this.cMS64.Location = new System.Drawing.Point(175, 3);
            this.cMS64.Name = "cMS64";
            this.cMS64.Size = new System.Drawing.Size(37, 17);
            this.cMS64.TabIndex = 9;
            this.cMS64.Text = "64";
            this.cMS64.UseVisualStyleBackColor = true;
            // 
            // cMSMix
            // 
            this.cMSMix.AutoSize = true;
            this.cMSMix.Location = new System.Drawing.Point(218, 3);
            this.cMSMix.Name = "cMSMix";
            this.cMSMix.Size = new System.Drawing.Size(53, 17);
            this.cMSMix.TabIndex = 9;
            this.cMSMix.Text = "Mixed";
            this.cMSMix.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 46);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(40, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Games";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 17);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Map Size";
            // 
            // HaliteMatchManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1205, 513);
            this.Controls.Add(this.cResults);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "HaliteMatchManager";
            this.Text = "Halite 3 Match";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox cBot0;
        private System.Windows.Forms.TextBox cBot1;
        private System.Windows.Forms.TextBox cResults;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton cMSMix;
        private System.Windows.Forms.RadioButton cMS64;
        private System.Windows.Forms.RadioButton cMS56;
        private System.Windows.Forms.RadioButton cMS48;
        private System.Windows.Forms.RadioButton cMS40;
        private System.Windows.Forms.RadioButton cMS32;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton cGame100;
        private System.Windows.Forms.RadioButton cGame1;
        private System.Windows.Forms.RadioButton cGame1000;
        private System.Windows.Forms.RadioButton cGame10;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label4;
    }
}

