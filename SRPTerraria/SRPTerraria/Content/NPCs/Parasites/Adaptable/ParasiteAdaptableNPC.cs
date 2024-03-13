using ReLogic.Utilities;
using Terraria;
using Terraria.ID;

namespace SRPTerraria.Content.NPCs.Parasites.Adaptable
{
    class ParasiteAdaptableNPC : ParasiteBaseNPC
    {
            //make sure to update these four variables when making a new NPC that inherits ParasiteAdaptableNPC
        /**/
        public static int AdaptationCap = 0; //How many "types" this parasite can adapt to. This needs to be set via a config
        public float AdaptationChance = 0f; //Probability to adapt to a hit
        public float AdaptationFireChanceFail = 0f; //Probability to fail adaptation to a hit when on fire
        public float AdaptationPercentPerSucces = 0f; //How much adaptation/DMG reduction increases with each successful adaption hit
        /**/

        // All "Types" of damage this parasite has adapted too
        public AdaptableType[] AdaptationList = new AdaptableType[AdaptationCap];

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            this.AdaptToProjectile(projectile);
        }

        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            this.AdaptToItem(item);
        }

        public void AdaptToProjectile(Projectile projectile)
        {
            bool HasAlreadyAdapted = false;
            int index = 0;

            foreach (AdaptableType adaptations in AdaptationList)
            {
                if (AdaptationList[index].projectileID == projectile.type)
                {
                    HasAlreadyAdapted = true;
                    AdaptationList[index].UpdateAdaptable(AdaptationChance, AdaptationFireChanceFail, AdaptationPercentPerSucces, this.isOnFire);
                    break;
                }

                index++;
            }

            for (int i = 0; i <= AdaptationCap; i++)
            {
                if (!HasAlreadyAdapted)
                {
                    if (AdaptationList[i] == null)
                    {
                        AdaptationList[i] = new AdaptableType(projectile.type, null);
                        break;
                    }
                }
            }
        }

        public void AdaptToItem(Item item)
        {
            bool HasAlreadyAdapted = false;
            int index = 0;

            foreach (AdaptableType adaptations in AdaptationList)
            {
                if (AdaptationList[index].item == item)
                {
                    HasAlreadyAdapted = true;
                    AdaptationList[index].UpdateAdaptable(AdaptationChance, AdaptationFireChanceFail, AdaptationPercentPerSucces, this.isOnFire);
                    break;
                }

                index++;
            }

            for (int i = 0; i <= AdaptationCap; i++)
            {
                if (!HasAlreadyAdapted)
                {
                    if (AdaptationList[i] == null)
                    {
                        AdaptationList[i] = new AdaptableType(ProjectileID.None, item);
                        break;
                    }
                }
            }
        }
    }
}