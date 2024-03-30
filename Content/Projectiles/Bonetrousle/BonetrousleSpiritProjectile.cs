using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace RappleMod.Content.Projectiles.Bonetrousle
{
	public class BonetrousleSpiritProjectile : ModProjectile
	{
        public override void SetStaticDefaults() {
            Main.projFrames[Projectile.type] = 3;
		}
		public override void SetDefaults() {
			Projectile.width = 24;
			Projectile.height = 30;
			Projectile.friendly = true; 
			Projectile.DamageType = DamageClass.MeleeNoSpeed;
			Projectile.penetrate = 1; 
            Projectile.tileCollide = false;
            Projectile.scale = 0.8f;
		}

        public override void AI()
        {
            NPC closestNPC = FindClosestNPC(480);
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
            if (++Projectile.frameCounter >= 5) {
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= Main.projFrames[Projectile.type])
					Projectile.frame = 0;
			}  

            Projectile.ai[0]++;

            if (Projectile.ai[0] > 30){
                if (closestNPC != null){
                    Vector2 desiredVelocity = Projectile.DirectionTo(closestNPC.Center) * 17;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 0.05f);
                }
            }

            if ((Projectile.ai[0] < 30 || closestNPC == null) && Projectile.velocity.Length() > 6f && Projectile.ai[0] % 5 == 0){
                Projectile.velocity *= 0.9f;
            }

            if (Projectile.ai[0] > 600) Projectile.Kill();
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

        public override Color? GetAlpha(Color lightColor) => Color.White;
    }
}
