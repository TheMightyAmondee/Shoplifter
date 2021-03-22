using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewValley;
using StardewModdingAPI.Events;
using Harmony;

namespace Shoplifter
{
    public class ModEntry
        : Mod
    {
        public static bool StolenToday = false;
        public override void Entry(IModHelper helper)
        {
            ShopMenuPatcher.gethelpers(this.Monitor, this.Helper);
            ShopStock.gethelpers(this.Monitor, this.Helper);

            var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);
            ShopMenuPatcher.Hook(harmony, this.Monitor);
        }
    }
}
