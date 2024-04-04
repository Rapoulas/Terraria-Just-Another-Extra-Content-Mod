using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace RappleMod.Content.Projectiles.DryadWardCopy
{
	public class DryadWardCopy : ModProjectile
	{
        double radius;
        float timer;
        NPC npc;
        public override void SetStaticDefaults() {
            Main.projFrames[Projectile.type] = 5;
		}

		public override void SetDefaults() {
			Projectile.width = 22;
			Projectile.height = 14;
            Projectile.damage = 100;
            Projectile.friendly = true;
			Projectile.DamageType = DamageClass.Default;
			Projectile.penetrate = -1; 
            Projectile.tileCollide = false;
            Projectile.localNPCHitCooldown = 90;
            Projectile.usesLocalNPCImmunity = true;
		}
        public override void AI()
        {
            if (++Projectile.frameCounter >= 8) {
				Projectile.frameCounter = 0;
				if (++Projectile.frame >= Main.projFrames[Projectile.type])
					Projectile.frame = 0;
			} 

            Player player = Main.player[Projectile.owner];
            
            if (Projectile.ai[2] == 0) 
                for (int i = 0; i < Main.maxNPCs; i++){
                    if (Main.npc[i].whoAmI == Projectile.ai[1]){
                        npc = Main.npc[i];
                        Projectile.Center = npc.Center;
                    }
                }
            else if (Projectile.ai[2] == 1) Projectile.Center = player.Center;

            if (!player.active || player.dead || player.noItems || player.CCed){
                Projectile.Kill();
                return;
		    }
            if (Projectile.ai[2] == 0 && !npc.active){
                Projectile.Kill();
                return;
            }
            Projectile.ai[0] += 1f;

            if (Projectile.localNPCHitCooldown > 45)
                Projectile.localNPCHitCooldown = Math.Min((int)MathHelper.Lerp(90, 45, (timer-400)/300), 90);

            Projectile.rotation += (float)Math.PI / Math.Max(300f - (timer/2f), 35f);
            Projectile.scale = Projectile.ai[0] / 100f;

            if (Projectile.scale > 1f)
            {
                Projectile.scale = 1f;
            }
            Projectile.alpha = (int)(255f * (1f - Projectile.scale));
            float distanceLeafPlayer = 32f;
            if (Projectile.ai[0] >= 100f)
            {
                distanceLeafPlayer = MathHelper.Lerp(32f, 208f, (Projectile.ai[0] - 100f) / 300f);
            }
            if (distanceLeafPlayer > 208f)
            {
                distanceLeafPlayer = 208f;
            }
            if (distanceLeafPlayer == 208f && Projectile.ai[0] <= 500f && player.GetModPlayer<SetBonusChangesPlayer>().MeleeHCSetReapply){
                Projectile.ai[0] = 400f;
                player.GetModPlayer<SetBonusChangesPlayer>().MeleeHCSetReapply = false;
            }
            if (Projectile.ai[0] >= 500f)
            {
                Projectile.alpha = (int)MathHelper.Lerp(0f, 255f, (Projectile.ai[0] - 500f) / 100f);
                distanceLeafPlayer = MathHelper.Lerp(208f, 288f, (Projectile.ai[0] - 500f) / 100f);
            }
            int num2 = 163;
            
            if (Main.rand.NextBool(2))
            {
                int dust = 0;
                Vector2 vector2 = new Vector2(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11));
                float num7 = Main.rand.Next(3, 9);
                vector2.Normalize();

                if (Projectile.ai[2] == 0){
                    dust = Dust.NewDust(new Vector2(npc.position.X, npc.position.Y), npc.width, npc.height, num2, 0f, 0f, 100, default(Color), 1.5f);
                    Main.dust[dust].position = npc.Center + vector2 * 30f;
                }
                else if (Projectile.ai[2] == 1){
                    dust = Dust.NewDust(new Vector2(player.position.X, player.position.Y), player.width, player.height, num2, 0f, 0f, 100, default(Color), 1.5f);
                    Main.dust[dust].position = player.Center + vector2 * 30f;
                }
                Main.dust[dust].noGravity = true;
                if (Main.rand.NextBool(8))
                {
                    Main.dust[dust].velocity = vector2 * (0f - num7) * 3f;
                    Main.dust[dust].scale += 0.5f;
                }
                else
                {
                    Main.dust[dust].velocity = vector2 * (0f - num7);
                }
            }
            if (Projectile.ai[0] >= 30f && Main.netMode != NetmodeID.Server)
            {
                if (player.active && !player.dead && player.Distance(Projectile.Center) <= distanceLeafPlayer && player.FindBuffIndex(165) == -1)
                {
                    player.AddBuff(165, 120);
                }
            }
            if (Projectile.ai[0] >= 30f && Projectile.ai[0] % 10f == 0f && Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < 200; i++)
                {
                    NPC nPC = Main.npc[i];
                    if (nPC.type != NPCID.TargetDummy && nPC.active && player.Distance(nPC.Center) <= distanceLeafPlayer)
                    {
                        if (nPC.townNPC && (nPC.FindBuffIndex(165) == -1 || nPC.buffTime[nPC.FindBuffIndex(165)] <= 20))
                        {
                            nPC.AddBuff(165, 120);
                        }
                        else if (!nPC.friendly && nPC.lifeMax > 5 && !nPC.dontTakeDamage && (nPC.FindBuffIndex(186) == -1 || nPC.buffTime[nPC.FindBuffIndex(186)] <= 20) && (nPC.dryadBane || Collision.CanHit(player.Center, 1, 1, nPC.position, nPC.width, nPC.height)))
                        {
                            nPC.AddBuff(186, 120);
                        }
                    }
                }
            }
            radius = distanceLeafPlayer;

            if (Projectile.ai[0] >= 570f)
            {
                Projectile.Kill();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Player player = Main.LocalPlayer;
            timer++;
            float distanceLeafPlayer = 100f;
				if (Projectile.ai[0] >= 100f)
				{
					distanceLeafPlayer = MathHelper.Lerp(100f, 600f, (Projectile.ai[0] - 100f) / 300f);
				}
				if (distanceLeafPlayer > 600f)
				{
					distanceLeafPlayer = 600f;
				}
				if (Projectile.ai[0] >= 500f)
				{
					distanceLeafPlayer = MathHelper.Lerp(600f, 900f, (Projectile.ai[0] - 500f) / 100f);
				}
                
                if (distanceLeafPlayer == 600f && player.GetModPlayer<SetBonusChangesPlayer>().MeleeHCSetReapply){
                    Projectile.ai[0] = 400f;
                    player.GetModPlayer<SetBonusChangesPlayer>().MeleeHCSetReapply = false;
                }
				float rotation = Projectile.rotation;
				Texture2D Texture = TextureAssets.Projectile[Projectile.type].Value;
				int frameCounter = (int)(timer / 6f);
				Vector2 leafCircle = new Vector2(0f, 0f - distanceLeafPlayer);
				for (int i = 0; (float)i < 10f; i++)
				{
					Rectangle textureFrame = Texture.Frame(1, 5, 0, (frameCounter + i) % 5);
					float leafRotation = rotation + (float)Math.PI / 5f * (float)i;
					Vector2 leafPositionInWorld = leafCircle.RotatedBy(leafRotation) / 3f + Projectile.Center;
					Color color = Projectile.GetAlpha(Lighting.GetColor(leafPositionInWorld.ToTileCoordinates()));
					color.A /= 2;
					Main.EntitySpriteDraw(Texture, leafPositionInWorld - Main.screenPosition, textureFrame, color, leafRotation, textureFrame.Size() / 2f, Projectile.scale, SpriteEffects.None);
                }
				

            return false;
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
