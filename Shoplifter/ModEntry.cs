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
        public static readonly PerScreen<bool> PerScreenStolenToday = new PerScreen<bool>(createNewState: () => false);

        public static Dictionary<string, string> shopliftingstrings = new Dictionary<string, string>();

        public static readonly PerScreen<ArrayList> PerScreenShopsBannedFrom = new PerScreen<ArrayList>(createNewState: () => new ArrayList());

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
            // Reset perscreenstolentoday boolean so player can shoplift again when the new day starts
            PerScreenStolenToday.Value = false;

            if (PerScreenShopsBannedFrom.Value.Count > 0)
            {
                // Clear perscreenshopsbannedfrom arraylist so player can enter shops again
                PerScreenShopsBannedFrom.Value.Clear();
                this.Monitor.Log("Cleared list of banned shops, steal away!", LogLevel.Debug);
            }
        }

        private void Launched(object sender, GameLaunchedEventArgs e)
        {
            // Add placeholder for missing strings
            shopliftingstrings.Add("Placeholder", "There's a string missing here...");

            // Because missing strings will cause harmony patches to fail, placeholder is necessary to stop crashes

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

                if (shopliftingstrings.Count < 19)
                {
                    this.Monitor.Log("The number of strings loaded seem a bit low, you may get some missing string problems...\nCheck that all strings are present in the Strings.json", LogLevel.Warn);
                }
            }
            catch
            {             
                this.Monitor.Log("Could not load strings... This will result in missing string problems, (Are you missing the Strings.json file?)", LogLevel.Error);
            }
           
        }
    }
}
