﻿using Aequus.Common.Net;
using System.IO;
using Terraria;
using Terraria.ID;

namespace Aequus.Content.Items.Tools.NameTag;

public class NameTagPacket : PacketHandler {
    public void Send(int i, string nameTag) {
        var p = GetPacket();
        p.Write(i);
        p.Write(nameTag);
        p.Send();
    }

    public override void Receive(BinaryReader reader, int sender) {
        int npcIndex = reader.ReadInt32();
        string nameTag = reader.ReadString();

        NameTag.NametagEffects(npcIndex, nameTag);

        if (Main.netMode == NetmodeID.Server) {
            Send(npcIndex, nameTag);
        }
    }
}