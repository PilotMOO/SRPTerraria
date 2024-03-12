using Microsoft.Xna.Framework;
using MonoMod.Cil;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using SRPTerraria.Content.Effects;

namespace SRPTerraria.Content.NPCs.Parasites
{
    public class ParasiteBaseNPC : ModNPC
    {
        public void FireWeakness(NPC target)
        {
            if (!target.HasBuff(ModContent.BuffType<ExtraFireDPSDebuff>()))
            {
                target.AddBuff(ModContent.BuffType<ExtraFireDPSDebuff>(), 600);
            }
        }
    }
}