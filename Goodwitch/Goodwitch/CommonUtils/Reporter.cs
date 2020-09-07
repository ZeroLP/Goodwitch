using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Goodwitch.ClientBridgeGate;

namespace Goodwitch.CommonUtils
{
    class Reporter
    {
        internal static void SendReport(string reportType, string reportContent)
        {
            ClientTelemetry.SendPacket($"GWReportType: {reportType} | {reportContent}");
        }
    }
}
