using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace RappleMod.Content.Weapons{
    
    public class Ouroboros : ModItem
    {
        public override string Texture => $"Terraria/Images/Item_{ItemID.Flamethrower}";
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
			Item.shoot = ProjectileID.Flames;
			Item.shootSpeed = 5f; 
			Item.useAmmo = AmmoID.Bullet;
			Item.UseSound = SoundID.Item11;
		}
		
		/*
		private void AI_193_Flamethrower()
		{
			this.localAI[0] += 1f;
			int num = 60;
			int num2 = 12;
			int num3 = num + num2;
			if (this.localAI[0] >= (float)num3)
			{
				this.Kill();
			}
			if (this.localAI[0] >= (float)num)
			{
				base.velocity *= 0.95f;
			}
			bool flag = this.ai[0] == 1f;
			int num4 = 50;
			int num5 = num4;
			if (flag)
			{
				num4 = 0;
				num5 = num;
			}
			if (this.localAI[0] < (float)num5 && Main.rand.NextFloat() < 0.25f)
			{
				short num6 = (short)(flag ? 135 : 6);
				Dust dust = Dust.NewDustDirect(base.Center + Main.rand.NextVector2Circular(60f, 60f) * Utils.Remap(this.localAI[0], 0f, 72f, 0.5f, 1f), 4, 4, num6, base.velocity.X * 0.2f, base.velocity.Y * 0.2f, 100);
				if (Main.rand.Next(4) == 0)
				{
					dust.noGravity = true;
					dust.scale *= 3f;
					dust.velocity.X *= 2f;
					dust.velocity.Y *= 2f;
				}
				else
				{
					dust.scale *= 1.5f;
				}
				dust.scale *= 1.5f;
				dust.velocity *= 1.2f;
				dust.velocity += base.velocity * 1f * Utils.Remap(this.localAI[0], 0f, (float)num * 0.75f, 1f, 0.1f) * Utils.Remap(this.localAI[0], 0f, (float)num * 0.1f, 0.1f, 1f);
				dust.customData = 1;
			}
			if (num4 > 0 && this.localAI[0] >= (float)num4 && Main.rand.NextFloat() < 0.5f)
			{
				Vector2 center = Main.player[this.owner].Center;
				Vector2 vector = (base.Center - center).SafeNormalize(Vector2.Zero).RotatedByRandom(0.19634954631328583) * 7f;
				short num7 = 31;
				Dust dust2 = Dust.NewDustDirect(base.Center + Main.rand.NextVector2Circular(50f, 50f) - vector * 2f, 4, 4, num7, 0f, 0f, 150, new Color(80, 80, 80));
				dust2.noGravity = true;
				dust2.velocity = vector;
				dust2.scale *= 1.1f + Main.rand.NextFloat() * 0.2f;
				dust2.customData = -0.3f - 0.15f * Main.rand.NextFloat();
			}
		}

		case 506:
			this.consumeAmmoOnFirstShotOnly = true;
			this.useStyle = 5;
			this.autoReuse = true;
			this.useAnimation = 30;
			this.useTime = 6;
			base.width = 50;
			base.height = 18;
			this.shoot = 85;
			this.useAmmo = AmmoID.Gel;
			this.UseSound = SoundID.Item34;
			this.damage = 35;
			this.knockBack = 0.3f;
			this.shootSpeed = 7f;
			this.noMelee = true;
			this.value = 500000;
			this.rare = 5;
			this.ranged = true;
			break;

		*/

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