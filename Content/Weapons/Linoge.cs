using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace RappleMod.Content.Weapons{
    
    public class Linoge : ModItem
    {
        public override void SetDefaults() {
			Item.width = 64;
			Item.height = 36;
			Item.scale = 0.75f;
			Item.rare = ItemRarityID.Lime;

			Item.useTime = 7; 
			Item.useAnimation = 7;
			Item.autoReuse = true;

			Item.DamageType = DamageClass.Ranged; 
			Item.damage = 27; 
			Item.knockBack = 5f; 
			Item.noMelee = true;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.shoot = ProjectileID.PurificationPowder;
			Item.shootSpeed = 5f; 
			Item.useAmmo = AmmoID.Bullet;
			Item.UseSound = SoundID.Item11;
		}
		
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			if (player.altFunctionUse != 2){
				for (int p = 0; p < 3; p++){
					Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(35));
					newVelocity *= 1f - Main.rand.NextFloat(0.3f);
					Projectile.NewProjectile(source, position, newVelocity, type, damage, 0, player.whoAmI);		
				}
				return false;
			}
            return true;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
			if (player.altFunctionUse != 2){
				Vector2 muzzleOffset = Vector2.Normalize(velocity) * 50f;
					
				if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0)) {
					position += muzzleOffset;
				}
			}
        }
    }
}