﻿using AequusRemake.Core.Entities.Items.Dedications;
using Terraria.DataStructures;

namespace AequusRemake.Content.Dedicated.StudiesOfTheInkblot;

public class StudiesOfTheInkblot : ModItem {
    public const string ALTERNATE_COLORS_TIMER = nameof(StudiesOfTheInkblot);

    public string DedicateeName => "starlight.mp4";

    public Color TextColor => new Color(110, 110, 128, 255);

    public override void Load() {
        DedicationRegistry.Register(this, new DefaultDedication("starlight.mp4", new Color(110, 110, 128)));
    }

    public override void SetStaticDefaults() {
        ItemSets.ItemsThatAllowRepeatedRightClick[Type] = true;
    }

    private void DefaultUse() {
        Item.useTime = 1;
        Item.useAnimation = 1;
        Item.mana = 2;
    }
    public override void SetDefaults() {
        Item.damage = 200;
        Item.width = 16;
        Item.height = 16;
        Item.knockBack = 0f;
        Item.DamageType = DamageClass.Magic;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.UseSound = AequusSounds.StudiesOfTheInkblotUse with { MaxInstances = 1 };
        Item.rare = ItemRarityID.Red;
        Item.shootSpeed = 10f;
        Item.autoReuse = true;
        Item.noMelee = true;
        Item.value = ItemRarityID.Red;
        Item.shoot = ModContent.ProjectileType<StudiesOfTheInkblotOrbiterProj>();
        DefaultUse();
    }

    public override bool AltFunctionUse(Player player) {
        return true;
    }

    public override bool CanUseItem(Player player) {
        if (player.altFunctionUse == 2) {
            Item.useTime = 62;
            Item.useAnimation = 62;
            Item.mana = 80;
        }
        else {
            DefaultUse();
        }
        return true;
    }

    public override void HoldItem(Player player) {
        var AequusRemake = player.GetModPlayer<AequusPlayer>();
        if (Main.myPlayer == player.whoAmI) {
            if (player.ownedProjectileCounts[Item.shoot] == 0 && !player.ItemAnimationActive && !AequusRemake.TimerActive(ALTERNATE_COLORS_TIMER)) {
                for (int i = 0; i < Main.maxProjectiles; i++) {
                    if (Main.projectile[i].active && Main.projectile[i].type == Item.shoot && Main.projectile[i].owner == player.whoAmI) {
                        Main.projectile[i].Kill();
                    }
                }
                StudiesOfTheInkblotOrbiterProj.Spawn4(new EntitySource_ItemUse_WithAmmo(player, Item, 0), player.whoAmI);
            }
        }
    }

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
        var AequusRemake = player.GetModPlayer<AequusPlayer>();
        float speed = velocity.Length();
        if (player.altFunctionUse == 2) {
            for (int i = 0; i < Main.maxProjectiles; i++) {
                if (Main.projectile[i].active && Main.projectile[i].type == Item.shoot && Main.projectile[i].owner == player.whoAmI) {
                    Main.projectile[i].Kill();
                }
            }
            int rand = Main.rand.Next(62);
            bool alternateColors = AequusRemake.TimerActive(ALTERNATE_COLORS_TIMER);
            for (int i = 0; i < 62; i++) {
                int p = Projectile.NewProjectile(source, position, new Vector2(speed * 0.3f, 0f).RotatedBy(MathHelper.TwoPi / 62f * i),
                    ModContent.ProjectileType<StudiesOfTheInkblotProj>(), damage * 3, knockback, player.whoAmI, 100f + 40f * (1f / 62f * ((i + rand) % 62f)), speed * 0.5f);
                Main.projectile[p].localAI[0] = 1.5f;
                Main.projectile[p].frame = alternateColors ? 1 : 5;
            }

            if (alternateColors && AequusRemake.TryGetTimer(ALTERNATE_COLORS_TIMER, out var timer)) {
                timer.TimePassed = timer.MaxTime - 1;
            }
            else {
                AequusRemake.SetTimer(ALTERNATE_COLORS_TIMER, Item.useTime * 2);
            }
            DefaultUse();
        }
        else {
            for (int i = 0; i < Main.maxProjectiles; i++) {
                if (Main.projectile[i].active && Main.projectile[i].type == Item.shoot && Main.projectile[i].owner == player.whoAmI) {
                    int p = Projectile.NewProjectile(source, Main.projectile[i].Center, Vector2.Normalize(position - Main.projectile[i].Center) * 0.01f, ModContent.ProjectileType<StudiesOfTheInkblotProj>(), damage, knockback, player.whoAmI, 0f, speed);
                    Main.projectile[p].localAI[0] = 1.5f;
                    Main.projectile[p].frame = Main.projectile[i].frame;
                }
            }
        }
        return false;
    }

    public override void AddRecipes() {
        CreateRecipe()
            .AddIngredient(ItemID.LunarFlareBook)
            .AddIngredient(ItemID.SpellTome)
            .AddIngredient(ItemID.Shrimp, 5)
            .AddIngredient(ItemID.SuspiciousLookingEye)
            .AddIngredient(ItemID.BlackInk)
            .AddTile(TileID.LunarCraftingStation)
            .Register();
    }
}