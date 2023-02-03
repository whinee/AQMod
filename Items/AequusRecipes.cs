﻿using Aequus.Items.Misc.Energies;
using Aequus.Items.Misc.Materials;
using Aequus.Items.Placeable.Moss;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Aequus.Items
{
    public class AequusRecipes : ModSystem
    {
        /// <summary>
        /// A condition which locks a recipe behind the <see cref="AequusWorld.downedOmegaStarite"/> flag.
        /// </summary>
        public static Recipe.Condition ConditionOmegaStarite { get; private set; }

        /// <summary>
        /// <see cref="RecipeGroup"/> for <see cref="ItemID.Ectoplasm"/> and <see cref="Hexoplasm"/>.
        /// </summary>
        public static RecipeGroup AnyEctoplasm { get; private set; }
        /// <summary>
        /// <see cref="RecipeGroup"/> for <see cref="ArgonMushroom"/>, <see cref="KryptonMushroom"/>, and <see cref="XenonMushroom"/>.
        /// </summary>
        public static RecipeGroup AnyMosshrooms { get; private set; }
        /// <summary>
        /// <see cref="RecipeGroup"/> for all IDs in <see cref="Main.anglerQuestItemNetIDs"/>.
        /// </summary>
        public static RecipeGroup AnyQuestFish { get; private set; }
        /// <summary>
        /// <see cref="RecipeGroup"/> for all IDs in <see cref="AequusItem.FruitIDs"/>.
        /// </summary>
        public static RecipeGroup AnyFruit { get; private set; }

        public override void AddRecipeGroups()
        {
            ConditionOmegaStarite = new Recipe.Condition(NetworkText.FromKey("Mods.Aequus.RecipeCondition.OmegaStarite"), (r) => AequusWorld.downedOmegaStarite);

            AnyEctoplasm = NewGroup("AnyEctoplasm",
                ItemID.Ectoplasm, ModContent.ItemType<Hexoplasm>());
            AnyMosshrooms = NewGroup("AnyMosshroom",
                ModContent.ItemType<ArgonMushroom>(), ModContent.ItemType<KryptonMushroom>(), ModContent.ItemType<XenonMushroom>());
            AnyQuestFish = NewGroup("AnyQuestFish", Main.anglerQuestItemNetIDs.CloneArray());
            AnyFruit = NewGroup("AnyFruit", AequusItem.FruitIDs.ToArray());
        }

        private static RecipeGroup NewGroup(string name, params int[] items)
        {
            var group = new RecipeGroup(() => AequusText.GetText("RecipeGroup." + name), items);
            RecipeGroup.RegisterGroup("Aequus:" + name, group);
            return group;
        }

        // Recipe Edits
        public override void PostAddRecipes()
        {
            for (int i = 0; i < Main.recipe.Length; i++)
            {
                Recipe r = Main.recipe[i];
                if (GameplayConfig.Instance.EarlyGravityGlobe)
                {
                    if (r.HasIngredient(ItemID.GravityGlobe) && !r.HasIngredient(ItemID.LunarBar))
                    {
                        r.AddIngredient(ItemID.LunarBar, 5);
                    }
                }
                if (GameplayConfig.Instance.EarlyPortalGun)
                {
                    if (r.HasIngredient(ItemID.PortalGun) && !r.HasIngredient(ItemID.LunarBar))
                    {
                        r.AddIngredient(ItemID.LunarBar, 5);
                    }
                }
                switch (r.createItem?.type)
                {
                    case ItemID.PumpkinMoonMedallion:
                    case ItemID.NaughtyPresent:
                        {
                            r.ReplaceItemWith(ItemID.Ectoplasm,
                                (r, i) => r.AddRecipeGroup(AnyEctoplasm, i.stack));
                        }
                        break;

                    case ItemID.VoidLens:
                    case ItemID.VoidVault:
                        {
                            if (!GameplayConfig.Instance.VoidBagRecipe)
                                continue;

                            for (int j = 0; j < r.requiredItem.Count; j++)
                            {
                                if (r.requiredItem[j].type == ItemID.JungleSpores)
                                {
                                    r.requiredItem[j].SetDefaults(ModContent.ItemType<DemonicEnergy>());
                                    r.requiredItem[j].stack = 1;
                                }
                            }
                        }
                        break;
                }
            }
        }

        public static Recipe CreateRecipe(Recipe parent)
        {
            var r = CreateRecipe(parent.createItem.type, parent.createItem.stack);

            for (int i = 0; i < parent.requiredItem.Count; i++)
            {
                r.requiredItem.Add(parent.requiredItem[i].Clone());
            }
            for (int i = 0; i < parent.requiredTile.Count; i++)
            {
                r.requiredTile.Add(parent.requiredTile[i]);
            }
            for (int i = 0; i < parent.acceptedGroups.Count; i++)
            {
                r.acceptedGroups.Add(parent.acceptedGroups[i]);
            }
            for (int i = 0; i < parent.Conditions.Count; i++)
            {
                r.Conditions.Add(parent.Conditions[i]); // Same object reference, but conditions shouldn't be carrying instanced data which gets changed so it shouldn't be a problem, I think
            }
            return r;
        }
        public static Recipe CreateRecipe(int result, int stack = 1)
        {
            return Recipe.Create(result, stack);
        }

        public static void CreateShimmerTransmutation(RecipeGroup ingredient, int result, Recipe.Condition condition = null)
        {
            Recipe.Create(result)
                .AddRecipeGroup(ingredient)
                .AddIngredient(condition != null ? ModContent.ItemType<CosmicEnergy>() : ItemID.FallenStar, condition != null ? 1 : 5)
                .AddTile(TileID.DemonAltar)
                .Register();
        }
        public static void CreateShimmerTransmutation(int ingredient, int result, Recipe.Condition condition = null)
        {
            Recipe.Create(result)
                .AddIngredient(ingredient)
                .AddIngredient(condition != null ? ModContent.ItemType<CosmicEnergy>() : ItemID.FallenStar, condition != null ? 1 : 5)
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}