﻿using Aequus.Graphics.ShaderData;
using Aequus.Items.Materials;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;

namespace Aequus.Items.Misc.Dyes.Ancient
{
    public class AncientScorchingDye : DyeItemBase
    {
        public override Ref<Effect> Effect => FromPath("Dyes/ScorchingDyeShader");
        public override string Pass => "RedSpritePass";
        public override int Rarity => ItemRarityID.Orange;

        public override ArmorShaderData CreateShaderData()
        {
            return new ArmorShaderDataModifyLightColor(Effect, Pass, (v) =>
            {
                return v * new Vector3(0.549f, 0f, 0.082f);
            }).UseColor(new Color(140, 0, 21, 255));
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.BottledWater)
                .AddIngredient<Fluorescence>()
                .AddTile(TileID.DyeVat)
                .AddCondition(Recipe.Condition.InGraveyardBiome)
                .TryRegisterAfter(ItemID.FlameDye);
        }
    }
}