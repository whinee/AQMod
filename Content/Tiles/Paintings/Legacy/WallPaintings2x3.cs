﻿namespace Aequus.Content.Tiles.Paintings.Legacy;

public class WallPaintings2x3() : LegacyPaintingTile(2, 3) {
    public override ushort[] ConvertIds() {
        return [
            Paintings.Instance.Narry!.TileType,
        ];
    }
}