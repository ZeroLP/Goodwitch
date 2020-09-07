using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using System.IO;

namespace Goodwitch.Server.CommonUtils
{
    internal class Logger
    {
        /// <summary>
        /// Allocates a console instance to the running process
        /// </summary>
        internal static void CreateLoggerInstance()
        {
            NativeImport.AllocConsole();

            var outFile = NativeImport.CreateFile("CONOUT$", NativeImport.ConsolePropertyModifiers.GENERIC_WRITE
                                                            | NativeImport.ConsolePropertyModifiers.GENERIC_READ,
                                                            NativeImport.ConsolePropertyModifiers.FILE_SHARE_WRITE,
                                                            0, NativeImport.ConsolePropertyModifiers.OPEN_EXISTING, /*FILE_ATTRIBUTE_NORMAL*/0, 0);

            var safeHandle = new SafeFileHandle(outFile, true);

            NativeImport.SetStdHandle(-11, outFile);

            FileStream fs = new FileStream(safeHandle, FileAccess.Write);
            StreamWriter writer = new StreamWriter(fs) { AutoFlush = true };

            Console.SetOut(writer);

            if (NativeImport.GetConsoleMode(outFile, out var cMode)) NativeImport.SetConsoleMode(outFile, cMode | 0x0200);
        }

        /// <summary>
        /// Logs the given string to the console
        /// </summary>
        /// <param name="dataToLog"></param>
        /// <param name="severity"></param>
        internal static void Log(string dataToLog, LogSeverity severity = LogSeverity.Neutral)
        {
            var ConsoleColour = Console.ForegroundColor;

            switch (severity)
            {
                case LogSeverity.Neutral:
                    ConsoleColour = ConsoleColor.Cyan;
                    break;
                case LogSeverity.Information:
                    ConsoleColour = ConsoleColor.Green;
                    break;
                case LogSeverity.Warning:
                    ConsoleColour = ConsoleColor.Yellow;
                    break;
                case LogSeverity.Danger:
                    ConsoleColour = ConsoleColor.Red;
                    break;
                default:
                    ConsoleColour = ConsoleColor.White;
                    break;
            }

            Console.ForegroundColor = ConsoleColour;

            if (string.IsNullOrEmpty(dataToLog))
                Console.WriteLine($"[-] StringNullOrEmpty occured while logging output.");
            else
                Console.WriteLine($"[{DateTime.Now.ToString("h:mm:ss tt")}]: {dataToLog}");
        }

        internal enum LogSeverity
        {
            Neutral,
            Information,
            Warning,
            Danger
        }
    }
}
