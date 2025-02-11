﻿using Aequus.Common.Structures.Conditions;
using Aequus.Common.Structures.Conversion;
using System.Collections.Generic;
using System.Linq;
using Terraria.GameContent.ItemDropRules;

namespace Aequus.Systems.Chests.DropRules;

/// <summary>This rule goes through each rule sequentually. This is similar to how items in the Dungeon are handled.</summary>    
public record class IndexedChestRule(int ChanceDemoninator, int ChanceNumerator, IChestLootRule[] Options, params Condition[] OptionalConditions)
    : IChestLootRule, IConvertDropRules {
    public int Index { get; private set; }
    public int RuleIndex => Index % Options.Length;

    public IndexedChestRule(int ChanceDenominator, IChestLootRule[] Options, params Condition[] OptionalConditions) : this(ChanceDenominator, 1, Options, OptionalConditions) { }

    public List<IChestLootChain> ChainedRules { get; set; } = IChestLootChain.GetFromSelfRules(Options);
    public ConditionCollection Conditions { get; set; } = new(OptionalConditions);

    public ChestLootResult AddItem(in ChestLootInfo info) {
        if (info.RNG.Next(ChanceDemoninator) != 0) {
            return ChestLootResult.FailedRandomRoll;
        }

        ChestLootResult result = ChestLootResult.DidNotRunCode;
        do {
            // Get the next rule index.
            IChestLootRule selectedRule = Options[RuleIndex];

            // Solve the rule.
            result = ChestLootDatabase.Instance.SolveSingleRule(selectedRule, in info);

            // Increment index.
            Index++;
        }
        // Repeat the above code if the rule doesn't fullfill conditions
        // For example, chests may have different loot in the Remix seed, making the rule not satisfy conditions.
        // So we increment to the next rule.
        while (result.State == ItemDropAttemptResultState.DoesntFillConditions);

        return result;
    }

    public void Reset() {
        // Reset the index upon world generation and etc.
        ResetIndex();
    }

    public void ResetIndex() {
        Index = 0;
    }

    public IChestLootRule ToChestDropRule() {
        return this;
    }

    public IItemDropRule ToItemDropRule() {
        return new OneFromRulesRule(1, Options.SelectWhereOfType<IConvertDropRules>()
            .Select(convert => convert.ToItemDropRule())
            .ToArray());
    }
}
