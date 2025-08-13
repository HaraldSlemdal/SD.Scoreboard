using System.ComponentModel;

namespace SD.Scoreboard;

partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.NumericUpDown nudActiveMinutes;
        private System.Windows.Forms.NumericUpDown nudActiveSeconds;
        private System.Windows.Forms.NumericUpDown nudPauseMinutes;
        private System.Windows.Forms.NumericUpDown nudPauseSeconds;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.Label lblActive;
        private System.Windows.Forms.Label lblPause;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.nudActiveMinutes = new System.Windows.Forms.NumericUpDown();
            this.nudActiveSeconds = new System.Windows.Forms.NumericUpDown();
            this.nudPauseMinutes = new System.Windows.Forms.NumericUpDown();
            this.nudPauseSeconds = new System.Windows.Forms.NumericUpDown();
            this.btnStart = new System.Windows.Forms.Button();
            this.lblActive = new System.Windows.Forms.Label();
            this.lblPause = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nudActiveMinutes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudActiveSeconds)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPauseMinutes)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPauseSeconds)).BeginInit();
            this.SuspendLayout();
            // 
            // nudActiveMinutes
            // 
            this.nudActiveMinutes.Location = new System.Drawing.Point(30, 40);
            this.nudActiveMinutes.Maximum = new decimal(new int[] { 59, 0, 0, 0 });
            this.nudActiveMinutes.Name = "nudActiveMinutes";
            this.nudActiveMinutes.Size = new System.Drawing.Size(60, 22);
            this.nudActiveMinutes.TabIndex = 0;
            // 
            // nudActiveSeconds
            // 
            this.nudActiveSeconds.Location = new System.Drawing.Point(100, 40);
            this.nudActiveSeconds.Maximum = new decimal(new int[] { 59, 0, 0, 0 });
            this.nudActiveSeconds.Name = "nudActiveSeconds";
            this.nudActiveSeconds.Size = new System.Drawing.Size(60, 22);
            this.nudActiveSeconds.TabIndex = 1;
            // 
            // nudPauseMinutes
            // 
            this.nudPauseMinutes.Location = new System.Drawing.Point(30, 110);
            this.nudPauseMinutes.Maximum = new decimal(new int[] { 59, 0, 0, 0 });
            this.nudPauseMinutes.Name = "nudPauseMinutes";
            this.nudPauseMinutes.Size = new System.Drawing.Size(60, 22);
            this.nudPauseMinutes.TabIndex = 2;
            // 
            // nudPauseSeconds
            // 
            this.nudPauseSeconds.Location = new System.Drawing.Point(100, 110);
            this.nudPauseSeconds.Maximum = new decimal(new int[] { 59, 0, 0, 0 });
            this.nudPauseSeconds.Name = "nudPauseSeconds";
            this.nudPauseSeconds.Size = new System.Drawing.Size(60, 22);
            this.nudPauseSeconds.TabIndex = 3;
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(30, 160);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(130, 40);
            this.btnStart.TabIndex = 4;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // lblActive
            // 
            this.lblActive.AutoSize = true;
            this.lblActive.Location = new System.Drawing.Point(27, 15);
            this.lblActive.Name = "lblActive";
            this.lblActive.Size = new System.Drawing.Size(137, 17);
            this.lblActive.TabIndex = 5;
            this.lblActive.Text = "Aktiv periode (mm:ss)";
            // 
            // lblPause
            // 
            this.lblPause.AutoSize = true;
            this.lblPause.Location = new System.Drawing.Point(27, 85);
            this.lblPause.Name = "lblPause";
            this.lblPause.Size = new System.Drawing.Size(118, 17);
            this.lblPause.TabIndex = 6;
            this.lblPause.Text = "Pause (mm:ss)";
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(200, 220);
            this.Controls.Add(this.lblPause);
            this.Controls.Add(this.lblActive);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.nudPauseSeconds);
            this.Controls.Add(this.nudPauseMinutes);
            this.Controls.Add(this.nudActiveSeconds);
            this.Controls.Add(this.nudActiveMinutes);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SD.Scoreboard - Oppsett";
            ((System.ComponentModel.ISupportInitialize)(this.nudActiveMinutes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudActiveSeconds)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPauseMinutes)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nudPauseSeconds)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
