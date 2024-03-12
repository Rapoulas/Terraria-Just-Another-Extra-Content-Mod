using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;


namespace RappleMod.Content.Projectiles.Kylie
{
    public class KylieProjectile : ModProjectile
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

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.ai[0] >= 35 && Projectile.ai[0] <= 70){
                modifiers.SetCrit();
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Vector2 velocity = Projectile.velocity;

            Projectile.netUpdate = true;
            for (int i = 0; i < 1; i++) {
                Collision.HitTiles(Projectile.position, velocity, Projectile.width, Projectile.height);
            }

            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);

            returnToPlayer = true;
            return false;
        }

        public override void AI(){
            Player player = Main.player[Projectile.owner];
            
            Projectile.ai[0]++;
            if (Projectile.ai[0] < 20) { }
            else if (Projectile.ai[0] < 40){
                Projectile.velocity *= 0.85f;
            }
            else if (Projectile.ai[0] >= 40 && Projectile.ai[0] <= 70){
                Projectile.velocity *= 0;
                Dust.NewDust(Projectile.Center, 5, 5, DustID.TreasureSparkle, 0f, 0f, 0, Color.Transparent);
            }
            else returnToPlayer = true;

            if (returnToPlayer){
                Projectile.velocity = Projectile.Center.DirectionTo(player.Center) * 10f;
                Projectile.tileCollide = false;
            }
            Projectile.rotation += 0.4f * Projectile.direction;

            if (Collision.CheckAABBvAABBCollision(player.position, player.Size, Projectile.position, Projectile.Size) && Projectile.ai[0] >= 10f){
                Projectile.Kill();
            }
        }
    }
}