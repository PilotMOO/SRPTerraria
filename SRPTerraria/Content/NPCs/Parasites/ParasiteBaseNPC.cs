using Terraria;
using Terraria.ModLoader;
using SRPTerraria.Content.Effects;
using Terraria.DataStructures;
using Terraria.ID;

namespace SRPTerraria.Content.NPCs.Parasites
{
    public class ParasiteBaseNPC : ModNPC
    {
        public bool CanDespawn = true;
        public int PointDeathDeduction = 0;
        public bool isOnFire;

        public override bool PreAI()
        {
            this.FireWeakness(NPC);

            return true;
        }

        public override void OnSpawn(IEntitySource source)
        {
            this.CheckParasiteCap();

            if (!CanDespawn)
            {
                NeedSaving();
            }
        }

        public override void OnKill()
        {
            this.OnKillPointReduction();
        }

        public void FireWeakness(NPC target)
        {
            if (NPC.HasBuff(BuffID.OnFire) | NPC.HasBuff(BuffID.OnFire3))
            {
                isOnFire = true;
            }
            if (isOnFire)
            {
                if (!target.HasBuff(ModContent.BuffType<ExtraFireDPSDebuff>()))
                {
                    target.AddBuff(ModContent.BuffType<ExtraFireDPSDebuff>(), 600);
                }
            }
        }

        public void CheckParasiteCap()
        {
            ModContent.GetInstance<SRPTerraria>().AllParasites.Add(this);
            if (ModContent.GetInstance<SRPTerraria>().AllParasites.Count > ModContent.GetInstance<SRPTerraria>().ParasiteCap() && CanDespawn)
            {
                NPC.EncourageDespawn(1);
                ModContent.GetInstance<SRPTerraria>().AllParasites.Remove(this);
            }
        }

        public void OnKillPointReduction()
        {
            ModContent.GetInstance<SRPTerraria>().AddPoints(PointDeathDeduction, true);
        }
    }
}