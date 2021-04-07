using System;
using StardewValley.Locations;
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
        private static IManifest manifest;

        private static int fineamount;
      
        public static void gethelpers(IMonitor monitor, IModHelper helper, IManifest manifest)
        {
            ShopMenuUtilities.monitor = monitor;
            ShopMenuUtilities.helper = helper;
            ShopMenuUtilities.manifest = manifest;
        }

        /// <summary>
        /// Applies shoplifting penalties, tracks whether to ban player
        /// </summary>
        /// <param name="__instance">The current location instance</param>
        public static void ShopliftingPenalties(GameLocation __instance)
        {
            // Subtract monetary penalty if it applies
            if (fineamount > 0)
            {
                Game1.player.Money -= fineamount;
            }

            string location = __instance.NameOrUniqueName;

            var data = Game1.player.modData;
            
            // Add a one to the end of the value for number of bans, each 1 is one time being caught shoplifting
            data[$"{manifest.UniqueID}_{location}"] = int.Parse(data[$"{manifest.UniqueID}_{location}"]) + 1.ToString();

            // After being caught three times "111" ban player from shop for three days, excluding day of ban
            if (data[$"{manifest.UniqueID}_{location}"] == "111")
            {
                data[$"{manifest.UniqueID}_{location}"] = "-1";
                Game1.warpFarmer(__instance.warps[0].TargetName, __instance.warps[0].TargetX, __instance.warps[0].TargetY, false);
                ModEntry.PerScreenShopsBannedFrom.Value.Add($"{location}");
                monitor.Log($"{location} added to banned shop list");
            }
        }


        /// <summary>
        /// Subtracts friendship from any npc that sees the player shoplifting
        /// </summary>
        /// <param name="__instance">The current location instance</param>
        /// <param name="who">The player</param>
        public static void SeenShoplifting(GameLocation __instance, Farmer who)
        {
            foreach(NPC i in __instance.characters)
            {
                // Is NPC in range?
                if (i.currentLocation == who.currentLocation && Utility.tileWithinRadiusOfPlayer(i.getTileX(), i.getTileY(), 7, who))
                {
                    // Emote NPC
                    i.doEmote(12, false, false);

                    // Has player met NPC?
                    if (Game1.player.friendshipData.ContainsKey(i.Name) == true)
                    {
                        // Lower friendship by 500 or frienship level, whichever is lower
                        int frienshiploss = -Math.Min(500, Game1.player.getFriendshipLevelForNPC(i.Name));
                        Game1.player.changeFriendship(frienshiploss, Game1.getCharacterFromName(i.Name, true));
                        monitor.Log($"{i.Name} saw you shoplifting... {-frienshiploss} friendship points lost");
                    }

                    else
                    {
                        monitor.Log($"{i.Name} saw you shoplifting... You've never talked to {i.Name}, no friendship to lose");
                    }
                }               
            }
        }

        /// <summary>
        /// Determines whether the player is caught, makes corrections to dialogue based on return value
        /// </summary>
        /// <param name="which">Who should catch the player</param>
        /// <param name="who">The player to catch</param>
        /// <returns>Whether the player was caught</returns>      
        public static bool ShouldBeCaught(string which, Farmer who)
        {
            NPC npc = Game1.getCharacterFromName(which);
            
            // Is NPC in range?
            if (npc != null && npc.currentLocation == who.currentLocation && Utility.tileWithinRadiusOfPlayer(npc.getTileX(), npc.getTileY(), 7, who))
            {
                string dialogue;
                fineamount = Math.Min(Game1.player.Money, 1000); 
                
                try
                {
                    // Is NPC primary shopowner
                    if (which == "Pierre" || which == "Willy" || which == "Robin" || which == "Marnie" || which == "Gus" || which == "Harvey" || which == "Clint")
                    {
                        // Yes, they have special dialogue

                        dialogue = (fineamount > 0)
                            ? ModEntry.shopliftingstrings[$"TheMightyAmondee.Shoplifter/Caught{which}"].Replace("{0}", fineamount.ToString())
                            : ModEntry.shopliftingstrings[$"TheMightyAmondee.Shoplifter/Caught{which}_NoMoney"];
                    }

                    else
                    {
                        // No, use generic dialogue
                        dialogue = (fineamount > 0)
                            ? ModEntry.shopliftingstrings[$"TheMightyAmondee.Shoplifter/CaughtGeneric"].Replace("{0}", fineamount.ToString())
                            : ModEntry.shopliftingstrings[$"TheMightyAmondee.Shoplifter/CaughtGeneric_NoMoney"];
                    }

                    npc.setNewDialogue(dialogue, add: true);
                }
                catch
                {
                    npc.setNewDialogue(ModEntry.shopliftingstrings["Placeholder"], add: true);
                }

                // Draw dialogue for NPC, dialogue box opens
                Game1.drawDialogue(npc);                
                monitor.Log($"{which} caught you shoplifting... You're banned from their shop for the day");

                return true;
            }

            return false;
        }

        /// <summary>
        /// Create the shoplifting menu with Fishshop stock if necessary
        /// </summary>
        /// <param name="__instance">The current location instance</param>
        public static void FishShopShopliftingMenu(GameLocation __instance)
        {
            // Willy can sell, don't do anything
            if (__instance.getCharacterFromName("Willy") != null && __instance.getCharacterFromName("Willy").getTileLocation().Y < (float)Game1.player.getTileY())
            {
                return;
            }

            // Player can steal
            else if (ModEntry.PerScreenStolenToday.Value == false)
            {
                // Create option to steal
                __instance.createQuestionDialogue("Shoplift?", __instance.createYesNoResponses(), delegate (Farmer _, string answer)
                {
                    // Player answered yes
                    if (answer == "Yes")
                    {
                        SeenShoplifting(__instance, Game1.player);

                        // Player is caught
                        if (ShouldBeCaught("Willy", Game1.player) == true)
                        {
                            // After dialogue, ban player from shop
                            Game1.afterDialogues = delegate
                            {
                                if (fineamount > 0)
                                {
                                    Game1.player.Money -= fineamount;
                                }
                                Game1.warpFarmer(__instance.warps[0].TargetName, __instance.warps[0].TargetX, __instance.warps[0].TargetY, false);
                                ModEntry.PerScreenShopsBannedFrom.Value.Add("FishShop");
                                monitor.Log("Fishshop added to banned shop list");
                            };

                            return;
                        }

                        // Not caught, generate stock for shoplifting, on purchase make sure player can't steal again
                        Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(3, 3, "FishShop"), 3, null, delegate
                        {
                            ModEntry.PerScreenStolenToday.Value = true;
                            return false;
                        }, null, "");
                    }
                });                               
            }
            
            // Player can't steal and Willy can't sell
            else
            {
                if (ModEntry.shopliftingstrings.ContainsKey("TheMightyAmondee.Shoplifter/AlreadyShoplifted") == true)
                {
                    Game1.drawObjectDialogue(ModEntry.shopliftingstrings["TheMightyAmondee.Shoplifter/AlreadyShoplifted"]);
                }

                else
                {
                    Game1.drawObjectDialogue(ModEntry.shopliftingstrings["Placeholder"]);
                }
            }
        }

        /// <summary>
        /// Create the shoplifting menu with Seedshop stock if necessary
        /// </summary>
        /// <param name="__instance">The current location instance</param>
        public static void SeedShopShopliftingMenu(GameLocation __instance)
        {
            // Pierre can sell
            if (__instance.getCharacterFromName("Pierre") != null && __instance.getCharacterFromName("Pierre").getTileLocation().Equals(new Vector2(4f, 17f)) && Game1.player.getTileY() > __instance.getCharacterFromName("Pierre").getTileY())
            {
                return;
            }

            // Pierre is not at shop and on island, player can purchase stock properly or steal
            else if (__instance.getCharacterFromName("Pierre") == null && Game1.IsVisitingIslandToday("Pierre"))
            {
                Game1.dialogueUp = false;
                Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:SeedShop_MoneyBox"));
                Game1.afterDialogues = delegate
                {
                    __instance.createQuestionDialogue("Shoplift?", __instance.createYesNoResponses(), delegate (Farmer _, string answer)
                    {
                        SeenShoplifting(__instance, Game1.player);

                        if (answer == "Yes")
                        {
                            if (ShouldBeCaught("Pierre", Game1.player) == true || ShouldBeCaught("Caroline", Game1.player) == true || ShouldBeCaught("Abigail", Game1.player) == true)
                            {
                                Game1.afterDialogues = delegate
                                {
                                    ShopliftingPenalties(__instance);
                                };

                                return;
                            }

                            Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(5, 5, "SeedShop"), 3, null, delegate
                            {
                                ModEntry.PerScreenStolenToday.Value = true;
                                return false;
                            }, null, "");
                        }

                        else
                        {
                            Game1.activeClickableMenu = new ShopMenu((__instance as SeedShop).shopStock());
                        }
                    });
                };
            }

            // Pierre not at counter, player can steal
            else
            {
                Game1.dialogueUp = false;
                __instance.createQuestionDialogue("Shoplift?", __instance.createYesNoResponses(), delegate (Farmer _, string answer)
                {
                    SeenShoplifting(__instance, Game1.player);

                    if (answer == "Yes")
                    {
                        if (ShouldBeCaught("Pierre", Game1.player) == true || ShouldBeCaught("Caroline", Game1.player) == true || ShouldBeCaught("Abigail", Game1.player) == true)
                        {
                            Game1.afterDialogues = delegate
                            {
                                ShopliftingPenalties(__instance);
                            };

                            return;
                        }

                        Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(5, 5, "SeedShop"), 3, null, delegate
                        {
                            ModEntry.PerScreenStolenToday.Value = true;
                            return false;
                        }, null, "");
                    }
                });
            }
        }

        /// <summary>
        /// Create the shoplifting menu with Carpenter stock if necessary
        /// </summary>
        /// <param name="__instance">The current location instance</param>
        /// <param name="who">The player</param>
        /// <param name="tileLocation">The clicked tilelocation</param>
        public static void CarpenterShopliftingMenu(GameLocation __instance, Farmer who, Location tileLocation)
        {
            // Player is in correct position for buying
            if (who.getTileY() > tileLocation.Y)
            {
                // Player can steal
                if (ModEntry.PerScreenStolenToday.Value == false)
                {
                    // Robin is on island and not at sciencehouse, she can't sell but player can purchase properly if they want
                    if (__instance.getCharacterFromName("Robin") == null && Game1.IsVisitingIslandToday("Robin"))
                    {
                        // Close any current dialogue boxes
                        Game1.dialogueUp = false;
                        // Show normal dialogue
                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_MoneyBox"));
                        // Create question to shoplift after dialogue box is exited
                        Game1.afterDialogues = delegate
                        {
                            // Create question dialogue
                            __instance.createQuestionDialogue("Shoplift?", __instance.createYesNoResponses(), delegate (Farmer _, string answer)
                            {
                                // Player answered yes
                                if (answer == "Yes")
                                {
                                    // All NPCs in area lose friendship
                                    SeenShoplifting(__instance, Game1.player);

                                    // Should player be caught by any shopowner?
                                    if (ShouldBeCaught("Robin", Game1.player) == true || ShouldBeCaught("Demetrius", Game1.player) == true || ShouldBeCaught("Maru", Game1.player) == true || ShouldBeCaught("Sebastian", Game1.player) == true)
                                    {
                                        // Yes, ban player from shop and show getting caught dialogue
                                        Game1.afterDialogues = delegate
                                        {
                                            if (fineamount > 0)
                                            {
                                                Game1.player.Money -= fineamount;
                                            }
                                            Game1.warpFarmer(__instance.warps[0].TargetName, __instance.warps[0].TargetX, __instance.warps[0].TargetY, false);
                                            ModEntry.PerScreenShopsBannedFrom.Value.Add("ScienceHouse");
                                            monitor.Log("ScienceHouse added to banned shop list");
                                        };

                                        // Leave method early
                                        return;
                                    }

                                    // Not caught, show shoplifting menu
                                    Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(2, 20, "Carpenters"), 3, null, delegate
                                    {
                                        // On purchase, make sure player can not steal again
                                        ModEntry.PerScreenStolenToday.Value = true;
                                        return false;
                                    }, null, "");
                                }

                                // Player answered no, bring up normal shop menu
                                else
                                {
                                    Game1.activeClickableMenu = new ShopMenu(Utility.getCarpenterStock());
                                }
                            });
                        };
                    }

                    // Robin is absent and can't sell, player can steal
                    else if (Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth).Equals("Tue") && __instance.carpenters(tileLocation) == true && __instance.getCharacterFromName("Robin") == null)
                    {
                        Game1.dialogueUp = false;
                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:ScienceHouse_RobinAbsent").Replace('\n', '^'));
                        Game1.afterDialogues = delegate
                        {
                            __instance.createQuestionDialogue("Shoplift?", __instance.createYesNoResponses(), delegate (Farmer _, string answer)
                            {
                                if (answer == "Yes")
                                {
                                    SeenShoplifting(__instance, Game1.player);

                                    if (ShouldBeCaught("Robin", Game1.player) == true || ShouldBeCaught("Demetrius", Game1.player) == true || ShouldBeCaught("Maru", Game1.player) == true || ShouldBeCaught("Sebastian", Game1.player) == true)
                                    {
                                        Game1.afterDialogues = delegate
                                        {
                                            if (fineamount > 0)
                                            {
                                                Game1.player.Money -= fineamount;
                                            }
                                            Game1.warpFarmer(__instance.warps[0].TargetName, __instance.warps[0].TargetX, __instance.warps[0].TargetY, false);
                                            ModEntry.PerScreenShopsBannedFrom.Value.Add("ScienceHouse");
                                            monitor.Log("ScienceHouse added to banned shop list");
                                        };

                                        return;
                                    }

                                    Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(2, 20, "Carpenters"), 3, null, delegate
                                    {
                                        ModEntry.PerScreenStolenToday.Value = true;
                                        return false;
                                    }, null, "");
                                }
                            });
                        };

                    }

                    // Robin can't sell. Period
                    else if (__instance.carpenters(tileLocation) == false)
                    {
                        __instance.createQuestionDialogue("Shoplift?", __instance.createYesNoResponses(), delegate (Farmer _, string answer)
                        {
                            if (answer == "Yes")
                            {
                                SeenShoplifting(__instance, Game1.player);

                                if (ShouldBeCaught("Robin", Game1.player) == true || ShouldBeCaught("Demetrius", Game1.player) == true || ShouldBeCaught("Maru", Game1.player) == true || ShouldBeCaught("Sebastian", Game1.player) == true)
                                {
                                    Game1.afterDialogues = delegate
                                    {
                                        if (fineamount > 0)
                                        {
                                            Game1.player.Money -= fineamount;
                                        }
                                        Game1.warpFarmer(__instance.warps[0].TargetName, __instance.warps[0].TargetX, __instance.warps[0].TargetY, false);
                                        ModEntry.PerScreenShopsBannedFrom.Value.Add("ScienceHouse");
                                        monitor.Log("ScienceHouse added to banned shop list");
                                    };

                                    return;
                                }

                                Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(2, 20, "Carpenters"), 3, null, delegate
                                {
                                    ModEntry.PerScreenStolenToday.Value = true;
                                    return false;
                                }, null, "");
                            }
                        });
                    }
                }

                // Robin can sell and player can't steal
                else if (__instance.carpenters(tileLocation) == true && ModEntry.PerScreenStolenToday.Value == true)
                {
                    return;
                }

                // Robin can't sell and player can't steal
                else
                {
                    if (ModEntry.shopliftingstrings.ContainsKey("TheMightyAmondee.Shoplifter/AlreadyShoplifted") == true)
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

        /// <summary>
        /// Create the shoplifting menu with AnimalShop stock if necessary
        /// </summary>
        /// <param name="__instance">The current location instance</param>
        /// <param name="who">The player</param>
        /// <param name="tileLocation">The clicked tilelocation</param>
        public static void AnimalShopShopliftingMenu(GameLocation __instance, Farmer who, Location tileLocation)
        {
            // Player is in correct position for buying
            if (who.getTileY() > tileLocation.Y)
            {
                // Player can steal
                if (ModEntry.PerScreenStolenToday.Value == false)
                {
                    // Marnie is not in the location, she is on the island
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
                                    SeenShoplifting(__instance, Game1.player);

                                    if (ShouldBeCaught("Marnie", Game1.player) == true || ShouldBeCaught("Shane", Game1.player) == true)
                                    {
                                        Game1.afterDialogues = delegate
                                        {
                                            if (fineamount > 0)
                                            {
                                                Game1.player.Money -= fineamount;
                                            }
                                            Game1.warpFarmer(__instance.warps[0].TargetName, __instance.warps[0].TargetX, __instance.warps[0].TargetY, false);
                                            ModEntry.PerScreenShopsBannedFrom.Value.Add("AnimalShop");
                                            monitor.Log("AnimalShop added to banned shop list");
                                        };

                                        return;
                                    }

                                    Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(1, 15, "AnimalShop"), 3, null, delegate
                                    {
                                        ModEntry.PerScreenStolenToday.Value = true;
                                        return false;
                                    }, null, "");
                                }

                                else
                                {
                                    Game1.activeClickableMenu = new ShopMenu(Utility.getAnimalShopStock());
                                }
                            });

                        };
                    }

                    // Marnie is not at the location and is absent for the day
                    else if (Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth).Equals("Tue") && __instance.animalShop(tileLocation) == true && __instance.getCharacterFromName("Marnie") == null)
                    {
                        Game1.dialogueUp = false;
                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:AnimalShop_Marnie_Absent").Replace('\n', '^'));
                        Game1.afterDialogues = delegate
                        {
                            __instance.createQuestionDialogue("Shoplift?", __instance.createYesNoResponses(), delegate (Farmer _, string answer)
                            {
                                if (answer == "Yes")
                                {
                                    SeenShoplifting(__instance, Game1.player);

                                    if (ShouldBeCaught("Marnie", Game1.player) == true || ShouldBeCaught("Shane", Game1.player) == true)
                                    {
                                        Game1.afterDialogues = delegate
                                        {
                                            if (fineamount > 0)
                                            {
                                                Game1.player.Money -= fineamount;
                                            }
                                            Game1.warpFarmer(__instance.warps[0].TargetName, __instance.warps[0].TargetX, __instance.warps[0].TargetY, false);
                                            ModEntry.PerScreenShopsBannedFrom.Value.Add("AnimalShop");
                                            monitor.Log("AnimalShop added to banned shop list");
                                        };

                                        return;
                                    }

                                    Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(1, 15, "AnimalShop"), 3, null, delegate
                                    {
                                        ModEntry.PerScreenStolenToday.Value = true;
                                        return false;
                                    }, null, "");
                                }
                            });
                        };
                    }

                    // Marnie can't sell. Period.
                    else if (__instance.animalShop(tileLocation) == false)
                    {
                        __instance.createQuestionDialogue("Shoplift?", __instance.createYesNoResponses(), delegate (Farmer _, string answer)
                        {
                            if (answer == "Yes")
                            {
                                SeenShoplifting(__instance, Game1.player);

                                if (ShouldBeCaught("Marnie", Game1.player) == true || ShouldBeCaught("Shane", Game1.player) == true)
                                {
                                    Game1.afterDialogues = delegate
                                    {
                                        if (fineamount > 0)
                                        {
                                            Game1.player.Money -= fineamount;
                                        }
                                        Game1.warpFarmer(__instance.warps[0].TargetName, __instance.warps[0].TargetX, __instance.warps[0].TargetY, false);
                                        ModEntry.PerScreenShopsBannedFrom.Value.Add("AnimalShop");
                                        monitor.Log("AnimalShop added to banned shop list");
                                    };

                                    return;
                                }

                                Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(1, 15, "AnimalShop"), 3, null, delegate
                                {
                                    ModEntry.PerScreenStolenToday.Value = true;
                                    return false;
                                }, null, "");
                            }
                        });
                    }
                }

                // Marnie can sell and player can't steal
                else if (__instance.animalShop(tileLocation) == true && ModEntry.PerScreenStolenToday.Value == true)
                {
                    return;
                }

                else
                {
                    if (ModEntry.shopliftingstrings.ContainsKey("TheMightyAmondee.Shoplifter/AlreadyShoplifted") == true)
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

        /// <summary>
        /// Create the shoplifting menu with Hospital stock if necessary
        /// </summary>
        /// <param name="__instance">The current location instance</param>
        /// <param name="who">The player</param>
        public static void HospitalShopliftingMenu(GameLocation __instance, Farmer who)
        {
            // Character is not at the required tile, noone can sell
            if (__instance.isCharacterAtTile(who.getTileLocation() + new Vector2(0f, -2f)) == null && __instance.isCharacterAtTile(who.getTileLocation() + new Vector2(-1f, -2f)) == null)
            {
                if (ModEntry.PerScreenStolenToday.Value == false)
                {
                    __instance.createQuestionDialogue("Shoplift?", __instance.createYesNoResponses(), delegate (Farmer _, string answer)
                    {
                        SeenShoplifting(__instance, Game1.player);

                        if (answer == "Yes")
                        {
                            if (ShouldBeCaught("Harvey", Game1.player) == true || ShouldBeCaught("Maru", Game1.player) == true)
                            {
                                Game1.afterDialogues = delegate
                                {
                                    if (fineamount > 0)
                                    {
                                        Game1.player.Money -= fineamount;
                                    }
                                    Game1.warpFarmer(__instance.warps[0].TargetName, __instance.warps[0].TargetX, __instance.warps[0].TargetY, false);
                                    ModEntry.PerScreenShopsBannedFrom.Value.Add("Hospital");
                                    monitor.Log("Hospital added to banned shop list");
                                };

                                return;
                            }

                            Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(1, 3, "HospitalShop"), 3, null, delegate
                            {
                                ModEntry.PerScreenStolenToday.Value = true;
                                return false;
                            }, null, "");
                        }
                    });
                }

                else
                {
                    if (ModEntry.shopliftingstrings.ContainsKey("TheMightyAmondee.Shoplifter/AlreadyShoplifted") == true)
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

        /// <summary>
        /// Create the shoplifting menu with Blacksmith stock if necessary
        /// </summary>
        /// <param name="__instance">The current location instance</param>
        /// <param name="tileLocation">The clicked tilelocation</param>
        public static void BlacksmithShopliftingMenu(GameLocation __instance, Location tileLocation)
        {
            // Clint can't sell. Period.
            if (__instance.blacksmith(tileLocation) == false)
            {
                if (ModEntry.PerScreenStolenToday.Value == false)
                {
                    __instance.createQuestionDialogue("Shoplift?", __instance.createYesNoResponses(), delegate (Farmer _, string answer)
                    {
                        if (answer == "Yes")
                        {
                            SeenShoplifting(__instance, Game1.player);

                            if (ShouldBeCaught("Clint", Game1.player) == true)
                            {
                                Game1.afterDialogues = delegate
                                {
                                    if (fineamount > 0)
                                    {
                                        Game1.player.Money -= fineamount;
                                    }
                                    Game1.warpFarmer(__instance.warps[0].TargetName, __instance.warps[0].TargetX, __instance.warps[0].TargetY, false);
                                    ModEntry.PerScreenShopsBannedFrom.Value.Add("Blacksmith");
                                    monitor.Log("Blacksmith added to banned shop list");
                                };

                                return;
                            }

                            Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(3, 10, "Blacksmith"), 3, null, delegate
                            {
                                ModEntry.PerScreenStolenToday.Value = true;
                                return false;
                            }, null, "");
                        }
                    });
                }

                else
                {
                    if (ModEntry.shopliftingstrings.ContainsKey("TheMightyAmondee.Shoplifter/AlreadyShoplifted") == true)
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

        /// <summary>
        /// Create the shoplifting menu with Saloon stock if necessary
        /// </summary>
        /// <param name="__instance">The current location instance</param>
        public static void SaloonShopliftingMenu(GameLocation __instance, Location tilelocation)
        {
            // Gus is not in the location, he is on the island
            if (__instance.getCharacterFromName("Gus") == null && Game1.IsVisitingIslandToday("Gus"))
            {
                if (ModEntry.PerScreenStolenToday.Value == false)
                {
                    Game1.dialogueUp = false;
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\Locations:Saloon_MoneyBox"));
                    Game1.afterDialogues = delegate
                    {
                        __instance.createQuestionDialogue("Shoplift?", __instance.createYesNoResponses(), delegate (Farmer _, string answer)
                        {
                            SeenShoplifting(__instance, Game1.player);

                            if (answer == "Yes")
                            {
                                if (ShouldBeCaught("Gus", Game1.player) == true || ShouldBeCaught("Emily", Game1.player) == true)
                                {
                                    Game1.afterDialogues = delegate
                                    {
                                        if (fineamount > 0)
                                        {
                                            Game1.player.Money -= fineamount;
                                        }
                                        Game1.warpFarmer(__instance.warps[0].TargetName, __instance.warps[0].TargetX, __instance.warps[0].TargetY, false);
                                        ModEntry.PerScreenShopsBannedFrom.Value.Add("Saloon");
                                        monitor.Log("Saloon added to banned shop list");
                                    };

                                    return;
                                }

                                Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(2, 1, "Saloon"), 3, null, delegate
                                {
                                    ModEntry.PerScreenStolenToday.Value = true;
                                    return false;
                                }, null, "");
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

                // Gus can sell, player can't steal
                else if (__instance.saloon(tilelocation) == true && ModEntry.PerScreenStolenToday.Value == true)
                {
                    return;
                }

                else
                {
                    if (ModEntry.shopliftingstrings.ContainsKey("TheMightyAmondee.Shoplifter/AlreadyShoplifted") == true)
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

            // Gus can't sell. Period.
            else if (__instance.saloon(tilelocation) == false)
            {
                if (ModEntry.PerScreenStolenToday.Value == false)
                {
                    __instance.createQuestionDialogue("Shoplift?", __instance.createYesNoResponses(), delegate (Farmer _, string answer)
                    {
                        SeenShoplifting(__instance, Game1.player);

                        if (answer == "Yes")
                        {
                            if (ShouldBeCaught("Gus", Game1.player) == true || ShouldBeCaught("Emily", Game1.player) == true)
                            {
                                Game1.afterDialogues = delegate
                                {
                                    if (fineamount > 0)
                                    {
                                        Game1.player.Money -= fineamount;
                                    }
                                    Game1.warpFarmer(__instance.warps[0].TargetName, __instance.warps[0].TargetX, __instance.warps[0].TargetY, false);
                                    ModEntry.PerScreenShopsBannedFrom.Value.Add("Saloon");
                                    monitor.Log("Saloon added to banned shop list");
                                };

                                return;
                            }

                            Game1.activeClickableMenu = new ShopMenu(ShopStock.generateRandomStock(2, 1, "Saloon"), 3, null, delegate
                            {
                                ModEntry.PerScreenStolenToday.Value = true;
                                return false;
                            }, null, "");
                        }
                    });
                }

                // Gus can't sell, player can't steal
                else
                {
                    if (ModEntry.shopliftingstrings.ContainsKey("TheMightyAmondee.Shoplifter/AlreadyShoplifted") == true)
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
    }
}
