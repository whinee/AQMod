﻿using Aequus.Buffs;
using Aequus.Projectiles.Misc;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Prefixes.Potions
{
    public class SplashPrefix : AequusPrefix
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("{$Mods.Aequus.PrefixName." + Name + "}");
        }

        public override void Apply(Item item)
        {
            item.useStyle = ItemUseStyleID.Swing;
            item.UseSound = SoundID.Item1;
            item.shoot = ModContent.ProjectileType<SplashPotionProj>();
            item.shootSpeed = 10f;
            item.noUseGraphic = true;
        }

        public override void ModifyValue(ref float valueMult)
        {
            valueMult = 1f;
        }

        public override bool CanRoll(Item item)
        {
            return item.buffType > 0 && item.buffTime > 600 && item.consumable && item.useStyle == ItemUseStyleID.DrinkLiquid
                && item.healLife <= 0 && item.healMana <= 0 && item.damage < 0 && item.shoot == ProjectileID.None && !Main.meleeBuff[item.buffType] &&
                !AequusBuff.ConcoctibleBuffsBlacklist.Contains(item.buffType);
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            for (int i = 0; i < tooltips.Count; i++)
            {
                if (tooltips[i].Mod == "Terraria" && tooltips[i].Name == "PrefixShootSpeed")
                {
                    tooltips[i] = new TooltipLine(Mod, "PrefixSplash", AequusText.GetText("Prefixes.SplashPotion")) { IsModifier = true, IsModifierBad = false, };
                }
            }
        }
    }
}