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
    {
        private static IMonitor monitor;
        private static IModHelper helper;

        public static void gethelpers(IMonitor monitor, IModHelper helper)
        {
            ShopMenuPatcher.monitor = monitor;
            ShopMenuPatcher.helper = helper;
        }

        // Initialise Harmony patches
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

        /// <summary>
        /// Determines whether the player is caught when shoplifting
        /// </summary>
        /// <param name="which">Which NPC can catch the player</param>
        /// <param name="who">The player</param>
        /// <param name="whichportrait">The portrait of the NPC to use in dialogue, angry portraits differ for each NPC</param>
        /// <returns>Whether the player was caught or not</returns>
        public static bool shouldbeCaught(string which, Farmer who, int whichportrait)
        {
            NPC npc = Game1.getCharacterFromName(which);

            if (npc != null && npc.currentLocation == who.currentLocation && Utility.tileWithinRadiusOfPlayer(npc.getTileX(), npc.getTileY(), 7, who))
            {
                npc.setNewDialogue($"What do you think you're doing!? Get out!!!${whichportrait}", add: true);
                Game1.drawDialogue(npc); 
                Game1.player.changeFriendship(-Math.Min(1500, Game1.player.getFriendshipLevelForNPC(which)), Game1.getCharacterFromName(which, true));
                return true;
            }
            return false;
        }

        public static void openShopMenu_Postfix(GameLocation __instance, string which) 
        {
            try
            {

                if (which.Equals("Fish"))
                {
                    if (ModEntry.StolenToday == false)
                    {
                        if (__instance.getCharacterFromName("Willy") != null && __instance.getCharacterFromName("Willy").getTileLocation().Y < (float)Game1.player.getTileY())
                        {
                            return;
                        }
                        else
                        {
                            ModEntry.StolenToday = true;
                            if (shouldbeCaught("Willy", Game1.player, 2) == true)
                            {
                                Game1.afterDialogues = delegate
                                {
                                    Game1.warpFarmer(__instance.warps[0].TargetName, __instance.warps[0].TargetX, __instance.warps[0].TargetY, false);
                                };
                                return;
                            }
                            Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(3, "FishShop"), 3, null);
                        }
                    }
                    
                }

                if (__instance is SeedShop)
                {
                    if (ModEntry.StolenToday == false)
                    {
                        if (__instance.getCharacterFromName("Pierre") != null && __instance.getCharacterFromName("Pierre").getTileLocation().Equals(new Vector2(4f, 17f)) && Game1.player.getTileY() > __instance.getCharacterFromName("Pierre").getTileY())
                        {
                            return;
                        }
                        else if (__instance.getCharacterFromName("Pierre") == null && Game1.IsVisitingIslandToday("Pierre"))
                        {
                            Game1.dialogueUp = false;
                            Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:SeedShop_MoneyBox"));
                            Game1.afterDialogues = delegate
                            {
                                __instance.createQuestionDialogue("Shoplift?", __instance.createYesNoResponses(), delegate (Farmer _, string answer)
                                {
                                    if (answer == "Yes")
                                    {
                                        ModEntry.StolenToday = true;
                                        if (shouldbeCaught("Pierre", Game1.player,3) == true || shouldbeCaught("Caroline", Game1.player,3) == true || shouldbeCaught("Abigail", Game1.player,5) == true)
                                        {
                                            Game1.afterDialogues = delegate
                                            {
                                                Game1.warpFarmer(__instance.warps[0].TargetName, __instance.warps[0].TargetX, __instance.warps[0].TargetY, false);
                                            };
                                            return;
                                        }
                                        Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(5, "SeedShop"), 3, null);
                                    }
                                    else
                                    {
                                        Game1.activeClickableMenu = new ShopMenu((__instance as SeedShop).shopStock());
                                    }
                                });                                                              
                            };
                        }
                        else
                        {
                            Game1.dialogueUp = false;
                            __instance.createQuestionDialogue("Shoplift?", __instance.createYesNoResponses(), delegate (Farmer _, string answer)
                            {
                                if (answer == "Yes")
                                {
                                    ModEntry.StolenToday = true;
                                    if (shouldbeCaught("Pierre", Game1.player, 3) == true || shouldbeCaught("Caroline", Game1.player, 3) == true || shouldbeCaught("Abigail", Game1.player, 5) == true)
                                    {
                                        Game1.afterDialogues = delegate
                                        {
                                            Game1.warpFarmer(__instance.warps[0].TargetName, __instance.warps[0].TargetX, __instance.warps[0].TargetY, false);
                                        };
                                        return;
                                    }
                                    Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(5, "SeedShop"), 3, null);
                                }
                            });
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
                    //GameLocation location = Game1.currentLocation;
                    string[] actionParams = action.Split(' ');
                    switch (actionParams[0])
                    {
                        case "HospitalShop":
                            if (__instance.isCharacterAtTile(who.getTileLocation() + new Vector2(0f, -2f)) == null || __instance.isCharacterAtTile(who.getTileLocation() + new Vector2(-1f, -2f)) == null)
                            {
                                if(ModEntry.StolenToday == false)
                                {
                                    __instance.createQuestionDialogue("Shoplift?", __instance.createYesNoResponses(), delegate (Farmer _, string answer)
                                    {
                                        if (answer == "Yes")
                                        {                                            
                                            //ModEntry.StolenToday = true;
                                            if (shouldbeCaught("Harvey", Game1.player, 5) == true || shouldbeCaught("Maru", Game1.player, 5) == true)
                                            {
                                                Game1.afterDialogues = delegate
                                                {
                                                    Game1.warpFarmer(__instance.warps[0].TargetName, __instance.warps[0].TargetX, __instance.warps[0].TargetY, false);
                                                };                                           
                                                return;
                                            }
                                            Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(1, "HospitalShop"), 3, null);
                                        }
                                    });
                                }
                                else
                                {
                                    Game1.drawObjectDialogue("You've already shoplifted today. That's enough...");
                                }                                
                            }
                            break;

                        case "Carpenter":
                            if (who.getTileY() > tileLocation.Y)
                            {
                                if(ModEntry.StolenToday == false)
                                {
                                    if (__instance.getCharacterFromName("Robin") == null && Game1.IsVisitingIslandToday("Robin"))
                                    {
                                        Game1.dialogueUp = false;
                                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_MoneyBox"));
                                        Game1.afterDialogues = delegate
                                        {
                                            __instance.createQuestionDialogue("Shoplift?", __instance.createYesNoResponses(), delegate (Farmer _, string answer)
                                            {
                                                if (answer == "Yes")
                                                {                                                   
                                                    //ModEntry.StolenToday = true;
                                                    if (shouldbeCaught("Robin", Game1.player, 3) == true || shouldbeCaught("Demetrius", Game1.player, 4) == true || shouldbeCaught("Maru", Game1.player, 5) == true || shouldbeCaught("Sebastian", Game1.player, 5) == true)
                                                    {
                                                        Game1.afterDialogues = delegate
                                                        {
                                                            Game1.warpFarmer(__instance.warps[0].TargetName, __instance.warps[0].TargetX, __instance.warps[0].TargetY, false);
                                                        };
                                                        return;
                                                    }
                                                    Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(6, "Carpenters"), 3, null);
                                                }
                                                else
                                                {
                                                    Game1.activeClickableMenu = new ShopMenu(Utility.getCarpenterStock());
                                                }
                                            });
                                        };
                                    }

                                    else if (Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth).Equals("Tue") && __instance.carpenters(tileLocation) == true)
                                    {
                                        //ModEntry.StolenToday = true;
                                        Game1.dialogueUp = false;
                                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_RobinAbsent").Replace('\n', '^'));
                                        Game1.afterDialogues = delegate
                                        {
                                            __instance.createQuestionDialogue("Shoplift?", __instance.createYesNoResponses(), delegate (Farmer _, string answer)
                                            {
                                                if (answer == "Yes")
                                                {
                                                    //ModEntry.StolenToday = true;
                                                    if (shouldbeCaught("Robin", Game1.player, 3) == true || shouldbeCaught("Demetrius", Game1.player, 4) == true || shouldbeCaught("Maru", Game1.player, 5) == true || shouldbeCaught("Sebastian", Game1.player, 5) == true)
                                                    {
                                                        Game1.afterDialogues = delegate
                                                        {
                                                            Game1.warpFarmer(__instance.warps[0].TargetName, __instance.warps[0].TargetX, __instance.warps[0].TargetY, false);
                                                        };
                                                        return;
                                                    }
                                                    Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(6, "Carpenters"), 3, null);
                                                }
                                            });
                                        };

                                    }

                                    else if ((__instance.isCharacterAtTile(who.getTileLocation() + new Vector2(0f, -2f)) == null || __instance.isCharacterAtTile(who.getTileLocation() + new Vector2(-1f, -2f)) == null))
                                    {
                                        __instance.createQuestionDialogue("Shoplift?", __instance.createYesNoResponses(), delegate (Farmer _, string answer)
                                        {
                                            if (answer == "Yes")
                                            {
                                                //ModEntry.StolenToday = true;
                                                if (shouldbeCaught("Robin", Game1.player, 3) == true || shouldbeCaught("Demetrius", Game1.player, 4) == true || shouldbeCaught("Maru", Game1.player, 5) == true || shouldbeCaught("Sebastian", Game1.player, 5) == true)
                                                {
                                                    Game1.afterDialogues = delegate
                                                    {
                                                        Game1.warpFarmer(__instance.warps[0].TargetName, __instance.warps[0].TargetX, __instance.warps[0].TargetY, false);
                                                    };
                                                    return;
                                                }
                                                Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(6, "Carpenters"), 3, null);
                                            }
                                        });
                                    }
                                }

                                else
                                {
                                    Game1.drawObjectDialogue("You've already shoplifted today. That's enough...");
                                }
                            }
                            break;

                        case "AnimalShop":
                            if (who.getTileY() > tileLocation.Y)
                            {
                                if(ModEntry.StolenToday == false)
                                {
                                    if (__instance.getCharacterFromName("Marnie") == null && Game1.IsVisitingIslandToday("Marnie"))
                                    {
                                        Game1.dialogueUp = false;
                                        //ModEntry.StolenToday = true;
                                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:AnimalShop_MoneyBox"));
                                        Game1.afterDialogues = delegate
                                        {
                                            __instance.createQuestionDialogue("Shoplift?", __instance.createYesNoResponses(), delegate (Farmer _, string answer)
                                            {
                                                if (answer == "Yes")
                                                {
                                                    if (shouldbeCaught("Marnie", Game1.player, 4) == true || shouldbeCaught("Shane", Game1.player, 5) == true)
                                                    {
                                                        Game1.afterDialogues = delegate
                                                        {
                                                            Game1.warpFarmer(__instance.warps[0].TargetName, __instance.warps[0].TargetX, __instance.warps[0].TargetY, false);
                                                        };
                                                        return;
                                                    }
                                                    //ModEntry.StolenToday = true;
                                                    Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(1, "AnimalShop"), 3, null);
                                                }
                                                else
                                                {
                                                    Game1.activeClickableMenu = new ShopMenu(Utility.getAnimalShopStock());
                                                }
                                            });

                                        };
                                    }

                                    else if (Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth).Equals("Tue") && __instance.animalShop(tileLocation) == true)
                                    {
                                        Game1.dialogueUp = false;
                                        ModEntry.StolenToday = true;
                                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:AnimalShop_Marnie_Absent").Replace('\n', '^'));
                                        Game1.afterDialogues = delegate
                                        {
                                            __instance.createQuestionDialogue("Shoplift?", __instance.createYesNoResponses(), delegate (Farmer _, string answer)
                                            {
                                                if (answer == "Yes")
                                                {
                                                    //ModEntry.StolenToday = true;
                                                    if (shouldbeCaught("Marnie", Game1.player, 4) == true || shouldbeCaught("Shane", Game1.player, 5) == true)
                                                    {
                                                        Game1.afterDialogues = delegate
                                                        {
                                                            Game1.warpFarmer(__instance.warps[0].TargetName, __instance.warps[0].TargetX, __instance.warps[0].TargetY, false);
                                                        };
                                                        return;
                                                    }                                                    
                                                    Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(1, "AnimalShop"), 3, null);
                                                }
                                            });
                                        };
                                    }

                                    else if (__instance.isCharacterAtTile(who.getTileLocation() + new Vector2(0f, -2f)) == null || __instance.isCharacterAtTile(who.getTileLocation() + new Vector2(-1f, -2f)) == null)
                                    {
                                        //ModEntry.StolenToday = true;
                                        __instance.createQuestionDialogue("Shoplift?", __instance.createYesNoResponses(), delegate (Farmer _, string answer)
                                        {
                                            if (answer == "Yes")
                                            {
                                                if (shouldbeCaught("Marnie", Game1.player, 4) == true || shouldbeCaught("Shane", Game1.player, 5) == true)
                                                {
                                                    Game1.afterDialogues = delegate
                                                    {
                                                        Game1.warpFarmer(__instance.warps[0].TargetName, __instance.warps[0].TargetX, __instance.warps[0].TargetY, false);
                                                    };
                                                    return;
                                                }
                                                //ModEntry.StolenToday = true;
                                                Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(1, "AnimalShop"), 3, null);
                                            }
                                        });
                                    }
                                }

                                else
                                {
                                    Game1.drawObjectDialogue("You've already shoplifted today. That's enough...");
                                }
                            }
                            break;

                        case "Blacksmith":
                            if (__instance.blacksmith(tileLocation) == false)
                            {
                                if(ModEntry.StolenToday == false)
                                {
                                    __instance.createQuestionDialogue("Shoplift?", __instance.createYesNoResponses(), delegate (Farmer _, string answer)
                                    {
                                        if (answer == "Yes")
                                        {
                                            ModEntry.StolenToday = true;
                                            Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(1, "Blacksmith"), 3, null);
                                        }
                                    });
                                }

                                else
                                {
                                    Game1.drawObjectDialogue("You've already shoplifted today. That's enough...");
                                }

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
