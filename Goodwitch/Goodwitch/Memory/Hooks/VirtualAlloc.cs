using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using Goodwitch.Utils;

namespace Goodwitch.Memory.Hooks
{
    internal class VirtualAlloc : IDisposable
    {
        internal static VirtualAlloc Instance { private get; set; } = new VirtualAlloc();
        private static Detour pDetour;


        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate IntPtr VirtualAllocDelegate(IntPtr lpAddress, IntPtr dwSize, uint flAllocationType, uint flProtect);

        private static VirtualAllocDelegate HookedInstance;
        private static VirtualAllocDelegate OriginalInstance;

        public void Dispose()
        {
            pDetour.Uninstall();
            Instance.Dispose();
        }

        internal bool Initialised
        {
            get
            {
                return pDetour != null;
            }
        }

        internal static void InstallHook()
        {
            HookedInstance = HookedVirtualAlloc;
            OriginalInstance = Marshal.GetDelegateForFunctionPointer<VirtualAllocDelegate>(MemUtils.GetFunctionAddress("Kernel32.dll", "VirtualAlloc"));

            pDetour = new Detour(MemUtils.GetFunctionAddress("Kernel32.dll", "VirtualAlloc"), Marshal.GetFunctionPointerForDelegate(HookedInstance));

            pDetour.Install();

            Console.WriteLine("Installed VirtualAlloc Hook");
        }

        private static unsafe IntPtr HookedVirtualAlloc(IntPtr lpAddress, IntPtr dwSize, uint flAllocationType, uint flProtect)
        {
            Console.WriteLine
            ($"\nVirtualAlloc is being called." +
            $"\nlpAddress: 0x{lpAddress.ToString("X")}" +
            $"\ndwSize: {dwSize}" +
            $"\nflAllocationType: {flAllocationType}" +
            $"\nflProtect: {flProtect}");

            return pDetour.CallOriginal<IntPtr>(OriginalInstance, new object[] { lpAddress, dwSize, flAllocationType, flProtect });
        }
    }
}
