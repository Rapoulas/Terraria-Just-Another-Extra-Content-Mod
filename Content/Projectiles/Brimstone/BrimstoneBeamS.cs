using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Shaders;
using Terraria.Graphics.Effects;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.Utils;

namespace RappleMod.Content.Projectiles.Brimstone
{
	public class BrimstoneBeamS : ModProjectile
	{
		public override string Texture => "RappleMod/Content/Projectiles/InvisibleProj";
        private const float MaxBeamSize = 1f;
        private const float MaxBeamLength = 2400f;
        private const float BeamHitboxCollisionWidth = 20f;
        private const int NumSamplePoints = 3;
        private const float BeamLengthChangeFactor = 1f;
        private const float OuterBeamOpacityMultiplier = 0.60f;
		private const float InnerBeamOpacityMultiplier = 0.1f;
        private const float BeamLightBrightness = 0.75f;
        public int timer = 0;

        private float HostBeamIndex {
			get => Projectile.ai[1];
			set => Projectile.ai[1] = value;
		}

        private float BeamLength {
			get => Projectile.localAI[1];
			set => Projectile.localAI[1] = value;
		}

        public override void SetDefaults() {
			Projectile.width = 10;
			Projectile.height = 10;
			Projectile.DamageType = DamageClass.Magic;
			Projectile.penetrate = -1;
			Projectile.alpha = 255;
            Projectile.friendly = true;
			Projectile.tileCollide = false;

			// Using local NPC immunity allows each beam to strike independently from one another.
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = 5;
		}

        public override void SendExtraAI(BinaryWriter writer) => writer.Write(BeamLength);
		public override void ReceiveExtraAI(BinaryReader reader) => BeamLength = reader.ReadSingle();

        public override void AI() {
			Projectile hostBeam = Main.projectile[(int)HostBeamIndex];
            Player player = Main.LocalPlayer;
			if (Projectile.type != ModContent.ProjectileType<BrimstoneBeamS>() || !hostBeam.active || hostBeam.type != ModContent.ProjectileType<BrimstoneHoldOut>()) {
                Projectile.Kill();
				return;
			}
			
            float chargeRatio = MathHelper.Clamp(((float)timer-30)/60, 0f, 1f);
			Vector2 hostBeamDir = Vector2.Normalize(hostBeam.velocity);
            timer++;
			
			if (timer > 30) {
				Projectile.scale = MathHelper.Lerp(MaxBeamSize, 0f, EaseIn(chargeRatio));
                if (Projectile.scale <= 0f) Projectile.Kill();
			}

			Projectile.Center = hostBeam.Center;
			Projectile.velocity = hostBeamDir;

			if (Projectile.velocity.HasNaNs() || Projectile.velocity == Vector2.Zero) {
				Projectile.velocity = -Vector2.UnitY;
			}
			Projectile.rotation = Projectile.velocity.ToRotation();

			float hitscanBeamLength = PerformBeamHitscan(hostBeam);
			BeamLength = MathHelper.Lerp(BeamLength, hitscanBeamLength, BeamLengthChangeFactor);

			Vector2 beamDims = new Vector2(Projectile.velocity.Length() * BeamLength, Projectile.width * Projectile.scale);

			Color beamColor = Color.Red;
			
				ProduceBeamDust(beamColor);

				if (Main.netMode != NetmodeID.Server) {
					ProduceWaterRipples(beamDims);
				}
			

			DelegateMethods.v3_1 = beamColor.ToVector3() * BeamLightBrightness * chargeRatio;
			Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity * BeamLength, beamDims.Y, new Utils.TileActionAttempt(DelegateMethods.CastLight));
		}

