﻿using AequusRemake.Core.CrossMod;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.Localization;

namespace AequusRemake.Content.CrossMod;

/// <summary><see href="link">https://github.com/JavidPack/BossChecklist/wiki/%5B1.4.4%5D-Boss-Log-Entry-Mod-Call#arguments</see></summary>
internal class BossChecklist : SupportedMod<BossChecklist> {
    public interface IProvideBossChecklistEntry {
        BossEntry ProvideEntry();
    }

    public struct BossEntry {
        /// <param name="internalName">The Internal Name for this boss. Try to not change it as it may break other mods.</param>
        /// <param name="progression">View <see href="link">https://github.com/JavidPack/BossChecklist/wiki/Boss-Progression-Values</see> for a catalogue of vanilla entry sorting.</param>
        /// <param name="downedFlag">The Downed Flag for this boss.</param>
        public BossEntry(string internalName, float progression, Func<bool> downedFlag) {
            InternalName = internalName;
            Progression = progression;
            DownedFlag = downedFlag;
            BossIds = new();
            SpawnItems = new();
            Collectibles = new();
            CustomPortrait = null;
        }

        public readonly string InternalName;
        public readonly float Progression;
        public readonly Func<bool> DownedFlag;

        public readonly List<int> BossIds;
        public readonly List<int> SpawnItems;
        public readonly List<int> Collectibles;

        public PortraitRenderDelegate CustomPortrait { get; set; }
    }

    /// <param name="spriteBatch">The SpriteBatch.</param>
    /// <param name="sourceRectangle">The bounds of the portrait.</param>
    /// <param name="displayColor">The Color of the entry. It is usually Black when hidden, and White when not hidden.</param>
    public delegate void PortraitRenderDelegate(SpriteBatch spriteBatch, Rectangle sourceRectangle, Color displayColor);

    public override void PostSetupContent() {
        if (Instance.Version < new Version(1, 6)) {
            return;
        }

        foreach (ModNPC boss in Mod.GetContent<ModNPC>().Where(m => m is IProvideBossChecklistEntry)) {
            IProvideBossChecklistEntry entryProvider = boss as IProvideBossChecklistEntry;
            BossEntry entry = entryProvider.ProvideEntry();

            Dictionary<string, object> optionalArguments = new() {
                ["spawnInfo"] = boss.GetLocalization("SpawnInfo"),
            };
            if (entry.SpawnItems.Count > 0) {
                optionalArguments["spawnItems"] = entry.SpawnItems;
            }
            if (entry.Collectibles.Count > 0) {
                optionalArguments["collectibles"] = entry.Collectibles;
            }

            PortraitRenderDelegate portrait = entry.CustomPortrait;
            if (portrait != null) {
                optionalArguments["customPortrait"] = Delegate.CreateDelegate(typeof(Action<SpriteBatch, Rectangle, Color>), portrait.Target, portrait.Method);
            }

            Language.GetOrRegister(boss.GetLocalizationKey("DespawnMessage"));

            // NOTE -- Despawn Messages can also be Func<NPC, LocalizedText>
            if (XLanguage.TryGet(boss.GetLocalizationKey("DespawnMessage"), out LocalizedText despawnMessage)) {
                optionalArguments["despawnMessage"] = despawnMessage;
            }

            Instance.Call(
                "LogBoss",
                Mod,
                entry.InternalName,
                entry.Progression,
                entry.DownedFlag,
                entry.BossIds.Count > 0 ? entry.BossIds : boss.Type,
                optionalArguments
            );
        }
    }
}
