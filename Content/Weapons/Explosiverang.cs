using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RappleMod.Content.Projectiles.Explosiverang;
using Microsoft.Xna.Framework;

namespace RappleMod.Content.Weapons
{
	public class Explosiverang : ModItem
	{
        // The Display Name and Tooltip of this item can be edited in the Localization/en-US_Mods.TutorialMod.hjson file.

		public override void SetDefaults()
		{
			Item.damage = 20;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = ItemRarityID.Orange;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
			Item.shootSpeed = 15f; 
			Item.shoot = ModContent.ProjectileType<ExplosiverangProjectile>();
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
			Vector2 muzzleOffset = Vector2.Normalize(velocity) * 8f;

			if (Collision.CanHit(position, 0, 0, position - muzzleOffset, 0, 0)) {
				position -= muzzleOffset;
			}
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.EnchantedBoomerang, 1);
			recipe.AddIngredient(ItemID.Dynamite, 99);
			recipe.AddTile(TileID.TinkerersWorkbench);
			recipe.Register();
		}
	}

}