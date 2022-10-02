﻿using Aequus.Content;
using Aequus.Items.Prefixes.Potions;
using Aequus.Particles.Dusts;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Misc
{
    public class MoonflowerPollen : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 25;
        }

        public override void SetDefaults()
        {
            Item.CloneDefaults(ItemID.PixieDust);
            Item.rare = ItemRarityID.Green;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 200);
        }

        public override void PostUpdate()
        {
            Lighting.AddLight(Item.Center, Color.BlueViolet.ToVector3() * 0.5f);
            if (Item.timeSinceItemSpawned % 30 == 0 || Main.rand.NextBool(12))
            {
                var d = Dust.NewDustDirect(Item.position, Item.width, Item.height, ModContent.DustType<MonoDust>(), newColor: Color.BlueViolet.UseA(128) * Main.rand.NextFloat(0.33f, 2f));
                d.velocity.X *= 0.6f;
                d.velocity.Y *= 0.35f;
                d.velocity.Y += Main.rand.NextFloat(-4.5f, -2f);
                d.fadeIn = d.scale;
                d.scale *= 0.5f;
            }
        }

        public override void AddRecipes()
        {
            for (int i = 0; i < ItemLoader.ItemCount; i++)
            {
                if (ConcoctionDatabase.ConcoctiblePotion(ContentSamples.ItemsByType[i]))
                {
                    var r = Recipe.Create(i, 1)
                        .AddIngredient(i)
                        .AddIngredient<MoonflowerPollen>()
                        .TryRegisterAfter(i);
                    r.createItem.Prefix(ModContent.PrefixType<DoubledTimePrefix>());
                }
            }
        }
    }
}