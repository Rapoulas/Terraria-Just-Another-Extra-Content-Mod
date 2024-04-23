using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace RappleMod.Content.Projectiles.SpiritStaff
{
	public class SpiritStaffProjectile : ModProjectile
	{
        bool runOnce = true;
        Vector2 towardsMouse;
        private enum AIState {
            Idle,
            Spawning,
            Moving
        }

        private AIState CurrentAIState{
            get => (AIState)Projectile.ai[0];
            set => Projectile.ai[0] = (float)value;
        }
        public override void SetStaticDefaults() {
            Main.projFrames[Projectile.type] = 5;
		}
		public override void SetDefaults() {
			Projectile.width = 30;
			Projectile.height = 30;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.penetrate = 1; 
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
            Projectile.timeLeft = 100;
            Projectile.alpha = 126;
		}

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (Projectile.alpha > 1) Projectile.alpha -= 7;
            
            Vector2 circle = new Vector2(0f, 0f - 300f);
            float circleRotation = (float)Math.PI / 5 * Projectile.ai[2];
            
            if (Projectile.ai[2] > 9){
                circle.Y = -500f;
                circleRotation = (float)Math.PI / 8 * (Projectile.ai[2] - 10);
            }
            Vector2 circlePositionInWorld = circle.RotatedBy(circleRotation) / 3f + player.Center;

            switch (CurrentAIState){
                case AIState.Idle: {
                    Projectile.timeLeft = 5;
                    Projectile.frameCounter++;
                    if (Projectile.frameCounter >= 8) {
                        Projectile.frameCounter = 0;
                        if (++Projectile.frame >= 4)
                            Projectile.frame = 0;
                    }

                    Projectile.velocity *= 0;
                    Projectile.Center = Vector2.Lerp(Projectile.Center, circlePositionInWorld, 0.2f);
                    if (!player.channel){
                        Projectile.timeLeft = 210;
                        CurrentAIState = AIState.Spawning;
                    }

                    break;
                }
                case AIState.Spawning: {
                    Projectile.friendly = true;

                    Projectile.ai[1]++;

                    if (Projectile.ai[1] > 30) CurrentAIState = AIState.Moving;

                    Vector2 awayFromPlayer = Projectile.Center.DirectionTo(player.Center) * -1f;
                    awayFromPlayer.Normalize();
                    Projectile.velocity = awayFromPlayer * 8f;
                    Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
                    break;
                }
                case AIState.Moving: {
                    NPC closestNPC = FindClosestNPC(2000);
                    Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

                    Projectile.ai[1]++;

                    if (closestNPC != null){
                        Vector2 desiredVelocity = Projectile.DirectionTo(closestNPC.Center) * 17;
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 0.05f);
                    }
                    else {
                        if (runOnce){
                            towardsMouse = Projectile.DirectionTo(Main.MouseWorld) * 17;
                            runOnce = false;
                        } 
                            
                        Projectile.velocity = Vector2.Lerp(Projectile.velocity, towardsMouse, 0.05f);
                    }
                    break;
                }
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 3; i++){
				Dust.NewDust(Projectile.Center, 1, 1, DustID.GoldCoin, Projectile.velocity.X/2, Projectile.velocity.Y/2, 1, Color.LightBlue);
			}
            SoundEngine.PlaySound(SoundID.NPCDeath39, Projectile.position);
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
            if (CurrentAIState == AIState.Moving || CurrentAIState == AIState.Spawning){
                Projectile.frame = 4;
                Projectile.scale = 0.8f;
                Texture2D projectileTexture = ModContent.Request<Texture2D>("RappleMod/Content/Projectiles/SpiritStaff/SpiritStaffProjectileTrail").Value;
                Vector2 drawPosition = Projectile.position + new Vector2(Projectile.width, Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
                Vector2 drawOrigin = new Vector2(projectileTexture.Width, projectileTexture.Height) / 2f;
                Color drawColor = Projectile.GetAlpha(lightColor);
                drawColor.A = 127;
                drawColor *= 0.5f;

                if (Projectile.ai[2] >= 10){
                    drawColor.R = 235;
                    drawColor.G = 138;
                    drawColor.B = 138;
                    Projectile.scale = 1f;
                }

                SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

                for (float transparency = 1f; transparency >= 0f; transparency -= 0.125f) {
                    float opacity = 1f - transparency;
                    Vector2 drawAdjustment = Projectile.velocity * -3 * transparency;
                    Main.EntitySpriteDraw(projectileTexture, drawPosition + drawAdjustment, null, drawColor * opacity, Projectile.rotation, drawOrigin, Projectile.scale * 1.15f * MathHelper.Lerp(0.5f, 1f, opacity), spriteEffects, 0);
                }
            }

            return true;
        }

        public override Color? GetAlpha(Color lightColor) {
            Color color = Color.White * (1f-(Projectile.alpha/255f));
            if (Projectile.ai[2] >= 10 && Projectile.frame == 4){
                Color newColor = new(235, 138, 138);
                return newColor;
            }
            return color;
        }
    }
}
