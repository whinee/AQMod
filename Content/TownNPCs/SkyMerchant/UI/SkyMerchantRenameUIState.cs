﻿using Aequus.Common.Renaming;
using Aequus.Core.UI;
using Aequus.Core.UI.Elements;
using ReLogic.Content;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Aequus.Content.TownNPCs.SkyMerchant.UI;

public class SkyMerchantRenameUIState : UIState, ILoad {
    public AequusItemSlotElement SendItem { get; private set; }
    public AequusItemSlotElement ReceiveItem { get; private set; }
    public RenameTextBox TextBox { get; private set; }

    public override void OnInitialize() {
        Left.Set(InventoryUI.LeftInventoryPosition, 0f);
        Top.Set(InventoryUI.BottomInventoryY + 12, 0f);
        Width.Set(510, 0f);
        Height.Set(100, 0f);

        TextBox = new RenameTextBox("") {
            DrawPanel = true
        };
        TextBox.OnTextChanged += (oldText, newText) => {
            if (SendItem.Item == null || SendItem.Item.IsAir) {
                return;
            }

            ReceiveItem.Item = SendItem.Item.Clone();
            RenameItem renameItem = ReceiveItem.Item.GetGlobalItem<RenameItem>();
            if (ReceiveItem.Item.Name == newText) {
                ReceiveItem.Item = null;
            }

            if (ReceiveItem.Item != null && !ReceiveItem.Item.IsAir) {
                renameItem.CustomName = newText.Trim();
                renameItem.UpdateCustomName(ReceiveItem.Item);
            }
        };
        TextBox.OnLeftClick += (sender, e) => (e as RenameTextBox).ToggleText();
        TextBox.SetTextMaxLength(60);
        TextBox.Width.Set(Width.Pixels, Width.Percent);
        TextBox.Height.Set(30, 0f);
        TextBox.Top.Set(50, 0f);
        Append(TextBox);

        SendItem = new AequusItemSlotElement(ItemSlot.Context.GuideItem, TextureAssets.InventoryBack.Value, AequusTextures.NameTagBlank) {
            CanPutItemIntoSlot = RenameItem.CanRename,
            CanTakeItemFromSlot = (i) => true,
        };
        SendItem.OnItemSwap += (from, to) => {
            SoundEngine.PlaySound(SoundID.Grab);

            ReceiveItem.Item?.TurnToAir();
            if (to == null || to.IsAir) {
                TextBox.SetText("");
                if (TextBox.IsWritingText) {
                    TextBox.ToggleText();
                }
                return;
            }

            TextBox.SetText(to.Name);

            if (!TextBox.IsWritingText) {
                TextBox.ToggleText();
            }
        };
        SendItem.CanHover = true;
        SendItem.ShowTooltip = true;
        SendItem.Width.Set(48f, 0f);
        SendItem.Height.Set(48f, 0f);

        Append(SendItem);

        ReceiveItem = new AequusItemSlotElement(ItemSlot.Context.GuideItem, TextureAssets.InventoryBack.Value, AequusTextures.NameTag) {
            CanTakeItemFromSlot = (i) => {
                ReceiveItem.GetDimensions();
                int price = RenameItem.GetRenamePrice(i);

                return price > 0 ? Main.LocalPlayer.CanAfford(price) : true;
            },
        };
        ReceiveItem.OnItemSwap += (from, to) => {
            Main.LocalPlayer.BuyItem(RenameItem.GetRenamePrice(ReceiveItem.Item));
            TextBox.SetText("");
            SendItem.Item.TurnToAir();
            SoundEngine.PlaySound(SoundID.Coins);

            if (TextBox.IsWritingText) {
                TextBox.ToggleText();
            }
        };
        ReceiveItem.CanHover = true;
        ReceiveItem.ShowTooltip = true;
        ReceiveItem.Width.Set(SendItem.Width.Pixels, SendItem.Width.Percent);
        ReceiveItem.Height.Set(SendItem.Height.Pixels, SendItem.Height.Percent);
        ReceiveItem.Left.Set(SendItem.Width.Pixels + 36f, SendItem.Width.Percent);

        Append(ReceiveItem);

        Asset<Texture2D> texturePackButtons = ModContent.Request<Texture2D>("Terraria/Images/UI/TexturePackButtons");
        UIImageFramed image = new UIImageFramed(texturePackButtons, texturePackButtons.Frame(horizontalFrames: 2, verticalFrames: 2, frameX: 1, frameY: 1));
        image.Left.Set(SendItem.Width.Pixels, SendItem.Width.Percent);
        image.Top.Set(SendItem.Top.Pixels + 8f, SendItem.Width.Percent);
        Append(image);
    }

    public override void OnActivate() {
        RemoveAllChildren();
        OnInitialize();
        Main.playerInventory = true;
        Main.npcChatText = "";
    }

    public override void OnDeactivate() {
        if (!SendItem.Item?.IsAir == true) {
            Main.LocalPlayer.QuickSpawnItem(new EntitySource_WorldEvent(), SendItem.Item, SendItem.Item.stack);
            SendItem.Item.TurnToAir();
        }
    }

    public override void Update(GameTime gameTime) {
        if (Main.LocalPlayer.TalkNPC?.ModNPC is not SkyMerchant) {
            ModContent.GetInstance<NPCChat>().Interface.SetState(null);
        }
        base.Update(gameTime);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch) {
        Main.hidePlayerCraftingMenu = true;

        CalculatedStyle style = GetDimensions();

        float savingsX = style.X + 140f;
        float savingsY = style.Y;
        ItemSlot.DrawSavings(Main.spriteBatch, savingsX, savingsY - 40f, true);

        if (!ReceiveItem.HasItem) {
            return;
        }
        int price = RenameItem.GetRenamePrice(SendItem.Item);

        string cost = Language.GetTextValue("LegacyInterface.46") + ": ";
        string priceText = ExtendLanguage.PriceTextColored(price, NoValueText: Language.GetTextValue("Mods.Aequus.Misc.PriceFree"), pulse: true);

        var font = FontAssets.MouseText.Value;
        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, cost, new Vector2(savingsX, savingsY + 24f), new Color(Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor, Main.mouseTextColor), 0f, Vector2.Zero, Vector2.One, -1f, 2f);
        ChatManager.DrawColorCodedStringWithShadow(Main.spriteBatch, font, priceText, new Vector2(savingsX + font.MeasureString(cost).X, savingsY + 24f), Color.White, 0f, Vector2.Zero, Vector2.One, -1f, 2f);
    }

    public void Load(Mod mod) { }

    public void Unload() { }
}