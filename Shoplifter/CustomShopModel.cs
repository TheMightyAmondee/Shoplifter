using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shoplifter
{
    public class CustomShopModel
    {
        public string ShopName { get; set; }
        public int OpenTime { get; set; }
        public int CloseTime { get; set; }
        public string PrimaryShopKeeper { get; set; }
        public string[] AdditionalShopKeepers { get; set; } = null;
        public string[] CaughtDialogue { get; set; } = null;
        public int[] ItemsForSale { get; set; } = { 802, 478, 486 };
        public bool VanillaShop { get; set; } = false;
    }
}
