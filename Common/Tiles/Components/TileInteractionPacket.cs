﻿using tModLoaderExtended.Networking;
using System.IO;

namespace Aequus.Common.Tiles.Components;

public class TileInteractionPacket : PacketHandler {
    public void Send(int i, int j, int toClient = -1, int ignoreClient = -1) {
        var packet = GetPacket();
        packet.Write(i);
        packet.Write(j);
        packet.Write(Main.tile[i, j].TileType);
        if (TileLoader.GetTile(Main.tile[i, j].TileType) is INetTileInteraction netTileInteractions) {
            netTileInteractions.Send(i, j, packet);
        }
        packet.Send(toClient, ignoreClient);
    }

    public override void Receive(BinaryReader reader, int sender) {
        int i = reader.ReadInt32();
        int j = reader.ReadInt32();
        ushort type = reader.ReadUInt16();
        if (TileLoader.GetTile(type) is INetTileInteraction netTileInteractions) {
            netTileInteractions.Receive(i, j, reader, sender);
        }
    }
}