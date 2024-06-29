using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace RappleMod.Content.Projectiles.ShroomiteVortexProj
{
	public class ShroomiteVortexProj : ModProjectile
	{
		public override void SetDefaults() {
			Projectile.width = 10;
			Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
            Projectile.alpha = 255;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
		}

        public override void AI(){   
            Projectile.alpha -= 10;

            NPC mainTarget = Main.npc[(int)Projectile.ai[0]];
            NPC closestNPC = FindClosestNPC(2000);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (mainTarget.active && mainTarget.CanBeChasedBy()){
                Vector2 desiredVelocity = Projectile.DirectionTo(mainTarget.Center) * 23;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 0.07f);
            }
            else if (closestNPC != null){
                Vector2 desiredVelocity = Projectile.DirectionTo(closestNPC.Center) * 23;
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 0.07f);
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 3; i++){
				Dust.NewDust(Projectile.Center, 1, 1, DustID.GoldCoin, Projectile.velocity.X/2, Projectile.velocity.Y/2, 1, Color.LightBlue);
			}
            SoundEngine.PlaySound(SoundID.DD2_GoblinBomb, Projectile.position);
        }

        public NPC FindClosestNPC(float maxDetectDistance) {
			NPC closestNPC = null;

			float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

			for (int k = 0; k < Main.maxNPCs; k++) {
				NPC target = Main.npc[k];
				if (target.CanBeChasedBy()) {
					float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

					if (sqrDistanceToTarget < sqrMaxDetectDistance) {
						sqrMaxDetectDistance = sqrDistanceToTarget;
						closestNPC = target;
					}
				}
			}
            return closestNPC;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Projectile.frame = 10;
            Projectile.scale = 0.8f;
            Texture2D projectileTexture = ModContent.Request<Texture2D>("RappleMod/Content/Projectiles/ShroomiteVortexProj/ShroomiteVortexProj").Value;
            Vector2 drawPosition = Projectile.position + new Vector2(Projectile.width, Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
            Vector2 drawOrigin = new Vector2(projectileTexture.Width, projectileTexture.Height) / 2f;
            Color drawColor = Projectile.GetAlpha(lightColor);
            drawColor.A = 127;
            drawColor *= 0.5f;

            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (float transparency = 1f; transparency >= 0f; transparency -= 0.05f) {
                float opacity = 1f - transparency;
                Vector2 drawAdjustment = Projectile.velocity * -4f * transparency;
                Main.EntitySpriteDraw(projectileTexture, drawPosition + drawAdjustment, null, drawColor * opacity, Projectile.rotation, drawOrigin, Projectile.scale * 1.15f * MathHelper.Lerp(0.5f, 1f, opacity), spriteEffects, 0);
            }

            return true;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) => modifiers.DisableCrit();

        public override Color? GetAlpha(Color lightColor) => Color.Orange;
    }
}
