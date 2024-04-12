using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;


namespace RappleMod.Content.Projectiles.Ouroboros
{
    public class OuroborosProjectile : ModProjectile
    {
		public bool runOnce = true;
		public bool facingDirection;
		Player player = Main.LocalPlayer;
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.Flames}";

		public override void SetStaticDefaults() {
            Main.projFrames[Projectile.type] = 7;
		}
        
        public override void SetDefaults() {
            Projectile.width = 6;
            Projectile.height = 6;
			Projectile.ArmorPenetration = 15;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 15;
            Projectile.DamageType = DamageClass.Ranged;
			Projectile.extraUpdates = 2;
            Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1;
        }

        public override void AI(){
			if (++Projectile.frameCounter >= 17) {
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= Main.projFrames[Projectile.type])
					Projectile.frame = 0;
			}

            Projectile.localAI[0] += 1f;
			int num = 124;
			int num2 = 12;
			int num3 = num + num2;
			if (Projectile.localAI[0] >= (float)num3)
				Projectile.Kill();
			
			if (Projectile.localAI[0] >= (float)num)
				Projectile.velocity *= 0.95f;
			
			if (runOnce && player.direction == -1){
				facingDirection = true;
				runOnce = false;
			}
			else if (runOnce && player.direction == 1){
				facingDirection = false;
				runOnce = false;
			}

			if (facingDirection)
				Projectile.rotation += 0.0007f;
			else if (!facingDirection)
				Projectile.rotation -= 0.0007f;
			Projectile.velocity = Projectile.velocity.RotatedBy(Projectile.rotation);

			if (Projectile.localAI[0] < 50f && Main.rand.NextFloat() < 0.25f){
				Dust dust = Dust.NewDustDirect(Projectile.Center + Main.rand.NextVector2Circular(60f, 60f) * Utils.Remap(Projectile.localAI[0], 0f, 72f, 0.5f, 1f), 4, 4, DustID.Torch, Projectile.velocity.X * 0.2f, Projectile.velocity.Y * 0.2f, 100);
				if (Main.rand.NextBool(4)){
					dust.noGravity = true;
					dust.scale *= 3f;
					dust.velocity.X *= 2f;
					dust.velocity.Y *= 2f;
				}
				else{
					dust.scale *= 1.5f;
				}
				dust.scale *= 1.5f;
				dust.velocity *= 1.2f;
				dust.velocity += Projectile.velocity * 1f * Utils.Remap(Projectile.localAI[0], 0f, (float)num * 0.75f, 1f, 0.1f) * Utils.Remap(Projectile.localAI[0], 0f, (float)num * 0.1f, 0.1f, 1f);
				dust.customData = 1;
			}
			if (Projectile.localAI[0] >= 50f && Main.rand.NextFloat() < 0.5f){
				Vector2 center = Main.player[Projectile.owner].Center;
				Vector2 vector = (Projectile.Center - center).SafeNormalize(Vector2.Zero).RotatedByRandom(0.19634954631328583) * 7f;
				Dust dust2 = Dust.NewDustDirect(Projectile.Center + Main.rand.NextVector2Circular(50f, 50f) - vector * 2f, 4, 4, DustID.Smoke, 0f, 0f, 150, new Color(80, 80, 80));
				dust2.noGravity = true;
				dust2.velocity = vector;
				dust2.scale *= 1.1f + Main.rand.NextFloat() * 0.2f;
				dust2.customData = -0.3f - 0.15f * Main.rand.NextFloat();
			}

