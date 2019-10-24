using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Goodwitch.Utils
{
    internal class ProcessManager
    {
        internal static Process GetProcessString(string ProcessName)
        {
            Process[] processes = Process.GetProcessesByName(ProcessName);
            if (processes.Length == 0)
            {
                //Process Name Could Not Be Found
                return null;
            }

            return Process.GetProcessesByName(ProcessName).FirstOrDefault();
        }

        internal static Process GetProcessByWindowName(string WindowName)
        {
            Process Prcs = null;

            foreach (Process process in Process.GetProcesses())
            {
                try
                {
                    if (process.MainWindowTitle.Equals(WindowName))
                    {
                        Prcs = process;
                        break;
                    }
                }
                catch(InvalidOperationException e) { }
            }

            return Prcs;
        }

        internal static List<string> EnumerateAllProcesses(int Limit = 0)
        {
            List<string> ProcessList = new List<string>();

            if (Limit == 0)
            {
                foreach (Process Prcs in Process.GetProcesses())
                {
                    try
                    {
                        ProcessList.Add(Prcs.ProcessName);
                    }
                    catch(InvalidOperationException e) { }
                }
            }
            else
            {
                foreach (Process Prcs in Process.GetProcesses().Take(Limit))
                {
                    try
                    {
                        ProcessList.Add(Prcs.ProcessName);
                    }
                    catch(InvalidOperationException e) { }
                }
            }

            return ProcessList;
        }


        internal static List<string> EnumerateWindow(int Limit = 0)
        {
            List<string> HWNDStorage = new List<string>();

            if (Limit == 0)
            {
                foreach (Process process in Process.GetProcesses())
                {
                    try
                    {
                        if (process.MainWindowTitle.Length != 0) HWNDStorage.Add(process.MainWindowTitle);
                    }
                    catch(InvalidOperationException e) { }
                }
            }
            else
            {
                foreach (Process process in Process.GetProcesses().Take(Limit))
                {
                    try
                    {
                        if (process.MainWindowTitle.Length != 0) HWNDStorage.Add(process.MainWindowTitle);
                    }
                    catch(InvalidOperationException e) { }
                }
            }

            return HWNDStorage;
        }
    }
}
