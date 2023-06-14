using System;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharp.CrestronSockets;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.CrestronThread;

namespace BroadcastPackets
{
    public class ControlSystem : CrestronControlSystem
    {
        private Thread _heartbeat;
        private bool _running;

        public ControlSystem() : base()
        {
            try
            {
                Thread.MaxNumberOfUserThreads = 20;

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
                _heartbeat = new Thread(HeartbeatThread, null);
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
                _running = false;
            }
        }

        object HeartbeatThread(object userObj)
        {
            var barker = new UDPServer();
            barker.EnableUDPServer("255.255.255.255", 55055);
            barker.ReceiveDataAsync(HeartbeatDataAsync);

            var msg = Encoding.UTF8.GetBytes("AHOY SAILOR!");

            _running = true;
            while (_running)
            {
                barker.SendDataAsync(msg, msg.Length, HeartbeatSendAsync);

                Thread.Sleep(1000);

                if (!_running)
                    break;

                Thread.Sleep(1000);

                if (!_running)
                    break;

                Thread.Sleep(1000);
            }

            barker.DisableUDPServer();

            return null;
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