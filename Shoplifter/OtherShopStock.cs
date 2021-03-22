using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.Events;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Minigames;
using StardewValley.Monsters;
using StardewValley.Objects;
using StardewValley.Quests;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using xTile.Dimensions;
using StardewValley;
using Harmony;

namespace Shoplifter
{
    public class OtherShopStock
    {
		public static void AddStock(Dictionary<ISalable, int[]> stock, Item obj, int buyPrice = -1, int limitedQuantity = -1)
		{
			int price = 0 * buyPrice;
			int stack = int.MaxValue;
			if (obj is StardewValley.Object && (obj as StardewValley.Object).IsRecipe)
			{
				stack = 1;
			}
			else if (limitedQuantity != -1)
			{
				stack = limitedQuantity;
			}
			stock.Add(obj, new int[2]
			{
				price,
				stack
			});
		}

		public static Dictionary<ISalable, int[]> generateRandomStock(int maxstock)
        {
			Dictionary<ISalable, int[]> stock = new Dictionary<ISalable, int[]>();
			HashSet<int> stockIndices = new HashSet<int>();
			Random random = new Random();
			int stocklimit = random.Next(1, maxstock + 1);

			for(int i = 0, i < stocklimit, i++)
            {
				Utility.addToStock(stock, stockIndices, new Object(628, 1), new int[2]
				{
					0,
					random.Next(1,6)
				});
			}

		}
	}
}
