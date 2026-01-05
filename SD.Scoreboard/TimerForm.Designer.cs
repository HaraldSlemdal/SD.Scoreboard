using System.ComponentModel;

namespace SD.Scoreboard;

partial class TimerForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblHomeScoreDescription;
        private System.Windows.Forms.Label lblAwayScoreDescription;
        private System.Windows.Forms.Label lblHomeScore;
        private System.Windows.Forms.Label lblAwayScore;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblTotal;

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
            lblHomeScoreDescription = new System.Windows.Forms.Label();
            lblAwayScoreDescription = new System.Windows.Forms.Label();
            lblHomeScore = new System.Windows.Forms.Label();
            lblAwayScore = new System.Windows.Forms.Label();
            lblTime = new System.Windows.Forms.Label();
            lblStatus = new System.Windows.Forms.Label();
            lblTotal = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // lblHomeScoreDescription
            // 
            lblHomeScoreDescription.Font = new System.Drawing.Font("Segoe UI", 20F);
            lblHomeScoreDescription.Location = new System.Drawing.Point(12, 0);
            lblHomeScoreDescription.Name = "lblHomeScoreDescription";
            lblHomeScoreDescription.Size = new System.Drawing.Size(200, 40);
            lblHomeScoreDescription.TabIndex = 5;
            lblHomeScoreDescription.Text = "Home";
            lblHomeScoreDescription.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblAwayScoreDescription
            // 
            lblAwayScoreDescription.Font = new System.Drawing.Font("Segoe UI", 20F);
            lblAwayScoreDescription.Location = new System.Drawing.Point(476, 0);
            lblAwayScoreDescription.Name = "lblAwayScoreDescription";
            lblAwayScoreDescription.Size = new System.Drawing.Size(200, 40);
            lblAwayScoreDescription.TabIndex = 6;
            lblAwayScoreDescription.Text = "Away";
            lblAwayScoreDescription.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblHomeScore
            // 
            lblHomeScore.Font = new System.Drawing.Font("Segoe UI", 80F);
            lblHomeScore.Location = new System.Drawing.Point(12, 27);
            lblHomeScore.Name = "lblHomeScore";
            lblHomeScore.Size = new System.Drawing.Size(200, 150);
            lblHomeScore.TabIndex = 0;
            lblHomeScore.Text = "0";
            lblHomeScore.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblAwayScore
            // 
            lblAwayScore.Font = new System.Drawing.Font("Segoe UI", 80F);
            lblAwayScore.Location = new System.Drawing.Point(476, 27);
            lblAwayScore.Name = "lblAwayScore";
            lblAwayScore.Size = new System.Drawing.Size(200, 150);
            lblAwayScore.TabIndex = 1;
            lblAwayScore.Text = "0";
            lblAwayScore.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTime
            // 
            lblTime.Font = new System.Drawing.Font("Segoe UI", 80F);
            lblTime.Location = new System.Drawing.Point(194, 27);
            lblTime.Name = "lblTime";
            lblTime.Size = new System.Drawing.Size(300, 150);
            lblTime.TabIndex = 2;
            lblTime.Text = "0:00";
            lblTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblStatus
            // 
            lblStatus.Font = new System.Drawing.Font("Segoe UI", 18F);
            lblStatus.Location = new System.Drawing.Point(12, 190);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new System.Drawing.Size(664, 40);
            lblStatus.TabIndex = 3;
            lblStatus.Text = "Aktiv periode";
            lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTotal
            // 
            lblTotal.Font = new System.Drawing.Font("Segoe UI", 12F);
            lblTotal.Location = new System.Drawing.Point(12, 250);
            lblTotal.Name = "lblTotal";
            lblTotal.Size = new System.Drawing.Size(664, 40);
            lblTotal.TabIndex = 4;
            lblTotal.Text = "0 - 0 - 0";
            lblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TimerForm
            // 
            ClientSize = new System.Drawing.Size(688, 308);
            Controls.Add(lblAwayScoreDescription);
            Controls.Add(lblHomeScoreDescription);
            Controls.Add(lblTotal);
            Controls.Add(lblStatus);
            Controls.Add(lblTime);
            Controls.Add(lblAwayScore);
            Controls.Add(lblHomeScore);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "SD.Scoreboard";
            FormClosing += TimerForm_FormClosing;
            ResumeLayout(false);
        }
    }