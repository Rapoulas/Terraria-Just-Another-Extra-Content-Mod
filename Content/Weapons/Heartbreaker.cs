using Microsoft.Xna.Framework;
using RappleMod.Content.Projectiles.Heartbreaker;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace RappleMod.Content.Weapons{
    
    public class Heartbreaker : ModItem
    {
        public override void SetDefaults() {
			Item.width = 52;
			Item.height = 20;
			Item.useTime = 45; 
			Item.useAnimation = 45;
			Item.autoReuse = true;
			Item.DamageType = DamageClass.Ranged; 
			Item.damage = 63; 
			Item.knockBack = 5f; 
			Item.crit = 6;
			Item.noMelee = true;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.shoot = ModContent.ProjectileType<HeartbreakerBullet>();
			Item.shootSpeed = 20f; 
			Item.rare = ItemRarityID.LightRed;
			Item.useAmmo = AmmoID.Bullet;
			Item.UseSound = SoundID.Item91;
			Item.value = Item.buyPrice(0, 10, 0, 0);
		}

		public override void AddRecipes() {
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.SoulofLight, 10);
			recipe.AddIngredient(ItemID.IllegalGunParts, 1);
			recipe.AddIngredient(ItemID.LifeCrystal, 1);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
			type = ModContent.ProjectileType<HeartbreakerBullet>();
        }
    }
}