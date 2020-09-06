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
    internal class CreateThread : IDisposable
    {
        internal static CreateThread Instance { private get; set; } = new CreateThread();
        private static Detour pDetour;


        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        internal delegate uint CreateThreadDelegate(UIntPtr lpThreadAttributes, uint dwStackSize, System.Threading.ThreadStart lpStartAddress, UIntPtr lpParameter, uint dwCreationFlags, ref uint lpThreadId);

        private static CreateThreadDelegate HookedInstance;
        private static CreateThreadDelegate OriginalInstance;

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
            HookedInstance = HookedCreateThread;
            OriginalInstance = Marshal.GetDelegateForFunctionPointer<CreateThreadDelegate>(MemUtils.GetFunctionAddress("Kernel32.dll", "CreateThread"));

            pDetour = new Detour(MemUtils.GetFunctionAddress("Kernel32.dll", "CreateThread"), Marshal.GetFunctionPointerForDelegate(HookedInstance));

            pDetour.Install();

            Console.WriteLine("Installed CreateThread Hook");
        }

        private unsafe static uint HookedCreateThread(UIntPtr lpThreadAttributes, uint dwStackSize, System.Threading.ThreadStart lpStartAddress, UIntPtr lpParameter, uint dwCreationFlags, ref uint lpThreadId)
        {
            Console.WriteLine
            ($"\nCreateThread is being called." +
            $"\nlpThreadAttributes: {lpThreadAttributes}" +
            $"\ndwStackSize: {dwStackSize}" +
            $"\nlpStartAddress: {lpStartAddress}" +
            $"\nlpParameter: {lpParameter}" +
            $"\ndwCreationFlags: {dwCreationFlags}" +
            $"\nlpThreadId: {lpThreadId}\n");

            return pDetour.CallOriginal<uint>(OriginalInstance, new object[] { lpThreadAttributes, dwStackSize, lpStartAddress, lpParameter, dwCreationFlags, lpThreadId });
        }
    }
}
