using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CheatDetectionDemo.Modules;
using System.IO;
using System.Diagnostics;

namespace CheatDetectionDemo
{
    public partial class Demo : Form
    {
        private static string TestResourcesDirectory = Directory.GetCurrentDirectory() + @"\Test Resources";

        public Demo()
        {
            InitializeComponent();
        }

        private void AssemblyLoadButton_Click(object sender, EventArgs e)
        {
            Loader LDR = new Loader("SpaceInvaders");

            LDR.LoadAndCallMethod($@"{TestResourcesDirectory}\SLTest.dll", "Init");
        }

        private void RunGoodwitchAndGameButton_Click(object sender, EventArgs e)
        {
            Process.Start($@"{TestResourcesDirectory}\Goodwitch and game\Goodwitch Server\Goodwitch.Server.exe");
            Process.Start($@"{TestResourcesDirectory}\Goodwitch and game\Game\SpaceInvaders.exe");
        }

        private void LoadDetectedAssemblyButton_Click(object sender, EventArgs e)
        {
            Loader LDR = new Loader("SpaceInvaders");

            LDR.LoadAndCallMethod($@"{TestResourcesDirectory}\CheatAssembly.dll", "Init");
        }

        private void LaunchDetectedCheatButton_Click(object sender, EventArgs e)
        {
            Process.Start($@"{TestResourcesDirectory}\CheatProcess.exe");
        }

        private void LaunchDebuggerButton_Click(object sender, EventArgs e)
        {
            Process.Start($@"{TestResourcesDirectory}\ReClassEx.exe");
        }
    }
}
