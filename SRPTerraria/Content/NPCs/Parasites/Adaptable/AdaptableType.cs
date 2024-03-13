using System;
using Terraria;
using Terraria.ID;

namespace SRPTerraria.Content.NPCs.Parasites.Adaptable
{
    //New Type for handling Adaptation
    public class AdaptableType
    {
        public int projectileID = 0;
        public Item item;
        public float adaptationPercent;

        Random random = new Random();

        //Constructers BABEEEEEEEEEEEEEEEYYYY
        public AdaptableType(int newprojectileID, Item newitem)
        {
            //If the Projectile arguement is 'None'...
            if (newprojectileID == ProjectileID.None)
            {
                //...then, if the item argument isn't null...
                if (newitem != null)
                {
                    //...Sets Variable item equal, then checks if the item can shoot a projectile...
                    item = newitem;
                    if (newitem.shoot != ProjectileID.None)
                    {
                        //...If yes, assign it to the projectileID field
                            //Dev note: I was quite proud of this tbf, felt smart -Pilot
                        projectileID = newitem.shoot;
                    }
                }
            }
            else
            {
                //...else, assign the Projectile Variable
                projectileID = newprojectileID;
            }

            //Cleanup, if any variables are still empty after that, set them to null or ProjectileID.None
            if (newitem == null)
            {
                item = null;
            }
            if (newprojectileID == ProjectileID.None)
            {
                projectileID = ProjectileID.None;
            }
        }

        public float UpdateAdaptable(float adaptationChance, float adaptationFireFalureRate, float AdaptationPercentOnSuccess, bool isOnFire)
        {
            //Randomization! gives us too random numbers between 0 and 10, then multiplies them by 0.1 to get a decimal
            double adaptationFireRoll = random.Next(0, 11) * 0.1;
            double adaptationSuccessRoll = random.Next(0, 11) * 0.1;

            //Checks bool argument isOnFire to see if this parasite is burning to death...
            if (isOnFire)
            {
                //...if yes, check to see if the random number generated a number bigger than the failure-rate...
                if (adaptationFireRoll > adaptationFireFalureRate)
                {
                    //if bigger, continue...
                    //...Then, check if the success roll is smaller than the chance to adapt...
                    if (adaptationSuccessRoll < adaptationChance)
                    {
                        //...If yes, finally check to see if the Adaptation is already maxed or not...
                        if (adaptationPercent < 1)
                        {
                            //...If not, add it to the total
                            adaptationPercent += AdaptationPercentOnSuccess;
                        }
                        else
                        {
                            //...If it is, set it to 1
                            adaptationPercent = 1;
                        }
                    }
                }
            }
            else
            {
                //...If not on fire, roll for adaption success, if smaller than adaptationChance...
                if (adaptationSuccessRoll < adaptationChance)
                {
                    //Success! Check if adpatation isn't already maxed...
                    if (adaptationPercent < 1)
                    {
                        //...If not, add it to the total
                        adaptationPercent += AdaptationPercentOnSuccess;
                    }
                    else
                    {
                        //...else set it to 1 and continue
                        adaptationPercent = 1;
                    }
                }
            }

            //Returns the current adaptationPercent
            return adaptationPercent;
        }

        //Returns a float of the damage reduction of the adaptation
        public float ReturnDamageReduction()
        {
            //If the adapationPercent isn't maxed...
            if (adaptationPercent < 1)
            {
                //...Return the difference of it to one multiplied by -1 (I.E. Adaptation% of 0.75 - 1 = -0.25, * -1 = 0.25. Multiply this by the current damage of a hit to reduce damage)
                return (adaptationPercent - 1) * -1;
            }
            else
            {
                //...Otherwise return zero. Makes the hit deal no damage*
                return 0;
            }
        }
    }
}
//*Terraria's damage system might not allow that, we might need to work around it