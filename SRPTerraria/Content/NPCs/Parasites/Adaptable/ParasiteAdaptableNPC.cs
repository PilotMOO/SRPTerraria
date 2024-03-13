using Microsoft.Xna.Framework;
using Microsoft.CodeAnalysis;
using ReLogic.Utilities;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.WorldBuilding;
using static System.Net.Mime.MediaTypeNames;

namespace SRPTerraria.Content.NPCs.Parasites.Adaptable
{
    //Base Class for all Parasites with the ability to adapt to damage
        //Relies heavily on AdaptableType
    public class ParasiteAdaptableNPC : ParasiteBaseNPC
    {
            //make sure to update these four variables when making a new NPC that inherits ParasiteAdaptableNPC
        /**/
        public static int AdaptationCap = 0; //How many "types" this parasite can adapt to. This needs to be set via a config
        public float AdaptationChance = 0f; //Probability to adapt to a hit
        public float AdaptationFireChanceFail = 0f; //Probability to fail adaptation to a hit when on fire
        public float AdaptationPercentPerSucces = 0f; //How much adaptation/DMG reduction increases with each successful adaption hit
        /**/

        // All "Types" of damage this parasite has adapted too
        public static AdaptableType[] AdaptationList = new AdaptableType[AdaptationCap];

        //Calls AdaptToHit() when hit by a projectile
        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            this.AdaptToHit(projectile.type);
        }

        //Same as above, but for melee weapons
        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            this.AdaptToHit(item);
        }

        //Calls AdaptationDamageReduction() when hit by a projectile
        public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            ParasiteAdaptableNPC.AdaptationDamageReduction(projectile, modifiers);
        }

        //Same as above, but for melee weapons (again)
        public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
        {
            ParasiteAdaptableNPC.AdaptationDamageReduction(item, modifiers);
        }

        //Adaptation Code for Projectiles
        public void AdaptToHit(int projectileID)
        {
            float AdaptationPercent = 0;
            bool HasAlreadyAdapted = false;
            int index = 0;

            //Checks all AdaptableTypes in AdaptationList, if any have the same ProjectileID as the arguement, calls UpdateAdaptable for that AdaptableType, sets HasAlreadyAdapted Bool to true, and breaks
            foreach (AdaptableType adaptations in ParasiteAdaptableNPC.AdaptationList)
            {
                if (ParasiteAdaptableNPC.AdaptationList[index].projectileID == projectileID)
                {
                    HasAlreadyAdapted = true;
                    AdaptationPercent = ParasiteAdaptableNPC.AdaptationList[index].UpdateAdaptable(AdaptationChance, AdaptationFireChanceFail, AdaptationPercentPerSucces, this.isOnFire);
                    break;
                }

                index++;
            }

            //If HasAlreadyAdapted is false...
            if (!HasAlreadyAdapted)
            {
                //...Then, for each slot inside of AdaptationList...
                for (int i = 0; i < AdaptationCap - 1; i++)
                {
                    //...If said slot is empty...
                    if (ParasiteAdaptableNPC.AdaptationList[i] == null)
                    {
                        //...Create a new AdaptableType then break
                        ParasiteAdaptableNPC.AdaptationList[i] = new AdaptableType(projectileID, null);
                        break;
                    }
                }
            }

            //If AdaptationPercent returns 1, create a new Purple CombatText stating that this parasite has fully adapted to said damage type
            if (AdaptationPercent == 1)
            {
                CombatText.NewText(new Rectangle((int)NPC.Center.X, (int)NPC.Top.Y, NPC.width, NPC.width / 2), new Color(139, 27, 163), "Fully Adapted!", true);
            }
        }

        //Same as above, but for melee weapons
        public void AdaptToHit(Item item)
        {
            float AdaptationPercent = 0;
            bool HasAlreadyAdapted = false;
            int index = 0;

            foreach (AdaptableType adaptations in ParasiteAdaptableNPC.AdaptationList)
            {
                if (ParasiteAdaptableNPC.AdaptationList[index].item == item)
                {
                    HasAlreadyAdapted = true;
                    AdaptationPercent = ParasiteAdaptableNPC.AdaptationList[index].UpdateAdaptable(AdaptationChance, AdaptationFireChanceFail, AdaptationPercentPerSucces, this.isOnFire);
                    break;
                }

                index++;
            }

            for (int i = 0; i < AdaptationCap - 1; i++)
            {
                if (!HasAlreadyAdapted)
                {
                    if (ParasiteAdaptableNPC.AdaptationList[i] == null)
                    {
                        ParasiteAdaptableNPC.AdaptationList[i] = new AdaptableType(ProjectileID.None, item);
                        break;
                    }
                }
            }

            if (AdaptationPercent == 1)
            {
                CombatText.NewText(new Rectangle((int)NPC.Center.X, (int)NPC.Top.Y, NPC.width, NPC.width / 2), new Color(139, 27, 163), "Fully Adapted!", true);
            }
        }

        //Attempts to modify the damage of a hit for projectiles scaling off of adaptation of said projectile
        public static void AdaptationDamageReduction(Projectile projectile, NPC.HitModifiers modifiers)
        {
            int index = 0;
            float DamageReductionPercent = 1;

            //Cycles through all slots...
            foreach (AdaptableType adaptations in ParasiteAdaptableNPC.AdaptationList)
            {
                //...If any return the same ProjectileID as the arguement...
                if (ParasiteAdaptableNPC.AdaptationList[index].projectileID == projectile.type)
                {
                    //Call ReturnDamageReduction() for that AdaptableType, then break
                    DamageReductionPercent = ParasiteAdaptableNPC.AdaptationList[index].ReturnDamageReduction();
                    break;
                }
                index++;
            }

            //Updates the damage modifier for the hit by multipling it by the returned value from ReturnDamageReduction()
            modifiers.SourceDamage *= DamageReductionPercent;
        }

        //Same as above but for melee weapons
        public static void AdaptationDamageReduction(Item item, NPC.HitModifiers modifiers)
        {
            int index = 0;
            float DamageReductionPercent = 1;

            foreach (AdaptableType adaptations in ParasiteAdaptableNPC.AdaptationList)
            {
                if (ParasiteAdaptableNPC.AdaptationList[index].item == item)
                {
                    DamageReductionPercent = ParasiteAdaptableNPC.AdaptationList[index].ReturnDamageReduction();
                    break;
                }

                index++;
            }

            modifiers.SourceDamage *= DamageReductionPercent;
        }
    }
}