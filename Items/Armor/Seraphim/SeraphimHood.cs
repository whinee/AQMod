﻿using Aequus.Buffs.Minion;
using Aequus.Items.Armor.Gravetender;
using Aequus.Items.Armor.Necromancer;
using Aequus.Items.Materials;
using Aequus.Projectiles.Summon.Misc;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Armor.Seraphim
{
    [AutoloadEquip(EquipType.Head)]
    public class SeraphimHood : NecromancerHood
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.defense = 9;
            Item.width = 20;
            Item.height = 20;
            Item.rare = ItemRarityID.Yellow;
            Item.shoot = ModContent.ProjectileType<NecromancerHoodSpawnerProj>();
            Item.value = Item.sellPrice(gold: 1);
            EnemySpawn = new int[]
            {
                NPCID.BlueArmoredBones,
                NPCID.BlueArmoredBonesMace,
                NPCID.BlueArmoredBonesNoPants,
                NPCID.BlueArmoredBonesSword,
                NPCID.HellArmoredBones,
                NPCID.HellArmoredBonesMace,
                NPCID.HellArmoredBonesSpikeShield,
                NPCID.HellArmoredBonesSword,
                NPCID.RustyArmoredBonesAxe,
                NPCID.RustyArmoredBonesFlail,
                NPCID.RustyArmoredBonesSword,
                NPCID.RustyArmoredBonesSwordNoArmor,
            };
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<SeraphimRobes>();
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = TextHelper.GetTextValue("ArmorSetBonus.Seraphim");
            var aequus = player.Aequus();
            aequus.armorNecromancerBattle = this;
            aequus.setSeraphim = Item;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage<SummonDamageClass>() += 0.2f;
            player.Aequus().ghostSlotsMax++;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<NecromancerHood>()
                .AddIngredient<Hexoplasm>(8)
                .AddTile(TileID.Loom)
                .TryRegisterBefore((ItemID.GravediggerShovel));
        }
    }

    public class SeraphimHoodSpawnerProj : NecromancerHoodSpawnerProj 
    {
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.Red;
        }
    }
}