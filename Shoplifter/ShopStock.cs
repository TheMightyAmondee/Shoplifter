using System;
using System.Reflection;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Objects;
using System.Collections;
using System.Collections.Generic;
using StardewValley.Locations;
using StardewValley;
using Harmony;
using StardewModdingAPI;

namespace Shoplifter
{	
	public class ShopStock
	{
		public static ArrayList CurrentStock = new ArrayList();

		private static IMonitor monitor;
		private static IModHelper helper;

		public static void gethelpers(IMonitor monitor, IModHelper helper)
		{
			ShopStock.monitor = monitor;
			ShopStock.helper = helper;
		}

		// Method from Utilities so stock can be added to a shop (why is it private?)
		private static bool addToStock(Dictionary<ISalable, int[]> stock, HashSet<int> stockIndices, StardewValley.Object objectToAdd, int[] listing)
		{
			int index = objectToAdd.ParentSheetIndex;
			if (!stockIndices.Contains(index))
			{
				stock.Add(objectToAdd, listing);
				stockIndices.Add(index);
				return true;
			}
			return false;
		}

		// Generates a random list of available stock for the given shop
		public static Dictionary<ISalable, int[]> generateRandomStock(int maxstock, string which)
		{
			GameLocation location = Game1.currentLocation;
			Dictionary<ISalable, int[]> stock = new Dictionary<ISalable, int[]>();
			HashSet<int> stockIndices = new HashSet<int>();
			Random random = new Random();		
			int stocklimit = random.Next(1, maxstock + 1);

			if (which == "SeedShop")
            {
				foreach (var shopstock in (location as SeedShop).shopStock())
				{
					int index = 0;

					// Stops wallpaper and furniture being added, will result in an error item
					if ((shopstock.Key as Wallpaper) != null || (shopstock.Key as Furniture) != null)
					{
						continue;
					}

					if ((shopstock.Key as StardewValley.Object) != null && (shopstock.Key as StardewValley.Object).bigCraftable == false)
					{
						index = (shopstock.Key as StardewValley.Object).parentSheetIndex;

						CurrentStock.Add(index);
					}
				}
			}

			else if (which == "FishShop")
            {
				foreach (var shopstock in Utility.getFishShopStock(Game1.player))
				{
					int index = 0;

					// Stops wallpaper and furniture being added, will result in an error item
					if ((shopstock.Key as Wallpaper) != null || (shopstock.Key as Furniture) != null)
					{
						continue;
					}

					if ((shopstock.Key as StardewValley.Object) != null && (shopstock.Key as StardewValley.Object).bigCraftable == false)
					{
						index = (shopstock.Key as StardewValley.Object).parentSheetIndex;

						CurrentStock.Add(index);
					}
				}
			}

			else if (which == "Carpenters")
			{
				foreach (var shopstock in Utility.getCarpenterStock())
				{
					int index = 0;

					// Stops wallpaper and furniture being added, will result in an error item
					if ((shopstock.Key as Wallpaper) != null || (shopstock.Key as Furniture) != null)
					{
						continue;
					}

					if ((shopstock.Key as StardewValley.Object) != null && (shopstock.Key as StardewValley.Object).bigCraftable == false)
					{
						index = (shopstock.Key as StardewValley.Object).parentSheetIndex;

						CurrentStock.Add(index);
					}
				}
			}
			else if (which == "AnimalShop")
			{
				foreach (var shopstock in Utility.getAnimalShopStock())
				{
					int index = 0;

					// Stops wallpaper and furniture being added, will result in an error item
					if ((shopstock.Key as Wallpaper) != null || (shopstock.Key as Furniture) != null)
					{
						continue;
					}

					if ((shopstock.Key as StardewValley.Object) != null && (shopstock.Key as StardewValley.Object).bigCraftable == false)
					{
						index = (shopstock.Key as StardewValley.Object).parentSheetIndex;

						CurrentStock.Add(index);
					}
				}
			}
			else if (which == "Blacksmith")
			{
				foreach (var shopstock in Utility.getBlacksmithStock())
				{
					int index = 0;

					// Stops wallpaper and furniture being added, will result in an error item
					if ((shopstock.Key as Wallpaper) != null || (shopstock.Key as Furniture) != null)
					{
						continue;
					}

					if ((shopstock.Key as StardewValley.Object) != null && (shopstock.Key as StardewValley.Object).bigCraftable == false)
					{
						index = (shopstock.Key as StardewValley.Object).parentSheetIndex;

						CurrentStock.Add(index);
					}
				}
			}

			else if (which == "HospitalShop")
            {
				CurrentStock.Add(349);
				CurrentStock.Add(351);
            }			

			for (int i = 0; i < stocklimit; i++)
			{
				int quantity = random.Next(1, 6);
				int item = random.Next(0, CurrentStock.Count);

                // Normal objects                
				if((int)CurrentStock[item] < 10000)
                {
					ShopStock.addToStock(stock, stockIndices, new StardewValley.Object((int)CurrentStock[item], quantity), new int[2]
					{
						0,
						quantity
					});
				}

				// Big Craftable objects
                else
                {
					ShopStock.addToStock(stock, stockIndices, new StardewValley.Object(Vector2.Zero, (int)CurrentStock[item] - 10000), new int[2]
					{
						0,
						1
					});
				}				
			}

			CurrentStock.Clear();

			return stock;
		}
	}
}