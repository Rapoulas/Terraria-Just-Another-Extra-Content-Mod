using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;


namespace RappleMod.Content.Projectiles.Ultrakill
{   
    public class UltrakillBullet : ModProjectile
    {
        int amountCoinsHit = 0;
        bool hitFirstCoin = false;
        bool noCoinLeft = false;
        bool alreadySpawnedText = false;
        
        public override void SetDefaults() {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = false;
            Projectile.extraUpdates = 10;
        }

        public override void AI(){
            Player player = Main.player[Projectile.owner];
            float maxEnemyHomeRange = 2000f;
            float maxCoinHomeRange = 4000f;

            Projectile.rotation = Projectile.velocity.ToRotation();

            for (int i=0; i < Main.maxProjectiles; i++){
                if (Main.projectile[i].type == ModContent.ProjectileType<UltrakillCoin>() && Main.projectile[i].active){
                    if (Main.projectile[i].Hitbox.Intersects(Projectile.Hitbox)){
                        amountCoinsHit++;
                        hitFirstCoin = true;
                        Projectile.tileCollide = false;

                        SoundStyle coinHit = new($"Terraria/Sounds/Item_35") {
                            Volume = 2f,
                            PitchVariance = 0.2f,
                            Pitch = 0.4f + (amountCoinsHit * 0.15f)
                        };
                        SoundEngine.PlaySound(coinHit, Projectile.Center);

                        Main.projectile[i].Kill();

                        Projectile closestCoin = FindClosestCoin(maxCoinHomeRange);
                        if (closestCoin != null){
                            Projectile.velocity = (closestCoin.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 3f * (1+ amountCoinsHit);
                        }
                        else {
                            NPC closestNPC = FindClosestNPC(maxEnemyHomeRange);
                            if (closestNPC != null){
                                Projectile.velocity = (closestNPC.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 6f;
                            }
                        }
                    }
                }
            }
            Projectile.ai[2]++;
            if (Projectile.ai[2] % 3 == 0 && hitFirstCoin){
                int bulletTrail = Dust.NewDust(Projectile.Center, 0, 0, DustID.Firework_Yellow, 0f, 0f, 0, Color.Transparent);
                Main.dust[bulletTrail].position = Projectile.Center;
                Main.dust[bulletTrail].scale = 1f;
                Main.dust[bulletTrail].noGravity = true;
            }
        }
        
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {   
            Player player = Main.player[Projectile.owner];
            noCoinLeft = player.ownedProjectileCounts[ModContent.ProjectileType<UltrakillCoin>()] == 0;

            if (amountCoinsHit == 4 && noCoinLeft && !alreadySpawnedText){
                CombatText.NewText(player.getRect(), Color.Gold, "ULTRAKILL!!!", true, false);
                alreadySpawnedText = true;
            }
            else if (amountCoinsHit == 3 && noCoinLeft && !alreadySpawnedText){
                CombatText.NewText(player.getRect(), Color.Red, "SUPREME!!", true, false);
                alreadySpawnedText = true;
            }
            else if (amountCoinsHit == 2 && noCoinLeft && !alreadySpawnedText){
                CombatText.NewText(player.getRect(), Color.LimeGreen, "CHAOTIC!", false, false);
                alreadySpawnedText = true;
            }
            else if (amountCoinsHit == 1 && noCoinLeft && !alreadySpawnedText){
                CombatText.NewText(player.getRect(), Color.Blue, "DESTRUCTIVE", false, false);
                alreadySpawnedText = true;
            }
            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Vector2 velocity = Projectile.velocity;

            Projectile.netUpdate = true;
            for (int i = 0; i < 1; i++) {
                Collision.HitTiles(Projectile.position, velocity, Projectile.width, Projectile.height);
            }

            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            Projectile.Kill();

            return false;
        }

        public Projectile FindClosestCoin(float maxDetectDistance) {
			Projectile closestCoin = null;

			float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

			for (int k = 0; k < Main.maxProjectiles; k++) {
				Projectile target = Main.projectile[k];
				
				if (target.type == ModContent.ProjectileType<UltrakillCoin>() && target.active) {
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

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D projectileTexture = ModContent.Request<Texture2D>("RappleMod/Content/Projectiles/Ultrakill/UltrakillBullet").Value;
            Vector2 drawOrigin = new Vector2(projectileTexture.Width, projectileTexture.Height);
            Vector2 drawPosition = Projectile.position + new Vector2(Projectile.width, Projectile.height) / 2f + Vector2.UnitY * Projectile.gfxOffY - Main.screenPosition;

            Main.EntitySpriteDraw(projectileTexture, drawPosition, null, Color.White, Projectile.rotation, drawOrigin, Projectile.scale, 0);
            return false;
        }
    }
}