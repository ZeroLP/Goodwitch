using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Goodwitch.CommonUtils
{
    class BanMessageDisplayer
    {
        internal static void DisplayBanMessage()
        {
            MessageBox.Show("A critical error has occured and the process must be terminated." +
                            "\n\nThis is usually due to the result of incompatible third party software being installed on your machine." +
                            "\n\nPlease disable any overlay, streaming, or experience 'enhancement' software. Additionally, you may need to whitelist this application with" +
                            " your installed security software." +
                            "\n\nif you need further assistance, please visit the support website for help.", "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            throw new Exception();
        }
    }
}
