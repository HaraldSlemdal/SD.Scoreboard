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
        private System.Windows.Forms.CheckBox chkThreeTeams;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            nudActiveMinutes = new System.Windows.Forms.NumericUpDown();
            nudActiveSeconds = new System.Windows.Forms.NumericUpDown();
            nudPauseMinutes = new System.Windows.Forms.NumericUpDown();
            nudPauseSeconds = new System.Windows.Forms.NumericUpDown();
            btnStart = new System.Windows.Forms.Button();
            lblActive = new System.Windows.Forms.Label();
            lblPause = new System.Windows.Forms.Label();
            chkThreeTeams = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)nudActiveMinutes).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudActiveSeconds).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudPauseMinutes).BeginInit();
            ((System.ComponentModel.ISupportInitialize)nudPauseSeconds).BeginInit();
            SuspendLayout();
            // 
            // nudActiveMinutes
            // 
            nudActiveMinutes.Location = new System.Drawing.Point(30, 40);
            nudActiveMinutes.Maximum = new decimal(new int[] { 59, 0, 0, 0 });
            nudActiveMinutes.Name = "nudActiveMinutes";
            nudActiveMinutes.Size = new System.Drawing.Size(60, 23);
            nudActiveMinutes.TabIndex = 0;
            // 
            // nudActiveSeconds
            // 
            nudActiveSeconds.Location = new System.Drawing.Point(100, 40);
            nudActiveSeconds.Maximum = new decimal(new int[] { 59, 0, 0, 0 });
            nudActiveSeconds.Name = "nudActiveSeconds";
            nudActiveSeconds.Size = new System.Drawing.Size(60, 23);
            nudActiveSeconds.TabIndex = 1;
            // 
            // nudPauseMinutes
            // 
            nudPauseMinutes.Location = new System.Drawing.Point(30, 110);
            nudPauseMinutes.Maximum = new decimal(new int[] { 59, 0, 0, 0 });
            nudPauseMinutes.Name = "nudPauseMinutes";
            nudPauseMinutes.Size = new System.Drawing.Size(60, 23);
            nudPauseMinutes.TabIndex = 2;
            // 
            // nudPauseSeconds
            // 
            nudPauseSeconds.Location = new System.Drawing.Point(100, 110);
            nudPauseSeconds.Maximum = new decimal(new int[] { 59, 0, 0, 0 });
            nudPauseSeconds.Name = "nudPauseSeconds";
            nudPauseSeconds.Size = new System.Drawing.Size(60, 23);
            nudPauseSeconds.TabIndex = 3;
            // 
            // chkThreeTeams
            // 
            chkThreeTeams.AutoSize = true;
            chkThreeTeams.Location = new System.Drawing.Point(30, 140);
            chkThreeTeams.Name = "chkThreeTeams";
            chkThreeTeams.Size = new System.Drawing.Size(120, 19);
            chkThreeTeams.TabIndex = 7;
            chkThreeTeams.Text = "Rullering (3 lag)";
            chkThreeTeams.UseVisualStyleBackColor = true;
            // 
            // btnStart
            // 
            btnStart.Location = new System.Drawing.Point(30, 170);
            btnStart.Name = "btnStart";
            btnStart.Size = new System.Drawing.Size(130, 40);
            btnStart.TabIndex = 4;
            btnStart.Text = "Start";
            btnStart.UseVisualStyleBackColor = true;
            btnStart.Click += btnStart_Click;
            // 
            // lblActive
            // 
            lblActive.AutoSize = true;
            lblActive.Location = new System.Drawing.Point(27, 15);
            lblActive.Name = "lblActive";
            lblActive.Size = new System.Drawing.Size(123, 15);
            lblActive.TabIndex = 5;
            lblActive.Text = "Aktiv periode (mm:ss)";
            // 
            // lblPause
            // 
            lblPause.AutoSize = true;
            lblPause.Location = new System.Drawing.Point(27, 85);
            lblPause.Name = "lblPause";
            lblPause.Size = new System.Drawing.Size(84, 15);
            lblPause.TabIndex = 6;
            lblPause.Text = "Pause (mm:ss)";
            // 
            // MainForm
            // 
            ClientSize = new System.Drawing.Size(200, 230);
            Controls.Add(lblPause);
            Controls.Add(lblActive);
            Controls.Add(btnStart);
            Controls.Add(nudPauseSeconds);
            Controls.Add(nudPauseMinutes);
            Controls.Add(nudActiveSeconds);
            Controls.Add(nudActiveMinutes);
            Controls.Add(chkThreeTeams);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "SD.Scoreboard - Oppsett";
            FormClosing += MainForm_FormClosing;
            ((System.ComponentModel.ISupportInitialize)nudActiveMinutes).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudActiveSeconds).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudPauseMinutes).EndInit();
            ((System.ComponentModel.ISupportInitialize)nudPauseSeconds).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
