﻿using Aequus.Common.Tiles;
using Microsoft.Xna.Framework;
using System;
using System.Runtime.CompilerServices;
using Terraria;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Aequus.Core.Utilities;

public static class TileHelper {
    public static bool IsSolidTileAnchor(this TileAnchorDirection tileAnchor) {
        return tileAnchor != TileAnchorDirection.Invalid && tileAnchor != TileAnchorDirection.Wall;
    }

    public static TileAnchorDirection GetGemFramingAnchor(int i, int j, bool[] invalidTiles = null) {
        var tile = Framing.GetTileSafely(i, j - 1);
        if (tile.HasTile && !tile.BottomSlope && tile.TileType >= 0 && invalidTiles?[tile.TileType] != true && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType]) {
            return TileAnchorDirection.Top;
        }

        tile = Framing.GetTileSafely(i, j + 1);
        if (tile.HasTile && !tile.IsHalfBlock && !tile.TopSlope && tile.TileType >= 0 && invalidTiles?[tile.TileType] != true && (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType])) {
            return TileAnchorDirection.Bottom;
        }

        tile = Framing.GetTileSafely(i - 1, j);
        if (tile.HasTile && !tile.IsHalfBlock && !tile.RightSlope && tile.TileType >= 0 && invalidTiles?[tile.TileType] != true && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType]) {
            return TileAnchorDirection.Left;
        }

        tile = Framing.GetTileSafely(i + 1, j);
        if (tile.HasTile && !tile.IsHalfBlock && !tile.LeftSlope && tile.TileType >= 0 && invalidTiles?[tile.TileType] != true && Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType]) {
            return TileAnchorDirection.Right;
        }

        return TileAnchorDirection.Invalid;
    }

    public static void GemFraming(int i, int j, bool[] invalidTiles = null) {
        var tile = Framing.GetTileSafely(i, j);
        var obj = TileObjectData.GetTileData(tile.TileType, 0);
        int coordinateFullHeight = obj?.CoordinateFullHeight ?? 18;
        switch (GetGemFramingAnchor(i, j, invalidTiles)) {
            case TileAnchorDirection.Bottom: {
                    tile.TileFrameY = (short)(WorldGen.genRand.Next(0, 3) * coordinateFullHeight);
                }
                break;
            case TileAnchorDirection.Top: {
                    tile.TileFrameY = (short)(WorldGen.genRand.Next(3, 6) * coordinateFullHeight);
                }
                break;
            case TileAnchorDirection.Left: {
                    tile.TileFrameY = (short)(WorldGen.genRand.Next(6, 9) * coordinateFullHeight);
                }
                break;
            case TileAnchorDirection.Right: {
                    tile.TileFrameY = (short)(WorldGen.genRand.Next(9, 12) * coordinateFullHeight);
                }
                break;
            default:
                WorldGen.KillTile(i, j);
                break;
        }
    }

    public static void CutTilesRectangle(Rectangle box, TileCuttingContext context, bool[] tileCutIgnore) {
        int left = Math.Max((int)(box.X / 16f), 1);
        int right = Math.Min((int)(left + box.Width / 16f) + 1, Main.maxTilesX);
        int top = Math.Max((int)(box.Y / 16f), 0);
        int bottom = Math.Min((int)(top + box.Height / 16f) + 1, Main.maxTilesY);
        for (int i = left; i < right; i++) {
            for (int j = top; j < bottom; j++) {
                if (Main.tile[i, j] != null && Main.tileCut[Main.tile[i, j].TileType] && !tileCutIgnore[Main.tile[i, j].TileType] && WorldGen.CanCutTile(i, j, context)) {
                    WorldGen.KillTile(i, j);
                    if (Main.netMode != NetmodeID.SinglePlayer) {
                        NetMessage.SendData(MessageID.TileManipulation, -1, -1, null, 0, i, j);
                    }
                }
            }
        }
    }

    public static void CutTilesRectangle(Rectangle box, TileCuttingContext context, Player player, bool fromTrap = false) {
        CutTilesRectangle(box, context, player.GetTileCutIgnorance(allowRegrowth: false, fromTrap));
    }

    public static void SetMerge<T>(this ModTile modTile) where T : ModTile {
        SetMerge(modTile, ModContent.TileType<T>());
    }
    public static void SetMerge(this ModTile modTile, int otherType) {
        SetMerge(modTile.Type, otherType);
    }
    public static void SetMerge(int myType, int otherType) {
        Main.tileMerge[myType][otherType] = true;
        Main.tileMerge[otherType][myType] = true;
    }

    /// <summary>
    /// Properly removes a liquid at the given tile coordinates.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="quiet"></param>
    public static void KillLiquid(int x, int y, bool quiet = false) {
        var tile = Main.tile[x, y];
        tile.LiquidType = 0;
        tile.LiquidAmount = 0;
        WorldGen.SquareTileFrame(x, y, resetFrame: false);
        if (Main.netMode == NetmodeID.MultiplayerClient && !quiet) {
            NetMessage.sendWater(x, y);
        }
        else {
            Liquid.AddWater(x, y);
        }
    }

    public static bool ScanDown(Point p, int limit, out Point result, params Utils.TileActionAttempt[] tileActionAttempt) {
        int endY = Math.Min(p.Y + limit, Main.maxTilesY - 36);
        result = p;
        for (int j = p.Y; j < endY; j++) {
            for (int k = 0; k < tileActionAttempt.Length; k++) {
                if (tileActionAttempt[k](p.X, j)) {
                    result.Y = j;
                    return true;
                }
            }
        }
        return false;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ScanDown(Point p, int limit, out Point result) {
        return ScanDown(p, limit, out result, IsSolid);
    }

    public static bool ScanUp(Point p, int limit, out Point result, params Utils.TileActionAttempt[] tileActionAttempt) {
        int endY = Math.Max(p.Y - limit, 36);
        result = p;
        for (int j = p.Y; j > endY; j--) {
            for (int k = 0; k < tileActionAttempt.Length; k++) {
                if (tileActionAttempt[k](p.X, j)) {
                    result.Y = j;
                    return true;
                }
            }
        }
        return false;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ScanUp(Point p, int limit, out Point result) {
        return ScanUp(p, limit, out result, IsFullySolid);
    }

    public static Utils.TileActionAttempt HasWallAction(int type) {
        return (i, j) => Main.tile[i, j].WallType == type;
    }
    public static Utils.TileActionAttempt HasWallAction(params int[] types) {
        return (i, j) => types.ContainsAny(Main.tile[i, j].WallType);
    }
    public static Utils.TileActionAttempt HasTileAction(int type) {
        return (i, j) => Main.tile[i, j].HasTile && Main.tile[i, j].TileType == type;
    }
    public static Utils.TileActionAttempt HasTileAction(params int[] types) {
        return (i, j) => Main.tile[i, j].HasTile && types.ContainsAny(Main.tile[i, j].TileType);
    }

    public static bool HasNoTileAndNoWall(Tile tile) {
        return !tile.HasTile && tile.WallType == WallID.None;
    }
    public static bool HasNoTileAndNoWall(int i, int j) {
        return HasNoTileAndNoWall(Main.tile[i, j]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasTile(Tile tile) {
        return tile.HasTile;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasTile(int i, int j) {
        return HasTile(Main.tile[i, j]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool CuttableOrNoTile(this Tile tile) {
        return !tile.HasTile || Main.tileCut[tile.TileType];
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool CuttableOrNoTile(int i, int j) {
        return CuttableOrNoTile(Main.tile[i, j]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool CuttableType(this Tile tile) {
        return Main.tileCut[tile.TileType];
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool CuttableType(int i, int j) {
        return CuttableType(Main.tile[i, j]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool SolidType(this Tile tile) {
        return Main.tileSolid[tile.TileType];
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool SolidType(int i, int j) {
        return SolidType(Main.tile[i, j]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool SolidTopType(this Tile tile) {
        return Main.tileSolidTop[tile.TileType];
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool SolidTopType(int i, int j) {
        return SolidTopType(Main.tile[i, j]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSolid(this Tile tile) {
        return tile.HasUnactuatedTile && SolidType(tile);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsSolid(int i, int j) {
        return IsSolid(Main.tile[i, j]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsFullySolid(this Tile tile) {
        return IsSolid(tile) && !SolidTopType(tile);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsFullySolid(int i, int j) {
        return IsFullySolid(Main.tile[i, j]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotSolid(this Tile tile) {
        return !IsSolid(tile);
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotSolid(int i, int j) {
        return IsNotSolid(Main.tile[i, j]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasNoLiquid(this Tile tile) {
        return tile.LiquidAmount == 0;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasNoLiquid(int i, int j) {
        return HasNoLiquid(Main.tile[i, j]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAnyLiquid(this Tile tile) {
        return tile.LiquidAmount > 0;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasAnyLiquid(int i, int j) {
        return HasAnyLiquid(Main.tile[i, j]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasShimmer(this Tile tile) {
        return tile.LiquidAmount > 0 && tile.LiquidType == LiquidID.Shimmer;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasShimmer(int i, int j) {
        return HasShimmer(Main.tile[i, j]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasMinecartRail(this Tile tile) {
        return tile.HasTile && tile.TileType == TileID.MinecartTrack;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasMinecartRail(int i, int j) {
        return HasMinecartRail(Main.tile[i, j]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasContainer(this Tile tile) {
        return tile.HasTile && TileID.Sets.IsAContainer[tile.TileType];
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasContainer(int i, int j) {
        return HasContainer(Main.tile[i, j]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsTree(int i, int j) {
        return Main.tile[i, j].HasTile && TileID.Sets.IsATreeTrunk[Main.tile[i, j].TileType];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ScanTilesSquare(int i, int j, int size, params Utils.TileActionAttempt[] tileActionAttempt) {
        return ScanTiles(new(i - size / 2, j - size / 2, size, size), tileActionAttempt);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasImportantTile(this Tile tile) {
        return !TileID.Sets.GeneralPlacementTiles[tile.TileType] || Main.wallDungeon[tile.WallType] || tile.WallType == WallID.LihzahrdBrickUnsafe;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool HasImportantTile(int i, int j) {
        return HasImportantTile(Main.tile[i, j]);
    }

    public static bool ScanTiles(Rectangle rect, params Utils.TileActionAttempt[] tileActionAttempt) {
        rect = rect.Fluffize();
        foreach (var attempt in tileActionAttempt) {
            for (int i = rect.X; i < rect.X + rect.Width; i++) {
                for (int j = rect.Y; j < rect.Y + rect.Height; j++) {
                    if (attempt(i, j)) {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public static Rectangle Fluffize(this Rectangle rectangle, int padding = 0) {
        rectangle.X = Math.Clamp(rectangle.X, padding, Main.maxTilesX - rectangle.Width - padding);
        rectangle.Y = Math.Clamp(rectangle.Y, padding, Main.maxTilesY - rectangle.Height - padding);
        return rectangle;
    }

    public static bool IsInvisible(this Tile tile) {
        return !Main.LocalPlayer.CanSeeInvisibleBlocks && !Main.SceneMetrics.EchoMonolith && tile.BlockColorAndCoating().Invisible;
    }
    public static bool IsInvisible(int x, int y) {
        return IsInvisible(Main.tile[x, y]);
    }

    public static bool IsSectionLoaded(int tileX, int tileY) {
        if (Main.netMode == NetmodeID.SinglePlayer || Main.sectionManager == null) {
            return true;
        }
        return Main.sectionManager.SectionLoaded(Netplay.GetSectionX(tileX), Netplay.GetSectionY(tileY));
    }
    public static bool IsSectionLoaded(Point p) {
        return IsSectionLoaded(p.X, p.Y);
    }

    #region Growth
    public static bool TryGrowGrass(int x, int y, int tileID) {
        for (int k = -1; k <= 1; k++) {
            for (int l = -1; l <= 1; l++) {
                if (!Main.tile[x + k, y + l].IsFullySolid()) {
                    Main.tile[x, y].TileType = (ushort)tileID;
                    return true;
                }
            }
        }
        return false;
    }
    public static void SpreadGrass(int i, int j, int dirt, int grass, int spread = 0, byte color = 0) {
        if (!WorldGen.InWorld(i, j, 6)) {
            return;
        }
        for (int k = i - 1; k <= i + 1; k++) {
            for (int l = j - 1; l <= j + 1; l++) {
                if (WorldGen.genRand.NextBool(8)) {
                    if (Main.tile[k, l].HasTile && Main.tile[k, l].TileType == dirt) {
                        if (TryGrowGrass(k, l, grass))
                            WorldGen.SquareTileFrame(k, l, resetFrame: true);
                        return;
                    }
                }
            }
        }
    }
    #endregion
}
