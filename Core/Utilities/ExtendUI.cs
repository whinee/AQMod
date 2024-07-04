﻿using AequusRemake.Core.UI;
using Terraria.UI;

namespace AequusRemake.Core.Utilities;

public static class ExtendUI {
    // TODO: Make this actually check the item slot contexts
    public static bool CurrentlyDrawingHotbarSlot => !Main.playerInventory;

    public static void SetState<T>(this UserInterface ui) where T : UIState, ILoadable {
        ui.SetState(ModContent.GetInstance<T>());
    }

    /// <summary>Activates the UI Layer.</summary>
    public static void Activate(this IUserInterfaceLayer layer) {
        if (layer.IsActive) {
            return;
        }

        layer.OnActivate();
        ModContent.GetInstance<UILayersSystem>()._active.AddLast(layer);
        layer.IsActive = true;
    }
    /// <summary>Deactivates the UI Layer. This does not instantly remove the layer, instead it will be removed on the next ui update.</summary>
    public static void Deactivate(this IUserInterfaceLayer layer) {
        if (!layer.IsActive) {
            return;
        }

        layer.OnDeactivate();
        layer.IsActive = false;
    }

    public static void DrawUIPanel(SpriteBatch sb, Texture2D texture, Rectangle rect, Color color = default(Color)) {
        Utils.DrawSplicedPanel(sb, texture, rect.X, rect.Y, rect.Width, rect.Height, 10, 10, 10, 10, color == default ? Color.White : color);
    }

    /// <summary>
    /// Draws something with its position set to the center of the inventory slot.
    /// Only use in Pre/PostDraw hooks for items.
    /// </summary>
    public static void InventoryDrawCentered(SpriteBatch spriteBatch, Texture2D texture, Vector2 itemPosition, Rectangle? frame, Color color, float rotation, Vector2 origin, float scale, SpriteEffects spriteEffects = SpriteEffects.None, Vector2 offset = default) {
        spriteBatch.Draw(texture, itemPosition + offset, frame, color, rotation, origin, scale, spriteEffects, 0f);
    }

    public static void HoverItem(Item item, int context = -1) {
        Main.hoverItemName = item.Name;
        Main.HoverItem = item.Clone();
        Main.HoverItem.tooltipContext = context;
        Main.instance.MouseText("");
    }

    public static int BottomLeftInventoryX(bool ignoreCreative = false) {
        int left = InventoryUI.LeftInventoryPosition;
        if (!ignoreCreative && Main.LocalPlayer.difficulty == 3 && !Main.CreativeMenu.Blocked) {
            left += 48;
        }
        return UISystem.bottomLeftInventoryOffsetX + left;
    }
}