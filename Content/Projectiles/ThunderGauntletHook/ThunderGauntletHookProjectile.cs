using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace RappleMod.Content.Projectiles.ThunderGauntletHook
{
	public class ThunderGauntletHookProjectile : ModProjectile
	{
		private static Asset<Texture2D> chainTexture;

		public override void Load() {
			chainTexture = ModContent.Request<Texture2D>("RappleMod/Content/Projectiles/ThunderGauntletHook/ThunderGauntletHookChain");
		}

		public override void Unload() {
            chainTexture = null;
		}

		public override void SetStaticDefaults() {
			ProjectileID.Sets.SingleGrappleHook[Type] = true;
		}

		public override void SetDefaults() {
			Projectile.netImportant = true;
			Projectile.width = 18;
			Projectile.height = 18;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.timeLeft *= 10;
		}

        public override void AI(){
			if (Main.player[Projectile.owner].dead || Main.player[Projectile.owner].stoned || Main.player[Projectile.owner].webbed || Main.player[Projectile.owner].frozen){
				Projectile.Kill();
				return;
			}

			if (Projectile.velocity == Vector2.Zero && Main.player[Projectile.owner].controlJump){
				Projectile.Kill();
				return;
			}
			Vector2 mountedCenter = Main.player[Projectile.owner].MountedCenter;
			Vector2 vector = new(Projectile.position.X + (float)Projectile.width * 0.5f, Projectile.position.Y + (float)Projectile.height * 0.5f);
			float num = mountedCenter.X - vector.X;
			float num12 = mountedCenter.Y - vector.Y;
			float num13 = (float)Math.Sqrt(num * num + num12 * num12);
			Projectile.rotation = (float)Math.Atan2(num12, num) - 1.57f;

			if (Main.myPlayer == Projectile.owner){
				int num16 = (int)(Projectile.Center.X / 16f);
				int num17 = (int)(Projectile.Center.Y / 16f);
				if (num16 > 0 && num17 > 0 && num16 < Main.maxTilesX && num17 < Main.maxTilesY && Main.tile[num16, num17].HasUnactuatedTile && TileID.Sets.CrackedBricks[Main.tile[num16, num17].TileType] && Main.rand.Next(16) == 0){
					WorldGen.KillTile(num16, num17);
					if (Main.netMode != NetmodeID.SinglePlayer){
						NetMessage.SendData(17, -1, -1, null, 20, num16, num17);
					}
				}
			}

			if (num13 > 2500f) Projectile.Kill();
		
			if (Projectile.ai[0] == 0f){
				int GrappleRange = 1000;
				if (num13 > (float)GrappleRange)
					Projectile.ai[0] = 1f;
				
				Vector2 vector3 = Projectile.Center - new Vector2(5f);
				Vector2 vector5 = Projectile.Center + new Vector2(5f);
				Point point = (vector3 - new Vector2(16f)).ToTileCoordinates();
				Point point4 = (vector5 + new Vector2(32f)).ToTileCoordinates();
				int num2 = point.X;
				int num3 = point4.X;
				int num4 = point.Y;
				int num5 = point4.Y;
				if (num2 < 0)
					num2 = 0;
				
				if (num3 > Main.maxTilesX)
					num3 = Main.maxTilesX;
				
				if (num4 < 0)
					num4 = 0;
				
				if (num5 > Main.maxTilesY)
					num5 = Main.maxTilesY;
				
				Player player = Main.player[Projectile.owner];
				List<Point> list = new List<Point>();
				for (int i = 0; i < player.grapCount; i++){
					Projectile projectile = Main.projectile[player.grappling[i]];
					if (projectile.type != ModContent.ProjectileType<ThunderGauntletHookProjectile>() || projectile.ai[0] != 2f)
						continue;
					
					Point pt = projectile.Center.ToTileCoordinates();
					Tile tileSafely = Framing.GetTileSafely(pt);
					if (tileSafely.TileType != 314 && !TileID.Sets.Platforms[tileSafely.TileType])
					{
						continue;
					}
					for (int j = -2; j <= 2; j++)
					{
						for (int k = -2; k <= 2; k++)
						{
							Point point2 = new Point(pt.X + j, pt.Y + k);
							Tile tileSafely2 = Framing.GetTileSafely(point2);
							if (tileSafely2.TileType == 314 || TileID.Sets.Platforms[tileSafely2.TileType])
							{
								list.Add(point2);
							}
						}
					}
				}
				Vector2 vector4 = default(Vector2);
				for (int l = num2; l < num3; l++){
					for (int m = num4; m < num5; m++){
						// if (Main.tile[l, m] == null){
						// 	Main.tile[l, m] = default(Tile);
						// }
						
						vector4.X = l * 16;
						vector4.Y = m * 16;
						if (!(vector3.X + 10f > vector4.X) || !(vector3.X < vector4.X + 16f) || !(vector3.Y + 10f > vector4.Y) || !(vector3.Y < vector4.Y + 16f))
							continue;
						
						Tile tile = Main.tile[l, m];
						if (!AI_007_GrapplingHooks_CanTileBeLatchedOnTo(l, m) || list.Contains(new Point(l, m)) || Main.player[Projectile.owner].IsBlacklistedForGrappling(new Point(l, m)))
							continue;
						
						if (Main.player[Projectile.owner].grapCount < 10){
							Main.player[Projectile.owner].grappling[Main.player[Projectile.owner].grapCount] = Projectile.whoAmI;
							Main.player[Projectile.owner].grapCount++;
						}
						if (Main.myPlayer != Projectile.owner)
							continue;
						
						int num6 = 0;
						int num7 = -1;
						int num8 = 100000;
						int num9 = 3;
						ProjectileLoader.NumGrappleHooks(Projectile, Main.player[Projectile.owner], ref num9);
						for (int num10 = 0; num10 < 1000; num10++){
							if (Main.projectile[num10].active && Main.projectile[num10].owner == Projectile.owner && Main.projectile[num10].type == ModContent.ProjectileType<ThunderGauntletHookProjectile>()){
								if (Main.projectile[num10].timeLeft < num8){
									num7 = num10;
									num8 = Main.projectile[num10].timeLeft;
								}
								num6++;
							}
						}
						if (num6 > num9)
							Main.projectile[num7].Kill();
						
						
						WorldGen.KillTile(l, m, fail: true, effectOnly: true);
						SoundEngine.PlaySound(SoundID.Dig, new Vector2(l * 16, m * 16));
						Projectile.velocity.X = 0f;
						Projectile.velocity.Y = 0f;
						Projectile.ai[0] = 2f;
						Projectile.position.X = l * 16 + 8 - Projectile.width / 2;
						Projectile.position.Y = m * 16 + 8 - Projectile.height / 2;
						Rectangle? tileVisualHitbox = WorldGen.GetTileVisualHitbox(l, m);
						if (tileVisualHitbox.HasValue)
							Projectile.Center = tileVisualHitbox.Value.Center.ToVector2();
						
						Projectile.damage = 0;
						Projectile.netUpdate = true;
						if (Main.myPlayer == Projectile.owner)
							NetMessage.SendData(13, -1, -1, null, Projectile.owner);
						
						break;
					}
					if (Projectile.ai[0] == 2f)
						break;
					
				}
			}
			else if (Projectile.ai[0] == 1f){
				float num11 = 30f;
				
				ProjectileLoader.GrappleRetreatSpeed(Projectile, Main.player[Projectile.owner], ref num11);
				if (num13 < 24f)
					Projectile.Kill();
				
				num13 = num11 / num13;
				num *= num13;
				num12 *= num13;
				Projectile.velocity.X = num;
				Projectile.velocity.Y = num12;
			}
			else if (Projectile.ai[0] == 2f){
				Point point3 = Projectile.Center.ToTileCoordinates();
				// if (Main.tile[point3.X, point3.Y] == null)
				// 	Main.tile[point3.X, point3.Y] = default(Tile);
				
				if (!AI_007_GrapplingHooks_CanTileBeLatchedOnTo(point3.X, point3.Y))
					Projectile.ai[0] = 1f;
				
				else if (Main.player[Projectile.owner].grapCount < 10){
					Main.player[Projectile.owner].grappling[Main.player[Projectile.owner].grapCount] = Projectile.whoAmI;
					Main.player[Projectile.owner].grapCount++;
				}
			}
        }

        private bool AI_007_GrapplingHooks_CanTileBeLatchedOnTo(int x, int y){
			Tile theTile = Main.tile[x, y];
			bool vanilla = Main.tileSolid[theTile.TileType] | (theTile.TileType == 314);
			vanilla &= theTile.HasUnactuatedTile;
			return ProjectileLoader.GrappleCanLatchOnTo(Projectile, Main.player[Projectile.owner], x, y) ?? vanilla;
		}
        public override bool PreDrawExtras() {
			Vector2 playerCenter = Main.player[Projectile.owner].MountedCenter;
			Vector2 center = Projectile.Center;
			Vector2 directionToPlayer = playerCenter - Projectile.Center;
			float chainRotation = directionToPlayer.ToRotation() - MathHelper.PiOver2;
			float distanceToPlayer = directionToPlayer.Length();

			while (distanceToPlayer > 20f && !float.IsNaN(distanceToPlayer)) {
				directionToPlayer /= distanceToPlayer;
				directionToPlayer *= chainTexture.Height();

				center += directionToPlayer;
				directionToPlayer = playerCenter - center;
				distanceToPlayer = directionToPlayer.Length();

				Color drawColor = Lighting.GetColor((int)center.X / 16, (int)(center.Y / 16));

				Main.EntitySpriteDraw(chainTexture.Value, center - Main.screenPosition,
					chainTexture.Value.Bounds, drawColor, chainRotation,
					chainTexture.Size() * 0.5f, 1f, SpriteEffects.None, 0);
			}
			return false;
		}
	}
}
