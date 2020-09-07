using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace Goodwitch.Server.ServerBridgeGate
{
    internal class FingerprintAuthService
    {
        private static string FINGERPRINT_DIRECTORY = Directory.GetCurrentDirectory() + @"\FingerprintDatabase.json";

        internal static void AuthenticateFingerprint(NetworkStream NStream, string Fingerprint)
        {
            string trueFingerPrint = Fingerprint.Replace("CheckGoodwitchFingerprint: ", "");
            string uniqueFingerPrint = trueFingerPrint.Replace("63ebcba8d37ddf1551d9a500133b4e91", "");

            Fingerprint = trueFingerPrint + uniqueFingerPrint;

            var fpDB = OpenFingerprintDatabase();

            if (fpDB["Fingerprints"].Contains(new Dictionary<string, object>() { { Fingerprint, "" } }))
            {
                ServerTelemetry.SendPacket(NStream, "ValidGoodwitchFingerprint");
            }
            else
            {
                RegisterFingerprint(fpDB, Fingerprint);
                ServerTelemetry.SendPacket(NStream, "GoodwitchFingerprintRegistered");
            }
        }

        private static void RegisterFingerprint(Dictionary<string, List<Dictionary<string, object>>> fingerPrintDB, string fingerPrint)
        {
            fingerPrintDB["Fingerprints"].Add(new Dictionary<string, object> { { fingerPrint, "" } });

            var serialisedDB = JsonConvert.SerializeObject(fingerPrintDB, Formatting.Indented);

            File.WriteAllText(FINGERPRINT_DIRECTORY, serialisedDB);
        }

        private static Dictionary<string, List<Dictionary<string, object>>> OpenFingerprintDatabase()
        {
            if (File.Exists(FINGERPRINT_DIRECTORY))
            {
                string jsonDBString = File.ReadAllText(FINGERPRINT_DIRECTORY);

                return JsonConvert.DeserializeObject<Dictionary<string, List<Dictionary<string, object>>>>(jsonDBString);
            }
            else
            {
                Dictionary<string, List<Dictionary<string, object>>> jsonDB = new Dictionary<string, List<Dictionary<string, object>>>();
                jsonDB.Add("Fingerprints", new List<Dictionary<string, object>>());

                var serialisedDB = JsonConvert.SerializeObject(jsonDB, Formatting.Indented);

                File.WriteAllText(FINGERPRINT_DIRECTORY, serialisedDB);

                return jsonDB;
            }
        }
    }
}
