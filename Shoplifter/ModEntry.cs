using System.Collections.Generic;
using System.Collections;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using Harmony;

namespace Shoplifter
{
    public class ModEntry
        : Mod
    {
        public static bool StolenToday = false;

        public static ArrayList ShopsBannedFrom { get; private set; } = new ArrayList();

        public static Dictionary<string, string> shopliftingstrings = new Dictionary<string, string>();

        public static readonly PerScreen<ArrayList> PerScreenShopsBannedFrom = new PerScreen<ArrayList>(createNewState: () => ShopsBannedFrom);

        public override void Entry(IModHelper helper)
        {
            ShopMenuPatcher.gethelpers(this.Monitor, this.Helper);
            ShopMenuUtilities.gethelpers(this.Monitor, this.Helper);
            helper.Events.GameLoop.DayStarted += this.DayStarted;
            helper.Events.GameLoop.GameLaunched += this.Launched;

            var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);
            ShopMenuPatcher.Hook(harmony, this.Monitor);

        }
        private void DayStarted(object sender, DayStartedEventArgs e)
        {
            // Reset stolentoday boolean so player can shoplift again when the new day starts
            StolenToday = false;
            if(PerScreenShopsBannedFrom.Value.Count > 0)
            {
                // Clear perscreenshopsbannedfrom arraylist so player can enter shops again
                PerScreenShopsBannedFrom.Value.Clear();
                this.Monitor.Log("Cleared list of banned shops, steal away!", LogLevel.Info);
            }                      
        }

        private void Launched(object sender, GameLaunchedEventArgs e)
        {
            try
            {
                // Get strings from assets folder and add them to a new dictionary
                Dictionary<string, string> strings = this.Helper.Content.Load<Dictionary<string, string>>("assets\\Strings.json", ContentSource.ModFolder);

                if (strings != null)
                {
                    foreach (string key in new List<string>(strings.Keys))
                    {
                        shopliftingstrings.Add(key, strings[key]);
                    }
                }
                this.Monitor.Log("Strings loaded from assets, ready to go!");
            }
            catch
            {
                // Add placeholder if strings can't be loaded to prevent crashes
                shopliftingstrings.Add("Placeholder", "Missing string... If you see this check you have the Strings.json file in the assets folder.");
                this.Monitor.Log("Could not load strings... This will likely result in problems, (Are you missing the Strings.json file?)", LogLevel.Error);
                this.Monitor.Log("Adding a placeholder string to stop crashes...", LogLevel.Info);
            }
           
        }
    }
}
