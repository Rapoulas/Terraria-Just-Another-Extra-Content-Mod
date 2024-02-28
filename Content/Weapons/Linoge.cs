using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace TutorialMod.Content.Weapons{
    
    public class Linoge : ModItem
    {
        public override void SetDefaults() {
			Item.width = 64;
			Item.height = 36;
			Item.scale = 0.75f;
			Item.rare = ItemRarityID.Orange;

			Item.useTime = 7; 
			Item.useAnimation = 7;
			Item.autoReuse = true;

			Item.DamageType = DamageClass.Ranged; 
			Item.damage = 20; 
			Item.knockBack = 5f; 
			Item.noMelee = true;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.shoot = ProjectileID.PurificationPowder;
			Item.shootSpeed = 5f; 
			Item.useAmmo = AmmoID.Bullet;
		}

		public override bool AltFunctionUse(Player player) {
			return true;
		}
		
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			if (player.altFunctionUse != 2){
				SoundEngine.PlaySound(SoundID.Item11, player.Center);
				Vector2 muzzleOffset = Vector2.Normalize(velocity) * 25f;
				
				if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0)) {
					position += muzzleOffset;
				}

				for (int p = 0; p < 3; p++){
					Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(35));
					newVelocity *= 1f - Main.rand.NextFloat(0.3f);
					if (damage > Item.damage*2) Projectile.NewProjectile(Projectile.InheritSource(player), position, newVelocity, type, 20, 0, player.whoAmI);
					else Projectile.NewProjectile(Projectile.InheritSource(player), position, newVelocity, type, damage, 0, player.whoAmI);		
				}
			}
            return false;
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
			if (player.altFunctionUse == 2) {
				type = 0;
				Item.DamageType = DamageClass.Melee;
				Item.noMelee = false;
				Item.useTime = 14; 
				Item.useAnimation = 14;
				Item.damage = 50;
				Item.useStyle = ItemUseStyleID.Swing;
				SoundEngine.PlaySound(SoundID.Item1, player.Center);
			} 
			else{
				Item.DamageType = DamageClass.Ranged;
				Item.noMelee = true;
				Item.useTime = 7; 
				Item.useAnimation = 7;
				Item.damage = 20;
				Item.useStyle = ItemUseStyleID.Shoot;
				type = ProjectileID.MeteorShot;
			}
			base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
		}
    }
}