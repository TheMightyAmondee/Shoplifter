using System.Collections.Generic;
using System.Collections;
using StardewModdingAPI;
using StardewValley;
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
            ShopMenuUtilities.gethelpers(this.Monitor, this.Helper, this.ModManifest);
            helper.Events.GameLoop.DayStarted += this.DayStarted;
            helper.Events.GameLoop.GameLaunched += this.Launched;           

            var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);
            ShopMenuPatcher.Hook(harmony, this.Monitor);

        }
        private void DayStarted(object sender, DayStartedEventArgs e)
        {
            // Reset perscreenstolentoday boolean so player can shoplift again when the new day starts
            PerScreenStolenToday.Value = false;

            // Clear banned shops
            if (PerScreenShopsBannedFrom.Value.Count > 0)
            {
                PerScreenShopsBannedFrom.Value.Clear();
            }

            var data = Game1.player.modData;

            // Add mod data if it isn't present
            if (data.ContainsKey($"{this.ModManifest.UniqueID}_SeedShop") == false)
            {
                data.Add($"{this.ModManifest.UniqueID}_SeedShop", "0");
                data.Add($"{this.ModManifest.UniqueID}_FishShop", "0");
                data.Add($"{this.ModManifest.UniqueID}_AnimalShop", "0");
                data.Add($"{this.ModManifest.UniqueID}_ScienceHouse", "0");
                data.Add($"{this.ModManifest.UniqueID}_Hospital", "0");
                data.Add($"{this.ModManifest.UniqueID}_Blacksmith", "0");
                data.Add($"{this.ModManifest.UniqueID}_Saloon", "0");
                this.Monitor.Log("Adding mod data...");
            }

            else
            {
                this.Monitor.Log("Found mod data...");

                var moddata = Game1.player.modData;

                foreach (string shopliftingdata in new List<string>(moddata.Keys))
                {
                    // Player has finished three day ban, remove shop from list
                    if (shopliftingdata.StartsWith($"{this.ModManifest.UniqueID}") && moddata[shopliftingdata] == "-111")
                    {
                        moddata[shopliftingdata] = "0";
                        string[] fields = shopliftingdata.Split('_');
                        PerScreenShopsBannedFrom.Value.Remove(fields[1]);
                        this.Monitor.Log($"You're no longer banned from {fields[1]}, steal away!", LogLevel.Info);
                    }

                    // Player is currently banned, add shop to list
                    else if (shopliftingdata.StartsWith($"{this.ModManifest.UniqueID}") && int.Parse(moddata[shopliftingdata]) < 0)
                    {
                        moddata[shopliftingdata] = int.Parse(moddata[shopliftingdata]) + 1.ToString();
                        string[] fields = shopliftingdata.Split('_');
                        PerScreenShopsBannedFrom.Value.Add(fields[1]);
                    }
                }
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

                if (shopliftingstrings.Count < 20)
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
