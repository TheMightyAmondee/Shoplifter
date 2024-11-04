using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shoplifter
{
    public class ModConfig
    {
        public EnabledShops ShopliftingLocations { get; set; } = new EnabledShops();
        public int MaxShopliftsPerDay { get; set; } = 1;
        public int MaxShopliftsPerStore { get; set; } = 1;
        public int MaxFine { get; set; } = 1000;
        public int FriendshipPenalty { get; set; } = 500;
        public int DaysBannedFor { get; set; } = 3;
        public int CatchesBeforeBan { get; set; } = 3;
        public int CaughtRadius { get; set; } = 5;
        public float RareStockChance { get; set; } = 0f;

        public class EnabledShops
        {
            public bool PierreShop { get; set; } = true;
            public bool WillyShop { get; set; } = true;
            public bool RobinShop { get; set; } = true;
            public bool MarnieShop { get; set; } = true;
            public bool Blacksmith { get; set; } = true;
            public bool Saloon { get; set; } = true;
            public bool SandyShop { get; set; } = true;
            public bool Clinic { get; set; } = true;
            public bool IceCreamStand { get; set; } = true;
            public bool ResortBar { get; set; } = true;
            public bool JojaMart { get; set; } = true;

        }
    }
}