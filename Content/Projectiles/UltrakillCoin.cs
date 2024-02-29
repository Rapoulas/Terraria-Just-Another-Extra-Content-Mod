using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;


namespace TutorialMod.Content.Projectiles
{
    public class UltrakillCoin : ModProjectile
    {
        public override void SetStaticDefaults() {
			Main.projFrames[Projectile.type] = 8;
		}
        public override void SetDefaults() {
            Projectile.width = 15;
            Projectile.height = 15;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = false;
            Projectile.timeLeft = 3000;
            Projectile.extraUpdates = 10;
        }

        public override void AI()
        {
            
            if (Projectile.ai[0] >= 100 && Projectile.ai[0] < 300){
                if (Projectile.ai[0] % 10 == 0){
                    Projectile.velocity *= 1-(Projectile.ai[0]/1500);
                }
            }
            if (Projectile.ai[0] >= 430){
                Projectile.velocity *= 0;
            }

            Projectile.ai[0]++;

            if (++Projectile.frameCounter >= 20) {
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= Main.projFrames[Projectile.type])
					Projectile.frame = 0;
			}
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Vector2 velocity = Projectile.velocity;

            if (oldVelocity.X != Projectile.velocity.X) {
				Projectile.velocity.X = (0f - oldVelocity.X) * 0.5f;
			}

			if (oldVelocity.Y != Projectile.velocity.Y) {
				Projectile.velocity.Y = (0f - oldVelocity.Y) * 0.5f;
			}

            for (int i = 0; i < 2; i++) {
				Collision.HitTiles(Projectile.position, velocity, Projectile.width, Projectile.height);
			}
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            
            return false;
        }
    }
}