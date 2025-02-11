﻿using Terraria.ModLoader.Default;

namespace Aequus.Content;

[LegacyName("Crabax")]
[LegacyName("GalaxyCommission")]
[LegacyName("HalloweenEnergy")]
[LegacyName("Heliosis")]
[LegacyName("ImpenetrableCoating")]
[LegacyName("LiquidGun")]
[LegacyName("LittleInferno")]
[LegacyName("Mendshroom")]
[LegacyName("Moro")]
[LegacyName("NecromancyPotion")]
[LegacyName("ShutterstockerClip")]
[LegacyName("ShutterstockerClipAmmo")]
[LegacyName("SilkHammer")]
[LegacyName("SilkPickaxe")]
[LegacyName("TubOfCookieDough")]
[LegacyName("UltraStariteBanner")]
[LegacyName("WhitePhial")]
[LegacyName("XmasEnergy")]
[LegacyName("DesertRoulette")]
[LegacyName("GoldenRoulette")]
[LegacyName("JungleSlotMachine")]
[LegacyName("Roulette")]
[LegacyName("ShadowRoulette")]
[LegacyName("SkyRoulette")]
[LegacyName("SnowRoulette")]
[LegacyName("Photobook")]
[LegacyName("PhotobookItem")]
[LegacyName("PeonyPhotobook")]
public class UnusedItem : ModItem {
    public override string Texture => ModContent.GetInstance<UnloadedItem>().Texture;

    public override void SetStaticDefaults() {
        Item.ResearchUnlockCount = 0;
    }

    public override void SetDefaults() {
        Item.width = 8;
        Item.height = 8;
        Item.rare = ItemRarityID.Gray;
    }
}
