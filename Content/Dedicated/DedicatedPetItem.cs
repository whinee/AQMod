﻿using AequusRemake.Core.ContentGeneration;

namespace AequusRemake.Content.Dedicated;

[Autoload(false)]
internal class DedicatedPetItem : InstancedPetItem {
    private readonly string _dedicateeName;
    private readonly string _displayedDedicateeName;
    private readonly Color _dedicateeColor;

    public DedicatedPetItem(UnifiedModPet modPet, string dedicateeName, Color dedicateeColor, bool nameHidden = false, int itemRarity = 3, int value = 50000, Color? alphaOverride = null) : base(modPet, itemRarity, value, alphaOverride) {
        _dedicateeName = dedicateeName;
        _displayedDedicateeName = nameHidden ? null : dedicateeName;
        _dedicateeColor = dedicateeColor;
    }

    public string DedicateeName => _dedicateeName;
    public string DisplayedDedicateeName => _displayedDedicateeName;
    public Color TextColor => _dedicateeColor;
}
