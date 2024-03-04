using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Terraria.GameContent;
using ReLogic.Content;
using RappleMod.Content.Projectiles;

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
			Item.value = Item.sellPrice(gold: 1, silver: 50);
			Item.DamageType = DamageClass.MeleeNoSpeed;
			Item.channel = true;
			Item.noMelee = true; // This makes sure the item does not deal damage from the swinging animation
		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.DirtBlock, 10);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}

}