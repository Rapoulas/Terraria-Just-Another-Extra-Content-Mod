using System;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using TutorialMod.Content.Weapons;
using XPT.Core.Audio.MP3Sharp.Decoding.Decoders.LayerIII;


namespace TutorialMod.Content.Projectiles
{
    public class UltrakillBullet : ModProjectile
    {
        int amountCoinsHit = 0;
        bool hitFirstCoin = false;
        bool noMoreCoinLeft = false;
        bool alreadySpawnedText;

        public override void SetDefaults() {
            Projectile.width = 15;
            Projectile.height = 15;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = false;
            Projectile.extraUpdates = 10;
        }

        public override void AI(){
            float maxEnemyHomeRange = 2000f;
            float maxCoinHomeRange = 4000f;

            Projectile.rotation = Projectile.velocity.ToRotation();

            for (int i=0; i < Main.maxProjectiles; i++){
                if (Main.projectile[i].type == 1302 && Main.projectile[i].active){
                    if (Main.projectile[i].Hitbox.Intersects(Projectile.Hitbox)){
                        amountCoinsHit++;
                        hitFirstCoin = true;
                        Projectile.tileCollide = false;

                        Main.projectile[i].Kill();
                        for (int j = 0; j < 6; j++){
                            Dust.NewDust(Projectile.Center, 0, 0, DustID.TreasureSparkle, 0f, 0f, 0, Color.Transparent);
                        }
                        SoundStyle coinHit = new($"Terraria/Sounds/Item_35") {
                            Volume = 2f,
                            PitchVariance = 0.2f,
                            Pitch = 0.9f
                        };
                        SoundEngine.PlaySound(coinHit, Projectile.Center);

                        Projectile closestCoin = FindClosestCoin(maxCoinHomeRange);
                        if (closestCoin != null){
                            Projectile.velocity = (closestCoin.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 3f * (1+ amountCoinsHit/2);
			                Projectile.rotation = Projectile.velocity.ToRotation();
                        }
                        else {
                            NPC closestNPC = FindClosestNPC(maxEnemyHomeRange);
                            if (closestNPC != null){
                                Projectile.velocity = (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 5f;
                                Projectile.rotation = Projectile.velocity.ToRotation();
                            }
                        }
                    }
                }
            }
            Projectile.ai[2]++;
            if (Projectile.ai[2] % 3 == 0 && hitFirstCoin){
                Projectile.ai[1]++;

                
                    int bulletTrail = Dust.NewDust(Projectile.Center, 0, 0, DustID.Firework_Yellow, 0f, 0f, 0, Color.Transparent);
                    Main.dust[bulletTrail].position = Projectile.Center;
                    Main.dust[bulletTrail].velocity = Projectile.velocity;
                    Main.dust[bulletTrail].scale = 1f;
                    Main.dust[bulletTrail].noGravity = true;
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {   
            Player player = Main.player[Projectile.owner];
            noMoreCoinLeft = player.ownedProjectileCounts[ModContent.ProjectileType<UltrakillCoin>()] == 0;

            if (amountCoinsHit == 4 && noMoreCoinLeft && !alreadySpawnedText){
                CombatText.NewText(player.getRect(), Color.Gold, "ULTRAKILL!!!", true, false);
                alreadySpawnedText = true;
            }
            else if (amountCoinsHit == 3 && noMoreCoinLeft && !alreadySpawnedText){
                CombatText.NewText(player.getRect(), Color.Red, "SUPREME!!", true, false);
                alreadySpawnedText = true;
            }
            else if (amountCoinsHit == 2 && noMoreCoinLeft && !alreadySpawnedText){
                CombatText.NewText(player.getRect(), Color.LimeGreen, "CHAOTIC!", false, false);
                alreadySpawnedText = true;
            }
            else if (amountCoinsHit == 1 && noMoreCoinLeft && !alreadySpawnedText){
                CombatText.NewText(player.getRect(), Color.Blue, "DESTRUCTIVE", false, false);
                alreadySpawnedText = true;
            }
            base.OnHitNPC(target, hit, damageDone);
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