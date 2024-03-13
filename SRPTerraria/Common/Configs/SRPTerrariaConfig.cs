using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace SRPTerraria.Common.Configs
{
    public class SRPTerrariaConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        [Header("Phases")]

        [DefaultValue(new int[] { 0, 400, 800, 1800, 20000, 200000, 5000000, 25000000, 500000000 })]
        [Label("The point requirements for each phase, from 0-8")]
        public int[] PhasePointRequirements;

        [DefaultValue(new string[] { "ZERO", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight"} )]
        [Label("The phase messages for each phase, from 0-8")]
        public string[] PhaseMessages;

        [DefaultValue(-1000)]
        [Range(0, int.MaxValue)]
        [Label("The default points that a world starts with")]
        [Tooltip("Setting this to a number larger than Phase 0 point requirement will cause the world to start on phase 0 rather than -1, same with the other numbers")]
        public int StartingPoints;

        [DefaultValue(72000)]
        [Range(0, int.MaxValue)]
        [Label("How long the point cooldown is for")]
        [Tooltip("After the phase increases, the parasites are incapible of gaining points for X time. This is measured in frames, so 60 = 1 second")]
        public int PhaseCooldown;

        [DefaultValue(100)]
        [Range(0, int.MaxValue)]
        [Label("How long it takes for sounds to return after a phase sound starts")]
        [Tooltip("After the phase increases, a special sound is played, halting all other sounds from playing for this specified duration. This is measured in frames, so 60 = 1 second")]
        public int PhaseSoundCooldownDefault;

        [Header("ParasiteGeneral")]

        [DefaultValue(100)]
        [Range(0, int.MaxValue)]
        [Label("Maximum Parasite Cap")]
        [Tooltip("How many parasites can exist in the world at a given time. If exeeded, will attempt to despawn any new parasites if they can be despawned")]
        public int ParasiteCap;
    }
}
