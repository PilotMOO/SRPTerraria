using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using SRPTerraria;

namespace SRPTerraria
{
    public class SRPTerrariaModSystem : ModSystem
    {
        public override void PreUpdateEntities()
        {
            if (ModContent.GetInstance<SRPTerraria>().PhaseCooldown != 0)
            {
                ModContent.GetInstance<SRPTerraria>().PhaseCooldown--;
            }
            if (SRPTerraria.PhaseSoundCooldown != 0)
            {
                SRPTerraria.PhaseSoundCooldown--;
                
                if (SRPTerraria.PhaseSoundCooldown == 0)
                {
                    SoundEngine.SoundPlayer.ResumeAll();
                }
            }
        }
    }
}