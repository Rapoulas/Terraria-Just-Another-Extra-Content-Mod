using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;


namespace TutorialMod.Content.Projectiles
{
    public class UltrakillBullet : ModProjectile
    {
        int amountCoinsHit = 0;
        public override void SetDefaults() {
            Projectile.width = 15;
            Projectile.height = 15;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 5;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = false;
            Projectile.extraUpdates = 10;
        }

        public override void AI(){
            float maxEnemyHomeRange = 800f;
            float maxCoinHomeRange = 1000f;

            for (int i=0; i < Main.maxProjectiles; i++){
                if (Main.projectile[i].type == 1302 && Main.projectile[i].active){
                    if (Main.projectile[i].Hitbox.Intersects(Projectile.Hitbox)){
                        amountCoinsHit++;
                        Projectile.tileCollide = false;
                        Main.projectile[i].Kill();
                        Projectile closestCoin = FindClosestCoin(maxCoinHomeRange);
                        if (closestCoin != null){
                            Projectile.velocity = (closestCoin.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 3f * amountCoinsHit;
			                Projectile.rotation = Projectile.velocity.ToRotation();
                        }
                        else {
                            NPC closestNPC = FindClosestNPC(maxEnemyHomeRange);
                            if (closestNPC != null){
                                Projectile.velocity = (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 7f;
                                Projectile.rotation = Projectile.velocity.ToRotation();
                            }
                        }
                    }
                }
            }
        }

        public Projectile FindClosestCoin(float maxDetectDistance) {
			Projectile closestCoin = null;

			float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

			for (int k = 0; k < Main.maxProjectiles; k++) {
				Projectile target = Main.projectile[k];
				
				if (target.type == 1302 && target.active) {
					float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Projectile.Center);

					if (sqrDistanceToTarget < sqrMaxDetectDistance) {
						sqrMaxDetectDistance = sqrDistanceToTarget;
						closestCoin = target;
					}
				}
			}

			return closestCoin;
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

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (amountCoinsHit > 0){
                modifiers.FinalDamage *= 1 + amountCoinsHit*amountCoinsHit/2.67f;
            }
            base.ModifyHitNPC(target, ref modifiers);
        }
    }
}