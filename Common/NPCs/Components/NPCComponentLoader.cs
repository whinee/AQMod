﻿namespace Aequus.Common.NPCs.Components;

public class NPCComponentLoader : ILoad {
    public void Load(Mod mod) {
        On_NPC.NPCLoot_DropItems += IPreDropItems.On_NPC_NPCLoot_DropItems;
    }

    public void Unload() {
    }
}