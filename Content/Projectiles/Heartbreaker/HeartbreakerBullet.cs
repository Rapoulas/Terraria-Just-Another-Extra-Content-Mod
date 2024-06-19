using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;


namespace RappleMod.Content.Projectiles.Heartbreaker
{   
    public class HeartbreakerBullet : ModProjectile
    {
        bool isHeartbeat = false;
        int heartbeatTimer = 0;
        int heartbeatProgress = 1;
        int direction = 1;
        
        public override void SetDefaults() {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.penetrate = 4;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = false;
            Projectile.timeLeft = 600;
            Projectile.alpha = 255;
            Projectile.scale = 1.25f;
            Projectile.light = 0.5f;
        }

        public override void AI(){
            if (Projectile.alpha > 0) Projectile.alpha -= 25;

            float rotation = 0;
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (isHeartbeat){
                if (heartbeatTimer == 0) heartbeatProgress = 1;
                else if (heartbeatTimer == 2) heartbeatProgress = 2;
                else if (heartbeatTimer == 8) heartbeatProgress = 3;
                else if (heartbeatTimer == 18) heartbeatProgress = 4;
                else if (heartbeatTimer == 26) heartbeatProgress = 5;
                else if (heartbeatTimer == 30) heartbeatProgress = 6;
                else if (heartbeatTimer == 34) heartbeatProgress = 7;

                if (heartbeatProgress == 1){
                    rotation = -70 * direction;
                    Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(rotation));
                    heartbeatProgress = -1;
                }
                else if (heartbeatProgress == 2){
                    rotation = 140 * direction;
                    Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(rotation));
                    heartbeatProgress = -1;
                }
                else if (heartbeatProgress == 3){
                    rotation = 210 * direction;
                    Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(rotation));
                    heartbeatProgress = -1;
                }
                else if (heartbeatProgress == 4){
                    rotation = 140 * direction;
                    Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(rotation));
                    heartbeatProgress = -1;
                }
                else if (heartbeatProgress == 5){
                    rotation = 210 * direction;
                    Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(rotation));
                    heartbeatProgress = -1;
                }
                else if (heartbeatProgress == 6){
                    rotation = 140 * direction;
                    Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(rotation));
                    heartbeatProgress = -1;
                }
                else if (heartbeatProgress == 7){
                    rotation = -50 * direction;
                    Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(rotation));
                    heartbeatProgress = -1;
                    isHeartbeat = false;
                    heartbeatTimer = -1;
                }
                
                heartbeatTimer++;
            }
            else Projectile.ai[0]++;

            if (Projectile.ai[0] % 60 == 0){
                    Projectile.ai[0] = 1;
                    isHeartbeat = true;
                    direction = Projectile.direction;
            }

            for (int i = 0; i < 10; i ++){
                Vector2 posOffset = Projectile.velocity.SafeNormalize(Vector2.One) * (3f * i);
                int dust = Dust.NewDust(Projectile.position, 1, 1, 60);
                
                Main.dust[dust].alpha = Projectile.alpha;
                Main.dust[dust].position += posOffset - 2f * Vector2.UnitY;
                Main.dust[dust].velocity *= 0f;
                Main.dust[dust].noGravity = true;
                Main.dust[dust].scale = 1.2f;
            }

            if (isHeartbeat) Projectile.extraUpdates = 1;
            else Projectile.extraUpdates = 0;
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

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (isHeartbeat){
                modifiers.FinalDamage *= 1.3f;
            }
        }

    }
}