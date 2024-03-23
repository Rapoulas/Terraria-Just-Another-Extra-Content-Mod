using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RappleMod.Content.Projectiles.ProminenceRevolt
{
	public class ProminenceRevoltProjectileAlt : ModProjectile
	{
		public override void SetDefaults() {
			Projectile.penetrate = 1;
			Projectile.timeLeft = 40;
			Projectile.width = 18;
			Projectile.height = 18;
			Projectile.damage = 25;
			Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = false;
        }

		private const int GravityDelay = 25;

		public int GravityDelayTimer {
			get => (int)Projectile.ai[2];
			set => Projectile.ai[2] = value;
		}

		public override void AI(){
			Projectile.velocity.Normalize();
			Projectile.velocity *= 20f;
			GravityDelayTimer++;

			if (GravityDelayTimer >= GravityDelay){
				GravityDelayTimer = GravityDelay;

				Projectile.velocity.Y += 0.25f;
			}

			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90f);
		}

		public override void OnKill(int timeLeft){
			Player player = Main.player[Projectile.owner];

			if (timeLeft == 0){
				int a = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y , 0, -1, 654, Projectile.damage, Projectile.knockBack, player.whoAmI);
				Main.projectile[a].friendly = true;
				Main.projectile[a].hostile = false;
				int b = Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y , 0, -1, 953, 0, Projectile.knockBack, player.whoAmI);
				Main.projectile[b].friendly = true;
				Main.projectile[b].hostile = false;
			}		
		}

		public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
			Player player = Main.player[Projectile.owner];

			int a = Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center.X, target.Center.Y , 0, -1, 654, Projectile.damage, Projectile.knockBack, player.whoAmI);
			Main.projectile[a].friendly = true;
			Main.projectile[a].hostile = false;
			int b = Projectile.NewProjectile(Projectile.GetSource_FromThis(), target.Center.X, target.Center.Y , 0, -1, 953, 0, Projectile.knockBack, player.whoAmI);
			Main.projectile[b].friendly = true;
			Main.projectile[b].hostile = false;
			if (Main.rand.NextBool(2)) {
				target.AddBuff(BuffID.OnFire, 300);
			}
		}

		 public override bool OnTileCollide(Vector2 oldVelocity){
			Player player = Main.player[Projectile.owner];

			Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y , 0, -1, 654, Projectile.damage, Projectile.knockBack, player.whoAmI);
			Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y , 0, -1, 953, 0, Projectile.knockBack, player.whoAmI);
			Projectile.Kill();
			return false;
		}
    }
}