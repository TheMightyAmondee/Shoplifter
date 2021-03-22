using System;
using System.Collections.Generic;
using StardewValley.Locations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley.Objects;
using StardewValley.Menus;
using StardewValley;
using StardewModdingAPI;
using Harmony;
using Microsoft.Xna.Framework;
using xTile.Dimensions;

namespace Shoplifter
{
    public class ShopMenuPatcher
		: GameLocation
    {
        private static IMonitor monitor;
        private static IModHelper helper;

        public static void gethelpers(IMonitor monitor, IModHelper helper)
        {
            ShopMenuPatcher.monitor = monitor;
            ShopMenuPatcher.helper = helper;
        }

        public static void Hook(HarmonyInstance harmony, IMonitor monitor)
        {
            ShopMenuPatcher.monitor = monitor;

            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.openShopMenu)),
                postfix: new HarmonyMethod(typeof(ShopMenuPatcher), nameof(ShopMenuPatcher.openShopMenu_Postfix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.performAction)),
                postfix: new HarmonyMethod(typeof(ShopMenuPatcher), nameof(ShopMenuPatcher.performAction_Postfix))
            );

        }

        public static void openShopMenu_Postfix(GameLocation __instance, string which) 
        {
            try
            {
                GameLocation location = Game1.currentLocation;

                if (which.Equals("Fish"))
                {
                    if (ModEntry.StolenToday == true)
                    {
                        //return true;
                    }

                    else if (location.getCharacterFromName("Willy") != null && location.getCharacterFromName("Willy").getTileLocation().Y < (float)Game1.player.getTileY())
                    {
                        Game1.activeClickableMenu = new ShopMenu(Utility.getFishShopStock(Game1.player), 0, "Willy");
                    }
                    else
                    {
                        Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(3, "FishShop"), 3, null);
                    }
                }

                if (location is SeedShop)
                {
                    if (ModEntry.StolenToday == true)
                    {
                        //return true;
                    }

                    else
                    {
                        if (location.getCharacterFromName("Pierre") != null && location.getCharacterFromName("Pierre").getTileLocation().Equals(new Vector2(4f, 17f)) && Game1.player.getTileY() > location.getCharacterFromName("Pierre").getTileY())
                        {
                            return;
                        }
                        else if (location.getCharacterFromName("Pierre") == null && Game1.IsVisitingIslandToday("Pierre"))
                        {
                            Game1.dialogueUp = false;
                            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:SeedShop_MoneyBox"));
                            Game1.afterDialogues = delegate
                            {
                                Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(5, "SeedShop"), 3, null);
                                //ModEntry.StolenToday = true;
                            };
                        }
                        else
                        {
                            Game1.dialogueUp = false;
                            Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(5, "SeedShop"), 3, null);
                            //ModEntry.StolenToday = true;
                        }
                    }
                }
            }
            catch(Exception e)
            {
                monitor.Log($"Failed to patch openShopMenu... Details\n{e}", LogLevel.Error);
            }			
        }

        public static void performAction_Postfix(GameLocation __instance, string action, Farmer who, Location tileLocation)
        {
            try
            {
                if (action != null && who.IsLocalPlayer)
                {
                    GameLocation location = Game1.currentLocation;
                    string[] actionParams = action.Split(' ');
                    switch (actionParams[0])
                    {
                        case "HospitalShop":
                            if (__instance.isCharacterAtTile(who.getTileLocation() + new Vector2(0f, -2f)) == null || __instance.isCharacterAtTile(who.getTileLocation() + new Vector2(-1f, -2f)) == null && ModEntry.StolenToday == false)
                            {
                                Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(1, "HospitalShop"), 3, null);
                                //ModEntry.StolenToday = true;
                            }
                            break;
                        case "Carpenter":
                            if (who.getTileY() > tileLocation.Y)
                            {
                                if (__instance.isCharacterAtTile(who.getTileLocation() + new Vector2(0f, -2f)) == null || __instance.isCharacterAtTile(who.getTileLocation() + new Vector2(-1f, -2f)) == null && ModEntry.StolenToday == false)
                                {
                                    Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(5, "Carpenters"), 3, null);
                                    //ModEntry.StolenToday = true;
                                }
                                if (location.getCharacterFromName("Robin") == null && Game1.IsVisitingIslandToday("Robin") && ModEntry.StolenToday == false)
                                {
                                    Game1.dialogueUp = false;
                                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_MoneyBox"));
                                    Game1.afterDialogues = delegate
                                    {
                                        Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(5, "Carpenters"), 3, null);
                                        //ModEntry.StolenToday = true;
                                    };
                                }
                                if (Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth).Equals("Tue") && ModEntry.StolenToday == false)
                                {
                                    Game1.dialogueUp = false;
                                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_RobinAbsent").Replace('\n', '^'));
                                    Game1.afterDialogues = delegate
                                    {
                                        Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(5, "Carpenters"), 3, null);
                                        //ModEntry.StolenToday = true;
                                    };
                                }
                            }
                            break;
                        case "AnimalShop":
                            if (who.getTileY() > tileLocation.Y)
                            {
                                if (__instance.isCharacterAtTile(who.getTileLocation() + new Vector2(0f, -2f)) == null || __instance.isCharacterAtTile(who.getTileLocation() + new Vector2(-1f, -2f)) == null && ModEntry.StolenToday == false)
                                {
                                    Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(5, "Carpenters"), 3, null);
                                }
                                if (location.getCharacterFromName("Marnie") == null && Game1.IsVisitingIslandToday("Marnie") && ModEntry.StolenToday == false)
                                {
                                    Game1.dialogueUp = false;
                                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:AnimalShop_MoneyBox"));
                                    Game1.afterDialogues = delegate
                                    {
                                        Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(2, "AnimalShop"), 3, null);
                                        //ModEntry.StolenToday = true;
                                    };
                                }
                                if (Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth).Equals("Tue") && ModEntry.StolenToday == false)
                                {
                                    Game1.dialogueUp = false;
                                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:AnimalShop_Marnie_Absent").Replace('\n', '^'));
                                    Game1.afterDialogues = delegate
                                    {
                                        Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(1 ,"AnimalShop"), 3, null);
                                        //ModEntry.StolenToday = true;
                                    };
                                }
                            }
                            break;
                        case "Blacksmith":
                            if (location.blacksmith(tileLocation) == false && ModEntry.StolenToday == false)
                            {
                                Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(4, "Blacksmith"), 3, null);
                                //ModEntry.StolenToday = true;
                            }
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                monitor.Log($"Failed to patch performAction... Details\n{e}", LogLevel.Error);
            }            
        }
    }
}
