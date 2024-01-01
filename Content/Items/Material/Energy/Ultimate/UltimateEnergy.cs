﻿using Aequus.Common.Items;
using Aequus.Content.Items.Material.Energy.Aquatic;
using Aequus.Content.Items.Material.Energy.Atmospheric;
using Aequus.Content.Items.Material.Energy.Cosmic;
using Aequus.Content.Items.Material.Energy.Demonic;
using Aequus.Content.Items.Material.Energy.Organic;
using Microsoft.Xna.Framework;

namespace Aequus.Content.Items.Material.Energy.Ultimate;

public class UltimateEnergy : EnergyItemBase<EnergyParticle> {
    protected override Vector3 LightColor => Main.DiscoColor.ToVector3() * 0.5f;
    public override int Rarity => ItemRarityID.Cyan;

    public override void SetStaticDefaults() {
        base.SetStaticDefaults();
        ItemID.Sets.SortingPriorityMaterials[Type] = ItemSortingPriority.Materials.UltimateEnergy;
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(ModContent.ItemType<DemonicEnergy>())
            .AddIngredient(ModContent.ItemType<AtmosphericEnergy>())
            .AddIngredient(ModContent.ItemType<OrganicEnergy>())
            .AddIngredient(ModContent.ItemType<AquaticEnergy>())
            .AddIngredient(ModContent.ItemType<CosmicEnergy>())
            .AddTile(TileID.LunarCraftingStation)
            .Register();
    }
}