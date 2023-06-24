using System;
using System.Threading;
using System.Text;
using System.Text.Json;
using Crestron.SimplSharp;
using Crestron.SimplSharp.CrestronSockets;
using Crestron.SimplSharpPro;

namespace BroadcastPackets
{
    public class ControlSystem : CrestronControlSystem
    {
        private Thread _heartbeat;
        private CancellationTokenSource _cts;
        
        private SystemInfo _info;

        public ControlSystem() : base()
        {
            try
            {
                CrestronEnvironment.ProgramStatusEventHandler += ProgramEventHandler;
            }
            catch (Exception e)
            {
                CrestronConsole.PrintLine("Error in the constructor: {0}", e.Message);
            }
        }

        public override void InitializeSystem()
        {
            try
            {
                // Select the LAN network adapter, gather system info
                var adapterID = CrestronEthernetHelper.GetAdapterdIdForSpecifiedAdapterType(EthernetAdapterType.EthernetLANAdapter);
                var name = CrestronEthernetHelper.GetEthernetParameter(CrestronEthernetHelper.ETHERNET_PARAMETER_TO_GET.GET_HOSTNAME, adapterID);
                var ipAddr = CrestronEthernetHelper.GetEthernetParameter(CrestronEthernetHelper.ETHERNET_PARAMETER_TO_GET.GET_CURRENT_IP_ADDRESS, adapterID);
                var desc = CrestronEnvironment.DevicePlatform.ToString();

                _info = new SystemInfo
                {
                    Name = name,
                    Description = desc,
                    HostAddress = ipAddr,
                    Sequence = 0,
                    Clock = 0
                };

                _cts = new CancellationTokenSource();
                _heartbeat = new Thread(HeartbeatThread);
                _heartbeat.Start(_cts);
            }
            catch (Exception e)
            {
                CrestronConsole.PrintLine("Error in InitializeSystem: {0}", e.Message);
            }
        }

        void ProgramEventHandler(eProgramStatusEventType type)
        {
            if (type == eProgramStatusEventType.Stopping)
            {
                _cts.Cancel();

                CrestronConsole.PrintLine("Joining HeartbeatThread...");
                _heartbeat.Join();
                CrestronConsole.PrintLine("All done, should be clean?");

                if (_heartbeat.IsAlive)
                    CrestronConsole.PrintLine("...but HeartbeatThread is still alive?!");
            }
        }

        void HeartbeatThread(object userObj)
        {
            if (userObj == null)
                return;

            // Grab the cancellation token
            var cts = (CancellationTokenSource)userObj;

            // Create a UDP broadcast server
            var barker = new UDPServer();
            barker.EnableUDPServer("255.255.255.255", 55055);
            barker.ReceiveDataAsync(HeartbeatDataAsync);

            CrestronConsole.PrintLine("HeartbeatThread running...");

            while (!cts.IsCancellationRequested)
            {
                // Update system info clock with current tick count
                _info.Clock = CrestronEnvironment.TickCount;

                // Sequence tracks the packet number
                _info.Sequence += 1;

                // Serialize system info and package into a byte array
                var msg = JsonSerializer.SerializeToUtf8Bytes(_info);

                // Broadcast system info to the network
                // barker.SendDataAsync(msg, msg.Length, HeartbeatSendAsync);
                barker.SendData(msg, msg.Length);

                Thread.Sleep(3000);
            }

            CrestronConsole.PrintLine("HeartbeatThread stopped!");

            barker.DisableUDPServer();
        }

        void HeartbeatDataAsync(UDPServer server, int numBytes)
        {
            if (server.DataAvailable)
            {
                var segment = new ArraySegment<byte>(server.IncomingDataBuffer, 0, numBytes);
                var msg = Encoding.UTF8.GetString(segment.Array);

                CrestronConsole.PrintLine("Received: {0}", msg);
            }
        }

        void HeartbeatSendAsync(UDPServer server, int numBytes)
        {
            if (numBytes < 1)
            {
                CrestronConsole.PrintLine("Error sending heartbeat message!");
            }
        }
    }
}