using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using RappleMod.Content.Weapons;


namespace RappleMod.Content.Projectiles.HawkEye
{
    public class HawkEyeReticle : ModProjectile
    {
        
        public override void SetDefaults() {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = false;
        }

        public override void AI()
        {
            Player player = Main.LocalPlayer;

            if (!player.GetModPlayer<MyPlayer>().isHoldingHawkEye){
                Projectile.Kill();
                player.GetModPlayer<MyPlayer>().spawnedReticle = false;
            }

            Projectile.position = Main.MouseWorld;
        }
    }

    public class HawkEyeBullet : ModProjectile
    {
        public override string Texture => "RappleMod/Content/Projectiles/InvisibleProj";

        public override void SetDefaults() {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 2;
            Projectile.tileCollide = false;
        }
    }
}