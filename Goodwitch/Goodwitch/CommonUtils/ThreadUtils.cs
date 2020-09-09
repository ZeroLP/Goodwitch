using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using Goodwitch.Enums;

namespace Goodwitch.CommonUtils
{
    public class ThreadUtils
    {
        public static void SuspendProcess()
        {
            var process = Process.GetCurrentProcess();

            foreach (ProcessThread pT in process.Threads)
            {
                IntPtr pOpenThread = NativeImport.OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);

                if (pOpenThread == IntPtr.Zero)
                {
                    continue;
                }

                NativeImport.SuspendThread(pOpenThread);

                NativeImport.CloseHandle(pOpenThread);
            }
        }

        public static void ResumeProcess()
        {
            var process = Process.GetCurrentProcess();

            foreach (ProcessThread pT in process.Threads)
            {
                IntPtr pOpenThread = NativeImport.OpenThread(ThreadAccess.SUSPEND_RESUME, false, (uint)pT.Id);

                if (pOpenThread == IntPtr.Zero)
                {
                    continue;
                }

                var suspendCount = 0;
                do
                {
                    suspendCount = NativeImport.ResumeThread(pOpenThread);
                } while (suspendCount > 0);

                NativeImport.CloseHandle(pOpenThread);
            }
        }
    }
}
