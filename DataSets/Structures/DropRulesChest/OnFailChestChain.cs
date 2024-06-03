﻿using Terraria.GameContent.ItemDropRules;

namespace Aequus.DataSets.Structures.DropRulesChest;

public class OnFailChestChain : IChestLootChain {
    public IChestLootRule RuleToChain { get; private set; }

    public OnFailChestChain(IChestLootRule rule) {
        RuleToChain = rule;
    }

    public bool CanChainIntoRule(ChestLootResult parentResult) {
        return parentResult.State != ItemDropAttemptResultState.Success;
    }
}
