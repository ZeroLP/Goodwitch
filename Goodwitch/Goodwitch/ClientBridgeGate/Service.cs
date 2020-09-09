using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Windows.Forms;
using System.Threading;
using Goodwitch.CommonUtils;

namespace Goodwitch.ClientBridgeGate
{
    class Service
    {
        private static TcpClient ServerSocket = new TcpClient();
        internal static NetworkStream NStream;

        private static Tuple<bool, string> ConnectToServer()
        {
            try
            {
                ServerSocket = new TcpClient();
                var Res = ServerSocket.BeginConnect("127.0.0.1", 8971, null, null);

                var success = Res.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(3));

                if (!success)
                {
                    return new Tuple<bool, string>(false, $"Exception: Failed to connect to the server.");
                }

                NStream = ServerSocket.GetStream();

                return new Tuple<bool, string>(true, "");
            }
            catch (Exception Ex)
            {
                if (Ex.GetType() == typeof(SocketException))
                    return new Tuple<bool, string>(false, $"Exception: Failed to connect to the server.");
                else return new Tuple<bool, string>(false, $"Exception: {Ex.Message}");
            }
        }

        internal static Tuple<bool, string> AuthenticateGoodwitchInstance()
        {
            var ConnectionToAuth = ConnectToServer();

            if (ConnectionToAuth.Item1)
            {
                var CheckedGFP = CheckGoodwitchFingerprint();

                if (CheckedGFP.Item1 == true)
                {
                    CommonUtils.Time.Tick.OnTick += GoodwitchHeartbeatCallback;
                    return CheckedGFP;
                }
                else return CheckedGFP;
            }
            else return ConnectionToAuth;
        }

        private static Tuple<bool, string> CheckGoodwitchFingerprint()
        {
            var GFP = FingerprintService.GenerateFingerprint();
            var GlobalKey = "c60b189c3ac4722971ddfb00a12524c9";

            var SentPacket = ClientTelemetry.SendPacket($"CheckGoodwitchFingerprint: {GFP}{GlobalKey}");

            if (SentPacket.Item1 == true)
            {
                var RecievedPacket = ClientTelemetry.ReadPacket();

                if (RecievedPacket.Item1 == true)
                {
                    if (RecievedPacket.Item2 == "GoodwitchFingerprintRegistered" || RecievedPacket.Item2 == "ValidGoodwitchFingerprint")
                        return new Tuple<bool, string>(true, "");
                    else if (RecievedPacket.Item2 == "InvalidGlobalKey") return new Tuple<bool, string>(false, "InvalidOperation");
                    else return new Tuple<bool, string>(false, "UnknownServerException" + RecievedPacket.Item2);
                }
                else return RecievedPacket;
            }
            else return SentPacket;
        }

        private static void GoodwitchHeartbeatCallback()
        {
            if (!Extension.IsConnected(ServerSocket.Client))
            {
                var mBoxThread = new Thread(() =>
                {
                    var mBoxRes = MessageBox.Show("Heartbeat roundtrip failed.", "Goodwitch Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                    if (mBoxRes == DialogResult.OK)
                        Environment.Exit(0);
                });

                mBoxThread.Start();
                ThreadUtils.SuspendProcess();
            }
        }

        private class Extension
        {
            internal static bool IsConnected(Socket client)
            {
                // This is how you can determine whether a socket is still connected.
                bool blockingState = client.Blocking;

                try
                {
                    byte[] tmp = new byte[1];

                    client.Blocking = false;
                    client.Send(tmp, 0, 0);

                    return true;
                }
                catch (SocketException e)
                {
                    // 10035 == WSAEWOULDBLOCK
                    if (e.NativeErrorCode.Equals(10035))
                        return true;
                    else return false;
                }
                finally { client.Blocking = blockingState; }
            }
        }
    }
}
