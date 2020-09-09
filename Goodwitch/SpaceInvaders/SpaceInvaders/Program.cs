using System;
using System.Windows.Forms;

namespace SpaceInvaders
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            var loadGoodwitchAC = Goodwitch.Main.InitialiseGoodwitch();

            if (loadGoodwitchAC.Item1)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
            else
            {
                MessageBox.Show(loadGoodwitchAC.Item2, "Goodwitch Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(0);
            }
        }
    }
}
