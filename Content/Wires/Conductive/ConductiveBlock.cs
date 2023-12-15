﻿using Aequus;
using Aequus.Common.Tiles;
using Aequus.Common.Tiles.Components;
using Aequus.Common.Wires;
using Aequus.Core.Graphics.Animations;
using Aequus.Core.Graphics.Tiles;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Content.Wires.Conductive;

[Autoload(false)]
internal class ConductiveBlock : InstancedTile, ISpecialTileRenderer, ICustomPlaceSound, ITouchEffects {
    private readonly Color _mapColor;
    private readonly int _dustType;

    public static Color ElectricColor { get; set; } = new(255, 210, 120, 0);
    public static int FlameOffsetCount { get; set; } = 7;

    public ConductiveBlock(string name, Color mapColor, int dustType) : base($"ConductiveBlock{name}", $"{typeof(ConductiveBlock).NamespaceFilePath()}/ConductiveBlock{name}") {
        _mapColor = mapColor;
        _dustType = dustType;
    }

    public override void SetStaticDefaults() {
        Main.tileSolid[Type] = true;
        Main.tileBlockLight[Type] = true;
        DustType = _dustType;
        HitSound = AequusSounds.ConductiveBlock;
        AddMapEntry(_mapColor, CreateMapEntryName());
    }

    public override bool Slope(int i, int j) {
        return false;
    }

    public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;

    public override void HitWire(int i, int j) {
        if (!Wiring.CheckMech(i, j, 60)) {
            return;
        }
        ModContent.GetInstance<CircuitSystem>().HitCircuit(i, j);
        //ActivateEffect(i, j);
        //if (!Wiring.CheckMech(i, j, ConductiveSystem.PoweredLocation == Point.Zero ? ConductiveSystem.ActivationDelay : 0)) {
        //    return;
        //}

        //if (ConductiveSystem.PoweredLocation == Point.Zero) {
        //    if (Main.netMode != NetmodeID.MultiplayerClient) {
        //        Projectile.NewProjectile(new EntitySource_Wiring(i, j), new(i * 16f + 8f, j * 16f + 8f), Vector2.Zero, ModContent.ProjectileType<ConductiveProjectile>(), 20, 1f, Main.myPlayer);
        //    }

        //    var oldPoweredLocation = ConductiveSystem.PoweredLocation;
        //    ConductiveSystem.PoweredLocation = new(i, j);
        //    ConductiveSystem.PoweredLocation = oldPoweredLocation;
        //    return;
        //}

        //if (Main.tile[ConductiveSystem.PoweredLocation].TileType != Type) {
        //    return;
        //}
    }

    public override bool PreDraw(int i, int j, SpriteBatch spriteBatch) {
        var tile = Main.tile[i, j];
        var drawCoordinates = (new Vector2(i * 16f, j * 16f) - Main.screenPosition + TileHelper.DrawOffset).Floor();
        var frame = new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16);
        var color = Lighting.GetColor(i, j);
        spriteBatch.Draw(TextureAssets.Tile[Type].Value, drawCoordinates, frame, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);

        if (AnimationSystem.TryGet<ConductiveAnimation>(i, j, out var animation)) {
            SpecialTileRenderer.AddSolid(i, j, TileRenderLayerID.PostDrawLiquids);
        }
        return false;
    }

    public void Render(int i, int j, byte layer) {
        if (!AnimationSystem.TryGet<ConductiveAnimation>(i, j, out var animation)) {
            return;
        }

        var tile = Main.tile[i, j];
        var drawCoordinates = (new Vector2(i * 16f + 8f, j * 16f + 8f) - Main.screenPosition).Floor();
        var frame = new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16);
        var electricColor = ElectricColor * animation.electricAnimation;
        var fastRandom = Helper.RandomTileCoordinates(i, j);
        float globalIntensity = animation.intensity;
        for (int k = 0; k < FlameOffsetCount; k++) {
            Main.spriteBatch.Draw(TextureAssets.Tile[Type].Value, (drawCoordinates + AnimationSystem.FlameOffsets[k] * animation.electricAnimation).Floor(), frame, electricColor * 0.5f * globalIntensity, 0f, new Vector2(8f), 1f, SpriteEffects.None, 0f);
        }

        if (!Aequus.highQualityEffects) {
            return;
        }

        var shockTexture = AequusTextures.BaseParticleTexture.Value;
        int amount = 12;
        for (int k = 0; k < amount; k++) {
            float time = Main.GlobalTimeWrappedHourly * fastRandom.NextFloat(1f, 5f) % 3f;
            float vectorRotationWave = Main.GlobalTimeWrappedHourly * fastRandom.NextFloat(-1f, 1f);
            float rotation = fastRandom.NextFloat(MathHelper.TwoPi) + vectorRotationWave;
            float positionMagnitude = fastRandom.NextFloat(14f, 40f);
            float colorMultiplier = fastRandom.NextFloat(0.5f, 1f);
            int frameY = fastRandom.Next(3);
            float scale = fastRandom.NextFloat(1f, 2f);
            float intensity = Math.Min(MathF.Pow(animation.electricAnimation, k) * 2f, 1f);
            if (time > 1f) {
                continue;
            }

            var shockFrame = shockTexture.Frame(verticalFrames: 3, frameY: frameY);
            float wave = MathF.Sin(time * MathHelper.Pi);
            Main.spriteBatch.Draw(shockTexture,
                (drawCoordinates + new Vector2((1f - MathF.Pow(1f - time, 2f)) * positionMagnitude, 0f).RotatedBy(rotation)).Floor(),
                shockFrame,
                electricColor * wave * 2f * colorMultiplier * intensity * globalIntensity,
                rotation,
                shockFrame.Size() / 2f,
                new Vector2(scale, 1f) * wave * 0.75f * intensity,
                SpriteEffects.None,
                0f);
        }
    }

    public void PlaySound(int i, int j, bool forced, int plr, int style, bool PlaceTile) {
        if (PlaceTile) {
            SoundEngine.PlaySound(AequusSounds.ConductiveBlockPlaced, new Vector2(i * 16f + 8f, j * 16f + 8f));
        }
    }

    public void Touch(int i, int j, Player player, AequusPlayer aequusPlayer) {
        if (!AnimationSystem.TryGet<ConductiveAnimation>(i, j, out var animation) || animation.electricAnimation < 0.5f) {
            return;
        }

        player.Hurt(PlayerHelper.CustomDeathReason("Mods.Aequus.Player.DeathMessage.Conductive." + Main.rand.Next(5), player.name), 120, 0, cooldownCounter: ImmunityCooldownID.TileContactDamage, knockback: 0f);
    }
}