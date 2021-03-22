using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.BellsAndWhistles;
using StardewValley.Objects;
using StardewValley;
using Harmony;


namespace Shoplifter
{
    public class SeedShopStock
    {
		private static void addFreeStock(Dictionary<ISalable, int[]> stock, int parentSheetIndex, int buyPrice = -1, string item_season = null)
		{
			int price = buyPrice;
			float price_multiplier = 0;
			StardewValley.Object obj = new StardewValley.Object(Vector2.Zero, parentSheetIndex, 1);
			if (buyPrice == -1)
			{
				price = 0;
			}
			else if (obj.isSapling())
			{
				price_multiplier *= 0;
			}
			if (item_season != null && item_season != Game1.currentSeason)
			{
				if (!Game1.MasterPlayer.hasOrWillReceiveMail("PierreStocklist"))
				{
					return;
				}
				price_multiplier *= 0;
			}
			price = (int)((float)price * price_multiplier);
			if (item_season != null)
			{
				foreach (KeyValuePair<ISalable, int[]> item in stock)
				{
					if (item.Key == null || !(item.Key is StardewValley.Object))
					{
						continue;
					}
					StardewValley.Object existing_item = item.Key as StardewValley.Object;
					if (Utility.IsNormalObjectAtParentSheetIndex(existing_item, parentSheetIndex))
					{
						if (item.Value.Length != 0 && price < item.Value[0])
						{
							item.Value[0] = price;
							stock[existing_item] = item.Value;
						}
						return;
					}
				}
			}
			stock.Add(obj, new int[2]
			{
				price,
				2147483647
			});
		}
		public static Dictionary<ISalable, int[]> shopStock()
		{
			GameLocation location = Game1.currentLocation;
			Dictionary<ISalable, int[]> stock = new Dictionary<ISalable, int[]>();
			addFreeStock(stock, 472, -1, "spring");
			addFreeStock(stock, 473, -1, "spring");
			addFreeStock(stock, 474, -1, "spring");
			addFreeStock(stock, 475, -1, "spring");
			addFreeStock(stock, 427, -1, "spring");
			addFreeStock(stock, 477, -1, "spring");
			addFreeStock(stock, 429, -1, "spring");
			if (Game1.year > 1)
			{
				addFreeStock(stock, 476, -1, "spring");
				addFreeStock(stock, 273, -1, "spring");
			}
			addFreeStock(stock, 479, -1, "summer");
			addFreeStock(stock, 480, -1, "summer");
			addFreeStock(stock, 481, -1, "summer");
			addFreeStock(stock, 482, -1, "summer");
			addFreeStock(stock, 483, -1, "summer");
			addFreeStock(stock, 484, -1, "summer");
			addFreeStock(stock, 453, -1, "summer");
			addFreeStock(stock, 455, -1, "summer");
			addFreeStock(stock, 302, -1, "summer");
			addFreeStock(stock, 487, -1, "summer");
			addFreeStock(stock, 431, -1, "summer");
			if (Game1.year > 1)
			{
				addFreeStock(stock, 485, -1, "summer");
			}
			addFreeStock(stock, 490, -1, "fall");
			addFreeStock(stock, 487, -1, "fall");
			addFreeStock(stock, 488, -1, "fall");
			addFreeStock(stock, 491, -1, "fall");
			addFreeStock(stock, 492, -1, "fall");
			addFreeStock(stock, 493, -1, "fall");
			addFreeStock(stock, 483, -1, "fall");
			addFreeStock(stock, 431, -1, "fall");
			addFreeStock(stock, 425, -1, "fall");
			addFreeStock(stock, 299, -1, "fall");
			addFreeStock(stock, 301, -1, "fall");
			if (Game1.year > 1)
			{
				addFreeStock(stock, 489, -1, "fall");
			}
			addFreeStock(stock, 297);
			if (!Game1.player.craftingRecipes.ContainsKey("Grass Starter"))
			{
				stock.Add(new StardewValley.Object(297, 1, isRecipe: true), new int[2]
				{
					-1,
					1
				});
			}
			addFreeStock(stock, 245);
			addFreeStock(stock, 246);
			addFreeStock(stock, 423);
			addFreeStock(stock, 247);
			addFreeStock(stock, 419);
			if ((int)Game1.stats.DaysPlayed >= 15)
			{
				addFreeStock(stock, 368, -1);
				addFreeStock(stock, 370, -1);
				addFreeStock(stock, 465, -1);
			}
			if (Game1.year > 1)
			{
				addFreeStock(stock, 369, -1);
				addFreeStock(stock, 371, -1);
				addFreeStock(stock, 466, -1);
			}
			Random r = new Random((int)Game1.stats.DaysPlayed + (int)Game1.uniqueIDForThisGame / 2);
			int wp = r.Next(112);
			if (wp == 21)
			{
				wp = 36;
			}
			Wallpaper wallpaper = new Wallpaper(wp);
			stock.Add(wallpaper, new int[2]
			{
				-1,
				2147483647
			});
			wallpaper = new Wallpaper(r.Next(56), isFloor: true);
			stock.Add(wallpaper, new int[2]
			{
				-1,
				2147483647
			});
			Furniture furniture = new Furniture(1308, Vector2.Zero);
			stock.Add(furniture, new int[2]
			{
				-1,
				2147483647
			});
			addFreeStock(stock, 628, -1);
			addFreeStock(stock, 629, -1);
			addFreeStock(stock, 630, -1);
			addFreeStock(stock, 631, -1);
			addFreeStock(stock, 632, -1);
			addFreeStock(stock, 633, -1);
			
			if (Game1.player.hasAFriendWithHeartLevel(8, datablesOnly: true))
			{
				addFreeStock(stock, 458, -1);
			}
			return stock;
		}
	}
}
