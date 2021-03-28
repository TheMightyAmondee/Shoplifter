using System;
using System.Collections.Generic;
using System.Collections;
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

        public static ArrayList ShopsBannedFrom = new ArrayList();

        public static Dictionary<string, string> shopliftingstrings = new Dictionary<string, string>();

        public override void Entry(IModHelper helper)
        {
            ShopMenuPatcher.gethelpers(this.Monitor, this.Helper);
            ShopStock.gethelpers(this.Monitor, this.Helper);
            helper.Events.GameLoop.DayStarted += this.DayStarted;
            helper.Events.GameLoop.GameLaunched += this.Launched;

            var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);
            ShopMenuPatcher.Hook(harmony, this.Monitor);

        }
        private void DayStarted(object sender, DayStartedEventArgs e)
        {
            // Reset stolentoday boolean so player can shoplift again when the new day starts
            StolenToday = false;
            // Clear shopsbannedfrom arraylist so player can enter shops again
            ShopsBannedFrom.Clear();
            
        }

        private void Launched(object sender, GameLaunchedEventArgs e)
        {
           Dictionary<string, string> strings = this.Helper.Content.Load<Dictionary<string, string>>("assets\\Strings.json", ContentSource.ModFolder);

            if (strings != null)
            {
                foreach (string key in new List<string>(strings.Keys))
                {
                    shopliftingstrings.Add(key, strings[key]);
                }
            }
        }
    }
}
