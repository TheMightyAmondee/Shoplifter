# Shoplifter

Shoplifter is a mod for [Stardew Valley](https://www.stardewvalley.net/) that allows the player to shoplift when the shopkeeper is not at the counter. 
Stock is randomly generated based on the save file and the number of days played for each shop. This is to ensure stock is the same throughout the day.
Only basic items that are currently available to your character can be shoplifted, it's unlikely you could get away with stealing a TV from Robin's (as she does point out...)

Ready for a spree of petty crime? Just don't get caught...

Any villager within 7 tiles of you when you shoplift will lose 2 hearts of friendship. If the villager also happens to be the shopowner or their family/employee they will also fine you your current funds up to 1000g.

If you get caught by the shopowner or their family/employee three times within a 28 day period, you'll receive a three day ban from entering the shop.

You can only successfully shoplift once per day, you're not that bad are you?

Feel free to edit any of the strings included in the mod, I'm not the best at coming up with lines. Just make sure no entries are deleted. Also, note the {0} in the getting caught strings are replaced with the value of the fine amount, make sure they are included. The BanFromSHop string must also start with #$b#.

The shops you can shoplift from are: Willy's Shop, Pierre's General Store, Carpenter, Marnie's Ranch, Harvey's Clinic, Saloon, Blacksmith. 
If you're wondering why all shops aren't included it's because the shopkeeper never leaves said store, you'll never get away with it.

The mod uses Harmony to patch some game methods.

Boring specifics are listed below:

Shop | Who can ban you | Stock exclusions | Max number of different items | Max quantity of each item
-----|-----------------|------------------|-------------------------------|--------------------------
Willy's Shop | Willy | Any furniture, fish tanks, fishing rods | 3 | 3
Pierre's General Store | Pierre, Caroline, Abigail | Recipes, wallpaper, flooring | 5 | 5
Carpenter | Robin, Demetrius, Maru, Sebastian | Recipes, any furniture, workbench | 2 | 20
Marnie's Ranch | Marnie, Shane | Decorations, tools, heater | 1 | 15
Harvey's Clinic | Harvey, Maru | None | 1 | 3
Saloon | Gus, Emily | Recipes | 2 | 1
Blacksmith | Clint | None | 3 | 10

## Versions ##
1.0.0 - Initial release



