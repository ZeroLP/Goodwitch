using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Goodwitch.Modules
{
    /// <summary>
    /// Lowest Hierarchical Module(3). Anti-Debugging and Process Detection.
    /// </summary>
    internal class KalidahsModule : ModuleBase
    {
        private static List<string> DebuggerList = new List<string>() { "OLLYDBG", "cheatengine-x86_64", "ReClassEx", "ReClassEx64", "x64dbg", "x32dbg", "IDA Pro", "Immunity Debugger", "Ghidra", "de4dot", "de4dot-x64", "ida", "ida64", "dotPeek64", "dotPeek32", "Fiddler", "dnSpy", "dnSpy-x86", "dnSpy.Console"};
        private static List<string> DebuggerWindowHandleList = new List<string>() { "Cheat Engine", "IDA", "IDA -", "JetBrains dotPeek", "OllyDbg", "x64dbg", "x32dbg", "Progress Telerik Fiddler", "dnSpy" };
        private static List<string> CheatList = new List<string>() { "" };

        internal override void StartModule()
        {
            CommonUtils.Time.Tick.OnTick += DetectDebuggers;

            base.StartModule();
        }

        private void DetectDebuggers()
        {
            if(IsDebuggerAttached() || IsRemoteDebuggerAttached()
              || IsDebuggerRunningPrcName() || IsDebuggerRunningHWND())
            {
                throw new Exception();
            }
        }

        private void DetectKnownCheatApplications()
        {

        }

        private bool IsDebuggerAttached()
        {
#if DEBUG
            return false;
#else
            return Utils.NativeImport.IsDebuggerPresent();
#endif
        }

        private bool IsRemoteDebuggerAttached()
        {
#if DEBUG
            return false;
#else
            bool isDebuggerPresent = false;

            bool bApiRet = Utils.NativeImport.CheckRemoteDebuggerPresent(Process.GetCurrentProcess().Handle, ref isDebuggerPresent);

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
            return DebuggerList.Intersect(Utils.ProcessManager.EnumerateAllProcesses()).Any();
#endif
        }

        private bool IsDebuggerRunningHWND()
        {
#if DEBUG
            return false;
#else
            bool CheckFlag = false;

            foreach (string HWND in Utils.ProcessManager.EnumerateWindow())
            {
                CheckFlag |= (DebuggerWindowHandleList.Any(HWND.Contains) || DebuggerWindowHandleList.ConvertAll(d => d.ToLower()).Any(HWND.Contains));
            }

            return CheckFlag;
#endif
        }
    }
}
