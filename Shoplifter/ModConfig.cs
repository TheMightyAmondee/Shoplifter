using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shoplifter
{
    public class ModConfig
    {
        public uint MaxShopliftsPerDay { get; set; } = 1;
        public uint MaxFine { get; set; } = 1000;
        public uint FriendshipPenalty { get; set; } = 500;
        public uint DaysBannedFor { get; set; } = 3;
        public uint CatchesBeforeBan { get; set; } = 3;

    }
}
