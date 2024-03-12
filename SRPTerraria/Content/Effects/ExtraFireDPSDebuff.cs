using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SRPTerraria.Content.Effects
{
	public class ExtraFireDPSDebuff : ModBuff
	{
        int FireDPS;

        public override void Update(NPC npc, ref int buffIndex)
        {
            bool Onfire = false;

            FireDPS = 0;

            if (npc.HasBuff(BuffID.OnFire))
            {
                FireDPS += 24 * 2;
                Onfire = true;
            }
            if (npc.HasBuff(BuffID.OnFire3))
            {
                FireDPS += 70 * 2;
                Onfire = true;
            }
            if (Onfire)
            {
                npc.lifeRegen -= FireDPS;
            }
        }
    }
}