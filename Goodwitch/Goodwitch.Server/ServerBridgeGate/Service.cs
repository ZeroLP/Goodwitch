using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
using Goodwitch.Server.CommonUtils;

namespace Goodwitch.Server.ServerBridgeGate
{
    internal class Service
    {
        private const string GLOBAL_KEY = "aa4c585baa1d383baf27a944a7896466";

        private static IPAddress LocalAddress = IPAddress.Any;
        private static TcpListener ServerSocket = new TcpListener(LocalAddress, 8971);
        private static TcpClient ClientSocket = default;

        private static List<TcpClient> ConnectedGoodwitchQueue = new List<TcpClient>();

        internal static async void StartServerAndListen()
        {
            try
            {
                await Task.Run(() => Logger.Log($"Starting server socket: {LocalAddress}:8971..."));

                await Task.Run(() => ServerSocket.Start());

                //await Task.Run(() => Objects.Tick.OnTick += GoodwitchHeartBeatCallback);

                await Task.Run(() => Logger.Log($"Started server socket: {LocalAddress}:8971"));

                await Task.Run(() => Logger.Log("Waiting for Goodwitch instances in queue."));

                while (true)
                {
                    ClientSocket = ServerSocket.AcceptTcpClient();
                    ThreadPool.QueueUserWorkItem(HandleGoodwitchConnection, ClientSocket);
                }
            }
            catch (Exception Ex)
            {
                await Task.Run(() => Logger.Log($"Exception while handling Goodwitch connection: " +
                               $"\nException: {Ex.ToString()}" +
                               $"\nSource: {Ex.Source}" +
                               $"\nStackTrace: {Ex.StackTrace}", Logger.LogSeverity.Danger));
            }
        }

        internal static void DisconnectAndReleaseAllGoodwitchInstanceInQueue()
        {
            List<TcpClient> DisconnectedGoodwitchs = new List<TcpClient>();

            if (ConnectedGoodwitchQueue.Count != 0)
            {
                Logger.Log($"Disconnecting all Goodwitch instances in the queue...");

                foreach (var Goodwitch in ConnectedGoodwitchQueue)
                {
                    try
                    {
                        if (!DisconnectedGoodwitchs.Contains(Goodwitch))
                        {
                            Goodwitch.Close();
                            DisconnectedGoodwitchs.Add(Goodwitch);
                            Logger.Log($"Disconnected Goodwitch#: {ConnectedGoodwitchQueue.IndexOf(Goodwitch) + 1}.");
                        }
                    }
                    catch (Exception Ex)
                    {
                        Logger.Log($"Exception occured while disconnecting Goodwitch#: {ConnectedGoodwitchQueue.IndexOf(Goodwitch) + 1}({((IPEndPoint)Goodwitch.Client.RemoteEndPoint).Address.ToString()})" +
                                       $"\nException: {Ex.ToString()}" +
                                       $"\nSource: {Ex.Source}" +
                                       $"\nStackTrace: {Ex.StackTrace}", Logger.LogSeverity.Danger);
                    }
                }

                Logger.Log($"Sucessfully disconnected all Goodwitch instances in queue. Exiting the auth server...");
            }
            else Logger.Log("No Goodwitch instances in queue to be disconnected. Exiting the auth server...");
        }

        private static async void HandleGoodwitchConnection(object obj)
        {
            var Goodwitch = (TcpClient)obj;
            NetworkStream NStream = null;

            await Task.Run(() => ConnectedGoodwitchQueue.Add(Goodwitch));

            await Task.Run(() => Logger.Log($"Goodwitch instance connected: {((IPEndPoint)Goodwitch.Client.RemoteEndPoint).Address.ToString()} | (Goodwitch#: {ConnectedGoodwitchQueue.IndexOf(Goodwitch) + 1} of {ConnectedGoodwitchQueue.Count})"));

            await Task.Run(() => NStream = Goodwitch.GetStream());

            await Task.Run(() =>
            {
                var RecievedPacket = ServerTelemetry.ReadPacket(NStream);
                var CurrentGoodwitchInstanceNum = ConnectedGoodwitchQueue.IndexOf(Goodwitch) + 1;

                if (RecievedPacket.Item1)
                {
                    var UniqueGoodwitchFingerprint = RecievedPacket.Item2.Replace("CheckGoodwitchFingerprint: ", "").Replace(GLOBAL_KEY, "");

                    Logger.Log($"Goodwitch instance#: {CurrentGoodwitchInstanceNum} has requested for fingerprint check.");

                    if (RecievedPacket.Item2.Contains("c60b189c3ac4722971ddfb00a12524c9"))
                    {
                        Logger.Log($"Valid global key for Goodwitch instance#: {CurrentGoodwitchInstanceNum}\n(Goodwitch fingerprint: {UniqueGoodwitchFingerprint})", Logger.LogSeverity.Information);
                        FingerprintAuthService.AuthenticateFingerprint(NStream, RecievedPacket.Item2);
                    }
                    else
                    {
                        Logger.Log($"Invalid global key for Goodwitch instance#: {CurrentGoodwitchInstanceNum}\n(GoodwitchFingerprint: {UniqueGoodwitchFingerprint})", Logger.LogSeverity.Warning);
                        ServerTelemetry.SendPacket(NStream, $"InvalidGlobalKey");
                    }
                }
                else
                {
                    Logger.Log($"Exception occured for Goodwitch instance#: {CurrentGoodwitchInstanceNum} while reading the sent packet: {RecievedPacket.Item2}.", Logger.LogSeverity.Danger);
                    ServerTelemetry.SendPacket(NStream, $"Exception ocurred while reading the sent packet: {RecievedPacket.Item2}");
                }
            });
        }
    }
}
