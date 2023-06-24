using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BroadcastPackets
{
    internal class SystemInfo
    {
        public int Sequence { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string HostAddress { get; set; }
        public int Clock { get; set; }
    }
}
