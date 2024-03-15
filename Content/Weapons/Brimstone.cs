using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RappleMod.Content.Projectiles.Brimstone;

namespace RappleMod.Content.Weapons
{
	public class Brimstone : ModItem
	{
		// You can use a vanilla texture for your item by using the format: "Terraria/Item_<Item ID>".
		public override string Texture => "Terraria/Images/Item_" + ItemID.LunarFlareBook;
		public static Color OverrideColor = new(122, 173, 255);

		public override void SetDefaults() {
            Item.width = 28;
            Item.height = 30;
			Item.mana = 4;
			Item.shoot = ModContent.ProjectileType<BrimstoneHoldOut>();
			Item.shootSpeed = 30f;
            Item.noMelee = true;
            Item.channel = true;
            Item.damage = 250;
            Item.noUseGraphic = false;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.rare = ItemRarityID.Red;
            Item.DamageType = DamageClass.Magic;
			Item.useStyle = ItemUseStyleID.Shoot;

			// Change the item's draw color so that it is visually distinct from the vanilla Last Prism.
			Item.color = OverrideColor;
		}

		public override void AddRecipes() {
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.LastPrism, 1);
			recipe.AddIngredient(ItemID.LunarFlareBook, 1);
			recipe.AddIngredient(ItemID.NebulaBlaze, 1);
			recipe.AddTile(TileID.LunarCraftingStation);
			recipe.Register();
		}

		// Because this weapon fires a holdout projectile, it needs to block usage if its projectile already exists.
		public override bool CanUseItem(Player player) {
			return player.ownedProjectileCounts[ModContent.ProjectileType<BrimstoneHoldOut>()] <= 0;
		}
	}
}