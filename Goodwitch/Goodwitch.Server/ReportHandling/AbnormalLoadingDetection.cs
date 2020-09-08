using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Goodwitch.Server.CommonUtils;

namespace Goodwitch.Server.ReportHandling
{
    class AbnormalLoadingDetection
    {
        internal static void HandleDetection(string flaggedInformation)
        {
            Logger.Log($"Instance was flagged for abnormal loading detection.", Logger.LogSeverity.Warning);
            ReportHandlerBase.WriteToLogFile($"reported information: {flaggedInformation}");
        }
    }
}
