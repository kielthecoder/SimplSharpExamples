using System;
using Crestron.SimplSharp;
using Crestron.SimplSharpPro;
using Crestron.SimplSharpPro.CrestronThread;

namespace EthernetAdapters
{
    public class ControlSystem : CrestronControlSystem
    {
        public ControlSystem() : base()
        {
            try
            {
                Thread.MaxNumberOfUserThreads = 20;
            }
            catch (Exception e)
            {
                CrestronConsole.PrintLine("Error in the constructor: {0}", e.Message);
            }
        }

        public override void InitializeSystem()
        {
            CrestronConsole.PrintLine("");
            CrestronConsole.PrintLine("Number of Ethernet Adapters: {0}", this.NumberOfEthernetAdapters);

            try
            {
                var eth1 = CrestronEthernetHelper.GetAdapterdIdForSpecifiedAdapterType(EthernetAdapterType.EthernetLANAdapter);

                CrestronConsole.PrintLine("");
                CrestronConsole.PrintLine("LAN IP Address:  {0}", CrestronEthernetHelper.GetEthernetParameter(CrestronEthernetHelper.ETHERNET_PARAMETER_TO_GET.GET_CURRENT_IP_ADDRESS, eth1));
                CrestronConsole.PrintLine("LAN Subnet Mask: {0}", CrestronEthernetHelper.GetEthernetParameter(CrestronEthernetHelper.ETHERNET_PARAMETER_TO_GET.GET_CURRENT_IP_MASK, eth1));
                CrestronConsole.PrintLine("LAN Gateway:     {0}", CrestronEthernetHelper.GetEthernetParameter(CrestronEthernetHelper.ETHERNET_PARAMETER_TO_GET.GET_CURRENT_ROUTER, eth1));
            }
            catch (Exception e)
            {
                CrestronConsole.PrintLine("Error getting LAN info: {0}", e.Message);
            }

            try
            { 
                var eth2 = CrestronEthernetHelper.GetAdapterdIdForSpecifiedAdapterType(EthernetAdapterType.EthernetCSAdapter);

                CrestronConsole.PrintLine("");
                CrestronConsole.PrintLine("CS IP Address:  {0}", CrestronEthernetHelper.GetEthernetParameter(CrestronEthernetHelper.ETHERNET_PARAMETER_TO_GET.GET_CURRENT_IP_ADDRESS, eth2));
                CrestronConsole.PrintLine("CS Subnet Mask: {0}", CrestronEthernetHelper.GetEthernetParameter(CrestronEthernetHelper.ETHERNET_PARAMETER_TO_GET.GET_CURRENT_IP_MASK, eth2));
                CrestronConsole.PrintLine("CS Gateway:     {0}", CrestronEthernetHelper.GetEthernetParameter(CrestronEthernetHelper.ETHERNET_PARAMETER_TO_GET.GET_CURRENT_ROUTER, eth2));
            }
            catch (Exception e)
            {
                CrestronConsole.PrintLine("Error getting CS info: {0}", e.Message);
            }

            try
            {
                var eth3 = CrestronEthernetHelper.GetAdapterdIdForSpecifiedAdapterType(EthernetAdapterType.EthernetLAN2Adapter);

                CrestronConsole.PrintLine("");
                CrestronConsole.PrintLine("LAN2 IP Address:  {0}", CrestronEthernetHelper.GetEthernetParameter(CrestronEthernetHelper.ETHERNET_PARAMETER_TO_GET.GET_CURRENT_IP_ADDRESS, eth3));
                CrestronConsole.PrintLine("LAN2 Subnet Mask: {0}", CrestronEthernetHelper.GetEthernetParameter(CrestronEthernetHelper.ETHERNET_PARAMETER_TO_GET.GET_CURRENT_IP_MASK, eth3));
                CrestronConsole.PrintLine("LAN2 Gateway:     {0}", CrestronEthernetHelper.GetEthernetParameter(CrestronEthernetHelper.ETHERNET_PARAMETER_TO_GET.GET_CURRENT_ROUTER, eth3));
            }
            catch (Exception e)
            {
                CrestronConsole.PrintLine("Error getting LAN2 info: {0}", e.Message);
            }
        }
    }
}