﻿using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.Items.Accessories
{
    [AutoloadEquip(EquipType.Back)]
    public class AmmoBackpack : ModItem
    {
        public static HashSet<int> AmmoBlacklist { get; private set; }

        public override void Load()
        {
            AmmoBlacklist = new HashSet<int>()
            {
                AmmoID.FallenStar,
                AmmoID.Gel,
                AmmoID.Solution,
            };
        }

        public override void Unload()
        {
            AmmoBlacklist?.Clear();
            AmmoBlacklist = null;
        }

        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToAccessory(20, 20);
            Item.rare = ItemRarityID.Blue;
            Item.value = Item.sellPrice(gold: 1);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.Aequus().accAmmoRenewalPack = Item;
        }

        public static void DropAmmo(Player player, NPC npc, Item ammoBackpack)
        {
            var neededAmmoTypes = GetAmmoTypesToSpawn(player, npc, ammoBackpack);
            if (neededAmmoTypes.Count > 0)
            {
                int chosenType = Main.rand.Next(neededAmmoTypes);
                int stack = DetermineStack(chosenType, player, npc, ammoBackpack);
                Item.NewItem(player.GetSource_Accessory(ammoBackpack), npc.getRect(), chosenType, stack);
            }
        }
        public static int DetermineStack(int itemToSpawn, Player player, NPC npc, Item ammoBackpack)
        {
            return (int)Math.Max((Main.rand.Next(30) + 1) * StackMultiplier(itemToSpawn, player, npc, ammoBackpack), 1);
        }
        public static float StackMultiplier(int itemToSpawn, Player player, NPC npc, Item ammoBackpack)
        {
            return 1f - Math.Clamp(ContentSamples.ItemsByType[itemToSpawn].value / (Item.silver * (npc.value / (Item.silver * 5f))), 0f, 1f);
        }
        public static List<int> GetAmmoTypesToSpawn(Player player, NPC npc, Item ammoBackpack)
        {
            var l = new List<int>();
            bool fullSlots = !player.inventory[Main.InventoryAmmoSlotsStart].IsAir && !player.inventory[Main.InventoryAmmoSlotsStart + 1].IsAir
                && !player.inventory[Main.InventoryAmmoSlotsStart + 2].IsAir && !player.inventory[Main.InventoryAmmoSlotsStart + 3].IsAir;

            for (int i = Main.InventoryAmmoSlotsStart; i < Main.InventoryAmmoSlotsStart + Main.InventoryAmmoSlotsCount; i++)
            {
                var item = player.inventory[i];
                if (item.IsAir || !item.consumable || item.makeNPC > 0 || item.damage == 0 || item.ammo <= ItemID.None || ContentSamples.ItemsByType[item.ammo].makeNPC > 0 || item.bait > 0)
                {
                    continue;
                }
                if ((!fullSlots || item.type == item.ammo) && !AmmoBlacklist.Contains(item.ammo) && !l.Contains(item.ammo))
                    l.Add(item.ammo);
                if (item.stack < item.maxStack && !AmmoBlacklist.Contains(item.type) && !l.Contains(item.type) && Main.rand.NextBool(3))
                    l.Add(item.type);
            }

            for (int i = Main.InventoryAmmoSlotsStart; i < Main.InventoryAmmoSlotsStart + Main.InventoryAmmoSlotsCount; i++)
            {
                if (!player.inventory[i].consumable)
                {
                    l.Remove(player.inventory[i].ammo);
                    l.Remove(player.inventory[i].type);
                }
            }
            return l;
        }
    }
}