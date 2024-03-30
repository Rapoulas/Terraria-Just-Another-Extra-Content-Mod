using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace RappleMod.Content.Projectiles.Bonetrousle
{
	public class BonetrousleProjectile : ModProjectile
	{
        public int timer;
        public override void SetStaticDefaults() {

			ProjectileID.Sets.YoyosLifeTimeMultiplier[Projectile.type] = 12f;
			ProjectileID.Sets.YoyosMaximumRange[Projectile.type] = 500f;
			ProjectileID.Sets.YoyosTopSpeed[Projectile.type] = 15f;
            Main.projFrames[Projectile.type] = 2;
		}

		public override void SetDefaults() {
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.aiStyle = ProjAIStyleID.Yoyo;
			Projectile.friendly = true; 
			Projectile.DamageType = DamageClass.MeleeNoSpeed;
			Projectile.penetrate = -1; 
            Projectile.tileCollide = false;
			// Projectile.scale = 1f; // The scale of the projectile. Most yoyos are 1f, but a few are larger. The Kraken is the largest at 1.2f
		}

        // notes for aiStyle 99: 
        // localAI[0] is used for timing up to YoyosLifeTimeMultiplier
        // localAI[1] can be used freely by specific types
        // ai[0] and ai[1] usually point towards the x and y world coordinate hover point
        // ai[0] is -1f once YoyosLifeTimeMultiplier is reached, when the player is stoned/frozen, when the yoyo is too far away, or the player is no longer clicking the shoot button.
        // ai[0] being negative makes the yoyo move back towards the player
        // Any AI method can be used for dust, spawning projectiles, etc specific to your yoyo.

        public override void AI()
        {
            timer++;

            if (timer % 30 == 0){
                float newDamage = Projectile.damage/3f;
                Vector2 randomDir = new(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                randomDir.Normalize();
                randomDir *= 15;
                if (!IsInsideTile())
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, randomDir, ModContent.ProjectileType<BonetrousleBoneProjectile>(), Projectile.damage, Projectile.knockBack);
                else 
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position, randomDir, ModContent.ProjectileType<BonetrousleSpiritProjectile>(), (int)newDamage, Projectile.knockBack);
                
                timer = 0;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {   
            if (IsInsideTile()) {
                Projectile.frame = 1;

				Texture2D projectileTexture = ModContent.Request<Texture2D>("RappleMod/Content/Projectiles/Bonetrousle/BonetrousleProjectile2").Value;
				Vector2 drawPosition = Projectile.position + new Vector2(Projectile.width, Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
				Vector2 drawOrigin = new Vector2(projectileTexture.Width, projectileTexture.Height) / 2f;
				Color drawColor = Projectile.GetAlpha(lightColor);
				drawColor.A = 127;
				drawColor *= 0.5f;
				int launchTimer = timer;
				if (launchTimer > 5) {
					launchTimer = 5;
				}

				SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

				for (float transparency = 1f; transparency >= 0f; transparency -= 0.125f) {
					float opacity = 1f - transparency;
					Vector2 drawAdjustment = Projectile.velocity * -launchTimer * transparency;
					Main.EntitySpriteDraw(projectileTexture, drawPosition + drawAdjustment, null, drawColor * opacity, Projectile.rotation, drawOrigin, Projectile.scale * 1.15f * MathHelper.Lerp(0.5f, 1f, opacity), spriteEffects, 0);
				}
			
            }
            else Projectile.frame = 0;
            
            return true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            if (IsInsideTile()) return Color.White;
            else return null;
        }
        bool IsInsideTile() => Collision.SolidCollision(Projectile.TopLeft, Projectile.width, Projectile.height);

    }
}
