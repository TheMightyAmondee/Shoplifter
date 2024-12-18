
using System.Collections.Generic;
using System.Collections;
using StardewModdingAPI;
using StardewValley;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley.Locations;
using xTile.Dimensions;
using StardewValley.WorldMaps;

namespace Shoplifter
{
    public class ModEntry
        : Mod
    {
        private ModConfig config;

        public static readonly PerScreen<bool> PerScreenStolen = new PerScreen<bool>(createNewState: () => false);

        public static readonly PerScreen<int> PerScreenShopliftCounter = new PerScreen<int>(createNewState: () => 0);

        public static readonly PerScreen<Dictionary<string, int>> PerScreenShopliftedShops = new PerScreen<Dictionary<string, int>>(createNewState: () => new Dictionary<string, int>());

        public static readonly PerScreen<ArrayList> PerScreenShopsBannedFrom = new PerScreen<ArrayList>(createNewState: () => new ArrayList());

        public static readonly List<string> shops = new List<string>() { "SeedShop", "FishShop", "AnimalShop", "ScienceHouse", "Hospital", "Blacksmith", "Saloon", "SandyHouse", "JojaMart" };

        public static IDynamicGameAssetsApi IDGAItem;

        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.DayStarted += this.DayStarted;
            helper.Events.GameLoop.GameLaunched += this.Launched;
            helper.Events.Input.ButtonPressed += this.Action;
            helper.Events.Player.Warped += this.Warped;
            helper.ConsoleCommands.Add("shoplifter_resetsave", "Removes and readds save data added by the mod to fix broken save data, only use if you're getting errors", this.ResetSave);
            try
            {
                this.config = helper.ReadConfig<ModConfig>();
            }
            catch
            {
                this.config = new ModConfig();
                this.Monitor.Log("Failed to parse config file, default options will be used.", LogLevel.Warn);
            }

            ShopMenuUtilities.gethelpers(this.Monitor, this.ModManifest, this.config);
            CustomShopUtilities.gethelpers(this.Monitor, this.ModManifest, this.config, this.Helper);
            i18n.gethelpers(this.Helper.Translation, this.config);
        }
        private void DayStarted(object sender, DayStartedEventArgs e)
        {
            // Reset perscreenstolentoday boolean so player can shoplift again when the new day starts
            PerScreenStolen.Value = false;
            PerScreenShopliftCounter.Value = 0;
            PerScreenShopliftedShops.Value.Clear();

            // Clear banned shops
            if (PerScreenShopsBannedFrom.Value.Count > 0)
            {
                PerScreenShopsBannedFrom.Value.Clear();
            }

            var data = Game1.player.modData;

            // Add mod data if it isn't present
            foreach (string shop in shops)
            {
                if (data.ContainsKey($"{this.ModManifest.UniqueID}_{shop}") == false)
                {
                    data.Add($"{this.ModManifest.UniqueID}_{shop}", "0/0");
                    this.Monitor.Log($"Adding mod data... {this.ModManifest.UniqueID}_{shop}");
                }
            }

            foreach (string shopliftingdata in new List<string>(data.Keys))
            {

                string[] values = data[shopliftingdata]?.Split('/') ?? new string[] { };
                string[] fields = shopliftingdata?.Split('_') ?? new string[] { };

                // Player has finished certain number of days ban, remove shop from list, also reset first day caught
                if (shopliftingdata.StartsWith($"{this.ModManifest.UniqueID}") && int.Parse(values[0]) <= -this.config.DaysBannedFor && values.Length == 2)
                {
                    values[0] = "0";
                    values[1] = "0";

                    PerScreenShopsBannedFrom.Value.Remove(fields[1]);
                    this.Monitor.Log($"You're no longer banned from {fields[1]}, steal away!", LogLevel.Info);
                }

                // Player is currently banned, add shop to list
                else if (shopliftingdata.StartsWith($"{this.ModManifest.UniqueID}") && int.Parse(values[0]) < 0 && values.Length == 2)
                {
                    values[0] = (int.Parse(values[0]) - 1).ToString();
                    PerScreenShopsBannedFrom.Value.Add(fields[1]);
                    this.Monitor.Log($"You're currently banned from {fields[1]}", LogLevel.Info);
                }

                // If 28 days have past and player was not caught a certain number of times, reset both fields
                if (shopliftingdata.StartsWith($"{this.ModManifest.UniqueID}") && int.Parse(values[0]) > 0 && values[1] == Game1.dayOfMonth.ToString() && values.Length == 2)
                {
                    values[0] = "0";
                    values[1] = "0";

                    this.Monitor.Log($"It's been 28 days since you first shoplifted {fields[1]}, they've forgotten about it now...", LogLevel.Info);
                }

                // After manipulation, join fields back together with "/" seperator
                data[shopliftingdata] = string.Join("/", values);
            }

        }

