﻿namespace Aequus
{
    public enum PacketType : byte
    {
        RequestTileSectionFromServer,
        SyncNecromancyOwner,
        SyncAequusPlayer,
        SyncSound,
        DemonSiegeSacrificeStatus,
        StartDemonSiege,
        RemoveDemonSiege,
        PumpinatorWindSpeed,
        SpawnHostileOccultist,
        PhysicsGunBlock,
        RequestGlimmerEvent,
        ExporterQuestsCompleted,
        SpawnOmegaStarite,
        GlimmerStatus,
        SyncNecromancyNPC,
        SyncDronePoint,
        CarpenterBountiesCompleted,
        AequusTileSquare,
        OnKillEffect,
        ApplyNameTagToNPC,
        RequestChestItems,
        RequestAnalysisQuest,
        SpawnShutterstockerClip,
        AnalysisRarity,
        ZombieConvertEffects,
        GravityChestPickupEffect,
        SpawnPixelCameraClip,
        PlacePixelPainting,
        RegisterPhotoClip,
        BrainCauliflowerNecromancyKill,
        AddBuilding,
        RemoveBuilding,
        CompleteCarpenterBounty,
        ResetCarpenterBounties,
        WabbajackNecromancyKill,
        SendDebuffFlatDamage,
        UniqueTileInteraction,
        NightfallOnHit,
        Count
    }
}