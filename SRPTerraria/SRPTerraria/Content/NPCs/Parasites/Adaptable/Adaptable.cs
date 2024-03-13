using System;
using Terraria;
using Terraria.ID;

namespace SRPTerraria.Content.NPCs.Parasites.Adaptable
{
    public class AdaptableType
    {
        public int projectileID = 0;
        public Item item;
        public float adaptationPercent;

        Random random = new Random();

        public AdaptableType(int newprojectileID, Item newitem)
        {
            if (newprojectileID == ProjectileID.None)
            {
                if (newitem != null)
                {
                    if (newitem.shoot != ProjectileID.None)
                    {
                        projectileID = newitem.shoot;
                        item = newitem;
                    }
                }
            }
            else
            {
                projectileID = newprojectileID;
            }

            if (newitem == null)
            {
                item = null;
            }
            if (newprojectileID == ProjectileID.None)
            {
                projectileID = ProjectileID.None;
            }
        }

        public void UpdateAdaptable(float adaptationChance, float adaptationFireFalureRate, float AdaptationPercentOnSuccess, bool isOnFire)
        {
            double adaptationFireRoll = random.Next(0, 11) * 0.1;
            double adaptationSuccessRoll = random.Next(0, 11) * 0.1;

            if (isOnFire)
            {
                if (adaptationFireRoll > adaptationFireFalureRate)
                {
                    if (adaptationSuccessRoll < adaptationChance)
                    {
                        if (adaptationPercent < 1)
                        {
                            adaptationPercent += AdaptationPercentOnSuccess;
                        }
                    }
                }
            }
            else
            {
                if (adaptationSuccessRoll < adaptationChance)
                {
                    if (adaptationPercent < 1)
                    {
                        adaptationPercent += AdaptationPercentOnSuccess;
                    }
                }
            }
        }
    }
}