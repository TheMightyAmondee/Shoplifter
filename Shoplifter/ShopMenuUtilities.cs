using System;
using System.Collections.Generic;
using StardewValley.Locations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewValley.Util;
using StardewValley.Menus;
using StardewValley;
using Microsoft.Xna.Framework;
using xTile.Dimensions;



namespace Shoplifter
{
    public class ShopMenuUtilities
    {
        private static IMonitor monitor;
        private static IModHelper helper;
      
        public static void gethelpers(IMonitor monitor, IModHelper helper)
        {
            ShopMenuUtilities.monitor = monitor;
            ShopMenuUtilities.helper = helper;
        }
       
        public static bool shouldbeCaught(string which, Farmer who)
        {
            NPC npc = Game1.getCharacterFromName(which);
            
            if (npc != null && npc.currentLocation == who.currentLocation && Utility.tileWithinRadiusOfPlayer(npc.getTileX(), npc.getTileY(), 7, who))
            {
                npc.doEmote(12, false, false);
                if(!ModEntry.shopliftingstrings.ContainsKey("Placeholder"))
                {
                    if (which == "Pierre" || which == "Willy" || which == "Robin" || which == "Marnie" || which == "Gus" || which == "Harvey" || which == "Clint")
                    {
                        npc.setNewDialogue(ModEntry.shopliftingstrings[$"TheMightyAmondee.Shoplifter/Caught{which}"], add: true);
                    }
                    else
                    {
                        npc.setNewDialogue(ModEntry.shopliftingstrings["TheMightyAmondee.Shoplifter/CaughtGeneric"], add: true);
                    }
                }
                else
                {
                    npc.setNewDialogue(ModEntry.shopliftingstrings["Placeholder"], add: true);
                }
                                
                Game1.drawDialogue(npc);

                if (Game1.player.friendshipData.ContainsKey(which) == true)
                {
                    int frienshiploss = -Math.Min(1000, Game1.player.getFriendshipLevelForNPC(which));
                    Game1.player.changeFriendship(frienshiploss, Game1.getCharacterFromName(which, true));
                    monitor.Log($"{which} caught you shoplifting... {-frienshiploss} friendship points lost");
                }
                else
                {
                    monitor.Log($"{which} caught you shoplifting... You've never talked to {which}, no friendship to lose");
                }
                return true;
            }
            return false;
        }

        public static void FishShopShopliftingMenu(GameLocation __instance)
        {
            if (__instance.getCharacterFromName("Willy") != null && __instance.getCharacterFromName("Willy").getTileLocation().Y < (float)Game1.player.getTileY())
            {
                return;
            }
            else
            {
                if (shouldbeCaught("Willy", Game1.player) == true)
                {
                    Game1.afterDialogues = delegate
                    {
                        Game1.warpFarmer(__instance.warps[0].TargetName, __instance.warps[0].TargetX, __instance.warps[0].TargetY, false);
                        ModEntry.PerScreenShopsBannedFrom.Value.Add("FishShop");
                        monitor.Log("Fishshop added to banned shop list", LogLevel.Debug);
                    };
                    return;
                }
                ModEntry.StolenToday = true;
                Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(3, 3, "FishShop"), 3, null);
            }
        }

