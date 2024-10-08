﻿using System;
using StardewModdingAPI;
using StardewValley.Menus;
using StardewValley;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;

namespace Shoplifter
{
    public class CustomShopUtilities 
    {
        private static IMonitor monitor;
        private static IManifest manifest;
        private static ModConfig config;
        private static IModHelper modHelper;


        public static Dictionary<string, ContentPackModel> CustomShops = new Dictionary<string, ContentPackModel>();
        public static void gethelpers(IMonitor monitor, IManifest manifest, ModConfig config, IModHelper modhelper)
        {
            CustomShopUtilities.monitor = monitor;
            CustomShopUtilities.manifest = manifest;
            CustomShopUtilities.config = config;
            CustomShopUtilities.modHelper = modhelper;
        }
        
        /// <summary>
        /// Determines whether a custom shop can be shoplifted based on the shop opening conditions
        /// </summary>
        /// <param name="shop">The custom shopliftable model</param>
        /// <returns>If the shop can be shoplifted from</returns>
        private static bool CanShopliftCustomShop(ContentPackModel shop)
        {
            int CorrectWeather()
            {
                if (shop.OpenConditions.Weather == null)
                {
                    return -1;
                }
                var weather = Game1.currentLocation;
                var trueweather = weather.GetWeather().Weather;
                if (weather.GetWeather().Weather == null)
                {
                    if (weather.IsLightningHere() == true)
                    {
                        trueweather = "Storm";
                    }
                    else if (weather.IsRainingHere() == true)
                    {
                        trueweather = "Rain";
                    }
                    else if (weather.IsSnowingHere() == true)
                    {
                        trueweather = "Snow";
                    }
                    else if (weather.IsDebrisWeatherHere() == true)
                    {
                        trueweather = "Wind";
                    }
                    else
                    {
                        trueweather = "Sun";
                    }
                }

                if (shop.OpenConditions.Weather.Contains(trueweather) == true)
                {
                    return 1;
                }

                return 0;
            }
        
            int QueriesTrue()
            {
                
                if (shop.OpenConditions.GameStateQueries == null)
                {
                    return -1;
                }
                foreach (var query in shop.OpenConditions.GameStateQueries)
                {
                    if (GameStateQuery.CheckConditions(query) == false)
                    {
                        return 0;
                    }
                }
                return 1;
            }

            int ShopKeeperPresent()
            {
                if (shop.OpenConditions.ShopKeeperRange == null)
                {
                    return -1;
                }

                foreach (var shopkeeperdata in shop.OpenConditions.ShopKeeperRange)
                {
                    var location = Game1.currentLocation;
                    var npc = location.getCharacterFromName(shopkeeperdata.Name);
                    var shopkeeperarea = new Microsoft.Xna.Framework.Rectangle(shopkeeperdata.TileX, shopkeeperdata.TileY, shopkeeperdata.Width, shopkeeperdata.Height);
                    if (npc != null)
                    {
                        Point tile = npc.TilePoint;
                        if (shopkeeperarea.Contains(tile) == true)
                        {
                            return 1;
                        }
                    }
                }

                return 0;
            }
            List<int> rawconditionstate = new List<int>() { ShopKeeperPresent(), QueriesTrue(), CorrectWeather() };
            List<int> trueconditionstate = new List<int>();

            foreach (var conditionstatedata in rawconditionstate)
            {
                if (conditionstatedata != -1)
                {
                    trueconditionstate.Add(conditionstatedata);
                }
            }
            return trueconditionstate.Contains(0);
        }

