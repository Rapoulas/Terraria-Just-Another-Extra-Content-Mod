using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RappleMod.Content.Weapons;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace RappleMod.Content.Projectiles.ThunderGauntlet
{
	public class ThunderGauntletHookProjectile : ModProjectile
	{
		public HookState State
        {
            get => (HookState)(int)Projectile.ai[0];
            set { Projectile.ai[0] = (int)value; }
        }

        public ref float Timer => ref Projectile.ai[1];

        public enum HookState
        {
            Thrown,
            Retracting,
            Grappling = 3 //Making this value "3" is important here, as it makes it so that i can put this projectile in the player grapple list while also never having it considered as "grappling" (aka ai[0] = 2)
        }
		private static Asset<Texture2D> chainTexture;
		public Player Owner => Main.player[Projectile.owner];
		readonly float MaxReach = 800;

		public override void Load() {
			chainTexture = ModContent.Request<Texture2D>("RappleMod/Content/Projectiles/ThunderGauntlet/ThunderGauntletHookChain");
		}

		public override void Unload() {
            chainTexture = null;
		}

		public override void SetStaticDefaults() {
			ProjectileID.Sets.SingleGrappleHook[Type] = true;
		}

		public override void SetDefaults() {
			Projectile.netImportant = true;
			Projectile.width = 26;
			Projectile.height = 30;
			Projectile.friendly = true;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.timeLeft *= 10;
            Projectile.extraUpdates = 3;
		}

		public override bool PreAI() => false;

        public override void PostAI(){
            Vector2 BetweenOwner = Owner.Center - Projectile.Center;

            if (Main.player[Projectile.owner].dead || Main.player[Projectile.owner].stoned || Main.player[Projectile.owner].webbed || Main.player[Projectile.owner].frozen){
                Projectile.Kill();
                return;
            }
            if (Projectile.velocity == Vector2.Zero && Main.player[Projectile.owner].controlJump){
                Projectile.Kill();
                return;
            }
            if (State == HookState.Grappling && (GauntletGrapple.GrappleKeybind.JustReleased || !GauntletGrapple.GrappleKeybind.Current)){
                Projectile.Kill();
                return;
            }
            if (State == HookState.Retracting && BetweenOwner.Length() < 25f){      
                Projectile.Kill();
                return;
            }
            
            if (Timer % 4 == 0){
                Projectile.rotation = BetweenOwner.ToRotation() - MathHelper.PiOver2;

                if (State == HookState.Thrown){
                    //Retract if too far.
                    if (MaxReach < BetweenOwner.Length())
                        State = HookState.Retracting;
                    
                    CheckTile();
                }
                else if (State == HookState.Retracting){
                    Projectile.velocity = BetweenOwner.SafeNormalize(Vector2.One) * 11.25f;
                    Projectile.Center += Vector2.UnitY * 0.5f;
                }
                else{
                    Point tilePos = Projectile.Center.ToTileCoordinates();
                    Tile tile = Main.tile[tilePos];
                    if (!tile.HasUnactuatedTile || !CanTileBeLatchedOnTo(tile) || Owner.IsBlacklistedForGrappling(tilePos))
                        State = HookState.Retracting;

                    Projectile.velocity = Vector2.Zero;

                    float playerSpeed = (float)Math.Sqrt(Owner.velocity.X * Owner.velocity.X + Owner.velocity.Y * Owner.velocity.Y);
                    playerSpeed *= 1.7f;
                    if (Math.Abs(Owner.velocity.X) >= Math.Abs(Owner.velocity.Y))
                        Owner.velocity.X *= 1.02f;
                    else
                        Owner.velocity.Y *= 1.02f;
                    
                    Vector2 directionToHook = Owner.DirectionTo(Projectile.Center);
                    Owner.velocity = Vector2.Lerp(Owner.velocity, new(directionToHook.X * playerSpeed, directionToHook.Y * playerSpeed), 0.055f + (Vector2.Distance(Owner.Center, Projectile.Center) * 0.000025f));
                    
                    if (MaxReach < BetweenOwner.Length()){
                        Owner.velocity = Owner.Center.DirectionTo(Projectile.Center) * 4f;
                    }
                }
            }

            if (State == HookState.Thrown)
                CheckTile();
            
			Timer++;
        }

		public void CheckTile(){
            Vector2 hitboxStart = Projectile.Center - new Vector2(5f);
            Vector2 hitboxEnd = Projectile.Center + new Vector2(5f);

            Point topLeftTile = (hitboxStart - new Vector2(16f)).ToTileCoordinates();
            Point bottomRightTile = (hitboxEnd + new Vector2(32f)).ToTileCoordinates();

            for (int x = topLeftTile.X; x < bottomRightTile.X; x++){
                for (int y = topLeftTile.Y; y < bottomRightTile.Y; y++){
                    Vector2 worldPos = new Vector2(x * 16f, y * 16f);
                    Point tilePos = new Point(x, y);

                    //Ignore tiles that arent being collided with
                    if (!(hitboxStart.X + 10f > worldPos.X) || !(hitboxStart.X < worldPos.X + 16f) || !(hitboxStart.Y + 10f > worldPos.Y) || !(hitboxStart.Y < worldPos.Y + 16f))
                        continue;

                    Tile tile = Main.tile[tilePos];

                    if (!tile.HasUnactuatedTile || !CanTileBeLatchedOnTo(tile) || Owner.IsBlacklistedForGrappling(tilePos))
                        continue;
                    if (Main.myPlayer != Owner.whoAmI)
                        continue;

                    OnGrapple(worldPos, x, y);
                    
                    break;
                }

                if (State == HookState.Grappling)
                    break;
            }
        }

		public static bool CanTileBeLatchedOnTo(Tile theTile, bool grappleOnTrees = false) => Main.tileSolid[theTile.TileType] | (theTile.TileType == 314) | (grappleOnTrees && TileID.Sets.IsATreeTrunk[theTile.TileType]) | (grappleOnTrees && theTile.TileType == 323);

		public void OnGrapple(Vector2 grapplePos, int x, int y){
            //Hook onto the tile
            Projectile.velocity = Vector2.Zero;
            State = HookState.Grappling;
            Projectile.Center = grapplePos + Vector2.One * 8f;
            //effects
            WorldGen.KillTile(x, y, fail: true, effectOnly: true);
            SoundEngine.PlaySound(SoundID.Dig, grapplePos);

            Rectangle? tileVisualHitbox = WorldGen.GetTileVisualHitbox(x, y);
            if (tileVisualHitbox.HasValue)
                Projectile.Center = tileVisualHitbox.Value.Center.ToVector2();

            Projectile.netUpdate = true;
            NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, Owner.whoAmI);
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
