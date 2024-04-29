using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;


namespace RappleMod.Content.Projectiles.Deliverance
{
    public class DeliveranceProjectile : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Item_{ItemID.Shotgun}";
        public override void SetDefaults() {
            Projectile.width = 40;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 600;
        }

        public override void AI(){
            Player player = Main.player[Projectile.owner];
            Projectile.ai[0]++;

            float rotation = 0f;
            if (Projectile.spriteDirection == -1)
                rotation = MathHelper.Pi;

            Projectile.rotation = Projectile.velocity.ToRotation() + rotation;
            Projectile.spriteDirection = Projectile.direction;

            if (Projectile.ai[0] > 20){
                NPC closestNPC = FindClosestNPC(2000);
                
                if (closestNPC != null){
                    Vector2 desiredVelocity = Projectile.DirectionTo(closestNPC.Center) * 5f;
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 0.05f);
                }

                if (Projectile.ai[0] % 30 == 0){
                    for (int i = 0; i < 6; ++i){
                        int type = (int)Projectile.ai[1];
                        Vector2 newVelocity1 = new(Main.rand.Next(-50, 51) * 0.025f, Main.rand.Next(-50, 51) * 0.025f);
                        Vector2 newVelocity2 = new(Projectile.velocity.X + newVelocity1.X, Projectile.velocity.Y + newVelocity1.Y);
                        newVelocity2.Normalize();
                        newVelocity2 *= 15f;

                        Vector2 shootOffset = Projectile.velocity;
                        shootOffset.Normalize();
                        shootOffset *= 10;

                        Item ammo = player.ChooseAmmo(player.HeldItem);

                        float baseBonus = player.GetDamage(DamageClass.Ranged).Base + player.GetDamage(DamageClass.Generic).Base;
                        float addBonus = player.GetDamage(DamageClass.Ranged).Additive + player.GetDamage(DamageClass.Generic).Additive - 1;
                        float multBonus = player.GetDamage(DamageClass.Ranged).Multiplicative + player.GetDamage(DamageClass.Generic).Multiplicative - 1;
                        float flatBonus = player.GetDamage(DamageClass.Ranged).Flat + player.GetDamage(DamageClass.Generic).Flat;

                        float damage = ((ammo.damage + baseBonus) * addBonus * multBonus) + flatBonus;
                        
                        Projectile.NewProjectile(player.GetSource_FromThis(), Projectile.Center + shootOffset, newVelocity2, type, Projectile.damage + (int)damage, Projectile.knockBack, player.whoAmI);
                        
                        SoundEngine.PlaySound(SoundID.Item36, Projectile.Center);
                    }
                }
            }
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

        public override void OnKill(int timeLeft)
        {
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

            Player player = Main.player[Projectile.owner];
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center.X, Projectile.Center.Y, 0, 0, ModContent.ProjectileType<DeliveranceExplosion>(), Projectile.damage*2, Projectile.knockBack, player.whoAmI);
        }
    }

    public class DeliveranceExplosion : ModProjectile
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