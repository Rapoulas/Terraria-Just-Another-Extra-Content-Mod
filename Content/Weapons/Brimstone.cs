using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RappleMod.Content.Projectiles.Brimstone;
using Terraria.DataStructures;
using Terraria.Audio;

namespace RappleMod.Content.Weapons
{
	public class Brimstone : ModItem
	{
		public override void SetDefaults() {
            Item.width = 28;
            Item.height = 30;
			Item.mana = 4;
			Item.shoot = ModContent.ProjectileType<BrimstoneHoldOut>();
			Item.shootSpeed = 30f;
            Item.noMelee = true;
            Item.channel = true;
            Item.damage = 200;
            Item.noUseGraphic = false;
            Item.useTime = 30;
            Item.useAnimation = 30;
            Item.rare = ItemRarityID.Red;
            Item.DamageType = DamageClass.Magic;
			Item.useStyle = ItemUseStyleID.Shoot;
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