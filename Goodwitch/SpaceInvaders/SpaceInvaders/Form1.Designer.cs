namespace SpaceInvaders
{
    partial class Form1
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
            this.spaceInvadersLabel = new System.Windows.Forms.Label();
            this.playLabel = new System.Windows.Forms.Label();
            this.scoreTableLabel = new System.Windows.Forms.Label();
            this.mainMenuLabel = new System.Windows.Forms.Label();
            this.scoreTextLabel = new System.Windows.Forms.Label();
            this.actualScoreLabel = new System.Windows.Forms.Label();
            this.actualLevelLabel = new System.Windows.Forms.Label();
            this.levelTextLabel = new System.Windows.Forms.Label();
            this.scoreTableUC1 = new SpaceInvaders.ScoreTableUC();
            this.pauseLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // spaceInvadersLabel
            // 
            this.spaceInvadersLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.spaceInvadersLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.spaceInvadersLabel.ForeColor = System.Drawing.Color.White;
            this.spaceInvadersLabel.Location = new System.Drawing.Point(12, 166);
            this.spaceInvadersLabel.Name = "spaceInvadersLabel";
            this.spaceInvadersLabel.Size = new System.Drawing.Size(1075, 56);
            this.spaceInvadersLabel.TabIndex = 0;
            this.spaceInvadersLabel.Text = "Space Invaders";
            this.spaceInvadersLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.spaceInvadersLabel.UseCompatibleTextRendering = true;
            // 
            // playLabel
            // 
            this.playLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.playLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.playLabel.ForeColor = System.Drawing.Color.White;
            this.playLabel.Location = new System.Drawing.Point(12, 219);
            this.playLabel.Name = "playLabel";
            this.playLabel.Size = new System.Drawing.Size(1075, 56);
            this.playLabel.TabIndex = 2;
            this.playLabel.Text = "click here to play!";
            this.playLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.playLabel.UseCompatibleTextRendering = true;
            this.playLabel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.playLabel_MouseClick);
            this.playLabel.MouseLeave += new System.EventHandler(this.clickableLabel_MouseLeave);
            this.playLabel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.clickableLabel_MouseMove);
            // 
            // scoreTableLabel
            // 
            this.scoreTableLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scoreTableLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.scoreTableLabel.ForeColor = System.Drawing.Color.White;
            this.scoreTableLabel.Location = new System.Drawing.Point(12, 360);
            this.scoreTableLabel.Name = "scoreTableLabel";
            this.scoreTableLabel.Size = new System.Drawing.Size(1075, 56);
            this.scoreTableLabel.TabIndex = 3;
            this.scoreTableLabel.Text = "*score advance table*";
            this.scoreTableLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.scoreTableLabel.UseCompatibleTextRendering = true;
            // 
            // mainMenuLabel
            // 
            this.mainMenuLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.mainMenuLabel.Enabled = false;
            this.mainMenuLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mainMenuLabel.ForeColor = System.Drawing.Color.White;
            this.mainMenuLabel.Location = new System.Drawing.Point(12, 427);
            this.mainMenuLabel.Name = "mainMenuLabel";
            this.mainMenuLabel.Size = new System.Drawing.Size(1075, 56);
            this.mainMenuLabel.TabIndex = 5;
            this.mainMenuLabel.Text = "Main menu";
            this.mainMenuLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.mainMenuLabel.UseCompatibleTextRendering = true;
            this.mainMenuLabel.Visible = false;
            this.mainMenuLabel.MouseClick += new System.Windows.Forms.MouseEventHandler(this.mainMenuLabel_MouseClick);
            this.mainMenuLabel.MouseLeave += new System.EventHandler(this.clickableLabel_MouseLeave);
            this.mainMenuLabel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.clickableLabel_MouseMove);
            // 
            // scoreTextLabel
            // 
            this.scoreTextLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.scoreTextLabel.ForeColor = System.Drawing.Color.White;
            this.scoreTextLabel.Location = new System.Drawing.Point(12, 9);
            this.scoreTextLabel.Name = "scoreTextLabel";
            this.scoreTextLabel.Size = new System.Drawing.Size(165, 56);
            this.scoreTextLabel.TabIndex = 6;
            this.scoreTextLabel.Text = "Score: ";
            this.scoreTextLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.scoreTextLabel.UseCompatibleTextRendering = true;
            this.scoreTextLabel.Visible = false;
            // 
            // actualScoreLabel
            // 
            this.actualScoreLabel.AutoSize = true;
            this.actualScoreLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.actualScoreLabel.ForeColor = System.Drawing.Color.White;
            this.actualScoreLabel.Location = new System.Drawing.Point(174, 7);
            this.actualScoreLabel.Name = "actualScoreLabel";
            this.actualScoreLabel.Size = new System.Drawing.Size(34, 50);
            this.actualScoreLabel.TabIndex = 7;
            this.actualScoreLabel.Text = "0";
            this.actualScoreLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.actualScoreLabel.UseCompatibleTextRendering = true;
            this.actualScoreLabel.Visible = false;
            // 
            // actualLevelLabel
            // 
            this.actualLevelLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.actualLevelLabel.AutoSize = true;
            this.actualLevelLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.actualLevelLabel.ForeColor = System.Drawing.Color.White;
            this.actualLevelLabel.Location = new System.Drawing.Point(1027, 7);
            this.actualLevelLabel.Name = "actualLevelLabel";
            this.actualLevelLabel.Size = new System.Drawing.Size(34, 50);
            this.actualLevelLabel.TabIndex = 9;
            this.actualLevelLabel.Text = "1";
            this.actualLevelLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.actualLevelLabel.UseCompatibleTextRendering = true;
            this.actualLevelLabel.Visible = false;
            // 
            // levelTextLabel
            // 
            this.levelTextLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.levelTextLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.levelTextLabel.ForeColor = System.Drawing.Color.White;
            this.levelTextLabel.Location = new System.Drawing.Point(865, 9);
            this.levelTextLabel.Name = "levelTextLabel";
            this.levelTextLabel.Size = new System.Drawing.Size(167, 56);
            this.levelTextLabel.TabIndex = 8;
            this.levelTextLabel.Text = "Level:";
            this.levelTextLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.levelTextLabel.UseCompatibleTextRendering = true;
            this.levelTextLabel.Visible = false;
            // 
            // scoreTableUC1
            // 
            this.scoreTableUC1.BackColor = System.Drawing.Color.Black;
            this.scoreTableUC1.ForeColor = System.Drawing.SystemColors.Control;
            this.scoreTableUC1.Location = new System.Drawing.Point(335, 436);
            this.scoreTableUC1.Name = "scoreTableUC1";
            this.scoreTableUC1.Size = new System.Drawing.Size(521, 231);
            this.scoreTableUC1.TabIndex = 4;
            // 
            // pauseLabel
            // 
            this.pauseLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pauseLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pauseLabel.ForeColor = System.Drawing.Color.White;
            this.pauseLabel.Location = new System.Drawing.Point(12, 292);
            this.pauseLabel.Name = "pauseLabel";
            this.pauseLabel.Size = new System.Drawing.Size(1075, 56);
            this.pauseLabel.TabIndex = 10;
            this.pauseLabel.Text = "PAUSED";
            this.pauseLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.pauseLabel.UseCompatibleTextRendering = true;
            this.pauseLabel.Visible = false;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1099, 738);
            this.Controls.Add(this.mainMenuLabel);
            this.Controls.Add(this.scoreTableUC1);
            this.Controls.Add(this.pauseLabel);
            this.Controls.Add(this.actualLevelLabel);
            this.Controls.Add(this.levelTextLabel);
            this.Controls.Add(this.actualScoreLabel);
            this.Controls.Add(this.scoreTextLabel);
            this.Controls.Add(this.scoreTableLabel);
            this.Controls.Add(this.playLabel);
            this.Controls.Add(this.spaceInvadersLabel);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(1115, 777);
            this.Name = "Form1";
            this.Text = "Space Invaders";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label spaceInvadersLabel;
        private System.Windows.Forms.Label playLabel;
        private System.Windows.Forms.Label scoreTableLabel;
        private ScoreTableUC scoreTableUC1;
        private System.Windows.Forms.Label mainMenuLabel;
        private System.Windows.Forms.Label scoreTextLabel;
        internal System.Windows.Forms.Label actualScoreLabel;
        internal System.Windows.Forms.Label actualLevelLabel;
        private System.Windows.Forms.Label levelTextLabel;
        internal System.Windows.Forms.Label pauseLabel;
    }
}

