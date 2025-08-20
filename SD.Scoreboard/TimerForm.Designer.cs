using System.ComponentModel;

namespace SD.Scoreboard;

partial class TimerForm
    {
        private System.ComponentModel.IContainer components = null;
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

        private void InitializeComponent()
        {
            this.lblHomeScore = new System.Windows.Forms.Label();
            this.lblAwayScore = new System.Windows.Forms.Label();
            this.lblTime = new System.Windows.Forms.Label();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblTotal = new System.Windows.Forms.Label();    
            this.SuspendLayout();
            // 
            // lblHomeScore
            // 
            this.lblHomeScore.Font = new System.Drawing.Font("Segoe UI", 80F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblHomeScore.Location = new System.Drawing.Point(12, 27);
            this.lblHomeScore.Name = "lblHomeScore";
            this.lblHomeScore.Size = new System.Drawing.Size(200, 150);
            this.lblHomeScore.TabIndex = 0;
            this.lblHomeScore.Text = "0";
            this.lblHomeScore.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblAwayScore
            // 
            this.lblAwayScore.Font = new System.Drawing.Font("Segoe UI", 80F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblAwayScore.Location = new System.Drawing.Point(476, 27);
            this.lblAwayScore.Name = "lblAwayScore";
            this.lblAwayScore.Size = new System.Drawing.Size(200, 150);
            this.lblAwayScore.TabIndex = 1;
            this.lblAwayScore.Text = "0";
            this.lblAwayScore.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTime
            // 
            this.lblTime.Font = new System.Drawing.Font("Segoe UI", 80F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblTime.Location = new System.Drawing.Point(194, 27);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(300, 150);
            this.lblTime.TabIndex = 2;
            this.lblTime.Text = "0:00";
            this.lblTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblStatus
            // 
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblStatus.Location = new System.Drawing.Point(12, 190);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(664, 40);
            this.lblStatus.TabIndex = 3;
            this.lblStatus.Text = "Aktiv periode";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblTotal
            // 
            this.lblTotal.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblTotal.Location = new System.Drawing.Point(12, 250);
            this.lblTotal.Name = "lblTotal";
            this.lblTotal.Size = new System.Drawing.Size(664, 40);
            this.lblTotal.TabIndex = 4;
            this.lblTotal.Text = "0 - 0 - 0";
            this.lblTotal.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // TimerForm
            // 
            this.ClientSize = new System.Drawing.Size(688, 240);
            this.Controls.Add(this.lblTotal);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.lblTime);
            this.Controls.Add(this.lblAwayScore);
            this.Controls.Add(this.lblHomeScore);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "TimerForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SD.Scoreboard";
            this.ResumeLayout(false);
        }
    }