using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Goodwitch.Modules
{
    internal class ModuleBase
    {
        internal virtual void StartModule() { }

        internal class Container
        {
            private List<ModuleBase> InitialisedModules;

            internal Container()
            {
                InitialisedModules = new List<ModuleBase>()
                {
                    new KalidahsModule(),
                    new GlyndaModule(),
                    new OscarModule()
                };
            }

            internal void StartAllModules()
            {
                foreach (var module in InitialisedModules)
                {
                    module.StartModule();
                }
            }
        }
    }
}
