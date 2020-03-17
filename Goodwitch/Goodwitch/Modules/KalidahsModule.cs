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
    internal class KalidahsModule
    {
        private static List<string> DebuggerList = new List<string>() { "OLLYDBG", "cheatengine-x86_64", "ReClassEx", "ReClassEx64", "x64dbg", "x32dbg", "IDA Pro", "Immunity Debugger", "Ghidra", "de4dot", "de4dot-x64", "ida", "ida64", "dotPeek64", "dotPeek32", "Fiddler", "dnSpy", "dnSpy-x86", "dnSpy.Console"};
        private static List<string> DebuggerWindowHandleList = new List<string>() { "Cheat Engine", "IDA", "IDA -", "JetBrains dotPeek", "OllyDbg", "x64dbg", "x32dbg", "Progress Telerik Fiddler", "dnSpy" };
        private static List<string> CheatList = new List<string>() { "" };

        
        internal static bool IsDebuggerAttached()
        {
            return Utils.NativeImport.IsDebuggerPresent();
        }

        internal static bool IsRemoteDebuggerAttached()
        {
            bool isDebuggerPresent = false;

            bool bApiRet = Utils.NativeImport.CheckRemoteDebuggerPresent(Process.GetCurrentProcess().Handle, ref isDebuggerPresent);

            if (bApiRet && isDebuggerPresent)
            {
                return true;
            }

            return false;
        }

        internal static bool IsDebuggerRunningPrcName()
        {
            return DebuggerList.Intersect(Utils.ProcessManager.EnumerateAllProcesses()).Any();
        }

        internal static bool IsDebuggerRunningHWND()
        {
            bool CheckFlag = false;

            foreach (string HWND in Utils.ProcessManager.EnumerateWindow())
            {
                CheckFlag |= (DebuggerWindowHandleList.Any(HWND.Contains) || DebuggerWindowHandleList.ConvertAll(d => d.ToLower()).Any(HWND.Contains));
            }

            return CheckFlag;
        }
    }
}