        public static void SeedShopShopliftingMenu(GameLocation __instance)
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
                            if (shouldbeCaught("Pierre", Game1.player) == true || shouldbeCaught("Caroline", Game1.player) == true || shouldbeCaught("Abigail", Game1.player) == true)
                            {
                                Game1.afterDialogues = delegate
                                {
                                    Game1.warpFarmer(__instance.warps[0].TargetName, __instance.warps[0].TargetX, __instance.warps[0].TargetY, false);
                                    ModEntry.PerScreenShopsBannedFrom.Value.Add("SeedShop");
                                    monitor.Log("Seedshop added to banned shop list", LogLevel.Debug);
                                };
                                return;
                            }
                            ModEntry.StolenToday = true;
                            Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(5, 5, "SeedShop"), 3, null);
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
                        if (shouldbeCaught("Pierre", Game1.player) == true || shouldbeCaught("Caroline", Game1.player) == true || shouldbeCaught("Abigail", Game1.player) == true)
                        {
                            Game1.afterDialogues = delegate
                            {
                                Game1.warpFarmer(__instance.warps[0].TargetName, __instance.warps[0].TargetX, __instance.warps[0].TargetY, false);
                                ModEntry.PerScreenShopsBannedFrom.Value.Add("SeedShop");
                                monitor.Log("Seedshop added to banned shop list", LogLevel.Debug);
                            };
                            return;
                        }
                        ModEntry.StolenToday = true;
                        Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(5, 5, "SeedShop"), 3, null);
                    }
                });
            }
        }

        public static void CarpenterShopliftingMenu(GameLocation __instance, Farmer who, Location tileLocation)
        {
            if (who.getTileY() > tileLocation.Y)
            {
                if (ModEntry.StolenToday == false)
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
                                    if (shouldbeCaught("Robin", Game1.player) == true || shouldbeCaught("Demetrius", Game1.player) == true || shouldbeCaught("Maru", Game1.player) == true || shouldbeCaught("Sebastian", Game1.player) == true)
                                    {
                                        Game1.afterDialogues = delegate
                                        {
                                            Game1.warpFarmer(__instance.warps[0].TargetName, __instance.warps[0].TargetX, __instance.warps[0].TargetY, false);
                                            ModEntry.PerScreenShopsBannedFrom.Value.Add("ScienceHouse");
                                            monitor.Log("ScienceHouse added to banned shop list", LogLevel.Debug);
                                        };
                                        return;
                                    }
                                    ModEntry.StolenToday = true;
                                    Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(6, 5, "Carpenters"), 3, null);
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
                        Game1.dialogueUp = false;
                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_RobinAbsent").Replace('\n', '^'));
                        Game1.afterDialogues = delegate
                        {
                            __instance.createQuestionDialogue("Shoplift?", __instance.createYesNoResponses(), delegate (Farmer _, string answer)
                            {
                                if (answer == "Yes")
                                {
                                    if (shouldbeCaught("Robin", Game1.player) == true || shouldbeCaught("Demetrius", Game1.player) == true || shouldbeCaught("Maru", Game1.player) == true || shouldbeCaught("Sebastian", Game1.player) == true)
                                    {
                                        Game1.afterDialogues = delegate
                                        {
                                            Game1.warpFarmer(__instance.warps[0].TargetName, __instance.warps[0].TargetX, __instance.warps[0].TargetY, false);
                                            ModEntry.PerScreenShopsBannedFrom.Value.Add("ScienceHouse");
                                            monitor.Log("ScienceHouse added to banned shop list", LogLevel.Debug);
                                        };
                                        return;
                                    }
                                    ModEntry.StolenToday = true;
                                    Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(6, 5, "Carpenters"), 3, null);
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
                                if (shouldbeCaught("Robin", Game1.player) == true || shouldbeCaught("Demetrius", Game1.player) == true || shouldbeCaught("Maru", Game1.player) == true || shouldbeCaught("Sebastian", Game1.player) == true)
                                {
                                    Game1.afterDialogues = delegate
                                    {
                                        Game1.warpFarmer(__instance.warps[0].TargetName, __instance.warps[0].TargetX, __instance.warps[0].TargetY, false);
                                        ModEntry.PerScreenShopsBannedFrom.Value.Add("ScienceHouse");
                                        monitor.Log("ScienceHouse added to banned shop list", LogLevel.Debug);
                                    };
                                    return;
                                }
                                ModEntry.StolenToday = true;
                                Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(6, 5, "Carpenters"), 3, null);
                            }
                        });
                    }
                }

                else
                {
                    if (!ModEntry.shopliftingstrings.ContainsKey("Placeholder"))
                    {
                        Game1.drawObjectDialogue(ModEntry.shopliftingstrings["TheMightyAmondee.Shoplifter/AlreadyShoplifted"]);
                    }
                    else
                    {
                        Game1.drawObjectDialogue(ModEntry.shopliftingstrings["Placeholder"]);
                    }                   
                }
            }
        }

        public static void AnimalShopShopliftingMenu(GameLocation __instance, Farmer who, Location tileLocation)
        {
            if (who.getTileY() > tileLocation.Y)
            {
                if (ModEntry.StolenToday == false)
                {
                    if (__instance.getCharacterFromName("Marnie") == null && Game1.IsVisitingIslandToday("Marnie"))
                    {
                        Game1.dialogueUp = false;
                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:AnimalShop_MoneyBox"));
                        Game1.afterDialogues = delegate
                        {
                            __instance.createQuestionDialogue("Shoplift?", __instance.createYesNoResponses(), delegate (Farmer _, string answer)
                            {
                                if (answer == "Yes")
                                {
                                    if (shouldbeCaught("Marnie", Game1.player) == true || shouldbeCaught("Shane", Game1.player) == true)
                                    {
                                        Game1.afterDialogues = delegate
                                        {
                                            Game1.warpFarmer(__instance.warps[0].TargetName, __instance.warps[0].TargetX, __instance.warps[0].TargetY, false);
                                            ModEntry.PerScreenShopsBannedFrom.Value.Add("AnimalShop");
                                            monitor.Log("AnimalShop added to banned shop list", LogLevel.Debug);
                                        };
                                        return;
                                    }
                                    ModEntry.StolenToday = true;
                                    Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(1, 10, "AnimalShop"), 3, null);
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
                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:AnimalShop_Marnie_Absent").Replace('\n', '^'));
                        Game1.afterDialogues = delegate
                        {
                            __instance.createQuestionDialogue("Shoplift?", __instance.createYesNoResponses(), delegate (Farmer _, string answer)
                            {
                                if (answer == "Yes")
                                {
                                    if (shouldbeCaught("Marnie", Game1.player) == true || shouldbeCaught("Shane", Game1.player) == true)
                                    {
                                        Game1.afterDialogues = delegate
                                        {
                                            Game1.warpFarmer(__instance.warps[0].TargetName, __instance.warps[0].TargetX, __instance.warps[0].TargetY, false);
                                            ModEntry.PerScreenShopsBannedFrom.Value.Add("AnimalShop");
                                            monitor.Log("AnimalShop added to banned shop list", LogLevel.Debug);
                                        };
                                        return;
                                    }
                                    ModEntry.StolenToday = true;
                                    Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(1, 10, "AnimalShop"), 3, null);
                                }
                            });
                        };
                    }

                    else if (__instance.isCharacterAtTile(who.getTileLocation() + new Vector2(0f, -2f)) == null || __instance.isCharacterAtTile(who.getTileLocation() + new Vector2(-1f, -2f)) == null)
                    {
                        __instance.createQuestionDialogue("Shoplift?", __instance.createYesNoResponses(), delegate (Farmer _, string answer)
                        {
                            if (answer == "Yes")
                            {
                                if (shouldbeCaught("Marnie", Game1.player) == true || shouldbeCaught("Shane", Game1.player) == true)
                                {
                                    Game1.afterDialogues = delegate
                                    {
                                        Game1.warpFarmer(__instance.warps[0].TargetName, __instance.warps[0].TargetX, __instance.warps[0].TargetY, false);
                                        ModEntry.PerScreenShopsBannedFrom.Value.Add("AnimalShop");
                                        monitor.Log("AnimalShop added to banned shop list", LogLevel.Debug);
                                    };
                                    return;
                                }
                                ModEntry.StolenToday = true;
                                Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(1, 10, "AnimalShop"), 3, null);
                            }
                        });
                    }
                }

                else
                {
                    if (!ModEntry.shopliftingstrings.ContainsKey("Placeholder"))
                    {
                        Game1.drawObjectDialogue(ModEntry.shopliftingstrings["TheMightyAmondee.Shoplifter/AlreadyShoplifted"]);
                    }
                    else
                    {
                        Game1.drawObjectDialogue(ModEntry.shopliftingstrings["Placeholder"]);
                    }
                }
            }
        }
       
        public static void HospitalShopliftingMenu(GameLocation __instance, Farmer who)
        {
            if (__instance.isCharacterAtTile(who.getTileLocation() + new Vector2(0f, -2f)) == null || __instance.isCharacterAtTile(who.getTileLocation() + new Vector2(-1f, -2f)) == null)
            {
                if (ModEntry.StolenToday == false)
                {
                    __instance.createQuestionDialogue("Shoplift?", __instance.createYesNoResponses(), delegate (Farmer _, string answer)
                    {
                        if (answer == "Yes")
                        {
                            if (shouldbeCaught("Harvey", Game1.player) == true || shouldbeCaught("Maru", Game1.player) == true)
                            {
                                Game1.afterDialogues = delegate
                                {
                                    Game1.warpFarmer(__instance.warps[0].TargetName, __instance.warps[0].TargetX, __instance.warps[0].TargetY, false);
                                    ModEntry.PerScreenShopsBannedFrom.Value.Add("Hospital");
                                    monitor.Log("Hospital added to banned shop list", LogLevel.Debug);
                                };
                                return;
                            }
                            ModEntry.StolenToday = true;
                            Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(1, 4, "HospitalShop"), 3, null);
                        }
                    });
                }

                else
                {
                    if (!ModEntry.shopliftingstrings.ContainsKey("Placeholder"))
                    {
                        Game1.drawObjectDialogue(ModEntry.shopliftingstrings["TheMightyAmondee.Shoplifter/AlreadyShoplifted"]);
                    }
                    else
                    {
                        Game1.drawObjectDialogue(ModEntry.shopliftingstrings["Placeholder"]);
                    }
                }
            }
        }

        public static void BlacksmithShopliftingMenu(GameLocation __instance, Location tileLocation)
        {
            if (__instance.blacksmith(tileLocation) == false)
            {
                if (ModEntry.StolenToday == false)
                {
                    __instance.createQuestionDialogue("Shoplift?", __instance.createYesNoResponses(), delegate (Farmer _, string answer)
                    {
                        if (answer == "Yes")
                        {
                            if (shouldbeCaught("Clint", Game1.player) == true)
                            {
                                Game1.afterDialogues = delegate
                                {
                                    Game1.warpFarmer(__instance.warps[0].TargetName, __instance.warps[0].TargetX, __instance.warps[0].TargetY, false);
                                    ModEntry.PerScreenShopsBannedFrom.Value.Add("Blacksmith");
                                    monitor.Log("BlackSmith added to banned shop list", LogLevel.Debug);
                                };
                                return;
                            }
                            ModEntry.StolenToday = true;
                            Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(1, 5, "Blacksmith"), 3, null);
                        }
                    });
                }

                else
                {
                    if (!ModEntry.shopliftingstrings.ContainsKey("Placeholder"))
                    {
                        Game1.drawObjectDialogue(ModEntry.shopliftingstrings["TheMightyAmondee.Shoplifter/AlreadyShoplifted"]);
                    }
                    else
                    {
                        Game1.drawObjectDialogue(ModEntry.shopliftingstrings["Placeholder"]);
                    }
                }

            }
        }

        public static void SaloonShopliftingMenu(GameLocation __instance)
        {            
            if (__instance.getCharacterFromName("Gus") == null && Game1.IsVisitingIslandToday("Gus"))
            {
                if (ModEntry.StolenToday == false)
                {
                    Game1.dialogueUp = false;
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Saloon_MoneyBox"));
                    Game1.afterDialogues = delegate
                    {
                        __instance.createQuestionDialogue("Shoplift?", __instance.createYesNoResponses(), delegate (Farmer _, string answer)
                        {
                            if (answer == "Yes")
                            {
                                if (shouldbeCaught("Gus", Game1.player) == true || shouldbeCaught("Emily", Game1.player) == true)
                                {
                                    Game1.afterDialogues = delegate
                                    {
                                        Game1.warpFarmer(__instance.warps[0].TargetName, __instance.warps[0].TargetX, __instance.warps[0].TargetY, false);
                                        ModEntry.PerScreenShopsBannedFrom.Value.Add("Saloon");
                                        monitor.Log("Saloon added to banned shop list", LogLevel.Debug);
                                    };
                                    return;
                                }
                                ModEntry.StolenToday = true;
                                Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(2, 1, "Saloon"), 3, null);
                            }

                            else
                            {
                                Game1.activeClickableMenu = new ShopMenu(Utility.getSaloonStock(), 0, null, delegate (ISalable item, Farmer farmer, int amount)
                                {
                                    Game1.player.team.synchronizedShopStock.OnItemPurchased(SynchronizedShopStock.SynchedShop.Saloon, item, amount);
                                    return false;
                                });
                            }
                        });
                    };
                }

                else
                {
                    if (!ModEntry.shopliftingstrings.ContainsKey("Placeholder"))
                    {
                        Game1.drawObjectDialogue(ModEntry.shopliftingstrings["TheMightyAmondee.Shoplifter/AlreadyShoplifted"]);
                    }
                    else
                    {
                        Game1.drawObjectDialogue(ModEntry.shopliftingstrings["Placeholder"]);
                    }
                }              
                return;
            }

            foreach (NPC i in __instance.characters)
            {
                if (i.Name.Equals("Gus"))
                {
                    if (i.getTileY() != Game1.player.getTileY() - 1 && i.getTileY() != Game1.player.getTileY() - 2)
                    {
                        if (ModEntry.StolenToday == false)
                        {
                            __instance.createQuestionDialogue("Shoplift?", __instance.createYesNoResponses(), delegate (Farmer _, string answer)
                            {
                                if (answer == "Yes")
                                {
                                    if (shouldbeCaught("Gus", Game1.player) == true || shouldbeCaught("Emily", Game1.player) == true)
                                    {
                                        Game1.afterDialogues = delegate
                                        {
                                            Game1.warpFarmer(__instance.warps[0].TargetName, __instance.warps[0].TargetX, __instance.warps[0].TargetY, false);
                                            ModEntry.PerScreenShopsBannedFrom.Value.Add("Saloon");
                                            monitor.Log("Saloon added to banned shop list", LogLevel.Debug);
                                        };
                                        return;
                                    }
                                    ModEntry.StolenToday = true;
                                    Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(2, 1, "Saloon"), 3, null);
                                }
                            });
                        }

                        else
                        {
                            if (!ModEntry.shopliftingstrings.ContainsKey("Placeholder"))
                            {
                                Game1.drawObjectDialogue(ModEntry.shopliftingstrings["TheMightyAmondee.Shoplifter/AlreadyShoplifted"]);
                            }
                            else
                            {
                                Game1.drawObjectDialogue(ModEntry.shopliftingstrings["Placeholder"]);
                            }
                        }
                    }

                    return;
                }
            }
        }
    }
}
