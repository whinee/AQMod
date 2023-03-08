﻿using Aequus.Common.Audio;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Buffs.Debuffs
{
    public class CorruptionHellfire : ModBuff
    {
        public static Color FireColor => new Color(100, 28, 160, 10);
        public static Color BloomColor => new Color(20, 2, 80, 10);

        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            AequusBuff.DemonSiegeEnemyImmunity.Add(Type);
            AequusBuff.IsFire.Add(Type);
            AequusBuff.PlayerDoTBuff.Add(Type);
        }

        public override bool ReApply(NPC npc, int time, int buffIndex)
        {
            npc.Aequus().corruptionHellfireStacks++;
            return false;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            var aequus = npc.Aequus();
            aequus.corruptionHellfireStacks = Math.Max(aequus.corruptionHellfireStacks, (byte)1);
        }

        public static void AddBuff(NPC npc, int time, bool inflictOnFireInsteadIfItsImmune = true)
        {
            if (npc.buffImmune[ModContent.BuffType<CorruptionHellfire>()])
            {
                if (inflictOnFireInsteadIfItsImmune)
                {
                    npc.AddBuff(BuffID.OnFire, time);
                }
                return;
            }

            AequusBuff.ApplyBuff<CorruptionHellfire>(npc, time, out bool canPlaySound);
            if (canPlaySound)
            {
                ModContent.GetInstance<BlueFireDebuffSound>().Play(npc.Center, pitchOverride: -0.2f);
            }
        }
    }
}