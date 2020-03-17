using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Goodwitch.Modules;
using System.Diagnostics;

namespace Goodwitch
{
    internal class Main
    {
        internal static void Test()
        {
            OscarModule.EnumerateLoadedModules();
            OscarModule.EnumerateReferencedAssemblies();

            Console.ReadLine();
        }
    }
}
