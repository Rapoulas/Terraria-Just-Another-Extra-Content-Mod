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
		 public static float MaxReach = 800;

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

		public override bool PreAI() => false;

        public override void PostAI(){
			if (Main.player[Projectile.owner].dead || Main.player[Projectile.owner].stoned || Main.player[Projectile.owner].webbed || Main.player[Projectile.owner].frozen){
				Projectile.Kill();
				return;
			}

			if (Projectile.velocity == Vector2.Zero && Main.player[Projectile.owner].controlJump){
				Projectile.Kill();
				return;
			}
			
			Vector2 BetweenOwner = Owner.Center - Projectile.Center;

			if (State == HookState.Thrown){
                //Retract if too far.
                if (MaxReach < BetweenOwner.Length())
                    State = HookState.Retracting;
				
                CheckTile();
            }
			else if (State == HookState.Retracting){
                Projectile.velocity = BetweenOwner.SafeNormalize(Vector2.One) * 20f;
                Projectile.Center += Vector2.UnitY * 0.5f;

                if (BetweenOwner.Length() < 25f)
                    Projectile.Kill();
            }
			else{
                float LengthToOwner = BetweenOwner.Length();

                if (LengthToOwner > (Owner.Center - Projectile.Center).Length() + 60f)
                    Projectile.Kill();
                

                Point tilePos = Projectile.Center.ToTileCoordinates();
                Tile tile = Main.tile[tilePos];
                if (!tile.HasUnactuatedTile || !CanTileBeLatchedOnTo(tile) || Owner.IsBlacklistedForGrappling(tilePos))
                    State = HookState.Retracting;

                Projectile.velocity = Vector2.Zero;

				float playerSpeed = (float)Math.Sqrt(Owner.velocity.X * Owner.velocity.X + Owner.velocity.Y * Owner.velocity.Y);
				Vector2 directionToHook = Owner.DirectionTo(Projectile.Center);
				Owner.velocity = Vector2.Lerp(Owner.velocity, new(directionToHook.X * 10f, directionToHook.Y * 10f), 0.05f);
				
			}

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

	public class VerletSimulatedSegment
    {
        public Vector2 position, oldPosition;
        public bool locked;

        public VerletSimulatedSegment(Vector2 _position, bool _locked = false)
        {
            position = _position;
            oldPosition = _position;
            locked = _locked;
        }

        public static List<VerletSimulatedSegment> SimpleSimulation(List<VerletSimulatedSegment> segments, float segmentDistance, int loops = 10, float gravity = 0.3f)
        {
            //https://youtu.be/PGk0rnyTa1U?t=400 verlet integration chains reference here
            foreach (VerletSimulatedSegment segment in segments)
            {
                if (!segment.locked)
                {
                    Vector2 positionBeforeUpdate = segment.position;

                    segment.position += (segment.position - segment.oldPosition); // This adds conservation of energy to the segments. This makes it super bouncy and shouldnt be used but it's really funny
                    segment.position += Vector2.UnitY * gravity; //=> This adds gravity to the segments. 

                    segment.oldPosition = positionBeforeUpdate;
                }
            }

            int segmentCount = segments.Count;

            for (int k = 0; k < loops; k++)
            {
                for (int j = 0; j < segmentCount - 1; j++)
                {
                    VerletSimulatedSegment pointA = segments[j];
                    VerletSimulatedSegment pointB = segments[j + 1];
                    Vector2 segmentCenter = (pointA.position + pointB.position) / 2f;
                    Vector2 segmentDirection = Terraria.Utils.SafeNormalize(pointA.position - pointB.position, Vector2.UnitY);

                    if (!pointA.locked)
                        pointA.position = segmentCenter + segmentDirection * segmentDistance / 2f;

                    if (!pointB.locked)
                        pointB.position = segmentCenter - segmentDirection * segmentDistance / 2f;

                    segments[j] = pointA;
                    segments[j + 1] = pointB;
                }
            }

            return segments;
        }
    }
}
