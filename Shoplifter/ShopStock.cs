using System;
using StardewValley.Objects;
using System.Collections;
using System.Collections.Generic;
using StardewValley.Locations;
using StardewValley;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using System.Runtime.CompilerServices;

namespace Shoplifter
{	
	public class ShopStock
	{
        private static IMonitor monitor;
        private static IModHelper helper;
        private static ModConfig config;

        public static ArrayList CurrentStock = new ArrayList();

        public static void gethelpers(IMonitor monitor, IModHelper helper, ModConfig config)
        {
            ShopStock.monitor = monitor;
            ShopStock.helper = helper;
            ShopStock.config = config;
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
			Random random = new Random((int)Game1.uniqueIDForThisGame / 2 + (int)Game1.stats.DaysPlayed + ModEntry.PerScreenShopliftCounter.Value);
			int stocklimit = random.Next(1, maxstock + 1);
			int index;

			switch (which)
            {
				// Pierre's shop
				case "SeedShop":
					foreach (var shopstock in (location as SeedShop).shopStock())
					{
                        // Stops illegal stock being added, will result in an error item
                        if ((shopstock.Key as StardewValley.Object) == null || (shopstock.Key as Wallpaper) != null || (shopstock.Key as Furniture) != null || (shopstock.Key as StardewValley.Object).bigCraftable.Value == true || (shopstock.Key as StardewValley.Object).IsRecipe == true || (shopstock.Key as Clothing) != null || (shopstock.Key as Ring) != null || (shopstock.Key as Boots) != null || (shopstock.Key as Hat) != null)
						{
							continue;
						}

						// Add object id to array
						if ((shopstock.Key as StardewValley.Object) != null && (shopstock.Key as StardewValley.Object).bigCraftable.Value == false)
						{
                            if (ModEntry.IDGAItem?.GetDGAItemId(shopstock.Key as StardewValley.Object) != null)
                            {
                                var id = ModEntry.IDGAItem.GetDGAItemId(shopstock.Key as StardewValley.Object);

                                CurrentStock.Add(id);
                            }

                            else
                            {
                                index = (shopstock.Key as StardewValley.Object).ParentSheetIndex;

                                CurrentStock.Add(index);
                            }
                        }
						
					}

                    // Grass starter if nothing available
                    if (CurrentStock.Count == 0)
                    {
                        CurrentStock.Add(297);
                    }
                    break;

				// Willy's shop
				case "FishShop":
					foreach (var shopstock in Utility.getFishShopStock(Game1.player))
					{

						// Stops illegal stock being added, will result in an error item
						if ((shopstock.Key as StardewValley.Object) == null || (shopstock.Key as Wallpaper) != null || (shopstock.Key as Furniture) != null || (shopstock.Key as StardewValley.Object).bigCraftable.Value == true || (shopstock.Key as StardewValley.Object).IsRecipe == true || (shopstock.Key as Clothing) != null || (shopstock.Key as Ring) != null || (shopstock.Key as Boots) != null || (shopstock.Key as Hat) != null)
						{
							continue;
						}

						// Add object id to array
						if ((shopstock.Key as StardewValley.Object) != null && (shopstock.Key as StardewValley.Object).bigCraftable.Value == false)
						{
                            if (ModEntry.IDGAItem?.GetDGAItemId(shopstock.Key as StardewValley.Object) != null)
                            {
                                var id = ModEntry.IDGAItem.GetDGAItemId(shopstock.Key as StardewValley.Object);

                                CurrentStock.Add(id);
                            }

                            else
                            {
                                index = (shopstock.Key as StardewValley.Object).ParentSheetIndex;

                                CurrentStock.Add(index);
                            }
                        }
                    }

                    // Trout soup if nothing available
                    if (CurrentStock.Count == 0)
                    {
                        CurrentStock.Add(219);
                    }
                    break;

				// Robin's shop
				case "Carpenters":
					foreach (var shopstock in Utility.getCarpenterStock())
					{
                        // Stops illegal stock being added, will result in an error item
                        if ((shopstock.Key as StardewValley.Object) == null || (shopstock.Key as Wallpaper) != null || (shopstock.Key as Furniture) != null || (shopstock.Key as StardewValley.Object).bigCraftable.Value == true || (shopstock.Key as StardewValley.Object).IsRecipe == true || (shopstock.Key as Clothing) != null || (shopstock.Key as Ring) != null || (shopstock.Key as Boots) != null || (shopstock.Key as Hat) != null)
						{
							continue;
						}

						// Add object id to array
						if ((shopstock.Key as StardewValley.Object) != null && (shopstock.Key as StardewValley.Object).bigCraftable.Value == false)
						{
                            if (ModEntry.IDGAItem?.GetDGAItemId(shopstock.Key as StardewValley.Object) != null)
                            {
                                var id = ModEntry.IDGAItem.GetDGAItemId(shopstock.Key as StardewValley.Object);

                                CurrentStock.Add(id);
                            }

                            else
                            {
                                index = (shopstock.Key as StardewValley.Object).ParentSheetIndex;

                                CurrentStock.Add(index);
                            }
                        }
                        
                    }
                    // Wood if nothing available
                    if (CurrentStock.Count == 0)
                    {
                        CurrentStock.Add(388);
                    }
                    break;

				// Marnie's shop
				case "AnimalShop":
					foreach (var shopstock in Utility.getAnimalShopStock())
					{
						// Stops illegal stock being added, will result in an error item
						if ((shopstock.Key as StardewValley.Object) == null || (shopstock.Key as Wallpaper) != null || (shopstock.Key as Furniture) != null || (shopstock.Key as StardewValley.Object).bigCraftable.Value == true || (shopstock.Key as StardewValley.Object).IsRecipe == true || (shopstock.Key as Clothing) != null || (shopstock.Key as Ring) != null || (shopstock.Key as Boots) != null || (shopstock.Key as Hat) != null)
						{
							continue;
						}

						// Add object id to array
						if ((shopstock.Key as StardewValley.Object) != null && (shopstock.Key as StardewValley.Object).bigCraftable.Value == false)
						{
                            if (ModEntry.IDGAItem?.GetDGAItemId(shopstock.Key as StardewValley.Object) != null)
                            {
                                var id = ModEntry.IDGAItem.GetDGAItemId(shopstock.Key as StardewValley.Object);

                                CurrentStock.Add(id);
                            }

                            else
                            {
                                index = (shopstock.Key as StardewValley.Object).ParentSheetIndex;

                                CurrentStock.Add(index);
                            }
                        }
					}
                    // Hay if nothing available
                    if (CurrentStock.Count == 0)
                    {
                        CurrentStock.Add(178);
                    }
                    break;

				// Clint's shop
				case "Blacksmith":
					foreach (var shopstock in Utility.getBlacksmithStock())
					{

						// Stops illegal stock being added, will result in an error item
						if ((shopstock.Key as StardewValley.Object) == null || (shopstock.Key as Wallpaper) != null || (shopstock.Key as Furniture) != null || (shopstock.Key as StardewValley.Object).bigCraftable.Value == true || (shopstock.Key as StardewValley.Object).IsRecipe == true || (shopstock.Key as Clothing) != null || (shopstock.Key as Ring) != null || (shopstock.Key as Boots) != null || (shopstock.Key as Hat) != null)
						{
							continue;
						}

						// Add object id to array
						if ((shopstock.Key as StardewValley.Object) != null && (shopstock.Key as StardewValley.Object).bigCraftable.Value == false)
						{
							if (ModEntry.IDGAItem?.GetDGAItemId(shopstock.Key as StardewValley.Object) != null)
							{
								var id = ModEntry.IDGAItem.GetDGAItemId(shopstock.Key as StardewValley.Object);

                                CurrentStock.Add(id);
                            }

							else
							{
                                index = (shopstock.Key as StardewValley.Object).ParentSheetIndex;

                                CurrentStock.Add(index);
                            }
						}
					}
                    // Coal if nothing available
                    if (CurrentStock.Count == 0)
                    {
                        CurrentStock.Add(382);
                    }
                    break;

				// Gus' shop
				case "Saloon":
					foreach (var shopstock in Utility.getSaloonStock())
					{

						// Stops illegal stock being added, will result in an error item
						if ((shopstock.Key as StardewValley.Object) == null || (shopstock.Key as Wallpaper) != null || (shopstock.Key as Furniture) != null || (shopstock.Key as StardewValley.Object).bigCraftable.Value == true || (shopstock.Key as StardewValley.Object).IsRecipe == true || (shopstock.Key as Clothing) != null || (shopstock.Key as Ring) != null || (shopstock.Key as Boots) != null || (shopstock.Key as Hat) != null)
						{
							continue;
						}

						// Add object id to array
						if ((shopstock.Key as StardewValley.Object) != null && (shopstock.Key as StardewValley.Object).bigCraftable.Value == false)
						{
                            if (ModEntry.IDGAItem?.GetDGAItemId(shopstock.Key as StardewValley.Object) != null)
                            {
                                var id = ModEntry.IDGAItem.GetDGAItemId(shopstock.Key as StardewValley.Object);

                                CurrentStock.Add(id);
                            }

                            else
                            {
                                index = (shopstock.Key as StardewValley.Object).ParentSheetIndex;

                                CurrentStock.Add(index);
                            }
                        }
					}
                    // Beer if nothing available
                    if (CurrentStock.Count == 0)
                    {
                        CurrentStock.Add(346);
                    }
                    break;

				// Harvey's shop
				case "HospitalShop":
					foreach (var shopstock in Utility.getHospitalStock())
					{

						// Stops illegal stock being added, will result in an error item
						if ((shopstock.Key as StardewValley.Object) == null || (shopstock.Key as Wallpaper) != null || (shopstock.Key as Furniture) != null || (shopstock.Key as StardewValley.Object).bigCraftable.Value == true || (shopstock.Key as StardewValley.Object).IsRecipe == true || (shopstock.Key as Clothing) != null || (shopstock.Key as Ring) != null || (shopstock.Key as Boots) != null || (shopstock.Key as Hat) != null)
						{
							continue;
						}

						// Add object id to array
						if ((shopstock.Key as StardewValley.Object) != null && (shopstock.Key as StardewValley.Object).bigCraftable.Value == false)
						{
                            if (ModEntry.IDGAItem?.GetDGAItemId(shopstock.Key as StardewValley.Object) != null)
                            {
                                var id = ModEntry.IDGAItem.GetDGAItemId(shopstock.Key as StardewValley.Object);

                                CurrentStock.Add(id);
                            }

                            else
                            {
                                index = (shopstock.Key as StardewValley.Object).ParentSheetIndex;

                                CurrentStock.Add(index);
                            }
                        }
					}
                    // Muscle Remedy if nothing available
                    if (CurrentStock.Count == 0)
                    {
                        CurrentStock.Add(351);
                    }
                    break;
				// Icecream Stand
				case "IceCreamStand":
					CurrentStock.Add(233);
                    break;

				// Sandy's shop
				case "SandyShop":
                    var sandyshopstock = helper.Reflection.GetMethod(new GameLocation(), "sandyShopStock").Invoke<Dictionary<ISalable, int[]>>();

                    foreach (var shopstock in sandyshopstock)
                    {
                        // Stops illegal stock being added, will result in an error item
                        if ((shopstock.Key as StardewValley.Object) == null || (shopstock.Key as Wallpaper) != null || (shopstock.Key as Furniture) != null || (shopstock.Key as StardewValley.Object).bigCraftable.Value == true || (shopstock.Key as StardewValley.Object).IsRecipe == true || (shopstock.Key as Clothing) != null || (shopstock.Key as Ring) != null || (shopstock.Key as Boots) != null || (shopstock.Key as Hat) != null)
                        {
                            continue;
                        }

                        // Add object id to array
                        if ((shopstock.Key as StardewValley.Object) != null && (shopstock.Key as StardewValley.Object).bigCraftable.Value == false)
                        {
                            if (ModEntry.IDGAItem?.GetDGAItemId(shopstock.Key as StardewValley.Object) != null)
                            {
                                var id = ModEntry.IDGAItem.GetDGAItemId(shopstock.Key as StardewValley.Object);

                                CurrentStock.Add(id);
                            }

                            else
                            {
                                index = (shopstock.Key as StardewValley.Object).ParentSheetIndex;

                                CurrentStock.Add(index);
                            }
                        }
                    }

                    if (CurrentStock.Count == 0)
                    {
                        CurrentStock.Add(340);
                    }
                    break;
                case "ResortBar":
                    CurrentStock.Add(873);
                    CurrentStock.Add(346);
                    CurrentStock.Add(303);
                    CurrentStock.Add(459);
                    CurrentStock.Add(612);
                    CurrentStock.Add(348);
                    break;
            }


			
			// Add generated stock to store from array
			for (int i = 0; i < stocklimit; i++)
			{
                int quantity = random.Next(1, maxquantity + 1);
				var item = random.Next(0, CurrentStock.Count);

				if (CurrentStock.Count == 0)
				{
					break;
				}

				if (CurrentStock[item] is String && ModEntry.IDGAItem.SpawnDGAItem(CurrentStock[item].ToString()) as StardewValley.ISalable as Item != null)
				{
					var dgaitem = (ModEntry.IDGAItem.SpawnDGAItem(CurrentStock[item].ToString()) as StardewValley.ISalable) as Item;
					dgaitem.Stack = quantity;
                    Utility.AddStock(stock, dgaitem, 0, quantity);
					CurrentStock.RemoveAt(item);
					continue;
				}

                // Make generic wine, gold quality mango wine if selected
                if (which == "ResortBar" && CurrentStock[item].Equals(348) == true)
                {
                    var wine = new StardewValley.Object(348, quantity);
                    var mango = new StardewValley.Object(834, 1);
                    wine.Name = mango.Name + " Wine";
                    wine.preserve.Value = StardewValley.Object.PreserveType.Wine;
                    wine.preservedParentSheetIndex.Value = mango.ParentSheetIndex;
                    wine.Quality = 2;
                    Utility.AddStock(stock, wine, 0, quantity);
                    CurrentStock.RemoveAt(item);
                    continue;
                }

                Utility.AddStock(stock, new StardewValley.Object((int)CurrentStock[item], quantity), 0, quantity);
                CurrentStock.RemoveAt(item);
            }

			// Clear stock array
			CurrentStock.Clear();

			return stock;
		}
	}
}