        private float PerformBeamHitscan(Projectile prism) {
			Vector2 samplingPoint = prism.Center;

			Player player = Main.player[Projectile.owner];
			if (!Collision.CanHitLine(player.Center, 0, 0, prism.Center, 0, 0)) {
				samplingPoint = player.Center;
			}

			float[] laserScanResults = new float[NumSamplePoints];
			Collision.LaserScan(samplingPoint, Projectile.velocity, 0 * Projectile.scale, MaxBeamLength, laserScanResults);
			float averageLengthSample = 0f;
			for (int i = 0; i < laserScanResults.Length; ++i) {
				averageLengthSample += laserScanResults[i];
			}
			averageLengthSample /= NumSamplePoints;

			return averageLengthSample;
		}

        private void ProduceBeamDust(Color beamColor) {
			const int type = 15;
			Vector2 endPosition = Projectile.Center + Projectile.velocity * (BeamLength - 14.5f * Projectile.scale);

            float angle = Projectile.rotation + (Main.rand.NextBool() ? 1f : -1f) * MathHelper.PiOver2;
			float startDistance = Main.rand.NextFloat(1f, 1.8f);
			float scale = Main.rand.NextFloat(0.7f, 1.1f);
			Vector2 velocity = angle.ToRotationVector2() * startDistance;
			Dust dust = Dust.NewDustDirect(endPosition, 0, 0, type, velocity.X, velocity.Y, 0, beamColor, scale);
			dust.color = beamColor;
			dust.noGravity = true;

			if (Projectile.scale > 0f) {
				dust.velocity *= Projectile.scale;
				dust.scale *= Projectile.scale;
			}
		}

        private void ProduceWaterRipples(Vector2 beamDims) {
			WaterShaderData shaderData = (WaterShaderData)Filters.Scene["WaterDistortion"].GetShader();

			// A universal time-based sinusoid which updates extremely rapidly. GlobalTime is 0 to 3600, measured in seconds.
			float waveSine = 0.1f * (float)Math.Sin(Main.GlobalTimeWrappedHourly * 20f);
			Vector2 ripplePos = Projectile.position + new Vector2(beamDims.X * 0.5f, 0f).RotatedBy(Projectile.rotation);

			// WaveData is encoded as a Color. Not really sure why.
			Color waveData = new Color(0.5f, 0.1f * Math.Sign(waveSine) + 0.5f, 0f, 1f) * Math.Abs(waveSine);
			shaderData.QueueRipple(ripplePos, waveData, beamDims, RippleShape.Square, Projectile.rotation);
		}
        

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
			if (projHitbox.Intersects(targetHitbox)) {
				return true;
			}

