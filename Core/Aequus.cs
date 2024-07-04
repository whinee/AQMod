global using AequusRemake.Core;
global using AequusRemake.Core.Utilities;
global using Microsoft.Xna.Framework;
global using Microsoft.Xna.Framework.Graphics;
global using Terraria;
global using Terraria.ID;
global using Terraria.ModLoader;
global using tModLoaderExtended;
global using static tModLoaderExtended.ExtendedMod;

#region Aliases
global using BestiaryTimeTag = Terraria.GameContent.Bestiary.BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times;
global using BestiaryBiomeTag = Terraria.GameContent.Bestiary.BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes;
global using BestiaryEventTag = Terraria.GameContent.Bestiary.BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Events;
global using BestiaryInvasionTag = Terraria.GameContent.Bestiary.BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Invasions;
global using ILoad = Terraria.ModLoader.ILoadable;
global using ItemSets = Terraria.ID.ItemID.Sets;
global using NPCSets = Terraria.ID.NPCID.Sets;
global using BuffSets = Terraria.ID.BuffID.Sets;
global using HelmetEquipSets = Terraria.ID.ArmorIDs.Head.Sets;
global using LegEquipSets = Terraria.ID.ArmorIDs.Legs.Sets;
global using BalloonEquipSets = Terraria.ID.ArmorIDs.Balloon.Sets;
global using ShoeEquipSets = Terraria.ID.ArmorIDs.Shoe.Sets;
global using GoreSets = Terraria.ID.GoreID.Sets;
#endregion

using Terraria.Localization;
using Terraria.Utilities;

namespace AequusRemake;

public partial class AequusRemake : ExtendedMod {
    public static new Mod Instance => ModContent.GetInstance<AequusRemake>();
    public static Mod MusicMod { get; private set; }

    public override void OnLoad() {
        MusicMod = ModLoader.GetMod("AequusRemakeMusic");
        LoadModCalls();
    }

    public override void OnUnload() {
        MusicMod = null;
        UnloadLoadingSteps();
        UnloadModCalls();
    }

    /// <returns>A random name. Used in naturally generated underworld tombstones.</returns>
    public static LocalizedText GetRandomName(UnifiedRandom random = null) {
        string filter = "Mods.AequusRemake.Names";
        return Language.SelectRandom((key, value) => key.StartsWith(filter), random);
    }
}