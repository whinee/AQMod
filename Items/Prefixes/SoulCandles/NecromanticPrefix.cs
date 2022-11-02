﻿using Aequus.Items.Weapons.Summon.Necro.Candles;
using System;
using Terraria;

namespace Aequus.Items.Prefixes.SoulCandles
{
    public class NecromanticPrefix : SoulCandlePrefix
    {
        public override void Apply(Item item)
        {
            if (item.ModItem is BaseSoulCandle candle)
            {
                candle.ClearPrefix();
                candle.soulLimit = Math.Max((int)(candle.soulLimit * 1.25f), candle.soulLimit + 1);
                candle.soulCost = Math.Max((int)(candle.soulCost * 0.75f), candle.soulCost - 1);
            }
        }

        public override void ModifyValue(ref float valueMult)
        {
            valueMult += 0.4f;
        }
    }
}