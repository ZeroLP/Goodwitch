using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using Goodwitch.CommonUtils;

namespace Goodwitch.Modules
{
    /// <summary>
    /// Lowest Hierarchical Module(3). Anti-Debugging and Process Detection.
    /// </summary>
    internal class KalidahsModule : ModuleBase
    {
        private static List<string> DebuggerList = new List<string>() { "OLLYDBG", "cheatengine-x86_64", "ReClassEx", "ReClassEx64", "x64dbg", "x32dbg", "IDA Pro", "Immunity Debugger", "Ghidra", "de4dot", "de4dot-x64", "ida", "ida64", "dotPeek64", "dotPeek32", "Fiddler", "dnSpy", "dnSpy-x86", "dnSpy.Console"};
        private static List<string> DebuggerWindowHandleList = new List<string>() { "Cheat Engine", "IDA", "IDA -", "JetBrains dotPeek", "OllyDbg", "x64dbg", "x32dbg", "Progress Telerik Fiddler", "dnSpy" };

        private static List<string> CheatList = new List<string>() { "CheatProcess" };
        private static List<string> CheatWindowHandleList = new List<string>() { "CheatProcess" };
        private static List<string> CheatProcessByteSignature = new List<string>() { };

        internal override void StartModule()
        {
            CommonUtils.Time.Tick.OnTick += DetectDebuggers;
            CommonUtils.Time.Tick.OnTick += DetectKnownCheatApplication;
            CommonUtils.Time.Tick.OnTick += DetectKnownCheatApplicationSignature;

            base.StartModule();
        }

        private void DetectDebuggers()
        {
            if(IsDebuggerAttached() || IsRemoteDebuggerAttached()
              || IsDebuggerRunningPrcName() || IsDebuggerRunningHWND())
            {
                BanMessageDisplayer.DisplayBanMessage();
            }
        }

        private void DetectKnownCheatApplication()
        {
            if (IsCheatRunningPrcName() || IsCheatRunningHWND())
            {
                BanMessageDisplayer.DisplayBanMessage();
            }
        }

        private void DetectKnownCheatApplicationSignature()
        {
            try
            {
                foreach (var proc in Process.GetProcesses())
                {
                    string currASMBytes = Convert.ToBase64String(File.ReadAllBytes(proc.MainModule.FileName));

                    foreach (var detectedSignature in CheatProcessByteSignature)
                    {
                        if (currASMBytes.Contains(detectedSignature))
                        {
                            BanMessageDisplayer.DisplayBanMessage();
                        }
                    }
                }
            }
            catch (Exception ex) { }
        }

        private bool IsCheatRunningPrcName()
        {
#if DEBUG
            return false;
#else
            return CheatList.Intersect(ProcessManager.EnumerateAllProcesses()).Any();
#endif
        }

        private bool IsCheatRunningHWND()
        {
#if DEBUG
            return false;
#else
            bool CheckFlag = false;

            foreach (string HWND in ProcessManager.EnumerateWindow())
            {
                CheckFlag |= (CheatWindowHandleList.Any(HWND.Contains) || CheatWindowHandleList.ConvertAll(d => d.ToLower()).Any(HWND.Contains));
            }

            return CheckFlag;
#endif
        }

        private bool IsDebuggerAttached()
        {
#if DEBUG
            return false;
#else
            return NativeImport.IsDebuggerPresent();
#endif
        }

        private bool IsRemoteDebuggerAttached()
        {
#if DEBUG
            return false;
#else
            bool isDebuggerPresent = false;

            bool bApiRet = NativeImport.CheckRemoteDebuggerPresent(Process.GetCurrentProcess().Handle, ref isDebuggerPresent);

            if (bApiRet && isDebuggerPresent)
            {
                return true;
            }

            return false;
#endif
        }

        private bool IsDebuggerRunningPrcName()
        {
#if DEBUG
            return false;
#else
            return DebuggerList.Intersect(ProcessManager.EnumerateAllProcesses()).Any();
#endif
        }

        private bool IsDebuggerRunningHWND()
        {
#if DEBUG
            return false;
#else
            bool CheckFlag = false;

            foreach (string HWND in ProcessManager.EnumerateWindow())
            {
                CheckFlag |= (DebuggerWindowHandleList.Any(HWND.Contains) || DebuggerWindowHandleList.ConvertAll(d => d.ToLower()).Any(HWND.Contains));
            }

            return CheckFlag;
#endif
        }
    }
}
