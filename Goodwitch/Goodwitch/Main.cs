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
    public class Main
    {
        public static Tuple<bool, string> InitialiseGoodwitch()
        {
            Tuple<bool, string> retVal = null;

            Task.Run(async () => {

                 await Task.Run(() => {

                     var authRes = ClientBridgeGate.Service.AuthenticateGoodwitchInstance();

                     if (!authRes.Item1)
                     {
                         retVal = authRes;
                     }
                     else
                         Logger.CreateLoggerInstance();
                         new ModuleBase.Container().StartAllModules();
                 });

             }).GetAwaiter().GetResult();

            return retVal ?? new Tuple<bool, string>(true, "");
        }
    }
}
