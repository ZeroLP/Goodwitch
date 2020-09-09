using System.Windows.Forms;

namespace SpaceInvaders
{
    public partial class ScoreTableUC : UserControl
    {
        public ScoreTableUC()
        {
            InitializeComponent();

            foreach (Control c in Controls)
            {
                if (c is Label)
                    Utils.ChangeFontToSpaceInvaders(c);
            }
        }
    }
}
