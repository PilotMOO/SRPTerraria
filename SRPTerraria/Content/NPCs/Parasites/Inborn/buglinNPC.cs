using Microsoft.Xna.Framework;
using MonoMod.Cil;
using SRPTerraria.Content.Items.ParasiteItems;
using SRPTerraria.Content.NPCs.Parasites.Inborn;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using SRPTerraria;
using SRPTerraria.Common.Configs.ParasiteConfigs;

namespace SRPTerraria.Content.NPCs.Parasites.Inborn
{
    public class BuglinNPC : ParasiteBaseNPC
    {
        private enum ActionState
        {
            Staying,
            Moving,
            Fleeing
        }

        public ref float AI_Timer => ref NPC.ai[0];
        public ref float AI_Evo_Timer => ref NPC.ai[1];
        public ref float AI_State => ref NPC.ai[2];

        bool isFleeing = false;
        bool OnFire = false;

        int FlameTimer = 0;

        Random rnd = new Random();

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 4;

            // Specify the debuffs it is immune to.
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
            //NPCID.Sets.SpecificDebuffImmunity[Type][ModContent.BuffType<Buffs.ExampleGravityDebuff>()] = true; use this to grant immunity to CoTH, Needler, etc.
        }

        public override void SetDefaults()
        {
            NPC.width = 28;
            NPC.height = 8;
            NPC.aiStyle = -1;
            NPC.damage = 0;
            NPC.defense = ModContent.GetInstance<InbornTierConfig>().BuglinDEF;
            NPC.lifeMax = ModContent.GetInstance<InbornTierConfig>().BuglinHP;
            NPC.HitSound = SoundID.NPCHit1; // This a the one below is in need of addressing/custom sounds
            NPC.DeathSound = SoundID.NPCDeath1; /**/
            NPC.value = 0f; // No money :[
            NPC.catchItem = ModContent.ItemType<BuglinItem>();
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            int CurPhase = ModContent.GetInstance<SRPTerraria>().Get_CurrentPhase();

            if (CurPhase > -1 && CurPhase < 3)
            {
                return SpawnCondition.Overworld.Chance * 0.3f;
            }
            else
            {
                return 0f;
            }
        }

        public override bool PreAI()
        {
            this.FireWeakness(NPC);

            if (NPC.HasBuff(BuffID.OnFire) | NPC.HasBuff(BuffID.OnFire3))
            {
                OnFire = true;
            }
            else { OnFire = false; }

            //Set to 7800 or similar after testing
            if (AI_Evo_Timer < 7800)
            {
                AI_Evo_Timer++;
            }
            else
            {
                Evolve();
                return false;
            }

            return true;
        }

        public override void AI()
        {
            float MoveSpeed = 0.25f;
            float MaxSpeed = MoveSpeed * 1.5f;

            NPC.TargetClosest(false);

            //Checks if AI_Timer is 0, resets as needed. AI_Timer is used to add some delay to switching between staying and moving
            if (AI_Timer != 0)
            {
                AI_Timer--;
            }
            else
            {
                AI_Timer = 240f;
            }

            //Checks if the buglin has collided with a block on the X axis, if yes reverse direction and sets AI_Timer to 0
            if (NPC.collideX)
            {
                NPC.direction *= -1;
                if (AI_State != 2)
                {
                    AI_Timer = 0;
                }
            }

            //Decides if it should be staying or moving, randomizes every 3~ seconds
            if (AI_Timer == 0)
            {
                if (Main.player[NPC.target].Distance(NPC.Center) < 400f | OnFire)
                {
                    AI_State = 2;
                }
                else if (AI_State != 2)
                {
                    AI_State = rnd.Next(2);
                }
            }

            switch (AI_State)
            {
                case 0:
                    if (NPC.velocity.X < 0)
                    {
                        NPC.velocity.X += 0.1f;
                    }
                    else if (NPC.velocity.X > 0)
                    {
                        NPC.velocity.X -= 0.1f;
                    }
                    break;
                case 1:
                    int random = rnd.Next(-3, 3);
                    if (AI_Timer == 0)
                    {
                        if (random < 0)
                        {
                            NPC.direction = 1;
                        }
                        else
                        {
                            NPC.direction = -1;
                        }
                    }
                    if (NPC.velocity.X < MaxSpeed)
                    {
                        if (NPC.velocity.X > MaxSpeed * -1)
                        {
                            NPC.velocity.X = NPC.direction * MoveSpeed;
                        }
                    }
                    break;
                case 2:
                    Flee();
                    break;

            }

            if (NPC.direction == 1)
            {
                if (NPC.velocity.X < 0)
                {
                    NPC.velocity.X *= -1;
                }
            }
            else if (NPC.direction == -1)
            {
                if (NPC.velocity.X > 0)
                {
                    NPC.velocity.X *= -1;
                }
            }
        }

