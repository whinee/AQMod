﻿using AequusRemake.Core.Graphics;

namespace AequusRemake.Core.Hooks;

public partial class TerrariaHooks {
    private static void On_Main_DrawDust(On_Main.orig_DrawDust orig, Main main) {
        orig(main);
        DrawLayers.Instance.PostDrawDust?.Draw(Main.spriteBatch);
    }
}
