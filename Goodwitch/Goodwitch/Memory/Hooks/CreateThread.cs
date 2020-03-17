using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
using Goodwitch.Utils;

namespace Goodwitch.Memory.Hooks
{
    internal class CreateThread : IDisposable
    {
        private static Detour pDetour;
        internal static CreateThread Instance { get; private set; } = new CreateThread();

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

        }
    }
}
