using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;


namespace RappleMod.Content.Projectiles
{
    public class NecroFossilBone : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.Bone}";
        public override void SetDefaults() {
            Projectile.aiStyle = ProjAIStyleID.ThrownProjectile;
            AIType = ProjectileID.Bone;
            Projectile.damage = 10;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
        }

        public override bool? CanHitNPC(NPC target)
        {
			if (target.whoAmI == Projectile.ai[1]){
				return false;
			}
            return null;
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
		
			Vector2 velocity = Projectile.velocity;

            Projectile.netUpdate = true;
            for (int i = 0; i < 1; i++) {
                Collision.HitTiles(Projectile.position, velocity, Projectile.width, Projectile.height);
            }

            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            Projectile.Kill();

			return false;
		}
    }
}
