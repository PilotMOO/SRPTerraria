using Terraria;
using Terraria.ModLoader;
using SRPTerraria;
using SRPTerraria.Content.Items;

namespace SRPTerraria.Content.Items
{
    public class ParasitePointWand : ModItem
    {
        int PointValue = 100;

        public override void SetDefaults()
        {
            Item.useStyle = 9;
            Item.autoReuse = true;
            Item.useTurn = true;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.maxStack = 1;
            Item.consumable = false;
            Item.width = 32;
            Item.height = 32;
            Item.noUseGraphic = true;
        }

        public override bool? UseItem(Player player)
        {
            if (player.altFunctionUse != 2)
            {
                if (ModContent.GetInstance<SRPTerraria>().AddPoints(PointValue, false))
                {
                    int CurPoints = SRPTerraria.CurrentPoints;
                    int CurPhase = ModContent.GetInstance<SRPTerraria>().Get_CurrentPhase();

                    Main.NewText("Added " + PointValue + " Points!");
                    Main.NewText("Current Points: " + CurPoints);
                    Main.NewText("Current Phase: " + CurPhase);

                    return true;
                }
                else
                {
                    Main.NewText("Cannot add more points :[");

                    return true;
                }
            }
            else { return false; }
        }

        public override bool AltFunctionUse(Player player)
        {
            if (ModContent.GetInstance<SRPTerraria>().AddPoints(PointValue, true))
            {
                int CurPoints = SRPTerraria.CurrentPoints;
                int CurPhase = ModContent.GetInstance<SRPTerraria>().Get_CurrentPhase();

                Main.NewText("Removed " + PointValue + " Points!");
                Main.NewText("Current Points: " + CurPoints);
                Main.NewText("Current Phase: " + CurPhase);

                return true;
            }
            else
            {
                Main.NewText("Cannot remove more points :[");

                return false;
            }
        }
    }
}