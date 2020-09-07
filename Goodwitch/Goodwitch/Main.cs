using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Goodwitch.Modules;
using System.Diagnostics;
using Goodwitch.CommonUtils;

namespace Goodwitch
{
    internal class Main
    {
        public static async void InitialiseGoodwitch()
        {
            await Task.Run(() => Logger.CreateLoggerInstance());

            await Task.Run(() => {

                var authRes = ClientBridgeGate.Service.AuthenticateGoodwitchInstance();

                if (!authRes.Item1)
                {
                    Logger.Log(authRes.Item2, Logger.LogSeverity.Danger);
                    Console.Read();

                    Environment.Exit(0);
                }
            });

            await Task.Run(() => new ModuleBase.Container().StartAllModules());
        }
    }
}
