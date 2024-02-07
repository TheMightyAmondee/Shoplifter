using System;
using StardewValley.Objects;
using System.Collections;
using System.Collections.Generic;
using StardewValley.Locations;
using StardewValley;
using System.Linq;
using StardewModdingAPI;
using StardewValley.GameData.Shops;
using StardewValley.GameData.Crops;
using StardewValley.Internal;

namespace Shoplifter
{	
	public class ShopStock
	{
		public static List<Item> BasicStock = new List<Item>();
        public static List<Item> RareStock = new List<Item>();

        /// <summary>
        /// Generates a random list of stock for the given shop
        /// </summary>
        /// <param name="maxstock">The maximum number of different stock items to generate</param>
        /// <param name="maxquantity">The maximum quantity of each stock</param>
        /// <param name="which">The shop to generate stock for</param>
        /// <returns>The generated stock list</returns>
        public static Dictionary<ISalable, ItemStockInformation> generateRandomStock(int maxstock, int maxquantity, string which, float rarestockchance)
		{
            //if (!System.Diagnostics.Debugger.IsAttached) { System.Diagnostics.Debugger.Launch(); }
            GameLocation location = Game1.currentLocation;
            Dictionary<ISalable, ItemStockInformation> stock = new Dictionary<ISalable, ItemStockInformation>();
			Random random = new Random((int)Game1.uniqueIDForThisGame / 2 + (int)Game1.stats.DaysPlayed + ModEntry.PerScreenShopliftCounter.Value);
			int stocklimit = random.Next(1, maxstock + 1);
            var shopstock = ShopBuilder.GetShopStock(which);
            var addrarestockchance = random.NextDouble();

            foreach (var stockinfo in shopstock)
            {
                if ((stockinfo.Key as StardewValley.Object) == null || (stockinfo.Key as StardewValley.Object).QualifiedItemId.StartsWith("(O)") == false || (stockinfo.Key as StardewValley.Object).IsRecipe == true)
                {
                    RareStock.Add(stockinfo.Key as Item);
                    continue;
                }

                // Add object id to array
                if ((stockinfo.Key as StardewValley.Object) != null && (stockinfo.Key as StardewValley.Object).bigCraftable.Value == false)
                {
                    if ((stockinfo.Key as StardewValley.Object).Category < -100)
                    {
                        RareStock.Add(stockinfo.Key as StardewValley.Object);
                        continue;
                    }

                    if (ModEntry.IDGAItem?.GetDGAItemId(stockinfo.Key as StardewValley.Object) != null)
                    {
                        var id = (ModEntry.IDGAItem.SpawnDGAItem(ModEntry.IDGAItem.GetDGAItemId(stockinfo.Key as StardewValley.Object)) as StardewValley.ISalable) as Item;

                        BasicStock.Add(id);
                    }

                    else
                    {
                        BasicStock.Add(stockinfo.Key as StardewValley.Object);
                    }
                }
            }

            if (BasicStock.Count == 0)
            {
                switch (which)
                {
                    case "SeedShop":
                        // Grass starter if nothing available
                        BasicStock.Add(new StardewValley.Object("297", 1));
                        break;
                    case "FishShop":
                        // Trout soup if nothing available
                        BasicStock.Add(new StardewValley.Object("219", 1));
                        break;
                    case "Carpenter":
                        // Wood if nothing available
                        BasicStock.Add(new StardewValley.Object("388", 1));
                        break;
                    case "Hospital":
                        // Muscle Remedy if nothing available
                        BasicStock.Add(new StardewValley.Object("351", 1));
                        break;
                    case "AnimalShop":
                        // Hay if nothing available
                        BasicStock.Add(new StardewValley.Object("178", 1));
                        break;
                    case "Saloon":
                        // Beer if nothing available
                        BasicStock.Add(new StardewValley.Object("346", 1));
                        break;
                    case "Blacksmith":
                        // Coal if nothing available
                        BasicStock.Add(new StardewValley.Object("382", 1));
                        break;
                    case "IceCreamStand":
                        // Icecream if nothing available
                        BasicStock.Add(new StardewValley.Object("233", 1));
                        break;
                    case "Sandy":
                        // Cactus seeds if nothing available
                        BasicStock.Add(new StardewValley.Object("802", 1));                       
                        break;
                }
                
            }          
			
			// Add generated stock to store from array
			for (int i = 0; i < stocklimit; i++)
			{
                int quantity = random.Next(1, maxquantity + 1);
				var itemindex = random.Next(0, BasicStock.Count);

                if (BasicStock.Count == 0)
                {
                    break;
                }
                stock.Add(BasicStock[itemindex], new ItemStockInformation(0, quantity, null, null, LimitedStockMode.None));
                BasicStock.RemoveAt(itemindex);
                //if (CurrentStock[itemindex] is String && ModEntry.IDGAItem.SpawnDGAItem(CurrentStock[itemindex].ToString()) as StardewValley.ISalable as Item != null)
                //{
                //	var dgaitem = (ModEntry.IDGAItem.SpawnDGAItem(CurrentStock[itemindex].ToString()) as StardewValley.ISalable) as Item;
                //	dgaitem.Stack = quantity;
                //                stock.Add(dgaitem);
                //                //Utility.AddStock(stock, dgaitem, 0, quantity);
                //	CurrentStock.RemoveAt(itemindex);
                //	continue;
                //}
            }

            if (RareStock.Count > 0 && addrarestockchance <= rarestockchance)
            {
                var itemindex = random.Next(0, RareStock.Count);               

                stock.Add(RareStock[itemindex], new ItemStockInformation(0, 1, null, null, LimitedStockMode.None));
            }

			// Clear stock list
			BasicStock.Clear();
            RareStock.Clear();

			return stock;
		}
	}
}