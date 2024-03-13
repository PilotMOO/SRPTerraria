using Terraria;
using SRPTerraria;
using SRPTerraria.Content.NPCs.Parasites.Adaptable;
using SRPTerraria.Common.Configs.ParasiteConfigs;
using SRPTerraria.Content.Items.ParasiteItems;
using Terraria.ID;
using Terraria.ModLoader;

namespace SRPTerraria.Content.NPCs.Parasites.Adaptable
{
    public class AdaptationParasiteTestNPC : ParasiteAdaptableNPC
    {
        public override void SetDefaults()
        {
            NPC.width = 64;
            NPC.height = 64;
            NPC.aiStyle = -1;
            NPC.damage = 40;
            NPC.defense = 50;
            NPC.lifeMax = 4000;
            NPC.HitSound = SoundID.NPCHit1; // This a the one below is in need of addressing/custom sounds
            NPC.DeathSound = SoundID.NPCDeath1; /**/
            NPC.value = 0f; // No money :[

            this.CanDespawn = false;
            this.PointDeathDeduction = 10;
            ParasiteAdaptableNPC.AdaptationCap = 6;
            this.AdaptationChance = 0.5f;
            this.AdaptationFireChanceFail = 0.7f;
            this.AdaptationPercentPerSucces = 0.1f;
        }
    }
}