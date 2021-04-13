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
					// Stops illegal stock being added, will result in an error item
					if ((shopstock.Key as StardewValley.Object) == null || (shopstock.Key as Wallpaper) != null || (shopstock.Key as Furniture) != null || (shopstock.Key as StardewValley.Object).bigCraftable == true || (shopstock.Key as StardewValley.Object).IsRecipe == true)
					{
						continue;
					}

					// Add object id to array
					if ((shopstock.Key as StardewValley.Object) != null && (shopstock.Key as StardewValley.Object).bigCraftable == false)
					{
						index = (shopstock.Key as StardewValley.Object).parentSheetIndex;

						CurrentStock.Add(index);
					}
				}
			}

			// Willy's shop
			else if (which == "FishShop")
            {
				foreach (var shopstock in Utility.getFishShopStock(Game1.player))
				{

					// Stops illegal stock being added, will result in an error item
					if ((shopstock.Key as StardewValley.Object) == null || (shopstock.Key as Wallpaper) != null || (shopstock.Key as Furniture) != null || (shopstock.Key as StardewValley.Object).bigCraftable == true || (shopstock.Key as StardewValley.Object).IsRecipe == true)
					{
						continue;
					}

					// Add object id to array
					if ((shopstock.Key as StardewValley.Object) != null && (shopstock.Key as StardewValley.Object).bigCraftable == false)
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
					// Stops illegal stock being added, will result in an error item
					if ((shopstock.Key as StardewValley.Object) == null || (shopstock.Key as Wallpaper) != null || (shopstock.Key as Furniture) != null || (shopstock.Key as StardewValley.Object).bigCraftable == true || (shopstock.Key as StardewValley.Object).IsRecipe == true)
					{
						continue;
					}

					// Add object id to array
					if ((shopstock.Key as StardewValley.Object) != null && (shopstock.Key as StardewValley.Object).bigCraftable == false)
					{
						index = (shopstock.Key as StardewValley.Object).parentSheetIndex;

						CurrentStock.Add(index);
					}
				}
			}

			// Marnie's shop
			else if (which == "AnimalShop")
			{
				foreach (var shopstock in Utility.getAnimalShopStock())
				{
					// Stops illegal stock being added, will result in an error item
					if ((shopstock.Key as StardewValley.Object) == null || (shopstock.Key as Wallpaper) != null || (shopstock.Key as Furniture) != null || (shopstock.Key as StardewValley.Object).bigCraftable == true || (shopstock.Key as StardewValley.Object).IsRecipe == true)
					{
						continue;
					}

					// Add object id to array
					if ((shopstock.Key as StardewValley.Object) != null && (shopstock.Key as StardewValley.Object).bigCraftable == false && CurrentStock.Contains((shopstock.Key as StardewValley.Object).parentSheetIndex) == false)
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

					// Stops illegal stock being added, will result in an error item
					if ((shopstock.Key as StardewValley.Object) == null || (shopstock.Key as Wallpaper) != null || (shopstock.Key as Furniture) != null || (shopstock.Key as StardewValley.Object).bigCraftable == true || (shopstock.Key as StardewValley.Object).IsRecipe == true)
					{
						continue;
					}

					// Add object id to array
					if ((shopstock.Key as StardewValley.Object) != null && (shopstock.Key as StardewValley.Object).bigCraftable == false)
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

					// Stops illegal stock being added, will result in an error item
					if ((shopstock.Key as StardewValley.Object) == null || (shopstock.Key as Wallpaper) != null || (shopstock.Key as Furniture) != null || (shopstock.Key as StardewValley.Object).bigCraftable == true || (shopstock.Key as StardewValley.Object).IsRecipe == true)
					{
						continue;
					}

					// Add object id to array
					if ((shopstock.Key as StardewValley.Object) != null && (shopstock.Key as StardewValley.Object).bigCraftable == false)
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

				ShopStock.addToStock(stock, stockIndices, new StardewValley.Object((int)CurrentStock[item], quantity), new int[2]
				{
					0,
					quantity
				});
			}

			// Clear stock array
			CurrentStock.Clear();

			return stock;
		}
	}
}