        private void Launched(object sender, GameLaunchedEventArgs e)
        {
            if (this.config.MaxShopliftsPerStore < 1)
            {
                this.config.MaxShopliftsPerStore = 1;
            }

            if (this.config.MaxShopliftsPerDay < 1)
            {
                this.config.MaxShopliftsPerDay = 1;
            }

            if (this.config.CatchesBeforeBan < 1)
            {
                this.config.CatchesBeforeBan = 1;
            }

            if (this.config.CaughtRadius < 1)
            {
                this.config.CaughtRadius = 1;
            }

            // Read owned content packs and register shops as shopliftable
            foreach (IContentPack contentPack in this.Helper.ContentPacks.GetOwned())
            {
                if (contentPack.HasFile("shopliftables.json") == false)
                {
                    this.Monitor.Log($"Skipping content pack \"{contentPack.Manifest.Name}\", it does not have a shopliftables.json", LogLevel.Warn);
                }
                else
                {
                    this.Monitor.Log($"Loading content pack {contentPack.Manifest.Name} {contentPack.Manifest.Version} by {contentPack.Manifest.Author} | {contentPack.Manifest.Description}", LogLevel.Info);
                    ContentPack data;

                    try
                    {
                        data = contentPack.ReadJsonFile<ContentPack>("shopliftables.json");
                    }
                    catch
                    {
                        this.Monitor.Log($"Error reading content pack {contentPack.Manifest.Name}", LogLevel.Error);
                        continue;
                    }

                    CustomShopUtilities.RegisterShopliftableShop(data, contentPack);
                }
            }

            this.BuildConfigMenu();
            if (this.Helper.ModRegistry.IsLoaded("spacechase0.DynamicGameAssets") == true)
            {
                IDGAItem = this.Helper.ModRegistry.GetApi<IDynamicGameAssetsApi>("spacechase0.DynamicGameAssets");
            }
        }

