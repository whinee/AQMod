﻿using Aequus.Content.CursorDyes.Items;
using Aequus.Items.Tools;
using Aequus.Items.Vanity.Pets.Light;
using Aequus.Items.Weapons.Melee;
using Aequus.Items.Weapons.Melee.BattleAxe;
using Aequus.Items.Weapons.Melee.Thrown;
using Aequus.Items.Weapons.Necromancy.Candles;
using Aequus.Items.Weapons.Necromancy.Scepters;
using Aequus.Items.Weapons.Ranged.Misc;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Aequus.Items.Accessories.Misc;
using Aequus.Items.Accessories.Combat.Necro;
using Aequus.Items.Accessories.Combat.OnHit.Debuff;

namespace Aequus.Content.CrossMod {
    internal class CerebralMod : ModSupport<CerebralMod>
    {
        private void AddCrafterRecipe(string crafterName, int tile = TileID.Anvils, params int[] items)
        {
            try
            {
                if (Instance.TryFind<ModItem>(crafterName, out var crafter) && crafter.Type > 0)
                {
                    foreach (var item in items)
                    {
                        Recipe.Create(item)
                            .AddIngredient(crafter.Type)
                            .AddTile(tile)
                            .Register();
                    }
                }
            }
            catch (Exception ex)
            {
                Mod.Logger.Error($"{ex.Message}\n{ex.StackTrace}");
            }
        }

        public override void AddRecipes()
        {
            if (Instance == null)
            {
                return;
            }

            AddCrafterRecipe("GoldenChestCrafter", TileID.Anvils,
                ModContent.ItemType<Bellows>(),
                ModContent.ItemType<BoneHawkRing>(),
                ModContent.ItemType<GlowCore>(),
                ModContent.ItemType<SwordCursor>(),
                ModContent.ItemType<MiningPetSpawner>());

            AddCrafterRecipe("FrozenChestCrafter", TileID.Anvils,
                ModContent.ItemType<CrystalDagger>());

            AddCrafterRecipe("SkywareChestCrafter", TileID.Anvils,
                ModContent.ItemType<Slingshot>());

            AddCrafterRecipe("ShadowOrbCrafter", TileID.Anvils,
                ModContent.ItemType<CorruptionCandle>());

            AddCrafterRecipe("CrimsonHeartCrafter", TileID.Anvils,
                ModContent.ItemType<CrimsonCandle>());

            AddCrafterRecipe("DungeonChestCrafter", TileID.Anvils,
                ModContent.ItemType<Valari>(),
                ModContent.ItemType<Revenant>(),
                ModContent.ItemType<DungeonCandle>(),
                ModContent.ItemType<PandorasBox>());
        }
    }
}