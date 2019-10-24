using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Goodwitch.Modules;

namespace Goodwitch
{
    internal class Main
    {
        internal static void Test()
        {
            if (KalidahsModule.IsDebuggerRunningPrcName() == true) Console.WriteLine("Debugger Is Running!");
            Console.WriteLine("Debugger Is Not Running!");
        }
    }
}
