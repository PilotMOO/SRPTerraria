using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace SRPTerraria.Common.Configs.ParasiteConfigs
{
    public class InbornTierConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [Header("Buglin")]

        [DefaultValue(50)]
        [Range(1, int.MaxValue)]
        [Label("Max HP")]
        public int BuglinHP;

        [DefaultValue(2)]
        [Range(0, int.MaxValue)]
        [Label("Armor")]
        public int BuglinDEF;

        [Header("Rupter")]

        [DefaultValue(600)]
        [Range(1, int.MaxValue)]
        [Label("Max HP")]
        public int RupterHP;

        [DefaultValue(25)]
        [Range(0, int.MaxValue)]
        [Label("Armor")]
        public int RupterDEF;

        [DefaultValue(70)]
        [Range(0, int.MaxValue)]
        [Label("Damage")]
        public int RupterDamage;

        [DefaultValue(500)]
        [Range(0, int.MaxValue)]
        [Label("Tracking Range")]
        [Tooltip("How far away the rupter can notice players")]
        public int RupterTrackingRange;
    }
}
