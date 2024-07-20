﻿using Terraria.DataStructures;
using Terraria.Localization;

namespace Aequus.Items.Accessories.Informational.Monocle;

public class MonocleBuilderToggle : BuilderToggle {
    public override bool Active() {
        return Main.LocalPlayer.TryGetModPlayer<AequusPlayer>(out var AequusPlayer) && AequusPlayer.accInfoMoneyMonocle;
    }

    public override string DisplayValue() {
        return Language.GetTextValue("Mods.Aequus.Misc.MonocleToggle" + (CurrentState == 0 ? "On" : "Off"));
    }

    public override bool Draw(SpriteBatch spriteBatch, ref BuilderToggleDrawParams drawParams) {
        drawParams.Color = CurrentState == 0 ? Color.White : Color.Gray;
        return true;
    }
}