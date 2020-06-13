using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Goodwitch.Utils;

namespace Goodwitch.Memory
{
    class MemUtils
    {
        internal static IntPtr GetFunctionAddress(string LibraryName, string FunctionName)
        {
            IntPtr HModule = NativeImport.LoadLibrary(LibraryName);
            return NativeImport.GetProcAddress(HModule, FunctionName);
        }
    }
}
