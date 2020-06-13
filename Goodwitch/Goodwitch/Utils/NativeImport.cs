using System;
using System.Runtime.InteropServices;

namespace Goodwitch.Utils
{
    internal class NativeImport
    {
        [DllImport("kernel32.dll")]
        internal static extern bool IsDebuggerPresent();

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool CheckRemoteDebuggerPresent(IntPtr hProcess, [MarshalAs(UnmanagedType.Bool)] ref bool isDebuggerPresent);

        [DllImport("kernel32.dll")]
        internal static extern bool VirtualProtect(IntPtr lpAddress, UIntPtr dwSize, Enums.VirtualProtectionType flNewProtect, out Enums.VirtualProtectionType lpflOldProtect);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr GetModuleHandle(string lpModuleName);

        [DllImport("kernel32", CharSet = CharSet.Ansi, ExactSpelling = true, SetLastError = true)]
        internal static extern IntPtr GetProcAddress(IntPtr hModule, string procName);

        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Ansi)]
        internal static extern IntPtr LoadLibrary([MarshalAs(UnmanagedType.LPStr)]string lpFileName);
    }
}
