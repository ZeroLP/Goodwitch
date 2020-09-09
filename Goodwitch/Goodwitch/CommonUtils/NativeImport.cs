using System;
using System.Runtime.InteropServices;

namespace Goodwitch.CommonUtils
{
    internal class NativeImport
    {
        internal class WindowsHookAdditionals
        {
            internal delegate IntPtr HookProc(int code, IntPtr wParam, IntPtr lParam);

            internal enum HookType : int
            {
                WH_JOURNALRECORD = 0,
                WH_JOURNALPLAYBACK = 1,
                WH_KEYBOARD = 2,
                WH_GETMESSAGE = 3,
                WH_CALLWNDPROC = 4,
                WH_CBT = 5,
                WH_SYSMSGFILTER = 6,
                WH_MOUSE = 7,
                WH_HARDWARE = 8,
                WH_DEBUG = 9,
                WH_SHELL = 10,
                WH_FOREGROUNDIDLE = 11,
                WH_CALLWNDPROCRET = 12,
                WH_KEYBOARD_LL = 13,
                WH_MOUSE_LL = 14
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern IntPtr SetWindowsHookEx(WindowsHookAdditionals.HookType hookType, WindowsHookAdditionals.HookProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll")]
        internal static extern bool UnhookWindowsHookEx(IntPtr hInstance);

        [DllImport("user32.dll")]
        internal static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        internal static extern IntPtr OpenThread(Enums.ThreadAccess dwDesiredAccess, bool bInheritHandle, uint dwThreadId);

        [DllImport("kernel32.dll")]
        internal static extern uint SuspendThread(IntPtr hThread);
        [DllImport("kernel32.dll")]
        internal static extern int ResumeThread(IntPtr hThread);
        [DllImport("kernel32", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern bool CloseHandle(IntPtr handle);

        internal class ConsolePropertyModifiers
        {
            internal const int SW_HIDE = 0;
            internal const int SW_SHOW = 5;
            internal const int MY_CODE_PAGE = 437;
            internal const uint GENERIC_WRITE = 0x40000000;
            internal const uint GENERIC_READ = 0x80000000;
            internal const uint FILE_SHARE_WRITE = 0x2;
            internal const uint OPEN_EXISTING = 0x3;

            internal const int WM_NCLBUTTONDOWN = 0xA1;
            internal const int HTCAPTION = 0x2;
        }

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        internal static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        internal static extern bool FreeConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern IntPtr CreateFile(
        string lpFileName,
        uint dwDesiredAccess,
        uint dwShareMode,
        uint lpSecurityAttributes,
        uint dwCreationDisposition,
        uint dwFlagsAndAttributes,
        uint hTemplateFile);

        [DllImport("user32.dll")]
        internal static extern IntPtr GetForegroundWindow();

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll")]
        internal static extern bool SetStdHandle(int nStdHandle, IntPtr hHandle);

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

        internal class NativeStructs
        {
            [StructLayout(LayoutKind.Sequential)]
            public class MSLLHOOKSTRUCT
            {
                public POINT pt;
                public int mouseData;
                public int flags;
                public int time;
                public UIntPtr dwExtraInfo;
            }

            [StructLayout(LayoutKind.Sequential)]
            public class KBDLLHOOKSTRUCT
            {
                public uint vkCode;
                public uint scanCode;
                public int flags;
                public uint time;
                public UIntPtr dwExtraInfo;
            }

            [Flags]
            public enum KBDLLHOOKSTRUCTFlags : uint
            {
                LLKHF_EXTENDED = 0x01,
                LLKHF_INJECTED = 0x10,
                LLKHF_ALTDOWN = 0x20,
                LLKHF_UP = 0x80,
            }

            [StructLayout(LayoutKind.Sequential)]
            public class CWPSTRUCT
            {
                public IntPtr lparam;
                public IntPtr wparam;
                public int message;
                public IntPtr hwnd;
            }

            [StructLayout(LayoutKind.Sequential)]
            public class CWPRETSTRUCT
            {
                public IntPtr lResult;
                public IntPtr lParam;
                public IntPtr wParam;
                public uint message;
                public IntPtr hWnd;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct POINT
            {
                public int X;
                public int Y;

                public POINT(int x, int y)
                {
                    this.X = x;
                    this.Y = y;
                }

                public static implicit operator System.Drawing.Point(POINT p)
                {
                    return new System.Drawing.Point(p.X, p.Y);
                }

                public static implicit operator POINT(System.Drawing.Point p)
                {
                    return new POINT(p.X, p.Y);
                }
            }
        }
    }
}
