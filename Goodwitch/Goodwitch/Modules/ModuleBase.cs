using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Goodwitch.CommonUtils;

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
                try
                {
                    InitialisedModules = new List<ModuleBase>()
                    {
                       new KalidahsModule(),
                       new GlyndaModule(),
                       new OscarModule()
                    };
                }
                catch(Exception Ex)
                {
                    Logger.Log($"Exception occured while initialising module base: {Ex.Message}", Logger.LogSeverity.Danger);
                }
            }

            internal void StartAllModules()
            {
                try
                {
                    foreach (var module in InitialisedModules)
                    {
                        Logger.Log($"Starting module: {module.GetType().Name}...");
                        module.StartModule();
                    }
                }
                catch(Exception Ex)
                {
                    Logger.Log($"Exception occured while starting modules: {Ex.Message}", Logger.LogSeverity.Danger);
                }
            }
        }
    }
}
