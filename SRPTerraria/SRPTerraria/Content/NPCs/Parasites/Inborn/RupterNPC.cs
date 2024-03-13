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
    public class RupterNPC : ParasiteBaseNPC
    {
        private enum ActionState
        {
            Staying,
            Moving,
            Fleeing,
            Chasing,
            Leaping
        }

        public ref float AI_Timer => ref NPC.ai[0];
        public ref float AI_Evo_Counter => ref NPC.ai[1];
        public ref float AI_State => ref NPC.ai[2];
        public ref float AI_Leap_CD => ref NPC.ai[3];

        bool isFleeing = false;
        bool OnFire = false;
        bool isLeaping = false;
        bool isClimbing = false;

        int FlameTimer = 0;
        int TargetRange = ModContent.GetInstance<InbornTierConfig>().RupterTrackingRange;

        Random rnd = new Random();

        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 2; //Needs sprite

            // Specify the debuffs it is immune to.
            NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Poisoned] = true;
            //NPCID.Sets.SpecificDebuffImmunity[Type][ModContent.BuffType<[something]>()] = true; use this to grant immunity to CoTH, Needler, etc.
        }

        public override void SetDefaults()
        {
            NPC.width = 32; //Needs sprite
            NPC.height = 32; /**/
            NPC.aiStyle = -1;
            NPC.damage = ModContent.GetInstance<InbornTierConfig>().RupterDamage;
            NPC.defense = ModContent.GetInstance<InbornTierConfig>().RupterDEF;
            NPC.lifeMax = ModContent.GetInstance<InbornTierConfig>().RupterHP;
            NPC.HitSound = SoundID.NPCHit1; // This a the one below is in need of addressing/custom sounds
            NPC.DeathSound = SoundID.NPCDeath1; /**/
            NPC.value = 100f;
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            int CurPhase = ModContent.GetInstance<SRPTerraria>().Get_CurrentPhase();

            if (CurPhase > 0 && CurPhase < 5)
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

            if (!NPC.collideX)
            {
                isClimbing = false;
            }

            if (!isLeaping)
            {
                if (AI_Leap_CD > 0)
                {
                    AI_Leap_CD--;
                }

                return true;
            }
            else
            {
                if (NPC.collideY | NPC.collideX)
                {
                    isLeaping = false;
                    
                    NPC.velocity.X = 0;
                }

                return false;
            }
        }

        public override void AI()
        {
            float MoveSpeed = 1.5f;
            float MaxSpeed = MoveSpeed * 2.0f;

            NPC.TargetClosest(false);

            if (NPC.collideX)
            {
                Climb();
            }

            //Checks if AI_Timer is 0, resets as needed. AI_Timer is used to add some delay to switching between staying and moving
            if (AI_Timer != 0)
            {
                AI_Timer--;
            }
            else
            {
                AI_Timer = 240f;
            }

            if (Main.player[NPC.target].Distance(NPC.Center) < TargetRange)
            {
                if (SRPTerraria.CurrentPhase >= 2)
                {
                    AI_State = 3;
                }
                else
                {
                    AI_State = 2;
                }
            }
            else
            {
                //Decides if it should be staying or moving, randomizes every 3~ seconds
                if (AI_Timer == 0)
                {
                    if (OnFire)
                    {
                        AI_State = 2;
                    }
                    else if (AI_State < 2)
                    {
                        AI_State = rnd.Next(2);
                    }
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
                case 3:
                    Chase();
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
            float MoveSpeed = 1.5f;
            float MaxSpeed = MoveSpeed * 3f;
            isFleeing = true;

            if (NPC.collideX)
            {
                Climb();
            }

            float DirecTo;

            if (OnFire)
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

                if (NPC.velocity.X < MaxSpeed)
                {
                    if (NPC.velocity.X > MaxSpeed * -1)
                    {
                        NPC.velocity.X = NPC.direction * MoveSpeed * 2f;
                    }
                    else
                    {
                        NPC.velocity.X = MaxSpeed * NPC.direction;
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

        private void Chase()
        {
            float MoveSpeed = 1.5f;
            float MaxSpeed = MoveSpeed * 3f;

            NPC.TargetClosest(true);

            if (NPC.collideX)
            {
                Climb();
            }

            if (NPC.velocity.X < MaxSpeed)
            {
                if (NPC.velocity.X > MaxSpeed * -1)
                {
                    NPC.velocity.X = NPC.direction * MoveSpeed * 2f;
                }
                else
                {
                    NPC.velocity.X = MaxSpeed * NPC.direction;
                }
            }

            if (Main.player[NPC.target].Distance(NPC.Center) < 400f && AI_Leap_CD == 0 && !isClimbing)
            {
                float DirecTo = NPC.Center.X - Main.player[NPC.target].Center.X;

                if (DirecTo > 0)
                {
                    NPC.direction = -1;
                }
                else if (DirecTo < 0)
                {
                    NPC.direction = 1;
                }

                Leap();
            }
        }

        private void Leap()
        {
            isLeaping = true;

            NPC.TargetClosest(false);

            AI_Leap_CD = 180;

            NPC.velocity += new Vector2(7.5f * NPC.direction, -6f);
        }

        private void Climb()
        {
            isClimbing = true;
            NPC.velocity.Y = -2f;
        }

        public override bool? CanFallThroughPlatforms()
        {
            if (Main.player[NPC.target].position.Y < NPC.position.Y)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public override void FindFrame(int frameHeight)
        {
            NPC.spriteDirection = NPC.direction;

            //TEMPORARY, fix this once you get some real sprites
            if (!isLeaping)
            {
                NPC.frame.Y = 0;
            }
            else
            {
                NPC.frame.Y = 32;
            }
        }

        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                new FlavorTextBestiaryInfoElement("Rupters. Hostile and agile, they are the harbinger of something much, MUCH worse... Best to not let them live and spread, least you get yourself into a situation of much greater peril."),
            });
        }
    }
}

/* disabledd cuz no item yet
namespace SRPTerraria.Content.Items.ParasiteItems
{
    public class RupterDropItem : ModItem
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
*/