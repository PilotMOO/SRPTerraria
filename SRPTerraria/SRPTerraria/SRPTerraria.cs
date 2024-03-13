using System.Collections;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.Audio;
using SRPTerraria.Content.NPCs.Parasites;
using SRPTerraria.Content.NPCs.Parasites.Inborn;
using SRPTerraria.Common.Configs;

namespace SRPTerraria
{
	public class SRPTerraria : Mod
	{
        //Max amount of parasites that can exist in the world (currenlty not used :/)
        public int ParasiteCap()
        {
            return ModContent.GetInstance<SRPTerrariaConfig>().ParasiteCap;
        }

        //All of the parasites currently in the world. Fuctions like Main.npc but it doesn't actually do anything right now
        public List<ParasiteBaseNPC> AllParasites = new List<ParasiteBaseNPC>();

        //All of the Parasite Types
        public static List<int> AllParasiteType = new List<int>
        {
            ModContent.NPCType<BuglinNPC>(), ModContent.NPCType<RupterNPC>()
        };

        //Self explainatory
        public static int CurrentPoints;
        public int StartingPoints()
        {
            return ModContent.GetInstance<SRPTerrariaConfig>().StartingPoints;
        }

        //Cooldown for gaining points
        public int PhaseCooldownDefault()
        {
            return ModContent.GetInstance<SRPTerrariaConfig>().PhaseCooldown;
        }
        public int PhaseCooldown = 0;
        public int PhaseSoundCooldownDefault()
        {
            return ModContent.GetInstance<SRPTerrariaConfig>().PhaseSoundCooldownDefault;
        }
        public static int PhaseSoundCooldown = 0;

        //Array of all required points for each phase, from 0-8
        public int PhasePointRequirements(int index)
        {
            int[] PhasePointRequirement = ModContent.GetInstance<SRPTerrariaConfig>().PhasePointRequirements;

            return PhasePointRequirement[index];
        }

        //All phase messages, from 0-8
        public string PhaseMessages(int index)
        {
            string[] PhaseMessage = ModContent.GetInstance<SRPTerrariaConfig>().PhaseMessages;

            return PhaseMessage[index];
        }

        //Resets parasite point progression
        public void ResetParasiteProgression()
        {
            CurrentPoints = StartingPoints();

            Get_CurrentPhase();
        }

        public static int CurrentPhase = -1;

        //Checks what current phase it is based off of how many points in CurrentPoints, returns Current Phase
        public int Get_CurrentPhase()
        {
            if (CurrentPoints > this.PhasePointRequirements(8))
            {
                return 8;
            }
            else
            {
                if (CurrentPoints > this.PhasePointRequirements(7))
                {
                    return 7;
                }
                else
                {
                    if (CurrentPoints > this.PhasePointRequirements(6))
                    {
                        return 6;
                    }
                    else
                    {
                        if (CurrentPoints > this.PhasePointRequirements(5))
                        {
                            return 5;
                        }
                        else
                        {
                            if (CurrentPoints > this.PhasePointRequirements(4))
                            {
                                return 4;
                            }
                            else
                            {
                                if (CurrentPoints > this.PhasePointRequirements(3))
                                {
                                    return 3;
                                }
                                else
                                {
                                    if (CurrentPoints > this.PhasePointRequirements(2))
                                    {
                                        return 2;
                                    }
                                    else
                                    {
                                        if (CurrentPoints > this.PhasePointRequirements(1))
                                        {
                                            return 1;
                                        }
                                        else
                                        {
                                            if (CurrentPoints > this.PhasePointRequirements(0))
                                            {
                                                return 0;
                                            }
                                            else
                                            {
                                                if (CurrentPoints > this.StartingPoints())
                                                {
                                                    return -1;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return -2;
        }

        //Adds/Removes points to CurrentPoints, bool returns true if successful
        public bool AddPoints(int value, bool isLoss)
        {
            if (!isLoss)
            {
                if (PhaseCooldown <= 0)
                {
                    CurrentPoints += value;

                    //if after adding points Get_CurrentPhase returns an int different from the current phase, run OnPhaseChange
                    if (CurrentPhase != this.Get_CurrentPhase())
                    {
                        OnPhaseChange();
                    }

                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                CurrentPoints -= value;

                if (CurrentPhase != this.Get_CurrentPhase())
                {
                    OnPhaseChange();
                }

                return true;
            }
        }

        //Checks what phase it is, sets cooldowns for messages and point gain, then calls the Cooldown Coroutines
        public void OnPhaseChange()
        {
            string CurrentPhaseMessage = null;

            if (CurrentPhase > this.Get_CurrentPhase())
            {
                Main.NewText("Phase Decreased!");
                CurrentPhase = this.Get_CurrentPhase();
            }
            else
            {
                CurrentPhase = this.Get_CurrentPhase();

                //Gets the phase message from PhaseNessages[]
                if (CurrentPhase == 0) { CurrentPhaseMessage = PhaseMessages(0); }
                if (CurrentPhase == 1) { CurrentPhaseMessage = PhaseMessages(1); }
                if (CurrentPhase == 2) { CurrentPhaseMessage = PhaseMessages(2); }
                if (CurrentPhase == 3) { CurrentPhaseMessage = PhaseMessages(3); }
                if (CurrentPhase == 4) { CurrentPhaseMessage = PhaseMessages(4); }
                if (CurrentPhase == 5) { CurrentPhaseMessage = PhaseMessages(5); }
                if (CurrentPhase == 6) { CurrentPhaseMessage = PhaseMessages(6); }
                if (CurrentPhase == 7) { CurrentPhaseMessage = PhaseMessages(7); }
                if (CurrentPhase == 8) { CurrentPhaseMessage = PhaseMessages(8); }

                if (!string.IsNullOrEmpty(CurrentPhaseMessage))
                {
                    Main.NewText(CurrentPhaseMessage);
                    if (CurrentPhase < 9 && CurrentPhase > 0)
                    {
                        SoundEngine.SoundPlayer.PauseAll();
                        SoundEngine.PlaySound(new SoundStyle("SRPTerraria/Assets/Sounds/PhaseSounds/evphase_" + CurrentPhase));
                    }

                    PhaseSoundCooldown = PhaseSoundCooldownDefault();
                }
                else
                {
                    Main.NewText("Some issues occured printing out CurrentPhaseMessage, sorry for the inconvenence :[");
                }

                PhaseCooldown = PhaseCooldownDefault();
            }
        }
    }
}