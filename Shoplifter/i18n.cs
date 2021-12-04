using System;
using StardewValley;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;


namespace Shoplifter
{
    internal static class i18n
    {
        private static ITranslationHelper translation;
        private static IManifest manifest;
        private static ModConfig config;
        public static void gethelpers(ITranslationHelper translation, ModConfig config)
        {
            i18n.translation = translation;
            i18n.config = config;
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

        public static Translation GetTranslation(string key, object tokens = null)
        {
            if (i18n.translation == null)
                throw new InvalidOperationException($"You must call {nameof(i18n)}.{nameof(i18n.gethelpers)} from the mod's entry method before reading translations.");

            return i18n.translation.Get(key, tokens);
        }
    }
}
