﻿using Aequus.Content.Items.Equipment.Accessories.GrandReward;
using Aequus.Core;
using Aequus.Core.Generator;

namespace Aequus;

public partial class AequusPlayer {
    /// <summary>
    /// Used to increase droprates. Rerolls the drop (amt of lootluck) times, if there is a decimal left, then it has a (lootluck decimal) chance of rerolling again.
    /// </summary>
    [ResetEffects()]
    public float dropRolls;
    [ResetEffects]
    public bool accGrandRewardDownside;

    [SaveData("CosmicChest")]
    [SaveDataAttribute.IsListedBoolean]
    public bool usedCosmicChest;

    private void UpdateCosmicChest() {
        if (usedCosmicChest) {
            dropRolls += CosmicChest.LuckIncrease;
        }
    }
}