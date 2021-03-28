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
       
        public static bool shouldbeCaught(string which, Farmer who, int whichportrait1, int whichportrait2)
        {
            NPC npc = Game1.getCharacterFromName(which);

            IDictionary<string, string> strings = helper.Content.Load<Dictionary<string, string>>("Strings.json", ContentSource.ModFolder);

            if (npc != null && npc.currentLocation == who.currentLocation && Utility.tileWithinRadiusOfPlayer(npc.getTileX(), npc.getTileY(), 7, who))
            {
                npc.doEmote(12, false, false);
                npc.setNewDialogue(strings["TheMightyAmondee.Shoplifter/CaughtGeneric"],Game1.player.Name, add: true);
                Game1.drawDialogue(npc);
                Game1.player.changeFriendship(-Math.Min(1500, Game1.player.getFriendshipLevelForNPC(which)), Game1.getCharacterFromName(which, true));
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
                if (shouldbeCaught("Willy", Game1.player, 2, 2) == true)
                {
                    Game1.afterDialogues = delegate
                    {
                        Game1.warpFarmer(__instance.warps[0].TargetName, __instance.warps[0].TargetX, __instance.warps[0].TargetY, false);
                        ModEntry.ShopsBannedFrom.Add("FishShop");
                    };
                    return;
                }
                ModEntry.StolenToday = true;
                Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(3, "FishShop"), 3, null);
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
                            if (shouldbeCaught("Pierre", Game1.player, 4, 3) == true || shouldbeCaught("Caroline", Game1.player, 2, 3) == true || shouldbeCaught("Abigail", Game1.player, 7, 5) == true)
                            {
                                Game1.afterDialogues = delegate
                                {
                                    Game1.warpFarmer(__instance.warps[0].TargetName, __instance.warps[0].TargetX, __instance.warps[0].TargetY, false);
                                    ModEntry.ShopsBannedFrom.Add("SeedShop");
                                };
                                return;
                            }
                            ModEntry.StolenToday = true;
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
                        if (shouldbeCaught("Pierre", Game1.player, 4, 3) == true || shouldbeCaught("Caroline", Game1.player, 2, 3) == true || shouldbeCaught("Abigail", Game1.player, 7, 5) == true)
                        {
                            Game1.afterDialogues = delegate
                            {
                                Game1.warpFarmer(__instance.warps[0].TargetName, __instance.warps[0].TargetX, __instance.warps[0].TargetY, false);
                                ModEntry.ShopsBannedFrom.Add("SeedShop");
                            };
                            return;
                        }
                        ModEntry.StolenToday = true;
                        Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(5, "SeedShop"), 3, null);
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
                                    if (shouldbeCaught("Robin", Game1.player, 2, 3) == true || shouldbeCaught("Demetrius", Game1.player, 6, 4) == true || shouldbeCaught("Maru", Game1.player, 9, 5) == true || shouldbeCaught("Sebastian", Game1.player, 2, 5) == true)
                                    {
                                        Game1.afterDialogues = delegate
                                        {
                                            Game1.warpFarmer(__instance.warps[0].TargetName, __instance.warps[0].TargetX, __instance.warps[0].TargetY, false);
                                            ModEntry.ShopsBannedFrom.Add("ScienceHouse");
                                        };
                                        return;
                                    }
                                    ModEntry.StolenToday = true;
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
                        Game1.dialogueUp = false;
                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_RobinAbsent").Replace('\n', '^'));
                        Game1.afterDialogues = delegate
                        {
                            __instance.createQuestionDialogue("Shoplift?", __instance.createYesNoResponses(), delegate (Farmer _, string answer)
                            {
                                if (answer == "Yes")
                                {
                                    if (shouldbeCaught("Robin", Game1.player, 2, 3) == true || shouldbeCaught("Demetrius", Game1.player, 6, 4) == true || shouldbeCaught("Maru", Game1.player, 9, 5) == true || shouldbeCaught("Sebastian", Game1.player, 2, 5) == true)
                                    {
                                        Game1.afterDialogues = delegate
                                        {
                                            Game1.warpFarmer(__instance.warps[0].TargetName, __instance.warps[0].TargetX, __instance.warps[0].TargetY, false);
                                            ModEntry.ShopsBannedFrom.Add("ScienceHouse");
                                        };
                                        return;
                                    }
                                    ModEntry.StolenToday = true;
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
                                if (shouldbeCaught("Robin", Game1.player, 2, 3) == true || shouldbeCaught("Demetrius", Game1.player, 6, 4) == true || shouldbeCaught("Maru", Game1.player, 9, 5) == true || shouldbeCaught("Sebastian", Game1.player, 2, 5) == true)
                                {
                                    Game1.afterDialogues = delegate
                                    {
                                        Game1.warpFarmer(__instance.warps[0].TargetName, __instance.warps[0].TargetX, __instance.warps[0].TargetY, false);
                                        ModEntry.ShopsBannedFrom.Add("ScienceHouse");
                                    };
                                    return;
                                }
                                ModEntry.StolenToday = true;
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
                                    if (shouldbeCaught("Marnie", Game1.player,4, 3) == true || shouldbeCaught("Shane", Game1.player, 10, 5) == true)
                                    {
                                        Game1.afterDialogues = delegate
                                        {
                                            Game1.warpFarmer(__instance.warps[0].TargetName, __instance.warps[0].TargetX, __instance.warps[0].TargetY, false);
                                            ModEntry.ShopsBannedFrom.Add("AnimalShop");
                                        };
                                        return;
                                    }
                                    ModEntry.StolenToday = true;
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
                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:AnimalShop_Marnie_Absent").Replace('\n', '^'));
                        Game1.afterDialogues = delegate
                        {
                            __instance.createQuestionDialogue("Shoplift?", __instance.createYesNoResponses(), delegate (Farmer _, string answer)
                            {
                                if (answer == "Yes")
                                {
                                    if (shouldbeCaught("Marnie", Game1.player, 4, 3) == true || shouldbeCaught("Shane", Game1.player, 10, 5) == true)
                                    {
                                        Game1.afterDialogues = delegate
                                        {
                                            Game1.warpFarmer(__instance.warps[0].TargetName, __instance.warps[0].TargetX, __instance.warps[0].TargetY, false);
                                            ModEntry.ShopsBannedFrom.Add("AnimalShop");
                                        };
                                        return;
                                    }
                                    ModEntry.StolenToday = true;
                                    Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(1, "AnimalShop"), 3, null);
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
                                if (shouldbeCaught("Marnie", Game1.player, 4, 3) == true || shouldbeCaught("Shane", Game1.player, 10, 5) == true)
                                {
                                    Game1.afterDialogues = delegate
                                    {
                                        Game1.warpFarmer(__instance.warps[0].TargetName, __instance.warps[0].TargetX, __instance.warps[0].TargetY, false);
                                        ModEntry.ShopsBannedFrom.Add("AnimalShop");
                                    };
                                    return;
                                }
                                ModEntry.StolenToday = true;
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
                            if (shouldbeCaught("Harvey", Game1.player, 8, 5) == true || shouldbeCaught("Maru", Game1.player, 4, 5) == true)
                            {
                                Game1.afterDialogues = delegate
                                {
                                    Game1.warpFarmer(__instance.warps[0].TargetName, __instance.warps[0].TargetX, __instance.warps[0].TargetY, false);
                                    ModEntry.ShopsBannedFrom.Add("Hospital");
                                };
                                return;
                            }
                            ModEntry.StolenToday = true;
                            Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(1, "HospitalShop"), 3, null);
                        }
                    });
                }

                else
                {
                    Game1.drawObjectDialogue("You've already shoplifted today. That's enough...");
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
        }

        public static void SaloonShopliftingMenu(GameLocation __instance)
        {
            if (__instance.getCharacterFromName("Gus") == null && Game1.IsVisitingIslandToday("Gus"))
            {
                Game1.dialogueUp = false;
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Saloon_MoneyBox"));
                Game1.afterDialogues = delegate
                {
                    __instance.createQuestionDialogue("Shoplift?", __instance.createYesNoResponses(), delegate (Farmer _, string answer)
                    {
                        if (answer == "Yes")
                        {
                            if (shouldbeCaught("Gus", Game1.player, 2, 3) == true || shouldbeCaught("Emily", Game1.player, 2, 5) == true)
                            {
                                Game1.afterDialogues = delegate
                                {
                                    Game1.warpFarmer(__instance.warps[0].TargetName, __instance.warps[0].TargetX, __instance.warps[0].TargetY, false);
                                    ModEntry.ShopsBannedFrom.Add("Saloon");
                                };
                                return;
                            }
                            ModEntry.StolenToday = true;
                            Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(2, "Saloon"), 3, null);
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

                return;
            }

            foreach (NPC i in __instance.characters)
            {
                if (i.Name.Equals("Gus"))
                {
                    if (i.getTileY() != Game1.player.getTileY() - 1 && i.getTileY() != Game1.player.getTileY() - 2)
                    {
                        __instance.createQuestionDialogue("Shoplift?", __instance.createYesNoResponses(), delegate (Farmer _, string answer)
                        {
                            if (answer == "Yes")
                            {
                                if (shouldbeCaught("Gus", Game1.player, 2, 3) == true || shouldbeCaught("Emily", Game1.player, 2, 5) == true)
                                {
                                    Game1.afterDialogues = delegate
                                    {
                                        Game1.warpFarmer(__instance.warps[0].TargetName, __instance.warps[0].TargetX, __instance.warps[0].TargetY, false);
                                        ModEntry.ShopsBannedFrom.Add("Saloon");
                                    };
                                    return;
                                }
                                ModEntry.StolenToday = true;
                                Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(2, "Saloon"), 3, null);
                            }
                        });
                    }

                    return;
                }
            }
        }
    }
}