        private void Flee()
        {
            float MoveSpeed = 0.25f;
            float MaxSpeed = MoveSpeed * 2f;

            if (Main.player[NPC.target].Distance(NPC.Center) < 400f)
            {
                isFleeing = true;

                float DirecTo;

                if (NPC.HasValidTarget)
                {
                    DirecTo = NPC.Center.X - Main.player[NPC.target].Center.X;

                    if (DirecTo < 0)
                    {
                        NPC.direction = -1;
                    }
                    else if (DirecTo > 0)
                    {
                        NPC.direction = 1; 
                    }
                    if (DirecTo == 0)
                    {
                        NPC.direction = -1;
                    }
                }
                else if (OnFire)
                {
                    int rndDirect = rnd.Next(-1, 2);

                    if (FlameTimer > 30 || FlameTimer == null) { FlameTimer = 0; }

                    if (rndDirect == 0 && FlameTimer == 0)
                    {
                        NPC.direction = -1;
                    }
                    else if (rndDirect == 1 && FlameTimer == 0)
                    {
                        NPC.direction = 1;
                    }

                    FlameTimer++;
                }

                if (NPC.velocity.X < MaxSpeed)
                {
                    if (NPC.velocity.X > MaxSpeed * -1)
                    {
                        NPC.velocity.X = NPC.direction * MoveSpeed * 2f;
                    }
                }

                if (NPC.collideX && NPC.collideY)
                {
                    NPC.velocity.Y -= 3;
                }
            }
            else
            {
                isFleeing = false;

                if (NPC.velocity.X < -0.1)
                {
                    NPC.velocity.X += 0.1f;
                }
                else if (NPC.velocity.X > 0.1)
                {
                    NPC.velocity.X -= 0.1f;
                }
                else
                {
                    AI_State = 0;
                }
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.spriteDirection = NPC.direction;

            if (NPC.frameCounter > 40)
            {
                NPC.frameCounter = 0;
            }
            
            if (NPC.velocity.X != 0)
            {
                if (NPC.frameCounter < 10)
                {
                    NPC.frame.Y = 0;
                }
                if (NPC.frameCounter > 10 && NPC.frameCounter < 20)
                {
                    NPC.frame.Y = 10;
                }
                if (NPC.frameCounter > 20 && NPC.frameCounter < 30)
                {
                    NPC.frame.Y = 20;
                }
                if (NPC.frameCounter > 30 && NPC.frameCounter < 40)
                {
                    NPC.frame.Y = 30;
                }

                NPC.frameCounter++;
                if (isFleeing) { NPC.frameCounter++; }
            }
            else
            {
                NPC.frame.Y = 0;
            }
        }

        private void Evolve()
        {
            NPC.life = 0;
            NPC.HitEffect();
            NPC.active = false;

            for (int i = 0; i < 16; i++)
            {
                Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, DustID.Smoke, rnd.Next(-2, 2), -1f, 0, default, 2f);
                if (Main.rand.NextBool(2))
                {
                    dust.noGravity = true;
                    dust.scale += 1.2f * NPC.scale;
                }
                else
                {
                    dust.scale += 0.7f * NPC.scale;
                }
            }

            int spawnRupterIndex = NPC.NewNPC(NPC.GetSource_None(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<RupterNPC>());
        }

        public override bool? CanBeCaughtBy(Item item, Player player)
        {
            return true;
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
				new FlavorTextBestiaryInfoElement("Small grub-like creatures, these little gray pills are basically harmless. But I've heard that these harmless things can grow up into something less-than-desirable to encounter..."),
            });
        }
    }
}

namespace SRPTerraria.Content.Items.ParasiteItems
{
    public class BuglinItem : ModItem
    {
        public override void SetDefaults()
        {
            Item.useStyle = 1;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.useAnimation = 15;
            Item.useTime = 10;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.width = 28;
            Item.height = 8;
            Item.makeNPC = ModContent.NPCType<BuglinNPC>();
            Item.noUseGraphic = true;
        }
    }
}