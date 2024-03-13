using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace SRPTerraria.Content.Items
{
	public class GrubStabber : ModItem
	{
        // The Display Name and Tooltip of this item can be edited in the Localization/en-US_Mods.SRPTerraria.hjson file.

		public override void SetDefaults()
		{
			Item.damage = 69;
			Item.DamageType = DamageClass.Melee;
			Item.width = 100;
			Item.height = 40;
			Item.useTime = 1;
			Item.useAnimation = 20;
			Item.useStyle = 1;
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = 2;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
			Item.shoot = ProjectileID.WoodenArrowFriendly;
			Item.shootSpeed = 20;
        }

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.DirtBlock, 105);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}
}