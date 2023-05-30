﻿using Aequus.Content;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Buffs {
    public class NeutronYogurtBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            PotionColorsDatabase.BuffToColor.Add(Type, new Color(61, 219, 255));
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.gravity *= 1.8f;
        }
    }
}