using System;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.CrestronThread;

namespace ProgramLifespan
{
    public class ControlSystem : CrestronControlSystem
    {
        private Thread _longRunning;
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
                ErrorLog.Error("Error in the constructor: {0}", e.Message);
            }
        }

        public override void InitializeSystem()
        {
            try
            {
                _longRunning = new Thread(WorkerThread, null);
            }
            catch (Exception e)
            {
                ErrorLog.Error("Error in InitializeSystem: {0}", e.Message);
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
    }
}