using System;
using StardewValley.Objects;
using System.Collections;
using System.Collections.Generic;
using StardewValley.Locations;
using StardewValley;

namespace Shoplifter
{	
	public class ShopStock
	{
		public static ArrayList CurrentStock = new ArrayList();

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

		/// <summary>
		/// Generates a random list of stock for the given shop
		/// </summary>
		/// <param name="maxstock">The maximum number of different stock items to generate</param>
		/// <param name="maxquantity">The maximum quantity of each stock</param>
		/// <param name="which">The shop to generate stock for</param>
		/// <returns>The generated stock list</returns>
		public static Dictionary<ISalable, int[]> generateRandomStock(int maxstock, int maxquantity, string which)
		{
			GameLocation location = Game1.currentLocation;
			Dictionary<ISalable, int[]> stock = new Dictionary<ISalable, int[]>();
			HashSet<int> stockIndices = new HashSet<int>();
			Random random = new Random((int)Game1.uniqueIDForThisGame / 2 + (int)Game1.stats.DaysPlayed);		
			int stocklimit = random.Next(1, maxstock + 1);
			int index;

			// Pierre's shop
			if (which == "SeedShop")
            {
				foreach (var shopstock in (location as SeedShop).shopStock())
				{
					// Stops wallpaper and furniture being added, will result in an error item
					if ((shopstock.Key as Wallpaper) != null || (shopstock.Key as Furniture) != null)
					{
						continue;
					}

					// Add object id to array
					if ((shopstock.Key as StardewValley.Object) != null && (shopstock.Key as StardewValley.Object).bigCraftable == false)
					{
						index = (shopstock.Key as StardewValley.Object).parentSheetIndex;

						CurrentStock.Add(index);
					}

					// Add big craftable objects with 10000 added to it's id so they can be marked as not being valid stock later
					else if ((shopstock.Key as StardewValley.Object) != null && (shopstock.Key as StardewValley.Object).bigCraftable == true && (shopstock.Key as StardewValley.Object).IsRecipe == false)
                    {
						index = (shopstock.Key as StardewValley.Object).parentSheetIndex;

						CurrentStock.Add(index + 10000);
					}
				}
			}

			// Willy's shop
			else if (which == "FishShop")
            {
				foreach (var shopstock in Utility.getFishShopStock(Game1.player))
				{

					// Stops wallpaper and furniture being added, will result in an error item
					if ((shopstock.Key as Wallpaper) != null || (shopstock.Key as Furniture) != null)
					{
						continue;
					}

					// Add object id to array
					if ((shopstock.Key as StardewValley.Object) != null && (shopstock.Key as StardewValley.Object).bigCraftable == false && (shopstock.Key as StardewValley.Object).IsRecipe == false)
					{
						index = (shopstock.Key as StardewValley.Object).parentSheetIndex;

						CurrentStock.Add(index);
					}
				}
			}

			// Robin's shop
			else if (which == "Carpenters")
			{
				foreach (var shopstock in Utility.getCarpenterStock())
				{
					// Stops wallpaper and furniture being added, will result in an error item otherwise
					if ((shopstock.Key as Wallpaper) != null || (shopstock.Key as Furniture) != null)
					{
						continue;
					}

					// Add object id to array
					if ((shopstock.Key as StardewValley.Object) != null && (shopstock.Key as StardewValley.Object).bigCraftable == false)
					{
						index = (shopstock.Key as StardewValley.Object).parentSheetIndex;

						CurrentStock.Add(index);
					}
					// If it is a big craftable add 10000 to id so it can be marked as such for later use
					else if ((shopstock.Key as StardewValley.Object) != null && (shopstock.Key as StardewValley.Object).bigCraftable == true && (shopstock.Key as StardewValley.Object).IsRecipe == false)
					{
						index = (shopstock.Key as StardewValley.Object).parentSheetIndex + 10000;

						CurrentStock.Add(index);
					}
				}
			}

			// Marnie's shop
			else if (which == "AnimalShop")
			{

				foreach (var shopstock in Utility.getAnimalShopStock())
				{
					// Stops wallpaper and furniture being added, will result in an error item otherwise
					if ((shopstock.Key as Wallpaper) != null || (shopstock.Key as Furniture) != null)
					{
						continue;
					}

					// Add object id to array
					if ((shopstock.Key as StardewValley.Object) != null && (shopstock.Key as StardewValley.Object).bigCraftable == false && (shopstock.Key as StardewValley.Object).IsRecipe == false)
					{
						index = (shopstock.Key as StardewValley.Object).parentSheetIndex;

						CurrentStock.Add(index);
					}
				}
			}

			// Clint's shop
			else if (which == "Blacksmith")
			{
				foreach (var shopstock in Utility.getBlacksmithStock())
				{

					// Stops wallpaper and furniture being added, will result in an error item otherwise
					if ((shopstock.Key as Wallpaper) != null || (shopstock.Key as Furniture) != null)
					{
						continue;
					}

					// Add object id to array
					if ((shopstock.Key as StardewValley.Object) != null && (shopstock.Key as StardewValley.Object).bigCraftable == false && (shopstock.Key as StardewValley.Object).IsRecipe == false)
					{
						index = (shopstock.Key as StardewValley.Object).parentSheetIndex;

						CurrentStock.Add(index);
					}
				}
			}

			// Gus' shop
			else if (which == "Saloon")
            {
				foreach (var shopstock in Utility.getSaloonStock())
				{

					// Stops wallpaper and furniture being added, will result in an error item otherwise
					if ((shopstock.Key as Wallpaper) != null || (shopstock.Key as Furniture) != null)
					{
						continue;
					}

					// Add object id to array
					if ((shopstock.Key as StardewValley.Object) != null && (shopstock.Key as StardewValley.Object).bigCraftable == false && (shopstock.Key as StardewValley.Object).IsRecipe == false)
					{
						index = (shopstock.Key as StardewValley.Object).parentSheetIndex;

						CurrentStock.Add(index);
					}
				}
			}

			// Harvey's clinic
			else if (which == "HospitalShop")
            {
				// Add object id to array
				CurrentStock.Add(349);
				CurrentStock.Add(351);
            }			

			// Add generated stock to store from array
			for (int i = 0; i < stocklimit; i++)
			{
				int quantity = random.Next(1, maxquantity + 1);
				int item = random.Next(0, CurrentStock.Count);

                // Normal objects                
				if ((int)CurrentStock[item] < 10000)
                {
					ShopStock.addToStock(stock, stockIndices, new StardewValley.Object((int)CurrentStock[item], quantity), new int[2]
					{
						0,
						quantity
					});
				}

				// Ignore iteration if an item that can't be added is selected
                else
                {
					i--;
                }
			}

			// Clear stock array
			CurrentStock.Clear();

			return stock;
		}
	}
}