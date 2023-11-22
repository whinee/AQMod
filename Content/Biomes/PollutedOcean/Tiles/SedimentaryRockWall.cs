﻿using Aequus.Common.Tiles;
using Terraria;
using Terraria.ModLoader;

namespace Aequus.Content.Biomes.PollutedOcean.Tiles;

public class SedimentaryRockWall : ModWall {
    public override void Load() {
        Mod.AddContent(new InstancedWallItem(this, dropItem: true));
    }

    public override void SetStaticDefaults() {
        Main.wallHouse[Type] = true;
        DustType = 209;
        AddMapEntry(new(70, 40, 20));
    }
}