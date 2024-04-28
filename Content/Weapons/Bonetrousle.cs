using RappleMod.Content.Projectiles.Bonetrousle;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RappleMod.Content.Weapons
{
	public class Bonetrousle : ModItem
	{
		public override void SetStaticDefaults() {
			ItemID.Sets.Yoyo[Item.type] = true;
			ItemID.Sets.GamepadExtraRange[Item.type] = 15; 
			ItemID.Sets.GamepadSmartQuickReach[Item.type] = true; 
		}

		public override void SetDefaults() {
			Item.width = 30; 
			Item.height = 26;

			Item.useStyle = ItemUseStyleID.Shoot; 
			Item.useTime = 25; // All vanilla yoyos have a useTime of 25.
			Item.useAnimation = 25; // All vanilla yoyos have a useAnimation of 25.
			Item.noMelee = true; 
			Item.noUseGraphic = true;
			Item.UseSound = SoundID.Item1; 
			Item.damage = 25; 
			Item.DamageType = DamageClass.MeleeNoSpeed;
			Item.knockBack = 2.5f;
			Item.channel = true; 
			Item.rare = ItemRarityID.Orange;
			Item.value = Item.buyPrice(gold: 5);

			Item.shoot = ModContent.ProjectileType<BonetrousleProjectile>(); 
			Item.shootSpeed = 16f;		
		}
		
	}
}
