﻿using Aequus.Core.ContentGeneration;

namespace Aequus.Content.Biomes.PollutedOcean.Water;

public class PollutedWater : UnifiedWaterStyle {
    public override void Load() {
        base.Load();

        (DropletType as InstancedWaterDroplet)?.OverrideSound(SoundID.Drip with { Volume = 0.066f });

        HairColor = Color.Turquoise;
    }
}