using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Goodwitch.Enums;

namespace Goodwitch.CommonUtils
{
    class FlagRaiserService
    {
        internal static void RaiseFlag(string dFlag, string flagInformation)
        {
            Reporter.SendReport(dFlag, flagInformation);
        }
    }
}
