﻿using Aequus.DataSets;

namespace Aequus.Content.Tools.Keys;

public class CopperKey : ModItem {
    public override void SetStaticDefaults() {
        ItemDataSet.KeychainData[Type] = new(NonConsumable: false, Color: new Color(183, 88, 25));
    }

    public override void SetDefaults() {
        Item.CloneDefaults(ItemID.GoldenKey);
        Item.rare = ItemRarityID.White;
    }
}
