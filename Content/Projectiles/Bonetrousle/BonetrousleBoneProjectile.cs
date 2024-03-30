using System;
using System.Threading;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace RappleMod.Content.Projectiles.Bonetrousle
{
	public class BonetrousleBoneProjectile : ModProjectile
	{
        public override string Texture => "Terraria/Images/Projectile_532";
		public override void SetDefaults() {
			Projectile.width = 16;
			Projectile.height = 16;
			Projectile.friendly = true; 
			Projectile.DamageType = DamageClass.MeleeNoSpeed;
			Projectile.penetrate = 3; 
            Projectile.tileCollide = true;
		}

        public override void AI()
        {
            Projectile.rotation += 0.4f * Projectile.direction;

			Projectile.ai[1]++;
			if (Projectile.ai[1] > 250) Projectile.Kill();

			Projectile.ai[0]++;
			Projectile.velocity *= 0.9f;
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 6; i++){
				Dust.NewDust(Projectile.Center, 1, 1, DustID.Bone);
			}
			SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
			Vector2 velocity = Projectile.velocity;

            Projectile.netUpdate = true;
            for (int i = 0; i < 1; i++) {
                Collision.HitTiles(Projectile.position, velocity, Projectile.width, Projectile.height);
            }

            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            return base.OnTileCollide(oldVelocity);
        }
    }
}
