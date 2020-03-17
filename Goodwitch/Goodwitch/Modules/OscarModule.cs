using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;

namespace Goodwitch.Modules
{
    /// <summary>
    /// Highest Hierarchical Module(1) - Detecting internal memory modification and memory related detection.
    /// </summary>
    internal class OscarModule
    {
        internal static void EnumerateReferencedAssemblies()
        {
            foreach(AssemblyName RefASM in Assembly.GetEntryAssembly().GetReferencedAssemblies())
            {
                Console.WriteLine($"{RefASM.FullName}");
            }
        }

        internal static void EnumerateLoadedModules()
        {
            foreach(ProcessModule PModule in Process.GetCurrentProcess().Modules)
            {
                Console.WriteLine($"{PModule.ModuleName}");
            }
        }
    }
}
