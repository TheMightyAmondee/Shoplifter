using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Collections;
using StardewModdingAPI;
using StardewValley;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley.Locations;
using Harmony;
using xTile.Dimensions;
using xTile.Tiles;
using xTile.Layers;

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
            ShopMenuUtilities.gethelpers(this.Monitor, this.ModManifest, this.Helper.Input);
            helper.Events.GameLoop.DayStarted += this.DayStarted;
            helper.Events.GameLoop.GameLaunched += this.Launched;
            helper.Events.Input.ButtonPressed += this.Action;
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
                data.Add($"{this.ModManifest.UniqueID}_SeedShop", "0/0");
                data.Add($"{this.ModManifest.UniqueID}_FishShop", "0/0");
                data.Add($"{this.ModManifest.UniqueID}_AnimalShop", "0/0");
                data.Add($"{this.ModManifest.UniqueID}_ScienceHouse", "0/0");
                data.Add($"{this.ModManifest.UniqueID}_Hospital", "0/0");
                data.Add($"{this.ModManifest.UniqueID}_Blacksmith", "0/0");
                data.Add($"{this.ModManifest.UniqueID}_Saloon", "0/0");
                this.Monitor.Log("Adding mod data...");
            }

            // Moddata exists, interpret
            else
            {
                this.Monitor.Log("Found mod data... Interpreting");

                var moddata = Game1.player.modData;

                foreach (string shopliftingdata in new List<string>(moddata.Keys))
                {

                    string[] values = moddata[shopliftingdata].Split('/');
                    string[] fields = shopliftingdata.Split('_');

                    // Player has finished three day ban, remove shop from list, also reset first day caught
                    if (shopliftingdata.StartsWith($"{this.ModManifest.UniqueID}") && values[0] == "-3")
                    {
                        values[0] = "0";
                        values[1] = "0";
                        
                        PerScreenShopsBannedFrom.Value.Remove(fields[1]);
                        this.Monitor.Log($"You're no longer banned from {fields[1]}, steal away!", LogLevel.Info);
                    }

                    // Player is currently banned, add shop to list
                    else if (shopliftingdata.StartsWith($"{this.ModManifest.UniqueID}") && int.Parse(values[0]) < 0)
                    {
                        values[0] = (int.Parse(values[0]) - 1).ToString();
                        PerScreenShopsBannedFrom.Value.Add(fields[1]);
                        this.Monitor.Log($"You're currently banned from {fields[1]}", LogLevel.Info);
                    }

                    // If 28 days have past and player was not caught 3 times, reset both fields
                    if (shopliftingdata.StartsWith($"{this.ModManifest.UniqueID}") && int.Parse(values[0]) > 0 && values[1] == Game1.dayOfMonth.ToString())
                    {
                        values[0] = "0";
                        values[1] = "0";

                        this.Monitor.Log($"It's been 28 days since you first shoplifted {fields[1]}, they've forgotten about it now...", LogLevel.Info);
                    }

                    // After manipulation, join fields back together with "/" seperator
                    moddata[shopliftingdata] = string.Join("/", values);
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

        private void Action(object sender, ButtonPressedEventArgs e)
        {
            GameLocation location = Game1.currentLocation;

            if (e.Button == SButton.MouseRight || e.Button == SButton.ControllerA && Game1.dialogueUp == false && Context.CanPlayerMove == true)
            {
                var TileX = e.Cursor.GrabTile.X;
                var TileY = e.Cursor.GrabTile.Y;

                Location tilelocation = new Location((int)TileX, (int)TileY);

                string[] split = location.doesTileHavePropertyNoNull((int)TileX, (int)TileY, "Action", "Buildings").Split(' ');

                if (split != null)
                {
                    switch (split[0])
                    {
                        case "LockedDoorWarp":
                            if (PerScreenShopsBannedFrom.Value.Contains($"{split[3]}"))
                            {
                                Helper.Input.Suppress(e.Button);

                                if (shopliftingstrings.ContainsKey("TheMightyAmondee.Shoplifter/Banned") == true)
                                {
                                    Game1.drawObjectDialogue(shopliftingstrings["TheMightyAmondee.Shoplifter/Banned"]);
                                }

                                else
                                {
                                    Game1.drawObjectDialogue(shopliftingstrings["Placeholder"]);
                                }
                            }
                            break;

                        case "HospitalShop":
                            ShopMenuUtilities.HospitalShopliftingMenu(location, Game1.player);
                            this.Monitor.Log("The hospital shop", LogLevel.Debug);
                            break;

                        case "Carpenter":
                            ShopMenuUtilities.CarpenterShopliftingMenu(location, Game1.player, tilelocation);
                            this.Monitor.Log("The carpenter shop", LogLevel.Debug);
                            break;

                        case "AnimalShop":
                            ShopMenuUtilities.AnimalShopShopliftingMenu(location, Game1.player, tilelocation);
                            this.Monitor.Log("The animal shop", LogLevel.Debug);
                            break;

                        case "Blacksmith":
                            ShopMenuUtilities.BlacksmithShopliftingMenu(location, tilelocation);
                            this.Monitor.Log("The blacksmith shop", LogLevel.Debug);
                            break;

                        case "Saloon":
                            ShopMenuUtilities.SaloonShopliftingMenu(location, tilelocation);
                            this.Monitor.Log("The saloon shop", LogLevel.Debug);
                            break;
                        case "Buy":
                            if (split[1] == "Fish")
                            {
                                ShopMenuUtilities.FishShopShopliftingMenu(location);
                                this.Monitor.Log("The fish shop", LogLevel.Debug);
                            }
                            else if (location is SeedShop)
                            {
                                ShopMenuUtilities.SeedShopShopliftingMenu(location);
                                this.Monitor.Log("The seed shop", LogLevel.Debug);
                            }
                            break;
                    }
                }               
            }
        }
    }
}
