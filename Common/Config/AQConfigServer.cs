﻿using AQMod.Localization;
using AQMod.NPCs.Boss.Starite;
using System.ComponentModel;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace AQMod.Common.Config
{
    [Label(AQText.ConfigNameKey + "AQConfigServer")]
    public class AQConfigServer : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        public static AQConfigServer Instance => ModContent.GetInstance<AQConfigServer>();

        [Header(AQText.ConfigHeaderKey + "World")]

        [Label(AQText.ConfigValueKey + "HarderOmegaStarite")]
        [Tooltip(AQText.ConfigValueKey + "HarderOmegaStariteTooltip")]
        [DefaultValue(false)]
        [ReloadRequired()]
        public bool harderOmegaStarite;

        [Label(AQText.ConfigValueKey + "EvilProgressionLock")]
        [Tooltip(AQText.ConfigValueKey + "EvilProgressionLockTooltip")]
        [DefaultValue(true)]
        public bool evilProgressionLock;

        [Label(AQText.ConfigValueKey + "ReduceSpawns")]
        [DefaultValue(false)]
        [ReloadRequired()]
        public bool reduceSpawns;

        [Header(AQText.ConfigHeaderKey + "Debug")]

        [Label(AQText.ConfigValueKey + "AprilFools")]
        [DefaultValue(false)]
        [ReloadRequired()]
        public bool aprilFools;

        [Label(AQText.ConfigValueKey + "DebugCommand")]
        [DefaultValue(false)]
        [ReloadRequired()]
        public bool debugCommand;

        public override bool NeedsReload(ModConfig pendingConfig)
        {
            var p = (AQConfigServer)pendingConfig;
            if (p.harderOmegaStarite != harderOmegaStarite)
            {
                return NPC.AnyNPCs(ModContent.NPCType<OmegaStarite>());
            }
            if (p.reduceSpawns != reduceSpawns)
            {
                return AQMod.reduceSpawnrates();
            }
            return base.NeedsReload(pendingConfig);
        }

        public override void OnLoaded()
        {
            if (!AQMod.AprilFools)
                AQMod.AprilFools = aprilFools;
        }

        public override void OnChanged()
        {
            AQMod.ApplyServerConfig(this);
        }
    }
}