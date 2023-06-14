using System;
using System.Text;
using Crestron.SimplSharp;
using Crestron.SimplSharp.CrestronSockets;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.CrestronThread;

namespace ServerCleanUp
{
    public class ControlSystem : CrestronControlSystem
    {
        private Thread _longRunning;
        private bool _running;
        private TCPServer _server;

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
                _longRunning = new Thread(WorkerThread, null);

                //_server = new TCPServer(50005, 10);
                //_server = new TCPServer(new IPEndPoint(IPAddress.Any, 50005), 1024, EthernetAdapterType.EthernetLANAdapter, 10);
                _server = new TCPServer(new IPEndPoint(IPAddress.Any, 50005), 1024, EthernetAdapterType.EthernetCSAdapter, 10);
                _server.SocketStatusChange += ServerStatusChange;
                _server.WaitForConnectionsAlways(ClientConnectedAsync);

                var adapter = CrestronEthernetHelper.GetAdapterdIdForSpecifiedAdapterType(_server.EthernetAdapterToBindTo);
                var ipAddress = CrestronEthernetHelper.GetEthernetParameter(CrestronEthernetHelper.ETHERNET_PARAMETER_TO_GET.GET_CURRENT_IP_ADDRESS, adapter);

                CrestronConsole.PrintLine("Server listening on {0}:{1}", ipAddress, _server.PortNumber);
            }
            catch (Exception e)
            {
                CrestronConsole.PrintLine("Error in InitializeSystem: {0}", e.Message);
            }
        }

        void ProgramEventHandler(eProgramStatusEventType type)
        {
            switch (type)
            {
                case (eProgramStatusEventType.Paused):
                    CrestronConsole.PrintLine("Program is paused, but what is WorkerThread doing?");
                    break;
                case (eProgramStatusEventType.Resumed):
                    CrestronConsole.PrintLine("Program is resuming, and what about WorkerThread?");
                    break;
                case (eProgramStatusEventType.Stopping):
                    CrestronConsole.PrintLine("Program is stopping, so end WorkerThread.");
                    _running = false;
                    CrestronConsole.PrintLine("Disconnect all clients.");
                    _server.DisconnectAll();
                    break;
            }
        }

        object WorkerThread(object userObj)
        {
            CrestronConsole.PrintLine("WorkerThread has started");

            var toggle = false;

            _running = true;
            while (_running)
            {
                Thread.Sleep(1000);
                CrestronConsole.Print(toggle ? "/" : "\\");

                toggle = !toggle;
            }

            CrestronConsole.PrintLine("WorkerThread has exited");
            return null;
        }

        void ServerStatusChange(TCPServer server, uint clientId, SocketStatus status)
        {
            if (status == SocketStatus.SOCKET_STATUS_CONNECTED)
                CrestronConsole.PrintLine("Client {0} connected to server", clientId);
            else
                CrestronConsole.PrintLine("Client {0} status change: {1}", clientId, status);
        }

        void ClientConnectedAsync(TCPServer server, uint clientId)
        {
            if (clientId > 0)
            {
                CrestronConsole.PrintLine("Client {0} connected on port {1}", clientId, server.PortNumber);
                server.ReceiveDataAsync(clientId, ClientDataReceivedAsync);
            }
            else
            {
                if ((server.State & ServerState.SERVER_NOT_LISTENING) > 0)
                    CrestronConsole.PrintLine("Server is no longer listening!");
                else
                    CrestronConsole.PrintLine("Unable to make connection!");
            }
        }

        void ClientDataReceivedAsync(TCPServer server, uint clientId, int numBytes)
        {
            if (numBytes <= 0)
            {
                CrestronConsole.PrintLine("Client {0} closed connection!", clientId);

                /* if (server.ClientConnected(clientId))
                    server.Disconnect(clientId); */
            }
            else
            {
                var rx = new byte[numBytes];
                Array.Copy(server.GetIncomingDataBufferForSpecificClient(clientId), rx, numBytes);

                var msg = Encoding.UTF8.GetString(rx, 0, numBytes).Trim();
                CrestronConsole.PrintLine("Client {0} sent: {1}", clientId, msg);

                byte[] tx;

                if (msg.ToUpper().StartsWith("BYE"))
                {
                    tx = Encoding.UTF8.GetBytes("Goodbye!\n");
                    server.SendData(clientId, tx, tx.Length);
                    server.Disconnect(clientId);
                }
                else
                {
                    tx = Encoding.UTF8.GetBytes("Got it!\n");
                    server.SendData(clientId, tx, tx.Length);
                    server.ReceiveDataAsync(clientId, ClientDataReceivedAsync);
                }
            }
        }
    }
}