﻿using Aequus.Content.CarpenterBounties;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Consumables
{
    public class CarpenterResetSheet : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 5;
        }

        public override void SetDefaults()
        {
            Item.DefaultToHoldUpItem();
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Blue;
            Item.maxStack = 9999;
            Item.value = Item.buyPrice(gold: 1);
            Item.consumable = true;
            Item.UseSound = SoundID.Item92;
        }

        public override bool? UseItem(Player player)
        {
            var bountyPlayer = player.GetModPlayer<CarpenterBountyPlayer>();
            bool hasBounty = bountyPlayer.CompletedBounties.Count > 0;
            bountyPlayer.CompletedBounties.Clear();
            return hasBounty;
        }
    }
}