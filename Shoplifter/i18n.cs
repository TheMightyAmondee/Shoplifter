using System;
using StardewValley;
using StardewModdingAPI;


namespace Shoplifter
{
    /// <summary>
    /// Class for determining translations
    /// </summary>
    internal static class i18n
    {
        private static ITranslationHelper translation;
        private static ModConfig config;
        public static void gethelpers(ITranslationHelper translation, ModConfig config)
        {
            i18n.translation = translation;
            i18n.config = config;
        }

        // Translations for GMCM
        public static string string_GMCM_PeriodSection()
        {
            return i18n.GetTranslation("TheMightyAmondee.Shoplifter/GMCM_PeriodSection");
        }
        public static string string_GMCM_MaxDay()
        {
            return i18n.GetTranslation("TheMightyAmondee.Shoplifter/GMCM_MaxDay");
        }
        public static string string_GMCM_MaxDayTooltip()
        {
            return i18n.GetTranslation("TheMightyAmondee.Shoplifter/GMCM_MaxDayTooltip");
        }
        public static string string_GMCM_MaxShop()
        {
            return i18n.GetTranslation("TheMightyAmondee.Shoplifter/GMCM_MaxShop");
        }
        public static string string_GMCM_MaxShopTooltip()
        {
            return i18n.GetTranslation("TheMightyAmondee.Shoplifter/GMCM_MaxShopTooltip");
        }
        public static string string_GMCM_PenaltySection()
        {
            return i18n.GetTranslation("TheMightyAmondee.Shoplifter/GMCM_PenaltySection");
        }
        public static string string_GMCM_MaxFine()
        {
            return i18n.GetTranslation("TheMightyAmondee.Shoplifter/GMCM_MaxFine");
        }
        public static string string_GMCM_MaxFineTooltip()
        {
            return i18n.GetTranslation("TheMightyAmondee.Shoplifter/GMCM_MaxFineTooltip");
        }
        public static string string_GMCM_MaxFriendship()
        {
            return i18n.GetTranslation("TheMightyAmondee.Shoplifter/GMCM_MaxFriendship");
        }
        public static string string_GMCM_MaxFriendshipTooltip()
        {
            return i18n.GetTranslation("TheMightyAmondee.Shoplifter/GMCM_MaxFriendshipTooltip");
        }
        public static string string_GMCM_MaxCatches()
        {
            return i18n.GetTranslation("TheMightyAmondee.Shoplifter/GMCM_MaxCatches");
        }
        public static string string_GMCM_MaxCatchesTooltip()
        {
            return i18n.GetTranslation("TheMightyAmondee.Shoplifter/GMCM_MaxCatchesTooltip");
        }
        public static string string_GMCM_MaxBanned()
        {
            return i18n.GetTranslation("TheMightyAmondee.Shoplifter/GMCM_MaxBanned");
        }
        public static string string_GMCM_MaxBannedTooltip()
        {
            return i18n.GetTranslation("TheMightyAmondee.Shoplifter/GMCM_MaxBannedTooltip");
        }
        public static string string_GMCM_MaxRadius()
        {
            return i18n.GetTranslation("TheMightyAmondee.Shoplifter/GMCM_MaxRadius");
        }
        public static string string_GMCM_MaxRadiusTooltip()
        {
            return i18n.GetTranslation("TheMightyAmondee.Shoplifter/GMCM_MaxRadiusTooltip");
        }
        public static string string_GMCM_RareStockChance()
        {
            return i18n.GetTranslation("TheMightyAmondee.Shoplifter/GMCM_RareStockChance");
        }
        public static string string_GMCM_RareStockChanceTooltip()
        {
            return i18n.GetTranslation("TheMightyAmondee.Shoplifter/GMCM_RareStockChanceTooltip");
        }
<<<<<<< HEAD
=======

