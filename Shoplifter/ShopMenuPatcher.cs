using System;
using StardewValley.Locations;
using StardewValley;
using StardewModdingAPI;
using Harmony;
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
        }

        public static void openShopMenu_Postfix(GameLocation __instance, string which) 
        {
            try
            {
                // Is it Willy's shop?
                if (which.Equals("Fish"))
                {
                    // Yes, do something
                    ShopMenuUtilities.FishShopShopliftingMenu(__instance);                    
                }

                // Is it Pierre's shop?
                else if (__instance is SeedShop && ModEntry.PerScreenStolenToday.Value == false)
                {
                    // Yes, do something
                    ShopMenuUtilities.SeedShopShopliftingMenu(__instance);
                }
            }
            catch (Exception ex)
            {
                monitor.Log($"Failed in {nameof(openShopMenu_Postfix)}:\n{ex}", LogLevel.Error);
            }			
        }

        public static void performAction_Postfix(GameLocation __instance, string action, Farmer who, Location tileLocation)
        {
            try
            {
                // If tile has an action property, check action
                if (action != null && who.IsLocalPlayer)
                {
                    string[] actionParams = action.Split(' ');

                    // Depending on action parameter, do something
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
                            ShopMenuUtilities.SaloonShopliftingMenu(__instance, tileLocation);
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                monitor.Log($"Failed in {nameof(performAction_Postfix)}:\n{ex}", LogLevel.Error);
            }            
        }
    }
}