        private void BuildConfigMenu()
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null) return;

            // register mod
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this.config)
            );

            configMenu.AddSectionTitle(this.ModManifest, () => i18n.string_GMCM_PeriodSection());
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => i18n.string_GMCM_MaxDay(),
                tooltip: () => i18n.string_GMCM_MaxDayTooltip(),
                getValue: () => this.config.MaxShopliftsPerDay,
                setValue: value => this.config.MaxShopliftsPerDay = value,
                min: 1
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => i18n.string_GMCM_MaxShop(),
                tooltip: () => i18n.string_GMCM_MaxShopTooltip(),
                getValue: () => this.config.MaxShopliftsPerStore,
                setValue: value => this.config.MaxShopliftsPerStore = value,
                min: 1
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => i18n.string_GMCM_RareStockChance(),
                tooltip: () => i18n.string_GMCM_RareStockChanceTooltip(),
                getValue: () => this.config.RareStockChance,
                setValue: value => this.config.RareStockChance = value,
                min: 0f, max: 1.0f, interval: 0.05f
            );
            configMenu.AddSectionTitle(this.ModManifest, () => i18n.string_GMCM_PenaltySection());
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => i18n.string_GMCM_MaxFine(),
                tooltip: () => i18n.string_GMCM_MaxFineTooltip(),
                getValue: () => this.config.MaxFine,
                setValue: value => this.config.MaxFine = value,
                min: 0
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => i18n.string_GMCM_MaxFriendship(),
                tooltip: () => i18n.string_GMCM_MaxFriendshipTooltip(),
                getValue: () => this.config.FriendshipPenalty,
                setValue: value => this.config.FriendshipPenalty = value,
                min: 0
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => i18n.string_GMCM_MaxCatches(),
                tooltip: () => i18n.string_GMCM_MaxCatchesTooltip(),
                getValue: () => this.config.CatchesBeforeBan,
                setValue: value => this.config.CatchesBeforeBan = value,
                min: 1, max: 100
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => i18n.string_GMCM_MaxBanned(),
                tooltip: () => i18n.string_GMCM_MaxBannedTooltip(),
                getValue: () => this.config.DaysBannedFor,
                setValue: value => this.config.DaysBannedFor = value,
                min: 0, max: 28
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => i18n.string_GMCM_MaxRadius(),
                tooltip: () => i18n.string_GMCM_MaxRadiusTooltip(),
                getValue: () => this.config.CaughtRadius,
                setValue: value => this.config.CaughtRadius = value,
                min: 0, max: 20
            );
            configMenu.AddPageLink(
                mod: this.ModManifest,
                pageId: "TheMightyAmondee.Shoplifter/EnabledShops",
                text: () => i18n.string_GMCM_Shopliftables());

            configMenu.AddPage(
                mod: this.ModManifest,
                pageId: "TheMightyAmondee.Shoplifter/EnabledShops",
                pageTitle: () => i18n.string_GMCM_Shopliftables());

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => i18n.string_GMCM_SeedShop(),
                getValue: () => this.config.ShopliftingLocations.PierreShop,
                setValue: value => this.config.ShopliftingLocations.PierreShop = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => i18n.string_GMCM_FishShop(),
                getValue: () => this.config.ShopliftingLocations.WillyShop,
                setValue: value => this.config.ShopliftingLocations.WillyShop = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => i18n.string_GMCM_Carpenter(),
                getValue: () => this.config.ShopliftingLocations.RobinShop,
                setValue: value => this.config.ShopliftingLocations.RobinShop = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => i18n.string_GMCM_AnimalShop(),
                getValue: () => this.config.ShopliftingLocations.MarnieShop,
                setValue: value => this.config.ShopliftingLocations.MarnieShop = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => i18n.string_GMCM_Blacksmith(),
                getValue: () => this.config.ShopliftingLocations.Blacksmith,
                setValue: value => this.config.ShopliftingLocations.Blacksmith = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => i18n.string_GMCM_Saloon(),
                getValue: () => this.config.ShopliftingLocations.Saloon,
                setValue: value => this.config.ShopliftingLocations.Saloon = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => i18n.string_GMCM_SandyShop(),
                getValue: () => this.config.ShopliftingLocations.SandyShop,
                setValue: value => this.config.ShopliftingLocations.SandyShop = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => i18n.string_GMCM_Hospital(),
                getValue: () => this.config.ShopliftingLocations.Clinic,
                setValue: value => this.config.ShopliftingLocations.Clinic = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => i18n.string_GMCM_IceCreamStand(),
                getValue: () => this.config.ShopliftingLocations.IceCreamStand,
                setValue: value => this.config.ShopliftingLocations.IceCreamStand = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => i18n.string_GMCM_IslandResort(),
                getValue: () => this.config.ShopliftingLocations.ResortBar,
                setValue: value => this.config.ShopliftingLocations.ResortBar = value
            );
            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => i18n.string_GMCM_JojaMart(),
                getValue: () => this.config.ShopliftingLocations.JojaMart,
                setValue: value => this.config.ShopliftingLocations.JojaMart = value
            );
        }

        private void Action(object sender, ButtonPressedEventArgs e)
        {
            GameLocation location = Game1.player.currentLocation;

            if ((e.Button.IsActionButton() == true
                ||
                e.Button == SButton.ControllerA)
                && Game1.dialogueUp == false
                && Context.CanPlayerMove == true
                && Context.IsWorldReady == true)
            {
                var TileX = e.Cursor.GrabTile.X;
                var TileY = e.Cursor.GrabTile.Y;

                // If using a controller, don't use cursor position if not facing wrong direction, check player is one tile under (Y - 1) tile with property
                if (e.Button == SButton.ControllerA && Game1.player.FacingDirection != 2)
                {
                    TileX = Game1.player.Tile.X;
                    TileY = Game1.player.Tile.Y - 1;
                }

                Location tilelocation = new Location((int)TileX, (int)TileY);

                // Island resort checked a different way, check this as well
                if (location.NameOrUniqueName == "IslandSouth"
                    && TileX == 14
                    && TileY == 22
                    && location as IslandSouth != null
                    && (location as IslandSouth).resortRestored.Value == true
                    && this.config.ShopliftingLocations.ResortBar == true)
                {
                    ShopMenuUtilities.ResortBarShopliftingMenu(location);
                }

                else if (location.NameOrUniqueName == "JojaMart"
                    && this.config.ShopliftingLocations.JojaMart == true
                    && TileX == 2
                    && (TileY == 24
                    || TileY == 25
                    || TileY == 26
                    ))
                {
                    ShopMenuUtilities.JojaShopliftingMenu(location);
                }

                foreach (var shopliftableshop in CustomShopUtilities.CustomShops.Values)
                {
                    bool openedshop = false;

                    foreach (var counterlocation in shopliftableshop.CounterLocation)
                    {
                        if (openedshop == true)
                        {
                            break;
                        }

                        if (counterlocation.LocationName == location.NameOrUniqueName && counterlocation.TileX == TileX && counterlocation.TileY == TileY && counterlocation.NeedsShopProperty == false)
                        {
                            openedshop = CustomShopUtilities.TryOpenCustomShopliftingMenu(shopliftableshop, location, TileX, TileY);
                            break;
                        }
                    }
                }

                // Get whether tile has action property and its' parameters
                string[] split = location.doesTileHavePropertyNoNull((int)TileX, (int)TileY, "Action", "Buildings").Split(' ');

                // Tile has desired property
                if (split != null)
                {
                    switch (split[0])
                    {
                        // If the door is a locked warp, check player can enter
                        case "LockedDoorWarp":
                        case "Warp":
                            // Player is banned from location they would warp to otherwise
                            if (PerScreenShopsBannedFrom.Value.Contains($"{split[3]}"))
                            {
                                // Supress button so game doesn't warp player (they're banned)
                                Helper.Input.Suppress(e.Button);

                                Game1.drawObjectDialogue(i18n.string_Banned());
                            }
                            break;
                        // For each action that would open a shop that can be shoplifted, check if it can be shoplifted and take appropriate action
                        case "HospitalShop":
                            if (this.config.ShopliftingLocations.Clinic == true)
                            {
                                ShopMenuUtilities.HospitalShopliftingMenu(location, Game1.player);
                            }
                            break;

                        case "Carpenter":
                            if (this.config.ShopliftingLocations.RobinShop == true)
                            {
                                ShopMenuUtilities.CarpenterShopliftingMenu(location, Game1.player, tilelocation);
                            }
                            break;

                        case "AnimalShop":
                            if (this.config.ShopliftingLocations.MarnieShop == true)
                            {
                                ShopMenuUtilities.AnimalShopShopliftingMenu(location, Game1.player, tilelocation);
                            }
                            break;

                        case "Blacksmith":
                            if (this.config.ShopliftingLocations.Blacksmith == true)
                            {
                                ShopMenuUtilities.BlacksmithShopliftingMenu(location, tilelocation);
                            }
                            break;

                        case "Saloon":
                            if (this.config.ShopliftingLocations.Saloon == true)
                            {
                                ShopMenuUtilities.SaloonShopliftingMenu(location, tilelocation);
                            }
                            break;
                        case "IceCreamStand":
                            if (this.config.ShopliftingLocations.IceCreamStand == true)
                            {
                                ShopMenuUtilities.IceCreamShopliftingMenu(location, tilelocation);
                            }
                            break;

                        case "Buy":
                            if (split[1] == "Fish" && this.config.ShopliftingLocations.WillyShop == true)
                            {
                                ShopMenuUtilities.FishShopShopliftingMenu(location);
                            }
                            else if (location is SeedShop && PerScreenShopliftCounter.Value < config.MaxShopliftsPerDay && this.config.ShopliftingLocations.PierreShop == true)
                            {
                                ShopMenuUtilities.SeedShopShopliftingMenu(location);
                            }
                            else if (location.Name.Equals("SandyHouse") == true && this.config.ShopliftingLocations.SandyShop == true)
                            {
                                ShopMenuUtilities.SandyShopShopliftingMenu(location);
                            }
                            break;
                        case "OpenShop":
                            foreach (var shopliftableshop in CustomShopUtilities.CustomShops.Values)
                            {
                                if (split[1] == shopliftableshop.ShopName && CustomShopUtilities.TryOpenCustomShopliftingMenu(shopliftableshop, location, TileX, TileY))
                                {
                                    break;
                                }
                            }
                            break;
                    }
                }
            }
        }

        private void Warped(object sender, WarpedEventArgs e)
        {
            if (PerScreenShopsBannedFrom.Value.Contains(e.NewLocation.NameOrUniqueName) == true)
            {
                Game1.warpFarmer(e.NewLocation.warps[0].TargetName, e.NewLocation.warps[0].TargetX, e.NewLocation.warps[0].TargetY, false);

                Game1.drawObjectDialogue(i18n.string_Banned());
            }
        }

        private void ResetSave(string command, string[] arg)
        {
            try
            {
                var data = Game1.player.modData;

                foreach (string moddata in new List<string>(data.Keys))
                {
                    if (moddata.StartsWith($"{this.ModManifest.UniqueID}") == true)
                    {
                        data.Remove(moddata);
                        data.Add(moddata, "0/0");
                    }
                }

                this.Monitor.Log("Reset shoplifting data... If this didn't fix the error on a new day, please report the error to the mod page", LogLevel.Info);
            }
            catch
            {
                this.Monitor.Log("Unable to execute command, check formatting is correct", LogLevel.Error);
            }
        }
    }
}