        public static string string_GMCM_Shopliftables()
        {
            return i18n.GetTranslation("TheMightyAmondee.Shoplifter/Shopliftables");
        }
        public static string string_GMCM_SeedShop()
        {
            return i18n.GetTranslation("TheMightyAmondee.Shoplifter/GMCM_SeedShop");
        }
        public static string string_GMCM_FishShop()
        {
            return i18n.GetTranslation("TheMightyAmondee.Shoplifter/GMCM_FishShop");
        }
        public static string string_GMCM_Carpenter()
        {
            return i18n.GetTranslation("TheMightyAmondee.Shoplifter/GMCM_Carpenter");
        }
        public static string string_GMCM_AnimalShop()
        {
            return i18n.GetTranslation("TheMightyAmondee.Shoplifter/GMCM_AnimalShop");
        }
        public static string string_GMCM_Blacksmith()
        {
            return i18n.GetTranslation("TheMightyAmondee.Shoplifter/GMCM_Blacksmith");
        }
        public static string string_GMCM_Saloon()
        {
            return i18n.GetTranslation("TheMightyAmondee.Shoplifter/GMCM_Saloon");
        }
        public static string string_GMCM_SandyShop()
        {
            return i18n.GetTranslation("TheMightyAmondee.Shoplifter/GMCM_SandyShop");
        }
        public static string string_GMCM_Hospital()
        {
            return i18n.GetTranslation("TheMightyAmondee.Shoplifter/GMCM_Hospital");
        }
        public static string string_GMCM_IceCreamStand()
        {
            return i18n.GetTranslation("TheMightyAmondee.Shoplifter/GMCM_IceCreamStand");
        }
        public static string string_GMCM_IslandResort()
        {
            return i18n.GetTranslation("TheMightyAmondee.Shoplifter/GMCM_IslandResort");
        }
        public static string string_GMCM_JojaMart()
        {
            return i18n.GetTranslation("TheMightyAmondee.Shoplifter/GMCM_JojaMart");
        }
>>>>>>> 1.3develop
        // End GMCM translations

        public static string string_Shoplift()
        {
            return i18n.GetTranslation("TheMightyAmondee.Shoplifter/Shoplift");
        }

        public static string string_Banned()
        {
            return i18n.GetTranslation("TheMightyAmondee.Shoplifter/Banned");
        }

        public static string string_Caught(string shopkeeper)
        {
            var fineamount = Math.Min(Game1.player.Money, (int)config.MaxFine);
            return i18n.GetTranslation($"TheMightyAmondee.Shoplifter/Caught{shopkeeper}", new { fineamount = fineamount });
        }

        public static string string_Caught_NoMoney(string shopkeeper)
        {
            return i18n.GetTranslation($"TheMightyAmondee.Shoplifter/Caught{shopkeeper}_NoMoney");
        }

        public static string string_BanFromShop()
        {
            return i18n.GetTranslation("TheMightyAmondee.Shoplifter/BanFromShop", new { daysbanned = config.DaysBannedFor });
        }

        public static string string_BanFromShop_Single()
        {
            return i18n.GetTranslation("TheMightyAmondee.Shoplifter/BanFromShop_Single");
        }

        public static string string_AlreadyShoplifted()
        {
            return i18n.GetTranslation("TheMightyAmondee.Shoplifter/AlreadyShoplifted", new { shopliftingamount = config.MaxShopliftsPerDay });
        }

        public static string string_AlreadyShoplifted_Single()
        {
            return i18n.GetTranslation("TheMightyAmondee.Shoplifter/AlreadyShoplifted_Single");
        }

        public static string string_AlreadyShopliftedSameShop()
        {
            return i18n.GetTranslation("TheMightyAmondee.Shoplifter/AlreadyShopliftedSameShop");
        }

        /// <summary>
        /// Gets the correct translation
        /// </summary>
        /// <param name="key">The translation key</param>
        /// <param name="tokens">Tokens, if any</param>
        /// <returns>The translated string</returns>
        public static Translation GetTranslation(string key, object tokens = null)
        {
            if (i18n.translation == null)
            {
                throw new InvalidOperationException($"You must call {nameof(i18n)}.{nameof(i18n.gethelpers)} from the mod's entry method before reading translations.");
            }

            return i18n.translation.Get(key, tokens);
        }
    }
}