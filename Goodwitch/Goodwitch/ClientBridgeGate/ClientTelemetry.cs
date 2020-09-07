using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Goodwitch.ClientBridgeGate
{
    class ClientTelemetry
    {
        internal static Tuple<bool, string> SendPacket(string Message)
        {
            try
            {
                var EncryptedPacket = EncryptPacket(Message); //XOR rawstring

                BinaryWriter writer = new BinaryWriter(Service.NStream);
                writer.Write(EncryptedPacket);

                return new Tuple<bool, string>(true, "");
            }
            catch (Exception Ex)
            {
                return new Tuple<bool, string>(false, $"Exception: {Ex.Message}");
            }
        }

        internal static Tuple<bool, string> ReadPacket()
        {
            try
            {
                BinaryReader reader = new BinaryReader(Service.NStream);
                var DecryptedPacket = DecryptPacket(reader.ReadString());

                return new Tuple<bool, string>(true, DecryptedPacket);
            }
            catch (Exception Ex)
            {
                return new Tuple<bool, string>(false, $"Exception: {Ex.Message}");
            }
        }

        private static string EncryptPacket(string Packet)
        {
            //XOR -> Bitshift

            string XORKey = "bbcd563cf93676e1312a34fc8347c993";

            char[] XORString = new char[Packet.Length];

            //XOR Operation
            for (int i = 0; i < Packet.Length; i++)
            {
                XORString[i] = (char)(Packet[i] ^ XORKey[i % XORKey.Length]);
            }

            char[] BitshiftedXORString = new char[XORString.Length];

            //Bitshifting XOR string
            for (int i = 0; i < XORString.Length; i++)
            {
                BitshiftedXORString[i] = (char)(XORString[i] << 4);
            }

            return new string(BitshiftedXORString);
        }

        private static string DecryptPacket(string Packet)
        {
            string XORKey = "bbcd563cf93676e1312a34fc8347c993";

            char[] ReverseBitshiftedString = new char[Packet.Length];
            for (int i = 0; i < Packet.Length; i++)
            {
                ReverseBitshiftedString[i] = (char)(Packet[i] >> 4);
            }

            char[] ReverseXORString = new char[ReverseBitshiftedString.Length];
            for (int i = 0; i < ReverseBitshiftedString.Length; i++)
            {
                ReverseXORString[i] = (char)(ReverseBitshiftedString[i] ^ XORKey[i % XORKey.Length]);
            }

            return new string(ReverseXORString);
        }
    }
}
