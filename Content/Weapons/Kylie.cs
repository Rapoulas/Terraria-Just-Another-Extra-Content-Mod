using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RappleMod.Content.Projectiles.Kylie;
using Microsoft.Xna.Framework;

namespace RappleMod.Content.Weapons
{
	public class Kylie : ModItem
	{
		public override void SetDefaults()
		{
			Item.damage = 50;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = ItemRarityID.LightRed;
			Item.UseSound = SoundID.Item1;
			Item.autoReuse = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
			Item.shootSpeed = 20f; 
			Item.shoot = ModContent.ProjectileType<KylieProjectile>();
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.TitaniumBar, 12)
				.AddIngredient(ItemID.WoodenBoomerang, 1)
				.AddTile(TileID.MythrilAnvil)
				.Register();
			
			CreateRecipe()
				.AddIngredient(ItemID.AdamantiteBar, 12)
				.AddIngredient(ItemID.WoodenBoomerang, 1)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
			Vector2 muzzleOffset = Vector2.Normalize(velocity) * 8f;

			if (Collision.CanHit(position, 0, 0, position - muzzleOffset, 0, 0)) {
				position -= muzzleOffset;
			}
		}
	}

}