using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheatDetectionDemo.Enums
{
    [Flags]
    internal enum FreeType
    {
        Decommit = 0x4000,
        Release = 0x8000,
    }
}
