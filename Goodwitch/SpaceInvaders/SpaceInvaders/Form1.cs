using System;
using System.Drawing;
using System.Media;
using System.Windows.Forms;
using SpaceInvaders.Properties;

namespace SpaceInvaders
{
    public partial class Form1 : Form
    {
        private Game g;
        SoundPlayer sp = new SoundPlayer();
        public Form1()
        {
            InitializeComponent();
            Icon = Resources.app;

            foreach (Control c in Controls)
                if (c is Label)
                    Utils.ChangeFontToSpaceInvaders(c);

            
            sp.Stream = Resources.spaceinvaders1;
            sp.PlayLooping();     
        }

        private void playLabel_MouseClick(object sender, MouseEventArgs e)
        {
            sp.Stop();
            SetGameMode();
            g = new Game(this);
            DisableResizing();
        }

        private void SetGameMode()
        {
            SetMenuVisibility(false);
            SetScoreAndLevelLabelVisibility(true);
        }

        private void DisableResizing()
        {
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
        }

        private void EnableResizing()
        {
            FormBorderStyle = FormBorderStyle.Sizable;
            MaximizeBox = true;
        }

        private void SetScoreAndLevelLabelVisibility(bool dState)
        {
            scoreTextLabel.Visible = dState;
            actualScoreLabel.Visible = dState;
            actualScoreLabel.Text = "0";
            levelTextLabel.Visible = dState;
            actualLevelLabel.Visible = dState;
            actualLevelLabel.Text = "1";
        }
        

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            g?.CheatKey(keyData);
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            scoreTableUC1.Location = new Point((int) (Constants.ScoreTableLocation.X * Width) - Constants.ScoreTableLocationShift, (int)(Constants.ScoreTableLocation.Y * Height));
        }

        private void SetMenuVisibility(bool dState)
        {
            playLabel.Enabled = dState;
            playLabel.Visible = dState;
            spaceInvadersLabel.Enabled = dState;
            spaceInvadersLabel.Visible = dState;
            scoreTableLabel.Enabled = dState;
            scoreTableLabel.Visible = dState;
            scoreTableLabel.Text = "*score advance table*";
            scoreTableUC1.Enabled = dState;
            scoreTableUC1.Visible = dState;
        }

        public void ShowGameOver()
        {
            Utils.Play(Resources.gameover);
            SetMenuVisibility(false);
            scoreTableLabel.Text = "game over!";
            scoreTableLabel.Enabled = true;
            scoreTableLabel.Visible = true;
            mainMenuLabel.Enabled = true;
            mainMenuLabel.Visible = true;
        }

        private void mainMenuLabel_MouseClick(object sender, MouseEventArgs e)
        {
            EnableResizing();
            sp.PlayLooping();
            SetMenuVisibility(true);
            mainMenuLabel.Enabled = false;
            mainMenuLabel.Visible = false;
            SetScoreAndLevelLabelVisibility(false);
            Refresh();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            g?.End();
        }

        private void clickableLabel_MouseMove(object sender, MouseEventArgs e)
        {
            Label s = (Label)sender;
            s.ForeColor = Color.LawnGreen;
            Cursor.Current = Cursors.Hand;
        }

        private void clickableLabel_MouseLeave(object sender, EventArgs e)
        {
            Label s = (Label)sender;
            s.ForeColor = Color.White;
        }
    }
}
