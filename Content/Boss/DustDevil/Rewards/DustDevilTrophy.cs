﻿using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Boss.DustDevil.Rewards
{
    public class DustDevilTrophy : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<Trophies>(), Trophies.DustDevil);
            Item.maxStack = 9999;
            Item.value = 50000;
            Item.rare = ItemRarityID.Blue;
        }
    }
}