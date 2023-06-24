using System;
using System.Threading.Tasks;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;

namespace ServerTasks
{
    public class ControlSystem : CrestronControlSystem
    {
        public ControlSystem() : base()
        {
            try
            {

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
                Action<object> action = (object obj) =>
                {
                    CrestronConsole.PrintLine("*** Inside the Action for {0}: {1} ***", Task.CurrentId, obj);
                };

                Task t1 = new Task(action, "t1");
                t1.Start();

                CrestronConsole.PrintLine("Task started, wait for it to finish...");
                t1.Wait();

                CrestronConsole.PrintLine("Task has completed!");
            }
            catch (Exception e)
            {
                CrestronConsole.PrintLine("Error in InitializeSystem: {0}", e.Message);
            }
        }
    }
}