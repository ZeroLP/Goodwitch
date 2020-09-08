using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Goodwitch.CommonUtils;

namespace Goodwitch.Modules
{
    /// <summary>
    /// Second Lowest Hierarchical Module(2) -  External memory modification and external related operation detection.
    /// </summary>
    internal class GlyndaModule : ModuleBase
    {
        private IntPtr wMSHookInstance = IntPtr.Zero;
        private IntPtr wKBHookInstance = IntPtr.Zero;

        private NativeImport.WindowsHookAdditionals.HookProc winMSHookCallbackDelegate = null;
        private NativeImport.WindowsHookAdditionals.HookProc winKBHookCallbackDelegate = null;

        internal override void StartModule()
        {
            //InstallHooks();
            base.StartModule();
        }

        private void InstallHooks()
        {
            winMSHookCallbackDelegate = new NativeImport.WindowsHookAdditionals.HookProc(HookedMSWindowsCallback);
            winKBHookCallbackDelegate = new NativeImport.WindowsHookAdditionals.HookProc(HookedKBWindowsCallback);

            var hinstance = NativeImport.LoadLibrary("User32");

            wMSHookInstance = NativeImport.SetWindowsHookEx(NativeImport.WindowsHookAdditionals.HookType.WH_MOUSE_LL, winMSHookCallbackDelegate, hinstance, 0);
            wKBHookInstance = NativeImport.SetWindowsHookEx(NativeImport.WindowsHookAdditionals.HookType.WH_KEYBOARD_LL, winKBHookCallbackDelegate, hinstance, 0);

            Logger.Log("Installed input hooks");
        }

        private IntPtr HookedMSWindowsCallback(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code >= 0 && NativeImport.GetForegroundWindow() == Process.GetCurrentProcess().MainWindowHandle)
            {
                NativeImport.NativeStructs.MSLLHOOKSTRUCT mouseStruct = new NativeImport.NativeStructs.MSLLHOOKSTRUCT();
                Marshal.PtrToStructure(lParam, mouseStruct);

                if ((mouseStruct.flags & 1) != 0) //LLMHF_INJECTED flag
                {
                    Console.WriteLine($"Blocked non generic mouse input call - Input raised LLMHF_INJECTED flag");
                    return (IntPtr)1;
                }

                return NativeImport.CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
            }
            else
                return NativeImport.CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
        }

        private IntPtr HookedKBWindowsCallback(int code, IntPtr wParam, IntPtr lParam)
        {
            if (code >= 0 && NativeImport.GetForegroundWindow() == Process.GetCurrentProcess().MainWindowHandle)
            {
                NativeImport.NativeStructs.KBDLLHOOKSTRUCT keyboardStruct = new NativeImport.NativeStructs.KBDLLHOOKSTRUCT();
                Marshal.PtrToStructure(lParam, keyboardStruct);

                if ((keyboardStruct.flags & 0x10) != 0) //LLKHF_INJECTED flag
                {
                    Console.WriteLine($"Blocked non generic keyboard input call - Input raised LLKHF_INJECTED");
                    return (IntPtr)1;
                }

                return NativeImport.CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
            }
            else
                return NativeImport.CallNextHookEx(IntPtr.Zero, code, wParam, lParam);
        }
    }
}
