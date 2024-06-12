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
		public override string Texture => "Terraria/Images/Item_" + ItemID.Revolver;
        public override void SetDefaults() {
			Item.width = 52;
			Item.height = 20;
			Item.useTime = 60; 
			Item.useAnimation = 60;
			Item.autoReuse = true;
			Item.DamageType = DamageClass.Ranged; 
			Item.damage = 40; 
			Item.knockBack = 5f; 
			Item.noMelee = true;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.shoot = ModContent.ProjectileType<HeartbreakerBullet>();
			Item.shootSpeed = 7f; 
			Item.useAmmo = AmmoID.Bullet;
			Item.UseSound = SoundID.Item11;
		}

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
			type = type = ModContent.ProjectileType<HeartbreakerBullet>();;
        }
    }
}