using System;
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
                ErrorLog.Error("Error in the constructor: {0}", e.Message);
            }
        }

        public override void InitializeSystem()
        {
            try
            {

            }
            catch (Exception e)
            {
                ErrorLog.Error("Error in InitializeSystem: {0}", e.Message);
            }
        }
    }
}