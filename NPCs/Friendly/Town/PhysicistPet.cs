﻿using Aequus.Common.Utilities;
using Aequus.Items.Consumables;
using Aequus.NPCs.GlobalNPCs;
using Aequus.UI.RenamingUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Aequus.NPCs.Friendly.Town
{
    public class PhysicistPet : ModNPC, IAddRecipes
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 4;
            NPCID.Sets.HatOffsetY[Type] = 2;
            NPCID.Sets.SpawnsWithCustomName[Type] = true;
            NPCID.Sets.ActsLikeTownNPC[Type] = true;

            NameTag.CanBeRenamedOverride.Add(Type, true);
        }

        public void AddRecipes(Aequus aequus)
        {
            BestiaryBuilder.MoveBestiaryEntry(this, ContentSamples.NpcBestiarySortingId[ModContent.NPCType<Physicist>()] - 1);
        }

        public override void SetDefaults()
        {
            NPC.friendly = true;
            NPC.width = 16;
            NPC.height = 20;
            NPC.aiStyle = -1;
            NPC.damage = 0;
            NPC.defense = 15;
            NPC.lifeMax = 250;
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 1f;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            this.CreateEntry(database, bestiaryEntry)
                .AddMainSpawn(BestiaryBuilder.DesertBiome);
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            int dustAmount = NPC.life > 0 ? 1 : 5;
            for (int k = 0; k < dustAmount; k++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Electric);
            }
        }

        public override List<string> SetNPCNameList()
        {
            return new List<string>()
            {
                "Quimble",
                "Pimble",
                "Nimble",
                "Zimble",

                "Spinzie",
                "Pinzie",
                "Zinzie",
                "Xinzie",

                "Squondle",
                "Mondle",
                "Chondle",
                "Wandle",

                "Squizzer",
                "Chizzer",
                "Whizzer",
                "Fizzer",
                "Zizzer",
                "Tizzer",

                "Skeebler",
                "Beebler",
                "Zeebler",
                "Xeebler",
                "Teebler",
                "Weebler",
                "Meebler",

                "Whibbler",
                "Blipper",
                "Bleeper",
                "Blooper",
                "Zipper",
                "Zooper",

                "Pooper",
            };
        }

        public override bool CanChat()
        {
            return true;
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.92");
        }

        public override void OnChatButtonClicked(bool firstButton, ref bool shop)
        {
            if (firstButton)
            {
                PlayPetSound();
            }
        }
        public void PlayPetSound()
        {
            if (NPC.TryGetGlobalNPC<NPCNameTag>(out var nameTagNPC) && nameTagNPC.HasNameTag)
            {
                switch (nameTagNPC.NameTag.ToLower())
                {
                    case "little zumbo":
                    case "littlezumbo":
                    case "pooper":
                        SoundEngine.PlaySound(SoundID.Item16);
                        return;
                }
            }
            if (NPC.GivenName.ToLower() == "pooper")
            {
                SoundEngine.PlaySound(SoundID.Item16);
                return;
            }
            SoundEngine.PlaySound(RenameItemUIState.SqueakSound);
        }

        public override string GetChat()
        {
            return Language.GetTextValue("Mods.Aequus.Chat.PhysicistPet." + Main.rand.Next(2));
        }

        public override void AI()
        {
            int npcOwnerIndex = (int)NPC.ai[0];
            if (!Main.npc[npcOwnerIndex].active || Main.npc[npcOwnerIndex].type != ModContent.NPCType<Physicist>())
            {
                npcOwnerIndex = NPC.FindFirstNPC(ModContent.NPCType<Physicist>());
            }
            if (npcOwnerIndex > NPC.whoAmI || npcOwnerIndex == -1) // Ensures the npc is in a slot after their parent.
            {
                NPC.life = -1;
                NPC.HitEffect();
                NPC.active = false;
                return;
            }

            var npcOwner = Main.npc[npcOwnerIndex];
            var gotoLocation = npcOwner.Center + new Vector2(npcOwner.width * 1.5f * npcOwner.direction, -18f);

            NPC.direction = npcOwner.direction;
            NPC.spriteDirection = npcOwner.spriteDirection;
            var difference = gotoLocation - NPC.Center;
            if (difference.Length() < 15f)
            {
                if (NPC.velocity.Length() < 0.2f)
                {
                    NPC.velocity += Main.rand.NextVector2Unit() * 0.1f;
                }
                if (NPC.velocity.Length() > 4f)
                {
                    NPC.velocity *= 0.85f;
                }
                NPC.velocity *= 0.99f;
            }
            else
            {
                NPC.velocity += difference / 300f;
                if (NPC.velocity.Length() > 7f)
                {
                    NPC.velocity.Normalize();
                    NPC.velocity *= 7f;
                }
                if (difference.Length() > 900f)
                {
                    NPC.Center = npcOwner.Center;
                    NPC.netUpdate = true;
                }
            }
            if (NPC.position.Y < npcOwner.position.Y)
            {
                NPC.velocity.X *= 0.98f;
                NPC.velocity.Y -= 0.02f;
                if (Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                {
                    NPC.velocity.X *= 0.95f;
                    NPC.velocity.Y -= 0.05f;
                }
            }
            var rect = NPC.getRect();
            if (npcOwner.getRect().Intersects(rect))
            {
                NPC.velocity += npcOwner.DirectionTo(NPC.Center).UnNaN() * 0.075f;
            }
            NPC.rotation = MathHelper.Clamp(NPC.velocity.X * 0.05f, -1f, 1f);
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter > 6.0)
            {
                NPC.frame.Y += frameHeight;
                if (NPC.frame.Y >= frameHeight * Main.npcFrameCount[Type])
                    NPC.frame.Y = 0;
                NPC.frameCounter = 0.0;
            }
        }

        public override bool NeedSaving()
        {
            return true;
        }

        public override void SaveData(TagCompound tag)
        {
            if (!string.IsNullOrEmpty(NPC.GivenName))
                tag["Name"] = NPC.GivenName;
            int physIndex = -1;
            int myPhysWhoAmI = (int)NPC.ai[0];
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<Physicist>())
                {
                    physIndex++;
                }
                if (i >= myPhysWhoAmI)
                {
                    break;
                }
            }
            tag["ParentApparentID"] = physIndex;
        }

        public override void LoadData(TagCompound tag)
        {
            if (tag.TryGet<string>("Name", out var value))
                NPC.GivenName = value;
            if (tag.TryGet("ParentApparentID", out int findPhys))
            {
                int physIndex = -1;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<Physicist>())
                    {
                        physIndex++;
                    }
                    if (physIndex >= findPhys)
                    {
                        NPC.ai[0] = i;
                        break;
                    }
                }
            }
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.GivenName ?? "Pooper");
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.GivenName = reader.ReadString();
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var texture = TextureAssets.Npc[Type].Value;
            var drawCoords = NPC.Center - screenPos;
            var origin = NPC.frame.Size() / 2f;
            var spriteEffects = NPC.spriteDirection == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            spriteBatch.Draw(texture, drawCoords, NPC.frame, NPC.GetNPCColorTintedByBuffs(drawColor), NPC.rotation, origin, NPC.scale, spriteEffects, 0f);
            spriteBatch.Draw(ModContent.Request<Texture2D>(Texture + "_Glow", AssetRequestMode.ImmediateLoad).Value, drawCoords, NPC.frame, Color.White, NPC.rotation, origin, NPC.scale, spriteEffects, 0f);
            return false;
        }
    }
}