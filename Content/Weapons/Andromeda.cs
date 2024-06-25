using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RappleMod.Content.Projectiles.Andromeda;

namespace RappleMod.Content.Weapons
{
	public class Andromeda : ModItem
	{

        public override void SetStaticDefaults() {
			// This line will make the damage shown in the tooltip twice the actual Item.damage. This multiplier is used to adjust for the dynamic damage capabilities of the projectile.
			// When thrown directly at enemies, the flail projectile will deal double Item.damage, matching the tooltip, but deals normal damage in other modes.
			ItemID.Sets.ToolTipDamageMultiplier[Type] = 2f;
		}

		public override void SetDefaults() {
			Item.useStyle = ItemUseStyleID.Shoot; 
			Item.useAnimation = 45; 
			Item.useTime = 45; 
			Item.knockBack = 5.5f;
			Item.width = 32;
			Item.height = 32;
			Item.damage = 15;
			Item.noUseGraphic = true;
			Item.shoot = ModContent.ProjectileType<AndromedaProjectile>();
			Item.shootSpeed = 12f;
			Item.UseSound = SoundID.Item1;
			Item.rare = ItemRarityID.Expert;
			Item.value = Item.buyPrice(0, 3, 0, 0);
			Item.DamageType = DamageClass.MeleeNoSpeed;
			Item.channel = true;
			Item.noMelee = true; // This makes sure the item does not deal damage from the swinging animation
			Item.expert = true;
		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.Sunfury, 1);
			recipe.AddIngredient(ItemID.FallenStar, 50);
			recipe.AddTile(TileID.Hellforge);
			recipe.Register();
		}
	}

}