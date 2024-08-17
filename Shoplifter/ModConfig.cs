using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shoplifter
{
    public class ModConfig
    {
        public int MaxShopliftsPerDay { get; set; } = 1;
        public int MaxShopliftsPerStore { get; set; } = 1;
        public int MaxFine { get; set; } = 1000;
        public int FriendshipPenalty { get; set; } = 500;
        public int DaysBannedFor { get; set; } = 3;
        public int CatchesBeforeBan { get; set; } = 3;
        public int CaughtRadius { get; set; } = 5;
        public float RareStockChance { get; set; } = 0f;
    }
}