using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;


namespace RappleMod.Content.Projectiles.Explosiverang
{
    public class ExplosiverangProjectile : ModProjectile
    {
        
        public override void SetDefaults() {
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = false;
        }

        private bool returnToPlayer;

        public override void OnHitNPC (NPC target, NPC.HitInfo hit, int damageDone) {
            Player player = Main.player[Projectile.owner];
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, 0, 0, ModContent.ProjectileType<ExplosiverangExplosion>(), Projectile.damage, Projectile.knockBack, player.whoAmI);
            Projectile.localNPCImmunity[target.whoAmI] = -1;
            target.immune[Projectile.owner] = 0;
		}

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Player player = Main.player[Projectile.owner];
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, 0, 0, ModContent.ProjectileType<ExplosiverangExplosion>(), Projectile.damage, Projectile.knockBack, player.whoAmI);
            returnToPlayer = true;
            return false;
        }

        public override void AI(){
            Player player = Main.player[Projectile.owner];

            Projectile.ai[0]++;
            if (Projectile.ai[0] < 20) { }
            else if (Projectile.ai[0] < 30){
                Projectile.velocity *= 0.85f;
            }
            else if (Projectile.ai[0] < 40){
                Projectile.velocity += Projectile.Center.DirectionTo(player.Center) * 1.5f;
                returnToPlayer = true;
            }
            else{
                Projectile.velocity = Projectile.Center.DirectionTo(player.Center) * 10f;
            }

            if (returnToPlayer){
                Projectile.velocity = Projectile.Center.DirectionTo(player.Center) * 10f;
                Projectile.tileCollide = false;
            }
            Projectile.rotation += 0.4f * Projectile.direction;

            if (Collision.CheckAABBvAABBCollision(player.position, player.Size, Projectile.position, Projectile.Size) && Projectile.ai[0] >= 10f){
                Projectile.Kill();
            }

            if (Projectile.ai[0] > 450) Projectile.Kill();
        }
    }

    public class ExplosiverangExplosion : ModProjectile
    {
        public override string Texture => "RappleMod/Content/Projectiles/InvisibleProj";
        readonly double radius = 50f;
        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 2;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override void AI()
        {   
            Player player = Main.player[Projectile.owner];

            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            for (int g = 0; g < 1; g++) {
				var goreSpawnPosition = new Vector2(Projectile.position.X + Projectile.width / 2 - 24f, Projectile.position.Y + Projectile.height / 2 - 24f);
				Gore gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64), 1f);
				gore.scale = 0.8f;
				gore.velocity.X += 1.5f;
				gore.velocity.Y += 1.5f;
				gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64), 1f);
				gore.scale = 0.8f;
				gore.velocity.X -= 1.5f;
				gore.velocity.Y += 1.5f;
				gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64), 1f);
				gore.scale = 0.8f;
				gore.velocity.X += 1.5f;
				gore.velocity.Y -= 1.5f;
				gore = Gore.NewGoreDirect(Projectile.GetSource_FromThis(), goreSpawnPosition, default, Main.rand.Next(61, 64), 1f);
				gore.scale = 0.8f;
				gore.velocity.X -= 1.5f;
				gore.velocity.Y -= 1.5f;
			}
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Rectangle center = new((int)Projectile.Center.X, (int)Projectile.Center.Y, 1, 1);
            if (center.Intersects(targetHitbox)) 
                return true;

            float topLeftDistance = Vector2.Distance(Projectile.Center, targetHitbox.TopLeft());
            float topRightDistance = Vector2.Distance(Projectile.Center, targetHitbox.TopRight());
            float bottomLeftDistance = Vector2.Distance(Projectile.Center, targetHitbox.BottomLeft());
            float bottomRightDistance = Vector2.Distance(Projectile.Center, targetHitbox.BottomRight());

            float distanceToClosestPoint = topLeftDistance;
            if (topRightDistance < distanceToClosestPoint)
                distanceToClosestPoint = topRightDistance;
            if (bottomLeftDistance < distanceToClosestPoint)
                distanceToClosestPoint = bottomLeftDistance;
            if (bottomRightDistance < distanceToClosestPoint)
                distanceToClosestPoint = bottomRightDistance;

            return distanceToClosestPoint <= radius;
        }
    }
}