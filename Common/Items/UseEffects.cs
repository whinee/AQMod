﻿using Aequus.Core.CodeGeneration;

namespace Aequus.Common.Items;

[Gen.AequusPlayer_Field<bool>("forceUseItem")]
[Gen.AequusPlayer_Field<byte>("disableItem")]
public class UseEffects : GlobalItem {
    public override bool? UseItem(Item item, Player player) {
        return player.GetModPlayer<AequusPlayer>().disableItem == 0 ? null : false;
    }

    public override bool CanShoot(Item item, Player player) {
        return player.GetModPlayer<AequusPlayer>().disableItem == 0;
    }

    [Gen.AequusPlayer_SetControls]
    internal static void ForceItemUse(Player player, AequusPlayer aequusPlayer) {
        if (aequusPlayer.forceUseItem) {
            player.controlUseItem = true;
            player.releaseUseItem = true;
            player.itemAnimation = 0;
        }
        aequusPlayer.forceUseItem = false;
    }
}