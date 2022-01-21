﻿using AQMod.Tiles.Furniture;
using Terraria.ID;
using Terraria.ModLoader;

namespace AQMod.Items.Placeable
{
    public class PetrifiedWoodPlatform : ModItem
    {
        public override void SetDefaults()
        {
            item.width = 18;
            item.height = 18;
            item.maxStack = 999;
            item.useTime = 10;
            item.useAnimation = 15;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.createTile = ModContent.TileType<AQPlatforms>();
            item.placeStyle = AQPlatforms.PetrifiedWood;
            item.consumable = true;
            item.useTurn = true;
            item.autoReuse = true;
        }
    }
}