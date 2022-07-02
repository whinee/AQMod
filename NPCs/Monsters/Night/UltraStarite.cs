﻿using Aequus.Biomes;
using Aequus.Buffs.Debuffs;
using Aequus.Graphics.Primitives;
using Aequus.Items.Accessories;
using Aequus.Items.Consumables.Foods;
using Aequus.Items.Placeable.Banners;
using Aequus.Projectiles.Monster;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;

namespace Aequus.NPCs.Monsters.Night
{
    public class UltraStarite : ModNPC
    {
        public const int STATE_FLYUP = 1;
        public const int STATE_IDLE = 0;
        public const int STATE_GOODBYE = -1;

        public static readonly Color SpotlightColor = new Color(100, 100, 10, 0);

        public override string Texture => AequusHelpers.GetPath<HyperStarite>();

        public int State { get => (int)NPC.ai[0]; set => NPC.ai[0] = value; }
        public float ArmsLength { get => NPC.ai[3]; set => NPC.ai[3] = value; }

        public float[] oldArmsLength;

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 3;
            NPCID.Sets.TrailingMode[Type] = 7;
            NPCID.Sets.TrailCacheLength[Type] = 15;
            NPCID.Sets.DebuffImmunitySets.Add(Type, new Terraria.DataStructures.NPCDebuffImmunityData()
            {
                SpecificallyImmuneTo = Starite.BuffImmunities,
            });
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers(0)
            {
                Scale = 0.6f,
            });
        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            this.CreateLoot(npcLoot)
                .Add<HyperCrystal>(chance: 4, stack: 1)
                .Add(ItemID.Nazar, chance: 50, stack: 1)
                .Add<NeutronYogurt>(chance: 1, stack: (1, 2));
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            this.CreateEntry(database, bestiaryEntry);
        }

        public override void SetDefaults()
        {
            NPC.width = 50;
            NPC.height = 50;
            NPC.lifeMax = 900;
            NPC.damage = 80;
            NPC.defense = 12;
            NPC.HitSound = SoundID.NPCHit5;
            NPC.DeathSound = SoundID.NPCDeath55;
            NPC.aiStyle = -1;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.knockBackResist = 0f;
            NPC.value = Item.buyPrice(gold: 2, silver: 50);
            NPC.npcSlots = 5f;

            this.SetBiome<GlimmerInvasion>();

            Banner = NPC.type;
            BannerItem = ModContent.ItemType<UltraStariteBanner>();

            oldArmsLength = new float[NPCID.Sets.TrailCacheLength[Type]];
        }

        public override void HitEffect(int hitDirection, double damage)
        {
            float x = NPC.velocity.X.Abs() * hitDirection;
            if (NPC.life <= 0)
            {
                for (int i = 0; i < 50; i++)
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 15);
                    Main.dust[d].velocity.X += x;
                    Main.dust[d].velocity.Y = -Main.rand.NextFloat(2f, 6f);
                }
                for (int i = 0; i < 70; i++)
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 57 + Main.rand.Next(2));
                    Main.dust[d].velocity.X += x;
                    Main.dust[d].velocity.Y = -Main.rand.NextFloat(2f, 6f);
                }
                for (int i = 0; i < 16; i++)
                {
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center, new Vector2(Main.rand.NextFloat(-5f, 5f) + x, Main.rand.NextFloat(-5f, 5f)), 16 + Main.rand.Next(2));
                }
            }
            else
            {
                for (int i = 0; i < 7; i++)
                {
                    int d = Dust.NewDust(NPC.position, NPC.width, NPC.height, 15);
                    Main.dust[d].velocity.X += x;
                    Main.dust[d].velocity.Y = -Main.rand.NextFloat(5f, 12f);
                }
                int d1 = Dust.NewDust(NPC.position, NPC.width, NPC.height, 57 + Main.rand.Next(2));
                Main.dust[d1].velocity.X += x;
                Main.dust[d1].velocity.Y = -Main.rand.NextFloat(2f, 6f);
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, new Vector2(Main.rand.NextFloat(-4f, 4f) + x * 0.75f, Main.rand.NextFloat(-4f, 4f)), 16 + Main.rand.Next(2));
            }
        }

        private bool PlayerCheck()
        {
            NPC.TargetClosest(faceTarget: false);
            if (!NPC.HasValidTarget || Main.player[NPC.target].dead)
            {
                NPC.ai[0] = -1f;
                return false;
            }
            else
            {
                return true;
            }
        }

        public override void AI()
        {
            if (Main.dayTime)
            {
                NPC.life = -1;
                NPC.HitEffect();
                NPC.active = false;
                return;
            }
            if (Main.rand.NextBool(8))
            {
                var d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Enchanted_Pink);
                d.velocity = (d.position - NPC.Center) / 8f;
            }
            if (Main.rand.NextBool(10))
            {
                var g = Gore.NewGoreDirect(NPC.GetSource_FromThis(), NPC.position + new Vector2(Main.rand.Next(NPC.width - 4), Main.rand.Next(NPC.height - 4)), new Vector2(Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f)), 16);
                g.velocity = (g.position - NPC.Center) / 8f;
                g.scale *= 0.6f;
            }
            Lighting.AddLight(NPC.Center, new Vector3(1.2f, 1.2f, 0.5f));
            Vector2 center = NPC.Center;
            if (NPC.ai[0] == -1f)
            {
                NPC.noTileCollide = true;
                NPC.velocity.X *= 0.95f;
                if (NPC.velocity.Y > 0f)
                    NPC.velocity.Y *= 0.96f;
                NPC.velocity.Y -= 0.075f;

                NPC.timeLeft = Math.Min(NPC.timeLeft, 100);
                NPC.rotation += NPC.velocity.Length() * 0.0157f;
                return;
            }

            Player player = Main.player[NPC.target];
            Vector2 plrCenter = player.Center;
            float armsWantedLength = 400f;
            oldArmsLength[0] = NPC.ai[3];
            AequusHelpers.UpdateCacheList(oldArmsLength);
            switch (State)
            {
                case STATE_IDLE:
                    {
                        NPC.TargetClosest(faceTarget: false);
                        if (NPC.HasValidTarget)
                        {
                            if (Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height) || NPC.life < NPC.lifeMax)
                            {
                                NPC.ai[0] = STATE_FLYUP;
                                NPC.ai[1] = 0f;
                                for (int i = 0; i < 5; i++)
                                {
                                    int damage = Main.expertMode ? 45 : 75;
                                    Projectile.NewProjectile(NPC.GetSource_FromAI(), center, new Vector2(0f, 0f), ModContent.ProjectileType<HyperStariteProj>(), damage, 1f, Main.myPlayer, NPC.whoAmI + 1, i);
                                }
                                NPC.netUpdate = true;
                            }
                            else
                            {
                                NPC.ai[1]++;
                                if (NPC.ai[1] >= 1200f)
                                {
                                    NPC.timeLeft = 0;
                                    NPC.ai[0] = -1f;
                                }
                                NPC.velocity *= 0.96f;
                                return;
                            }
                        }
                        else
                        {
                            if (Main.player[NPC.target].dead)
                            {
                                NPC.ai[0] = -1f;
                                NPC.ai[1] = -0f;
                                NPC.netUpdate = true;
                            }
                            NPC.ai[1]++;
                            if (NPC.ai[1] >= 1200f)
                            {
                                NPC.timeLeft = 0;
                                NPC.ai[0] = -1f;
                                NPC.netUpdate = true;
                            }
                            NPC.velocity *= 0.96f;
                            return;
                        }
                    }
                    break;

                case STATE_FLYUP:
                    {
                        NPC.ai[1]++;
                        NPC.velocity.Y -= 0.45f;
                        if (NPC.ai[1] > 20f && PlayerCheck())
                        {
                            State = STATE_FLYUP;
                            NPC.ai[1] = 0f;
                        }
                    }
                    break;
            }
            if (NPC.velocity.Length() < 1.5f && center.Y + 160f > plrCenter.Y && Collision.SolidCollision(NPC.position, NPC.width, NPC.height))
                NPC.velocity.Y -= 0.6f;
        }

        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            if (Main.expertMode)
            {
                target.AddBuff(ModContent.BuffType<BlueFire>(), 360);
                target.AddBuff(BuffID.Blackout, 1800);
                if (Main.rand.NextBool(4))
                    target.AddBuff(BuffID.Cursed, 120);
            }
            else
            {
                if (Main.rand.NextBool(4))
                    target.AddBuff(BuffID.OnFire, 360);
                if (Main.rand.NextBool())
                    target.AddBuff(BuffID.Darkness, 1800);
                if (Main.rand.NextBool(12))
                    target.AddBuff(BuffID.Cursed, 120);
            }
        }

        public override void OnKill()
        {
            AequusWorld.MarkAsDefeated(ref AequusWorld.downedEventCosmic, Type);
        }

        public override int SpawnNPC(int tileX, int tileY)
        {
            return NPC.NewNPC(null, tileX * 16 + 8, tileY * 16 - 80, NPC.type);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            var armTrail = TrailRenderer.NewRenderer(1, 50f, Color.Blue);
            var texture = TextureAssets.Npc[Type].Value;
            var origin = NPC.frame.Size() / 2f;
            var offset = new Vector2(NPC.width / 2f, NPC.height / 2f);
            float mult = 1f / NPCID.Sets.TrailCacheLength[NPC.type];
            var armFrame = NPC.frame;
            var coreFrame = new Rectangle(NPC.frame.X, NPC.frame.Y + NPC.frame.Height * 2, NPC.frame.Width, NPC.frame.Height);
            var bloom = TextureCache.Bloom[0].Value;
            var bloomFrame = new Rectangle(0, 0, bloom.Width, bloom.Height);
            var bloomOrigin = bloomFrame.Size() / 2f;

            var armLength = (NPC.height + 56f) * NPC.scale;
            if (NPC.IsABestiaryIconDummy)
            {
                armLength -= 24f * NPC.scale;
            }

            Main.spriteBatch.Draw(bloom, new Vector2((int)(NPC.position.X + offset.X - screenPos.X), (int)(NPC.position.Y + offset.Y - screenPos.Y)), bloomFrame, SpotlightColor, 0f, bloomOrigin, NPC.scale * 2, SpriteEffects.None, 0f);
            if (!NPC.IsABestiaryIconDummy)
            {
                int trailLength = NPCID.Sets.TrailCacheLength[Type];
                int armTrailLength = (int)(trailLength * MathHelper.Clamp((float)Math.Pow(ArmsLength / 240f, 1.2f), 0f, 1f));
                var armPositions = new List<Vector2>[5];

                for (int j = 0; j < 5; j++)
                    armPositions[j] = new List<Vector2>();

                for (int i = 0; i < trailLength; i++)
                {
                    var pos = NPC.oldPos[i] + offset - screenPos;
                    float progress = AequusHelpers.CalcProgress(trailLength, i);
                    Color color = new Color(45, 35, 60, 0) * (mult * (NPCID.Sets.TrailCacheLength[NPC.type] - i));
                    Main.spriteBatch.Draw(texture, pos.Floor(), coreFrame, color, 0f, origin, NPC.scale * progress * progress, SpriteEffects.None, 0f);
                    color = new Color(30, 25, 140, 4) * (mult * (NPCID.Sets.TrailCacheLength[NPC.type] - i)) * 0.6f;
                    if (i > armTrailLength || (i > 1 && (NPC.oldRot[i] - NPC.oldRot[i - 1]).Abs() < 0.002f))
                        continue;
                    for (int j = 0; j < 5; j++)
                    {
                        float rotation = NPC.oldRot[i] + MathHelper.TwoPi / 5f * j;
                        var armPos = NPC.position + offset + (rotation - MathHelper.PiOver2).ToRotationVector2() * (armLength + oldArmsLength[i]) - screenPos;
                        armPositions[j].Add(armPos + screenPos);
                        //Main.spriteBatch.Draw(texture, armPos.Floor(), armFrame, color, rotation, origin, NPC.scale, SpriteEffects.None, 0f);
                    }
                }

                for (int j = 0; j < 5; j++)
                    armTrail.Draw(armPositions[j].ToArray());
            }
            var armSegmentFrame = new Rectangle(NPC.frame.X, NPC.frame.Y + NPC.frame.Height, NPC.frame.Width, NPC.frame.Height);

            float segmentLength = (NPC.height - 10f) * NPC.scale;
            if (NPC.IsABestiaryIconDummy)
            {
                segmentLength -= 10f * NPC.scale;
            }
            for (int i = 0; i < 5; i++)
            {
                float rotation = NPC.rotation + MathHelper.TwoPi / 5f * i;
                var armPos = NPC.position + offset + (rotation - MathHelper.PiOver2).ToRotationVector2() * (armLength + NPC.ai[3]) - screenPos;
                Main.spriteBatch.Draw(texture, armPos.Floor(), armFrame, Color.White, rotation, origin, NPC.scale, SpriteEffects.None, 0f);

                rotation += MathHelper.TwoPi / 10f;
                armPos = NPC.position + offset + (rotation - MathHelper.PiOver2).ToRotationVector2() * segmentLength - screenPos;
                Main.spriteBatch.Draw(texture, armPos.Floor(), armSegmentFrame, Color.White, rotation, origin, NPC.scale, SpriteEffects.None, 0f);
            }
            Main.spriteBatch.Draw(texture, new Vector2((int)(NPC.position.X + offset.X - screenPos.X), (int)(NPC.position.Y + offset.Y - screenPos.Y)), coreFrame, new Color(255, 255, 255, 255), 0f, origin, NPC.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}