using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Audio;
using Microsoft.Xna.Framework.Graphics;
using RappleMod.Content.Buffs;


namespace RappleMod.Content.Projectiles.Heartbreaker
{   
    public class HeartbreakerBullet : ModProjectile
    {
        bool isHeartbeat = false;
        int heartbeatTimer = 0;
        int heartbeatProgress = 1;
        int direction = 1;
        
        public override void SetDefaults() {
            Projectile.width = 10;
            Projectile.height = 6;
            Projectile.friendly = true;
            Projectile.penetrate = 4;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.usesLocalNPCImmunity = false;
            Projectile.timeLeft = 800;
            Projectile.alpha = 255;
            Projectile.scale = 1.25f;
            Projectile.light = 0.5f;
        }

        public override void AI(){
            Player player = Main.player[Projectile.owner];

            float currentHP = player.statLife;
            float maxHP = player.statLifeMax2;
            float hpPercent = 100/(maxHP/currentHP);
            int timerReducer = 80-(int)(hpPercent/1.25f);

            if (Projectile.alpha > 0) Projectile.alpha -= 25;

            float rotation = 0;
            Projectile.rotation = Projectile.velocity.ToRotation();

            if (isHeartbeat){
                Projectile.tileCollide = false;
                if (heartbeatTimer == 0) heartbeatProgress = 1;
                else if (heartbeatTimer == 2) heartbeatProgress = 2;
                else if (heartbeatTimer == 6) heartbeatProgress = 3;
                else if (heartbeatTimer == 13) heartbeatProgress = 4;
                else if (heartbeatTimer == 21) heartbeatProgress = 5;
                else if (heartbeatTimer == 25) heartbeatProgress = 6;
                else if (heartbeatTimer == 28) heartbeatProgress = 7;

                switch (heartbeatProgress){
                    case 1:
                        rotation = -70 * direction;
                        Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(rotation));
                        heartbeatProgress = -1;
                        break;
                    case 2:
                        rotation = 140 * direction;
                        Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(rotation));
                        heartbeatProgress = -1;
                        break;
                    case 3:
                        rotation = 210 * direction;
                        Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(rotation));
                        heartbeatProgress = -1;
                        break;
                    case 4:
                        rotation = 140 * direction;
                        Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(rotation));
                        heartbeatProgress = -1;
                        break;
                    case 5:
                        rotation = 210 * direction;
                        Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(rotation));
                        heartbeatProgress = -1;
                        break;
                    case 6:
                        rotation = 140 * direction;
                        Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(rotation));
                        heartbeatProgress = -1;
                        break;
                    case 7:
                        rotation = -50 * direction;
                        Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(rotation));
                        heartbeatProgress = -1;
                        isHeartbeat = false;
                        heartbeatTimer = -1;
                        break;
                }
                heartbeatTimer++;
            }
            else {
                Projectile.tileCollide = true;
                Projectile.ai[0]++;
            }

            if (Projectile.ai[0] % (55-MathHelper.Min(timerReducer, 52)) == 0){
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

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (isHeartbeat && heartbeatTimer > 6 && heartbeatTimer < 13){
                Rectangle hitbox = new((int)Projectile.position.X, (int)Projectile.position.Y, Projectile.width+20, Projectile.height+20);
                if (hitbox.Intersects(targetHitbox)) return true;
            }
            else if (isHeartbeat && heartbeatTimer >= 13 && heartbeatTimer < 21){
                Rectangle hitbox = new((int)Projectile.position.X-30, (int)Projectile.position.Y-30, Projectile.width+40, Projectile.height+40);
                if (hitbox.Intersects(targetHitbox)) return true;
            }

            return base.Colliding(projHitbox, targetHitbox);
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

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (isHeartbeat) {
                Projectile.penetrate += 1;
                target.AddBuff(ModContent.BuffType<HeartbrokenDebuff>(), 300);
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (isHeartbeat) target.AddBuff(ModContent.BuffType<HeartbrokenDebuff>(), 300);
        }

    }
}