using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace RappleMod.Content.Projectiles.PurgatoryProjectile
{
	public class PurgatoryProjectile : ModProjectile
	{
        bool runOnce = true;
        Vector2 towardsMouse;
		public override void SetDefaults() {
			Projectile.width = 30;
			Projectile.height = 30;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 120;
            Projectile.alpha = 126;
            Projectile.friendly = true;
		}

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            
            Player player = Main.player[Projectile.owner];
            float baseBonus = player.GetDamage(Projectile.DamageType).Base + player.GetDamage(DamageClass.Generic).Base;
            float addBonus = player.GetDamage(Projectile.DamageType).Additive + player.GetDamage(DamageClass.Generic).Additive - 1;
            float multBonus = player.GetDamage(Projectile.DamageType).Multiplicative + player.GetDamage(DamageClass.Generic).Multiplicative - 1;
            float flatBonus = player.GetDamage(Projectile.DamageType).Flat + player.GetDamage(DamageClass.Generic).Flat;
            modifiers.FinalDamage += baseBonus;
            modifiers.FinalDamage *= addBonus;
            modifiers.FinalDamage *= multBonus;
            modifiers.FinalDamage += flatBonus;

        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (Projectile.ai[1] == 1) Projectile.DamageType = DamageClass.Melee;
			else if (Projectile.ai[1] == 2) Projectile.DamageType = DamageClass.Magic;
			else if (Projectile.ai[1] == 3) Projectile.DamageType = DamageClass.Ranged;
			else if (Projectile.ai[1] == 4) Projectile.DamageType = DamageClass.Summon;

            //Used to test damage multipliers
            // float baseBonus = player.GetDamage(Projectile.DamageType).Base + player.GetDamage(DamageClass.Generic).Base;
            // float addBonus = player.GetDamage(Projectile.DamageType).Additive + player.GetDamage(DamageClass.Generic).Additive - 1;
            // float multBonus = player.GetDamage(Projectile.DamageType).Multiplicative + player.GetDamage(DamageClass.Generic).Multiplicative - 1;
            // float flatBonus = player.GetDamage(Projectile.DamageType).Flat + player.GetDamage(DamageClass.Generic).Flat;
            
            // Main.NewText(baseBonus + " , " + addBonus + " , " + multBonus + " , " + flatBonus);
            
            NPC closestNPC = FindClosestNPC(2000);
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;

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
            Projectile.frame = 10;
            Projectile.scale = 0.8f;
            Texture2D projectileTexture = ModContent.Request<Texture2D>("RappleMod/Content/Projectiles/PurgatoryProjectile/PurgatoryProjectileTrail").Value;
            Vector2 drawPosition = Projectile.position + new Vector2(Projectile.width, Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;
            Vector2 drawOrigin = new Vector2(projectileTexture.Width, projectileTexture.Height) / 2f;
            Color drawColor = Projectile.GetAlpha(lightColor);
            drawColor.A = 127;
            drawColor *= 0.5f;

            SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            for (float transparency = 1f; transparency >= 0f; transparency -= 0.125f) {
                float opacity = 1f - transparency;
                Vector2 drawAdjustment = Projectile.velocity * -3 * transparency;
                Main.EntitySpriteDraw(projectileTexture, drawPosition + drawAdjustment, null, drawColor * opacity, Projectile.rotation, drawOrigin, Projectile.scale * 1.15f * MathHelper.Lerp(0.5f, 1f, opacity), spriteEffects, 0);
            }

            return true;
        }

        public override Color? GetAlpha(Color lightColor) => Color.Red;
    }
}
