using System;
using System.Collections.Generic;
using StardewValley.Locations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            monitor.Log("Initialising harmony patches...");

            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.openShopMenu)),
                postfix: new HarmonyMethod(typeof(ShopMenuPatcher), nameof(ShopMenuPatcher.openShopMenu_Postfix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.performAction)),
                postfix: new HarmonyMethod(typeof(ShopMenuPatcher), nameof(ShopMenuPatcher.performAction_Postfix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.performAction)),
               prefix: new HarmonyMethod(typeof(ShopMenuPatcher), nameof(ShopMenuPatcher.performAction_Prefix))
           );           
        }

        public static void openShopMenu_Postfix(GameLocation __instance, string which) 
        {
            try
            {
                if (which.Equals("Fish") && ModEntry.StolenToday == false)
                {
                    ShopMenuUtilities.FishShopShopliftingMenu(__instance);
                    
                }

                else if (__instance is SeedShop && ModEntry.StolenToday == false)
                {
                    ShopMenuUtilities.SeedShopShopliftingMenu(__instance);
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
                    string[] actionParams = action.Split(' ');
                    switch (actionParams[0])
                    {
                        case "HospitalShop":
                            ShopMenuUtilities.HospitalShopliftingMenu(__instance, who);
                            break;

                        case "Carpenter":
                            ShopMenuUtilities.CarpenterShopliftingMenu(__instance, who, tileLocation);
                            break;

                        case "AnimalShop":
                            ShopMenuUtilities.AnimalShopShopliftingMenu(__instance, who, tileLocation);
                            break;

                        case "Blacksmith":
                            ShopMenuUtilities.BlacksmithShopliftingMenu(__instance, tileLocation);
                            break;

                        case "Saloon":
                            ShopMenuUtilities.SaloonShopliftingMenu(__instance);
                            break;
                    }
                }
            }

            catch (Exception e)
            {
                monitor.Log($"Failed to patch performActionpostfix... Details\n{e}", LogLevel.Error);
            }            
        }

        public static bool performAction_Prefix(string action, Farmer who)
        {
            try
            {
                if (action != null && who.IsLocalPlayer)
                {
                    string[] actionParams = action.Split(' ');
                    switch (actionParams[0])
                    {
                        case "LockedDoorWarp":
                            if(ModEntry.PerScreenShopsBannedFrom.Value.Contains(actionParams[3]))
                            {
                                if (!ModEntry.shopliftingstrings.ContainsKey("Placeholder"))
                                {
                                    Game1.drawObjectDialogue(ModEntry.shopliftingstrings["TheMightyAmondee.Shoplifter/Banned"]);
                                }
                                else
                                {
                                    Game1.drawObjectDialogue(ModEntry.shopliftingstrings["Placeholder"]);
                                }
                                
                                return false;
                            }
                            return true;                           
                    }
                    
                }
                return true;
            }
            catch (Exception e)
            {
                monitor.Log($"Failed to patch performActionprefix... Details\n{e}", LogLevel.Error);
                return true;
            }
        }
    }
}