			if (Projectile.wet && !Projectile.lavaWet)
				Projectile.Kill();
        }

        public override bool PreDraw(ref Color lightColor){
			float num = 124f;
			float num10 = 12f;
			float fromMax = num + num10;
			Texture2D value = TextureAssets.Projectile[Projectile.type].Value;
			_ = Color.Transparent;
			Color color = new Color(255, 80, 20, 200);
			Color color2 = new Color(255, 95, 8, 70);
			Color color3 = Color.Lerp(new Color(255, 80, 20, 100), color2, 0.25f);
			Color color4 = new Color(80, 80, 80, 100);
			float num11 = 0.35f;
			float num12 = 0.7f;
			float num13 = 0.85f;
			float num14 = ((Projectile.localAI[0] > num - 10f) ? 0.175f : 0.2f);
			int verticalFrames = 7;
			float num15 = Utils.Remap(Projectile.localAI[0], num, fromMax, 1f, 0f);
			float num2 = Math.Min(Projectile.localAI[0], 20f);
			float num3 = Utils.Remap(Projectile.localAI[0], 0f, fromMax, 0f, 1f);
			float num4 = Utils.Remap(num3, 0.2f, 0.5f, 0.25f, 1f);
			num4 *= MathHelper.Min(1f + player.GetModPlayer<MyPlayer>().amountEnemiesOnFire/10f, 2.5f);
			Rectangle rectangle = value.Frame(1, verticalFrames, 0, 3);
			if (!(num3 < 1f)){
				return false;
			}
			for (int i = 0; i < 2; i++){
				for (float num5 = 1f; num5 >= 0f; num5 -= num14){
					Color obj = ((num3 < 0.1f) ? Color.Lerp(Color.Transparent, color, Utils.GetLerpValue(0f, 0.1f, num3, clamped: true)) : ((num3 < 0.2f) ? Color.Lerp(color, color2, Utils.GetLerpValue(0.1f, 0.2f, num3, clamped: true)) : ((num3 < num11) ? color2 : ((num3 < num12) ? Color.Lerp(color2, color3, Utils.GetLerpValue(num11, num12, num3, clamped: true)) : ((num3 < num13) ? Color.Lerp(color3, color4, Utils.GetLerpValue(num12, num13, num3, clamped: true)) : ((!(num3 < 1f)) ? Color.Transparent : Color.Lerp(color4, Color.Transparent, Utils.GetLerpValue(num13, 1f, num3, clamped: true))))))));
					float num6 = (1f - num5) * Utils.Remap(num3, 0f, 0.2f, 0f, 1f);
					Vector2 vector = Projectile.Center - Main.screenPosition + Projectile.velocity * (0f - num2) * num5;
					Color color5 = obj * num6;
					Color color6 = color5;

					color6.G /= 2;
					color6.B /= 2;
					color6.A = (byte)Math.Min((float)(int)color5.A + 80f * num6, 255f);
					Utils.Remap(Projectile.localAI[0], 20f, fromMax, 0f, 1f);
					
					float num7 = 1f / num14 * (num5 + 1f);
					float num8 = Projectile.rotation + num5 * ((float)Math.PI / 2f) + Main.GlobalTimeWrappedHourly * num7 * 2f;
					float num9 = Projectile.rotation - num5 * ((float)Math.PI / 2f) - Main.GlobalTimeWrappedHourly * num7 * 2f;
					switch (i){
					case 0:
						Main.EntitySpriteDraw(value, vector + Projectile.velocity * (0f - num2) * num14 * 0.5f, rectangle, color6 * num15 * 0.25f, num8 + (float)Math.PI / 4f, rectangle.Size() / 2f, num4, SpriteEffects.None);
						Main.EntitySpriteDraw(value, vector, rectangle, color6 * num15, num9, rectangle.Size() / 2f, num4, SpriteEffects.None);
						break;
					case 1:
						Main.EntitySpriteDraw(value, vector + Projectile.velocity * (0f - num2) * num14 * 0.2f, rectangle, color5 * num15 * 0.25f, num8 + (float)Math.PI / 2f, rectangle.Size() / 2f, num4 * 0.75f, SpriteEffects.None);
						Main.EntitySpriteDraw(value, vector, rectangle, color5 * num15, num9 + (float)Math.PI / 2f, rectangle.Size() / 2f, num4 * 0.75f, SpriteEffects.None);
						break;
					}
				}
			}

            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity){
			Projectile.velocity = oldVelocity * 0.95f;
			Projectile.position -= Projectile.velocity;
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox){
			float x = MathHelper.Min(1f + player.GetModPlayer<MyPlayer>().amountEnemiesOnFire/10f, 2.5f);
			Rectangle result = new Rectangle((int)Projectile.position.X, (int)Projectile.position.Y, Projectile.width, Projectile.height);
			int num = (int)Utils.Remap(Projectile.localAI[0], 0f, 72f, 10f, 40f);
			result.Inflate(num, num);
			result.Width *= (int)x;
			result.Height *= (int)x;

            if (!result.Intersects(targetHitbox))
				return false;
				
			return Collision.CanHit(Projectile.Center, 0, 0, targetHitbox.Center.ToVector2(), 0, 0);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone){
            target.AddBuff(BuffID.OnFire, 240);
			target.AddBuff(BuffID.OnFire3, 120);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
			modifiers.FinalDamage *= MathHelper.Min(1f + player.GetModPlayer<MyPlayer>().amountEnemiesOnFire/60f, 1.25f);
            base.ModifyHitNPC(target, ref modifiers);
        }
    }

}