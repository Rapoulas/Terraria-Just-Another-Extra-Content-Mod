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
            Projectile.width = 64;
            Projectile.height = 64;
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
            }

            Projectile.Center = Main.MouseWorld;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
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
            Projectile.DamageType = DamageClass.Ranged; 
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            Rectangle center = new((int)target.Center.X, (int)target.Center.Y, 1, 1);
            if (Projectile.Hitbox.Intersects(center)) modifiers.SetCrit();
            else modifiers.DisableCrit();
        }

         public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {   
            Player player = Main.player[Projectile.owner];

            if (Main.rand.NextBool(500))
                CombatText.NewText(player.getRect(), Color.Gold, "TRICKSHOT!!", true, false);
        }
    }
}