        /// <summary>
        /// For a custom shop, determines whether the player is caught, makes corrections to dialogue based on return value
        /// </summary>
        /// <param name="who">The player to catch</param>
        /// <param name="location">The current location instance</param>
        /// <param name="shop">The shop to check</param>
        /// <returns>Whether the player was caught</returns>
        public static bool CustomShop_ShouldBeCaught(ContentPackModel shop, Farmer who, GameLocation location)
        {
            if (shop == null)
            {
                return false;
            }

            var which = shop.ShopKeepers;
            var dialoguedictionary = shop.CaughtDialogue;

            foreach (string character in which)
            {
                foreach (var npc in location.characters)
                {
                    // Is NPC in range?
                    if (npc.Name == character 
                        && npc.currentLocation == who.currentLocation 
                        && Utility.tileWithinRadiusOfPlayer((int)npc.Tile.X, (int)npc.Tile.Y, (int)config.CaughtRadius, who))
                    {
                        string dialogue;
                        string banneddialogue = (config.DaysBannedFor == 1)
                            ? i18n.string_BanFromShop_Single()
                            : i18n.string_BanFromShop();

                        ShopMenuUtilities.fineamount = Math.Min(Game1.player.Money, (int)config.MaxFine);

                        // Is NPC primary shopowner
                        if (dialoguedictionary != null)
                        {
                            dialogue = TranslatedDialogue(npc.Name, shop, ShopMenuUtilities.fineamount) 
                                ?? (ShopMenuUtilities.fineamount > 0 
                                    ? i18n.string_Caught("Generic") 
                                    : i18n.string_Caught_NoMoney("Generic"));
                        }

                        else
                        {
                            // No, use generic dialogue
                            dialogue = (ShopMenuUtilities.fineamount > 0)
                                ? i18n.string_Caught("Generic")
                                : i18n.string_Caught_NoMoney("Generic");
                        }

                        // Is the player now banned? (uses catch before as dialogue is loaded before count is adjusted) Append additional dialogue
                        dialogue = (Game1.player.modData.ContainsKey($"{manifest.UniqueID}_{location.NameOrUniqueName}") == true 
                            && Game1.player.modData[$"{manifest.UniqueID}_{location.NameOrUniqueName}"].StartsWith($"{config.CatchesBeforeBan - 1}") == true 
                            && shop.Bannable == true)
                            ? dialogue + banneddialogue
                            : dialogue;

                        npc.CurrentDialogue.Push(new Dialogue(npc, "", dialogue));

                        // Draw dialogue for NPC, dialogue box opens
                        Game1.drawDialogue(npc);
                        monitor.Log($"{character} caught you shoplifting... You were fined {ShopMenuUtilities.fineamount}g");

                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Applies shoplifting penalties, tracks whether to ban player
        /// </summary>
        /// <param name="location">The current location instance</param>
        /// <param name="bannable">Whether the player can be banned from the shop</param>
        public static void CustomShop_ShopliftingPenalties(GameLocation location, ContentPackModel shop)
        {
            // Subtract monetary penalty if it applies
            if (ShopMenuUtilities.fineamount > 0)
            {
                Game1.player.Money -= ShopMenuUtilities.fineamount;
            }

            if (shop.Bannable == true || shop.Exit != null)
            {
                
                if (shop.Exit == null)
                {
                    var exit = location.GetFirstPlayerWarp();
                    Game1.warpFarmer(exit.TargetName, exit.TargetX, exit.TargetY, false);
                }
                else
                {
                    Game1.warpFarmer(shop.Exit.LocationName, shop.Exit.TileX, shop.Exit.TileY, false);
                }
                
            }

            string locationname = location.NameOrUniqueName;

            var data = Game1.player.modData;

            if (config.DaysBannedFor == 0 || shop.Bannable == false)
            {
                return;
            }

            string[] fields = data[$"{manifest.UniqueID}_{locationname}"].Split('/') ?? new string[] { };

            // Add one to first part of data (shoplifting count)
            fields[0] = (int.Parse(fields[0]) + 1).ToString();

            // If this is the first shoplift, record day of month in second part of data (day of first steal)
            if (fields[0] == "1")
            {
                fields[1] = Game1.dayOfMonth.ToString();
            }

            // After being caught some times (within 28 days) ban player from shop for three days
            if (int.Parse(fields[0]) == config.CatchesBeforeBan)
            {
                fields[0] = "-1";
                ModEntry.PerScreenShopsBannedFrom.Value.Add($"{locationname}");
                monitor.Log($"{locationname} added to banned shop list");
            }

            // Join manipulated data for recording in save file
            data[$"{manifest.UniqueID}_{locationname}"] = string.Join("/", fields);
        }

        /// <summary>
        /// Create the shoplifting menu for a custom shop and determine what to do depending on the option pressed
        /// </summary>
        /// <param name="location">The current location instance</param>
        /// <param name="shop">The shop to check</param>
        public static void CustomShop_ShopliftingMenu(ContentPackModel shop, GameLocation location)
        {
            // Check player hasn't reached daily shoplifting limit
            if (ModEntry.PerScreenShopliftCounter.Value >= config.MaxShopliftsPerDay)
            {
                // Player has reached limit, exit method after displaying dialogue box
                Game1.drawObjectDialogue(config.MaxShopliftsPerDay != 1
                        ? i18n.string_AlreadyShoplifted()
                        : i18n.string_AlreadyShoplifted_Single());
                return;
            }

            // Check player hasn't reached daily shoplifting limit for the shop
            else if (ModEntry.PerScreenShopliftedShops.Value.ContainsKey(shop.UniqueShopId) == true && ModEntry.PerScreenShopliftedShops.Value[shop.UniqueShopId] >= config.MaxShopliftsPerStore)
            {
                // Player has reached limit, exit method after displaying dialogue box
                Game1.drawObjectDialogue(i18n.string_AlreadyShopliftedSameShop());
                return;
            }

            // Create option to steal
            location.createQuestionDialogue(i18n.string_Shoplift(), location.createYesNoResponses(), delegate (Farmer _, string answer)
            {
                // Player answered yes
                if (answer == "Yes")
                {
                    ShopMenuUtilities.SeenShoplifting(location, Game1.player);

                    // Player is caught
                    if (CustomShop_ShouldBeCaught(shop, Game1.player, location) == true)
                    {
                        // After dialogue, apply penalties
                        Game1.afterDialogues = delegate
                        {
                            CustomShop_ShopliftingPenalties(location, shop);
                        };

                        return;
                    }

                    // Set stolen to true so counters will increment
                    ModEntry.PerScreenStolen.Value = false;
                    // Not caught, generate stock for shoplifting, on purchase adjust counters
                    Game1.activeClickableMenu = new ShopMenu("ShopliftMenu", ShopStock.generateRandomStock(shop.MaxStockQuantity, shop.MaxStackPerItem, shop.ShopName, config.RareStockChance), 0, null, delegate
                    {
                        if (ModEntry.PerScreenStolen.Value == false)
                        {
                            // Increment times shoplifted by one
                            ModEntry.PerScreenShopliftCounter.Value++;
                        }

                        if (ModEntry.PerScreenShopliftedShops.Value.ContainsKey(shop.UniqueShopId) == false)
                        {
                            // Add shop to list of shoplifted shops if needed
                            ModEntry.PerScreenShopliftedShops.Value.Add(shop.UniqueShopId, 1);
                        }

                        else if (ModEntry.PerScreenStolen.Value == false && config.MaxShopliftsPerStore > 1 && ModEntry.PerScreenShopliftedShops.Value.ContainsKey(shop.UniqueShopId) == true)
                        {
                            // Shop on shoplifted list, increment value by one
                            ModEntry.PerScreenShopliftedShops.Value[shop.UniqueShopId]++;
                        }

                        // Set stolen to false so counters won't increment on next purchase while in shoplifting window, will become true next time window opens so it can increment again
                        ModEntry.PerScreenStolen.Value = true;

                        return false;
                    }, null, true);
                }               
            });
        }

        /// <summary>
        /// Registers a shop added by a contentpack as shopliftable
        /// </summary>
        /// <param name="data">The shopliftables data model from the contentpack</param>
        /// <param name="contentPack">The contentpack which added the shop</param>
        public static void RegisterShopliftableShop(ContentPack data, IContentPack contentPack)
        {
            // Does content pack contain data?
            if (data.MakeShopliftable != null)
            {
                // Yes, check each entry
                foreach (var shop in data.MakeShopliftable)
                {
                    // Ensure entry is valid
                    if (ValidateContentPackModel(shop) == false)
                    {
                        monitor.Log($"{contentPack.Manifest.Name} has an invalid shop at index {data.MakeShopliftable.IndexOf(shop)}! A required field is missing. This shop will be ignored.", LogLevel.Warn);
                        continue;
                    }

                    // Ensure entry doesn't already exist
                    else if (CustomShops.ContainsKey(shop.UniqueShopId) == true)
                    {
                        monitor.Log($"{contentPack.Manifest.Name} tried to add a shopliftable location with a unique id of {shop.UniqueShopId} but a shop with this id already exists! This shop will be ignored.", LogLevel.Warn);
                        continue;
                    }

                    foreach(var counterlocation in shop.CounterLocation)
                    {
                        // Add location of shop to save data if player can be banned
                        if (shop.Bannable == true && ModEntry.shops.Contains(counterlocation.LocationName) == false)
                        {
                            ModEntry.shops.Add(counterlocation.LocationName);
                        }
                    }

                   

                    // Add shop data to customshops dictionary
                    CustomShops.Add(shop.UniqueShopId, shop);
                    shop.ContentModelPath = contentPack.DirectoryPath;
                    monitor.Log($"{contentPack.Manifest.Name} added shop {shop.UniqueShopId} as a shopliftable shop.", LogLevel.Info);
                }
            }
        }

        /// <summary>
        /// Attempt to open a shoplifting menu for the specified shop
        /// </summary>
        /// <param name="shop">The shop to open the menu for</param>
        /// <param name="location">The current location instance</param>
        /// <returns>Whether the menu was successfully opened</returns>
        public static bool TryOpenCustomShopliftingMenu(ContentPackModel shop, GameLocation location, float TileX, float TileY)
        {
            bool success = false;
            bool correctlocation = false;

            foreach (var counterlocation in shop.CounterLocation)
            {
                // Add location of shop to save data if player can be banned
                if (counterlocation.LocationName == location.NameOrUniqueName && counterlocation.TileX == TileX && counterlocation.TileY == TileY)
                {
                    correctlocation = true;
                    break;
                }
            }

            if (correctlocation == true)
            {
               if (CanShopliftCustomShop(shop) == true)
               {
                    CustomShop_ShopliftingMenu(shop, location);
                    success = true;
               }
            }
            
            return success;
        }

        /// <summary>
        /// Validates a shop model to ensure all required fields are present. This does not validate if the field is in the correct type
        /// </summary>
        /// <param name="shop">The shop to check</param>
        /// <returns>Whether the contentpack was successfully validated</returns>
        private static bool ValidateContentPackModel(ContentPackModel shop)
        {
            bool validated = false;

            if (shop.OpenConditions.ShopKeeperRange != null)
            {
                foreach(var shopkeeperdata in shop.OpenConditions.ShopKeeperRange)
                {
                    if (shopkeeperdata.Name == null)
                    {
                        return validated;
                    }
                }                
            }

            if (shop.CounterLocation == null)
            {
                return validated;
            }

            else
            {
                foreach(var counterlocation in shop.CounterLocation)
                {
                    if(counterlocation.LocationName == null || counterlocation.TileX == -1 || counterlocation.TileX == -1)
                    {
                        return validated;
                    }
                }
            }

            if (shop.UniqueShopId == null || shop.ShopName == null || shop.ShopKeepers.Count == 0)
            {
                return validated;
            }

            validated = true;

            return validated;

        }

        /// <summary>
        /// Gets the correct translation (content pack)
        /// </summary>
        /// <param name="contentPack">The content pack to get the translation from</param>
        /// <param name="key">The translation key</param>
        /// <param name="tokens">Tokens, if any</param>
        /// <returns>The translated string</returns>
        private static Translation GetTranslation(IContentPack contentPack, string key, object tokens = null)
        {
            return contentPack.Translation.Get(key, tokens);
        }

        /// <summary>
        /// Gets the translated caughtdialogue for the specified npc
        /// </summary>
        /// <param name="name">the name of the npc</param>
        /// <param name="shop">the shopliftables shop data for the player was caught shoplifting</param>
        /// <param name="fineamount">the amount the player was fined</param>
        /// <returns>The translated string, or generic dialogue</returns>
        private static string TranslatedDialogue(string name, ContentPackModel shop, int fineamount)
        {
            var tempcontentpack = modHelper.ContentPacks.CreateFake(shop.ContentModelPath);
            if (fineamount > 0)
            {
                if (shop.CaughtDialogue.ContainsKey(name) == true)
                {
                    if (shop.CaughtDialogue[name].StartsWith("{{i18n:") == true && shop.CaughtDialogue[name].EndsWith("}}") == true)
                    {
                        var trimmedkey = shop.CaughtDialogue[name].Replace("{{i18n:", "").Replace("}}","");
                        return GetTranslation(tempcontentpack, trimmedkey, new { fineamount = fineamount });
                    }
                    else
                    {
                        return string.Format(shop.CaughtDialogue[name], fineamount);
                    }                   
                }

                else
                {
                    return i18n.string_Caught("Generic");
                }
            }

            else
            {
                if (shop.CaughtDialogue.ContainsKey($"{name}_NoMoney") == true)
                {
                    if (shop.CaughtDialogue[$"{name}_NoMoney"].StartsWith("{{i18n:") == true && shop.CaughtDialogue[name].EndsWith("}}") == true)
                    {
                        var trimmedkey = shop.CaughtDialogue[$"{name}_NoMoney"].Replace("{{i18n:", "").Replace("}}", "");
                        return GetTranslation(tempcontentpack, trimmedkey);
                    }
                    else
                    {
                        return string.Format(shop.CaughtDialogue[$"{name}_NoMoney"], fineamount);
                    }
                }
                
                else
                {
                    return i18n.string_Caught_NoMoney("Generic");
                }
            }

        }
    }
}