			float _ = float.NaN;
			Vector2 beamEndPos = Projectile.Center + Projectile.velocity * BeamLength;
			return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, beamEndPos, BeamHitboxCollisionWidth * Projectile.scale, ref _);
		}

        public override bool PreDraw(ref Color lightColor) {
			if (Projectile.velocity == Vector2.Zero) {
				return false;
			}
			Texture2D brimstoneStartTexture = ModContent.Request<Texture2D>("RappleMod/Content/Projectiles/Brimstone/BrimstoneBeamSstart", AssetRequestMode.ImmediateLoad).Value;
            Texture2D brimstoneBodyTexture = ModContent.Request<Texture2D>("RappleMod/Content/Projectiles/Brimstone/BrimstoneBeamSmid", AssetRequestMode.ImmediateLoad).Value;
            Texture2D brimstoneEndTexture = ModContent.Request<Texture2D>("RappleMod/Content/Projectiles/Brimstone/BrimstoneBeamSend", AssetRequestMode.ImmediateLoad).Value;
            
			Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
			Vector2 centerFloored = Projectile.Center.Floor() + Projectile.velocity * Projectile.scale * 10.5f;
			Vector2 drawScale = new Vector2(Projectile.scale);

			float visualBeamLength = BeamLength - 15f * Projectile.scale * Projectile.scale;

			DelegateMethods.f_1 = 1f;
			Vector2 startPosition = centerFloored - Main.screenPosition;
			Vector2 endPosition = startPosition + Projectile.velocity * visualBeamLength;
			
			Utils.LaserLineFraming lineFraming = new Utils.LaserLineFraming(BrimstoneLaserDraw);

			DelegateMethods.c_1 = Color.Red;
			DrawBeamStart(Main.spriteBatch, brimstoneStartTexture, startPosition, endPosition, drawScale, Color.Red * OuterBeamOpacityMultiplier * Projectile.Opacity, lineFraming, timer);
			DrawBeamBody(Main.spriteBatch, brimstoneBodyTexture, startPosition, endPosition, drawScale, Color.Red * OuterBeamOpacityMultiplier * Projectile.Opacity, lineFraming, timer);
			DrawBeamEnd(Main.spriteBatch, brimstoneEndTexture, startPosition, endPosition, drawScale, Color.Red * OuterBeamOpacityMultiplier * Projectile.Opacity, lineFraming, timer);
			
			drawScale *= 0.5f;
			DrawBeamStart(Main.spriteBatch, brimstoneStartTexture, startPosition, endPosition, drawScale, Color.Red * InnerBeamOpacityMultiplier * Projectile.Opacity, lineFraming, timer);
			DrawBeamBody(Main.spriteBatch, brimstoneBodyTexture, startPosition, endPosition, drawScale, Color.Red * InnerBeamOpacityMultiplier * Projectile.Opacity, lineFraming, timer);
			DrawBeamEnd(Main.spriteBatch, brimstoneEndTexture, startPosition, endPosition, drawScale, Color.Red * InnerBeamOpacityMultiplier * Projectile.Opacity, lineFraming, timer);
			
			return false;
		}

		public static void BrimstoneLaserDraw(int stage, Vector2 currentPosition, float distanceLeft, Rectangle lastFrame, out float distCovered, out Rectangle frame, out Vector2 origin, out Color color)
		{
			color = Color.Red;
			switch (stage)
			{
				case 0:
					distCovered = 33f;
					frame = new Rectangle(0, 0, 26, 22);
					origin = frame.Size() / 2f;
					break;
				case 1:
					frame = new Rectangle(0, 0, 26, 30);
					distCovered = frame.Height;
					origin = new Vector2(frame.Width / 2, 0f);
					break;
				case 2:
					distCovered = 22f;
					frame = new Rectangle(0, 0, 26, 30);
					origin = new Vector2(frame.Width / 2, 1f);
					break;
				default:
					distCovered = 9999f;
					frame = Rectangle.Empty;
					origin = Vector2.Zero;
					color = Color.Transparent;
					break;
			}
		}

		public static void DrawBeamStart(SpriteBatch sb, Texture2D tex, Vector2 start, Vector2 end, Vector2 scale, Color beamColor, LaserLineFraming framing, int timer){
			Vector2 vector = start;
			Vector2 vector2 = Vector2.Normalize(end - start);
			float num = (end - start).Length();
			float rotation = vector2.ToRotation() - MathF.PI / 2f;
			if (vector2.HasNaNs())
			{
				return;
			}
			Texture2D brimstoneStartTexture = ModContent.Request<Texture2D>("RappleMod/Content/Projectiles/Brimstone/BrimstoneBeamSstart", AssetRequestMode.ImmediateLoad).Value;

			Rectangle brimstoneStartRectangle = new Rectangle(0, 30 * (timer / 7 % 4), brimstoneStartTexture.Width, 22);
			// timer / (ticks per animation frame) % amount of frames in animation
			// switch 'frame' for 'brimstoneStartRectangle'
			framing(0, vector, num, default(Rectangle), out var distanceCovered, out var frame, out var origin, out var color);
			sb.Draw(tex, vector, brimstoneStartRectangle, color, rotation, frame.Size() / 2f, scale, SpriteEffects.None, 0f);
    	}

		public static void DrawBeamBody(SpriteBatch sb, Texture2D tex, Vector2 start, Vector2 end, Vector2 scale, Color beamColor, LaserLineFraming framing, int timer){
			Vector2 vector = start;
			Vector2 vector2 = Vector2.Normalize(end - start);
			float num = (end - start).Length();
			float rotation = vector2.ToRotation() - MathF.PI / 2f;
			if (vector2.HasNaNs())
			{
				return;
			}
			Texture2D brimstoneBodyTexture = ModContent.Request<Texture2D>("RappleMod/Content/Projectiles/Brimstone/BrimstoneBeamSmid", AssetRequestMode.ImmediateLoad).Value;

			framing(0, vector, num, default(Rectangle), out var distanceCovered, out var frame, out var origin, out var color);
			num -= distanceCovered * scale.Y;
			vector += vector2 * ((float)frame.Height - origin.Y) * scale.Y;
			if (num > 0f)
			{
				float num2 = 0f;
				while (num2 + 1f < num)
				{
					framing(1, vector, num - num2, frame, out distanceCovered, out frame, out origin, out color);
					if (num - num2 < (float)frame.Height)
					{
						distanceCovered *= (num - num2) / (float)frame.Height;
						frame.Height = (int)(num - num2);
					}
					
					Rectangle brimstoneBodyRectangle = new Rectangle(0, 30 * (timer / 7 % 4), brimstoneBodyTexture.Width, 30);
					// timer / (ticks per animation frame) % amount of frames in animation
					// switch 'frame' for 'brimstoneBodyRectangle'
					sb.Draw(tex, vector, brimstoneBodyRectangle, color, rotation, origin, scale, SpriteEffects.None, 0f);
					num2 += distanceCovered * scale.Y;
					vector += vector2 * distanceCovered * scale.Y;
				}
			}

			framing(2, vector, num, default(Rectangle), out distanceCovered, out frame, out origin, out color);
    	}	

		public static void DrawBeamEnd(SpriteBatch sb, Texture2D tex, Vector2 start, Vector2 end, Vector2 scale, Color beamColor, LaserLineFraming framing, int timer){
			Vector2 vector = start;
			Vector2 vector2 = Vector2.Normalize(end - start);
			float num = (end - start).Length();
			float rotation = vector2.ToRotation() - MathF.PI / 2f;
			if (vector2.HasNaNs())
			{
				return;
			}
			Texture2D brimstoneEndTexture = ModContent.Request<Texture2D>("RappleMod/Content/Projectiles/Brimstone/BrimstoneBeamSend", AssetRequestMode.ImmediateLoad).Value;
            
			framing(0, vector, num, default(Rectangle), out var distanceCovered, out var frame, out var origin, out var color);
			num -= distanceCovered * scale.Y;
			vector += vector2 * ((float)frame.Height - origin.Y) * scale.Y;
			if (num > 0f)
			{
				float num2 = 0f;
				while (num2 + 1f < num)
				{
					framing(1, vector, num - num2, frame, out distanceCovered, out frame, out origin, out color);
					if (num - num2 < (float)frame.Height)
					{
						distanceCovered *= (num - num2) / (float)frame.Height;
						frame.Height = (int)(num - num2);
					}

					num2 += distanceCovered * scale.Y;
					vector += vector2 * distanceCovered * scale.Y;
				}
			}
			Rectangle brimstoneEndRectangle = new Rectangle(0, 30 * (timer / 7 % 4), brimstoneEndTexture.Width, 30);
			// timer / (ticks per animation frame) % amount of frames in animation
			// switch 'frame' for 'brimstoneEndRectangle'
			framing(2, vector, num, default(Rectangle), out distanceCovered, out frame, out origin, out color);
			sb.Draw(tex, vector, brimstoneEndRectangle, color, rotation, origin, scale, SpriteEffects.None, 0f);
    	}

        public override void CutTiles() {
			DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
			Utils.TileActionAttempt cut = new Utils.TileActionAttempt(DelegateMethods.CutTiles);
			Vector2 beamStartPos = Projectile.Center;
			Vector2 beamEndPos = beamStartPos + Projectile.velocity * BeamLength;

		    Utils.PlotTileLine(beamStartPos, beamEndPos, Projectile.width * Projectile.scale, cut);
		}

		private float EaseIn(float t){
			return t * t * (3.0f - 2.0f * t);
		}
    }

}