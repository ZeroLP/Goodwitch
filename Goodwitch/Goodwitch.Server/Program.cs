using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Goodwitch.Server.ServerBridgeGate;
using Newtonsoft.Json;

namespace Goodwitch.Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Service.StartServerAndListen();

            Console.Read();
        }
    }
}
