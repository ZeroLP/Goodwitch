using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Goodwitch.Server.ReportHandling
{
    class ReportHandlerBase
    {
        private static string logFileDirectory = Directory.GetCurrentDirectory() + @"\ReportLog.txt";

        internal static void WriteToLogFile(string reportedContent)
        {
            if (File.Exists(logFileDirectory))
            {
                List<string> contentLines = File.ReadAllLines(logFileDirectory).ToList();

                contentLines.Add($"\n{reportedContent}");

                File.WriteAllLines(logFileDirectory, contentLines.ToArray());
            }
            else
            {
                File.CreateText(logFileDirectory).Close();
                File.WriteAllText(logFileDirectory, reportedContent);
            }
        }
    }
}
