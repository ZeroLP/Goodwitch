using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheatAssembly
{
    public class Main
    {
        //Required for Native->C# module recognition
        public static void DllMain(IntPtr dllInstance, int reason, IntPtr reserved) { }

        [DllExport("Init")]
        public static void Init()
        {

        }
    }
}
