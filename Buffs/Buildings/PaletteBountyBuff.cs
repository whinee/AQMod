﻿using Terraria;
using Terraria.ModLoader;

namespace Aequus.Buffs.Buildings {
    public class PaletteBountyBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.Aequus().spawnrateMultiplier = 1f;
            player.Aequus().maxSpawnsDivider += 1f;
            player.Aequus().villagerHappiness += 0.1f;
        }

        public override bool RightClick(int buffIndex)
        {
            return false;
        }
    }
}