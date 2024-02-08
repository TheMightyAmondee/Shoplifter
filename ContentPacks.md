# Creating Content Packs for Shoplifter

Content Packs can be created to allow for custom shops to be made shopliftable. Content Packs for Shoplifter require two files, a manifest.json and a shopliftables.json.

The manifest.json tells SMAPI that your content pack is readable by Shoplifter, it is similar in format to other content pack manifest files. However, the "ContentPackFor" value should be "TheMightyAmondee.Shoplifter".

For example:
``{
    "Name": "Test",
    "Author": "Test",
    "Version": "1.0.0",
    "Description": "...",
    "UniqueID": "TheMightyAmondee.Test",
    "UpdateKeys": [], 
    "ContentPackFor": {
        "UniqueID": "TheMightyAmondee.Shoplifter"
    }
}``

The shopliftables.json is the important one. Here, new shopliftable shops are defined. The shopliftables.json is made up of a "MakeShopliftable" list of shopliftable shops.

Each entry in "MakeShopliftable" has a few required and non-required fields, see below!

Field | Type | Required? | What it does | Notes
------|------|-----------|--------------|------
UniqueShopId | string | Yes | A unique identifier for the shop, anything unique will do! | -
ShopName | string | Yes | The id of the shop, as defined by the game or custom shop, the mod uses this to get available stock | -
CounterLocation | ShopCounterLocation | Yes | Where the counter for the shop is located (the tile location to click on to open the store) | See ShopCounterLocation model below
ShopKeepers | List<string> | Yes | A list of all the shop's shopkeepers (anyone who can catch, fine and ban the player) | -
CaughtDialogue |  Dictionary<string (ShopKeeperName?_NoMoney), string (Dialogue)> | No | Unique dialogue for the shopkeeper to say when the player is caught. | Each shopkeeper requires two entries, one with just their name, and another with their name followed by "_NoMoney". Shopkeepers with no or too few entries will use generic dialogue. In entries with just their name the text "{0}" will be replaced with the fine amount.
OpenConditions | ShopliftableConditions | No | Under what conditions the store is normally open and items can be purchased | See ShopliftableConditions model below
MaxStockQuantity | int | No | The maximum number of different stock items that can appear in each shoplift attempt | Default value is 1
MaxStackPerItem | int | No | The maximum stack size of each stock item | Default value is 1
Bannable | bool | No | Whether the player can be banned from the shop | Default value is false. I wouldn't recommend outdoor shops be bannable, since players will no longer be able to enter that outdoor area.

## ShopCounterLocation model
This model describes where the store is located

Field | Type | Required? | What it does | Notes
------|------|-----------|--------------|------
LocationName | string | Yes | The location name where the shop is located | This is also the location the player won't be able to enter if banned.
TileX | int | No | The X tile coordinate of the shop counter | Default is 0 if not specified
TileY | int | No | The Y tile coordinate of the shop counter | Default is 0 if not specified

## ShopliftableConditions model
This model describes when the store is normally open. 
If any named condition is false the shop will be considered shopliftable

Field | Type | Required? | What it does | Notes
------|------|-----------|--------------|------
OpenTime | int | No | The time the store opens | Defaults to no open time (open first thing in the morning)
CloseTime | int | No | The time the store closes | Defaults to no close time (shop won't close during the day)
Weather | List<string> | No | Under what weather conditions the store is open | By default store is considered open in all weather conditions
ShopKeeperRange | List<ShopKeeperConditions> | No | The defined range each shopkeeper must be within the store for it to be considered open

Any conditions not defined will not count towards determining shop accessibility. In the case of time, both OpenTime and CloseTime must be undefined.
If no conditions are defined, shop is considered always open and not shopliftable.

## ShopKeeperConditions model
This model describes when shopkeepers are present at the store (can sell)
Fields describe a rectangle area that the shopkeeper must be present in for the shop to be accessible. This is generally the tile behind the shop counter.

Field | Type | Required? | What it does | Notes
------|------|-----------|--------------|------
Name | string | Yes | The name of the shopkeeper | -
TileX | int | Yes | The X tile coordinate of the upper left rectangle point | -
TileY | int | Yes | The Y tile coordinate of the upper left rectangle pointr | -
Width | int | No | The width of the rectangle the shopkeeper must be within | Default is 1
Height | int | No | The height of the rectangle the shopkeeper must be within | Default is 1