using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rail;
using ReLogic.Content;
using System;
using System.ComponentModel.DataAnnotations;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace RappleMod.Content.Projectiles
{
    public class AndromedaProjectile : ModProjectile
    {	
        private const string ChainTexturePath = "TutorialMod/Content/Projectiles/AndromedaProjectileChain";
        private const string ChainTextureExtraPath = "TutorialMod/Content/Projectiles/AndromedaProjectileChainExtra";  // This texture and related code is optional and used for a unique effect

        private enum AIState
		{
			Spinning,
			LaunchingForward,
			Retracting,
			UnusedState,
			ForcedRetracting,
			Ricochet,
			Dropping
		}

        private AIState CurrentAIState {
			get => (AIState)Projectile.ai[0];
			set => Projectile.ai[0] = (float)value;
		}
		public ref float StateTimer => ref Projectile.ai[1];
		public ref float CollisionCounter => ref Projectile.localAI[0];
		public ref float SpinningStateTimer => ref Projectile.localAI[1];
		int starReleaseQuant;
		

        public override void SetStaticDefaults() {
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}
        public override void SetDefaults() {
			Projectile.netImportant = true; 
			Projectile.width = 22; 
			Projectile.height = 22; 
			Projectile.friendly = true; 
			Projectile.penetrate = -1; 
			Projectile.DamageType = DamageClass.Melee; 
			Projectile.scale = 0.8f;
			Projectile.usesLocalNPCImmunity = true; 
			Projectile.localNPCHitCooldown = 10;
		}

       public override void AI() {
			Player player = Main.player[Projectile.owner];
			// Kill the projectile if the player dies or gets crowd controlled
			if (!player.active || player.dead || player.noItems || player.CCed || Vector2.Distance(Projectile.Center, player.Center) > 900f) {
				Projectile.Kill();
				return;
			}
			if (Main.myPlayer == Projectile.owner && Main.mapFullscreen) {
				Projectile.Kill();
				return;
			}

			Vector2 mountedCenter = player.MountedCenter;
			bool doFastThrowDust = false;
			bool shouldOwnerHitCheck = false;
			float launchTimeLimit = 35;  // How much time the projectile can go before retracting (speed and shootTimer will set the flail's range)
			float launchSpeed = 20f; // How fast the projectile can move
			float maxLaunchLength = 800f; // How far the projectile's chain can stretch before being forced to retract when in launched state
			float retractAcceleration = 5f; // How quickly the projectile will accelerate back towards the player while retracting
			float maxRetractSpeed = 16f; // The max speed the projectile will have while retracting
			float forcedRetractAcceleration = 5f; // How quickly the projectile will accelerate back towards the player while being forced to retract
			float maxForcedRetractSpeed = 16f; // The max speed the projectile will have while being forced to retract
			int defaultHitCooldown = 10; // How often your flail hits when resting on the ground, or retracting
			int spinHitCooldown = 10; // How often your flail hits when spinning
			int movingHitCooldown = 10; // How often your flail hits when moving
			float ricochetTimeLimit = launchTimeLimit + 5;

			// Scaling these speeds and accelerations by the players melee speed makes the weapon more responsive if the player boosts it or general weapon speed
			float meleeSpeedMultiplier = player.GetTotalAttackSpeed(DamageClass.Melee);
			launchTimeLimit *= meleeSpeedMultiplier;
			launchSpeed *= meleeSpeedMultiplier;
			retractAcceleration *= meleeSpeedMultiplier;
			maxRetractSpeed *= meleeSpeedMultiplier;
			forcedRetractAcceleration *= meleeSpeedMultiplier;
			maxForcedRetractSpeed *= meleeSpeedMultiplier;
			float launchRange = launchSpeed * launchTimeLimit;
			float maxDroppedRange = launchRange + 160f;
			Projectile.localNPCHitCooldown = defaultHitCooldown;

			switch (CurrentAIState) {
				case AIState.Spinning: {
						shouldOwnerHitCheck = true;
						if (Projectile.owner == Main.myPlayer) {
							Projectile.ai[2]++;
							Vector2 unitVectorTowardsMouse = mountedCenter.DirectionTo(Main.MouseWorld).SafeNormalize(Vector2.UnitX * player.direction);
							player.ChangeDir((unitVectorTowardsMouse.X > 0f).ToDirectionInt());
							if (!player.channel) // If the player releases then change to moving forward mode
							{
								CurrentAIState = AIState.LaunchingForward;
								StateTimer = 0f;
								Projectile.velocity = unitVectorTowardsMouse * launchSpeed + player.velocity;
								Projectile.Center = mountedCenter;
								Projectile.netUpdate = true;
								Projectile.ResetLocalNPCHitImmunity();
								Projectile.localNPCHitCooldown = movingHitCooldown;
								break;
							}
						}
						SpinningStateTimer += 1f;
						
						// This line creates a unit vector that is constantly rotated around the player. 20f controls how fast the projectile visually spins around the player
						Vector2 offsetFromPlayer;
						Vector2 velocity = new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
						velocity.Normalize();
						velocity *= 20f;

						
						if (Projectile.ai[2] <= 180f){
							if (Projectile.ai[2] % 60 == 0){
								Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center, velocity, 955, Projectile.damage, Projectile.knockBack, player.whoAmI);
							}
							starReleaseQuant = 3;
							spinHitCooldown = 38;
						}
						else if (Projectile.ai[2] <= 360f){
							if (Projectile.ai[2] % 40 == 0){
								Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center, velocity, 955, (int)(Projectile.damage * 1.1), Projectile.knockBack, player.whoAmI);
							}
							starReleaseQuant = 10;
							spinHitCooldown = 27;
						}
						else if(Projectile.ai[2] <= 540f){
							if (Projectile.ai[2] % 26 == 0){
								Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center, velocity, 955, (int)(Projectile.damage * 1.15), Projectile.knockBack, player.whoAmI);
							}
							starReleaseQuant = 23; 
							spinHitCooldown = 15;
						}
						else if(Projectile.ai[2] > 540f){
							if (Projectile.ai[2] % 17 == 0){
								Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center, velocity, 955, (int)(Projectile.damage * 1.33), Projectile.knockBack, player.whoAmI);
							}
							starReleaseQuant = 40;
							spinHitCooldown = 8;
						}

						if (Projectile.ai[2] == 180f){SoundEngine.PlaySound(SoundID.MaxMana with {
							Pitch = -1f
							}, Projectile.position);
						}
						else if (Projectile.ai[2] == 360f){SoundEngine.PlaySound(SoundID.MaxMana with {
							Pitch = -0.5f
							}, Projectile.position);
						}
						else if (Projectile.ai[2] == 540f){SoundEngine.PlaySound(SoundID.MaxMana with {
							Pitch = 0f
							}, Projectile.position);
						}

						if (Projectile.ai[2] <= 540f){
							offsetFromPlayer = new Vector2(player.direction).RotatedBy((float)Math.PI * (Projectile.ai[2] / 27) * (SpinningStateTimer / 60f) * player.direction);
						}
						else{	
							offsetFromPlayer = new Vector2(player.direction).RotatedBy((float)Math.PI * 20f * (SpinningStateTimer / 60f) * player.direction);
						}

						offsetFromPlayer.Y *= 0.8f;
						if (offsetFromPlayer.Y * player.gravDir > 0f) {
							offsetFromPlayer.Y *= 0.5f;
						}
						Projectile.Center = mountedCenter + offsetFromPlayer * 30f;
						Projectile.velocity = Vector2.Zero;
						Projectile.localNPCHitCooldown = spinHitCooldown; // set the hit speed to the spinning hit speed
						break;
					}
				case AIState.LaunchingForward: {
						doFastThrowDust = true;
						bool shouldSwitchToRetracting = StateTimer++ >= launchTimeLimit;
						shouldSwitchToRetracting |= Projectile.Distance(mountedCenter) >= maxLaunchLength;
						Projectile.ai[2] = 0;
						Vector2 starVelocity = Projectile.velocity;
						starVelocity.Normalize();
						starVelocity *= 10f;
						
						if (starReleaseQuant > 0){
							if (Projectile.ai[2] % 3 == 0){
								// 0.34f radians ~~ 20 degrees
								Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center, starVelocity.RotatedByRandom(0.34f), 955, (int)(Projectile.damage * 1.33), Projectile.knockBack, player.whoAmI);
								starReleaseQuant--;
							}
								Projectile.ai[2]++;
						}

						if (player.controlUseItem) // If the player clicks, transition to the Dropping state
						{
							CurrentAIState = AIState.Dropping;
							StateTimer = 0f;
							Projectile.netUpdate = true;
							Projectile.velocity *= 0.2f;
							break;
						}

                        if (shouldSwitchToRetracting)
						{
							CurrentAIState = AIState.Retracting;
							StateTimer = 0f;
							Projectile.netUpdate = true;
							Projectile.velocity *= 0.3f;
						}
						player.ChangeDir((player.Center.X < Projectile.Center.X).ToDirectionInt());
						Projectile.localNPCHitCooldown = movingHitCooldown;
						break;
					}
				case AIState.Retracting: {
						Vector2 unitVectorTowardsPlayer = Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero);
						Vector2 starVelocity = Projectile.velocity * -1;
						starVelocity.Normalize();
						starVelocity *= 10f;

						if (starReleaseQuant > 0){
							if (Projectile.ai[2] % 3 == 0){
								// 0.34f radians ~~ 20 degrees
								Projectile.NewProjectile(Projectile.GetSource_FromThis(), player.Center, starVelocity.RotatedByRandom(0.34f) , 955, Projectile.damage, Projectile.knockBack, player.whoAmI);
								starReleaseQuant--;
							}
								Projectile.ai[2]++;
						}

						if (Projectile.Distance(mountedCenter) <= maxRetractSpeed) {
							Projectile.Kill(); // Kill the projectile once it is close enough to the player
							return;
						}

						if (player.controlUseItem) // If the player clicks, transition to the Dropping state
						{
							CurrentAIState = AIState.Dropping;
							StateTimer = 0f;
							Projectile.netUpdate = true;
							Projectile.velocity *= 0.2f;
						}
						else {
							Projectile.velocity *= 0.98f;
							Projectile.velocity = Projectile.velocity.MoveTowards(unitVectorTowardsPlayer * maxRetractSpeed, retractAcceleration);
							player.ChangeDir((player.Center.X < Projectile.Center.X).ToDirectionInt());
						}
						break;
					}
				case AIState.ForcedRetracting: {
						Projectile.tileCollide = false;
						Vector2 unitVectorTowardsPlayer = Projectile.DirectionTo(mountedCenter).SafeNormalize(Vector2.Zero);
						if (Projectile.Distance(mountedCenter) <= maxForcedRetractSpeed) {
							Projectile.Kill(); // Kill the projectile once it is close enough to the player
							return;
						}
						Projectile.velocity *= 0.98f;
						Projectile.velocity = Projectile.velocity.MoveTowards(unitVectorTowardsPlayer * maxForcedRetractSpeed, forcedRetractAcceleration);
						Vector2 target = Projectile.Center + Projectile.velocity;
						Vector2 value = mountedCenter.DirectionFrom(target).SafeNormalize(Vector2.Zero);
						if (Vector2.Dot(unitVectorTowardsPlayer, value) < 0f) {
							Projectile.Kill(); // Kill projectile if it will pass the player
							return;
						}
						player.ChangeDir((player.Center.X < Projectile.Center.X).ToDirectionInt());
						break;
					}
				case AIState.Ricochet:
					if (StateTimer++ >= ricochetTimeLimit) {
						CurrentAIState = AIState.Dropping;
						StateTimer = 0f;
						Projectile.netUpdate = true;
					}
					else {
						Projectile.localNPCHitCooldown = movingHitCooldown;
						Projectile.velocity.Y += 0.6f;
						Projectile.velocity.X *= 0.95f;
						player.ChangeDir((player.Center.X < Projectile.Center.X).ToDirectionInt());
					}
					break;
				case AIState.Dropping:
					if (!player.controlUseItem || Projectile.Distance(mountedCenter) > maxDroppedRange) {
						CurrentAIState = AIState.ForcedRetracting;
						StateTimer = 0f;
						Projectile.netUpdate = true;
					}
					else {
						Projectile.velocity.Y += 0.8f;
						Projectile.velocity.X *= 0.95f;
						player.ChangeDir((player.Center.X < Projectile.Center.X).ToDirectionInt());
					}
					break;
			}


			Projectile.direction = (Projectile.velocity.X > 0f).ToDirectionInt();
			Projectile.spriteDirection = Projectile.direction;
			Projectile.ownerHitCheck = shouldOwnerHitCheck; // This prevents attempting to damage enemies without line of sight to the player. The custom Colliding code for spinning makes this necessary

			
			if (Projectile.velocity.Length() > 1f)
				Projectile.rotation = Projectile.velocity.ToRotation() + Projectile.velocity.X * 0.1f; // skid
			else
				Projectile.rotation += Projectile.velocity.X * 0.1f; // roll

			Projectile.timeLeft = 2; // Makes sure the flail doesn't die (good when the flail is resting on the ground)
			player.heldProj = Projectile.whoAmI;
			player.SetDummyItemTime(2); //Add a delay so the player can't button mash the flail
			player.itemRotation = Projectile.DirectionFrom(mountedCenter).ToRotation();
			if (Projectile.Center.X < mountedCenter.X) {
				player.itemRotation += (float)Math.PI;
			}
			player.itemRotation = MathHelper.WrapAngle(player.itemRotation);

			// Spawning dust. We spawn dust more often when in the LaunchingForward state
			int dustRate = 30;
			if (doFastThrowDust)
				dustRate = 2;

			if (Main.rand.NextBool(dustRate))
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 344, 0f, 0f, 150, default(Color), 1.3f);
		}

        public override bool OnTileCollide(Vector2 oldVelocity) {
			int defaultLocalNPCHitCooldown = 10;
			int impactIntensity = 0;
			Vector2 velocity = Projectile.velocity;
			float bounceFactor = 0.2f;
			if (CurrentAIState == AIState.LaunchingForward || CurrentAIState == AIState.Ricochet) {
				bounceFactor = 0.4f;
			}

			if (CurrentAIState == AIState.Dropping) {
				bounceFactor = 0f;
			}

			if (oldVelocity.X != Projectile.velocity.X) {
				if (Math.Abs(oldVelocity.X) > 4f) {
					impactIntensity = 1;
				}

				Projectile.velocity.X = (0f - oldVelocity.X) * bounceFactor;
				CollisionCounter += 1f;
			}

			if (oldVelocity.Y != Projectile.velocity.Y) {
				if (Math.Abs(oldVelocity.Y) > 4f) {
					impactIntensity = 1;
				}

				Projectile.velocity.Y = (0f - oldVelocity.Y) * bounceFactor;
				CollisionCounter += 1f;
			}

			// If in the Launched state, spawn sparks
			if (CurrentAIState == AIState.LaunchingForward) {
				CurrentAIState = AIState.Ricochet;
				Projectile.localNPCHitCooldown = defaultLocalNPCHitCooldown;
				Projectile.netUpdate = true;
				Point scanAreaStart = Projectile.TopLeft.ToTileCoordinates();
				Point scanAreaEnd = Projectile.BottomRight.ToTileCoordinates();
				impactIntensity = 2;
				Projectile.CreateImpactExplosion(2, Projectile.Center, ref scanAreaStart, ref scanAreaEnd, Projectile.width, out bool causedShockwaves);
				Projectile.CreateImpactExplosion2_FlailTileCollision(Projectile.Center, causedShockwaves, velocity);
				Projectile.position -= velocity;
			}

			// Here the tiles spawn dust indicating they've been hit
			if (impactIntensity > 0) {
				Projectile.netUpdate = true;
				for (int i = 0; i < impactIntensity; i++) {
					Collision.HitTiles(Projectile.position, velocity, Projectile.width, Projectile.height);
				}

				SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
			}

			// Force retraction if stuck on tiles while retracting
			if (CurrentAIState != AIState.UnusedState && CurrentAIState != AIState.Spinning && CurrentAIState != AIState.Ricochet && CurrentAIState != AIState.Dropping && CollisionCounter >= 10f) {
				CurrentAIState = AIState.ForcedRetracting;
				Projectile.netUpdate = true;
			}

			return false;
		}

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
			if (CurrentAIState == AIState.Spinning) {
				Vector2 mountedCenter = Main.player[Projectile.owner].MountedCenter;
				Vector2 shortestVectorFromPlayerToTarget = targetHitbox.ClosestPointInRect(mountedCenter) - mountedCenter;
				shortestVectorFromPlayerToTarget.Y /= 0.8f; // Makes the hit area an ellipse. Vertical hit distance is smaller due to this math.
				float hitRadius = 55f; // The length of the semi-major radius of the ellipse (the long end)
				return shortestVectorFromPlayerToTarget.Length() <= hitRadius;
			}
			// Regular collision logic happens otherwise.
			return base.Colliding(projHitbox, targetHitbox);
		}

       public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers) {
			if (CurrentAIState == AIState.Spinning) {
				modifiers.SourceDamage *= 1.2f;
			}
			else if (CurrentAIState == AIState.LaunchingForward || CurrentAIState == AIState.Retracting) {
				modifiers.SourceDamage *= 2f;
			}

			modifiers.HitDirectionOverride = (Main.player[Projectile.owner].Center.X < target.Center.X).ToDirectionInt();

			if (CurrentAIState == AIState.Spinning) {
				modifiers.Knockback *= 0.25f;
			}
			else if (CurrentAIState == AIState.Dropping) {
				modifiers.Knockback *= 0.5f;
			}
		}

        public override bool PreDraw(ref Color lightColor) {
			Vector2 playerArmPosition = Main.GetPlayerArmPosition(Projectile);

			// This fixes a vanilla GetPlayerArmPosition bug causing the chain to draw incorrectly when stepping up slopes. The flail itself still draws incorrectly due to another similar bug. This should be removed once the vanilla bug is fixed.
			playerArmPosition.Y -= Main.player[Projectile.owner].gfxOffY;

			Asset<Texture2D> chainTexture = ModContent.Request<Texture2D>(ChainTexturePath);
			Asset<Texture2D> chainTextureExtra = ModContent.Request<Texture2D>(ChainTextureExtraPath); // This texture and related code is optional and used for a unique effect

			Rectangle? chainSourceRectangle = null;
			float chainHeightAdjustment = 0f; // Use this to adjust the chain overlap. 

			Vector2 chainOrigin = chainSourceRectangle.HasValue ? (chainSourceRectangle.Value.Size() / 2f) : (chainTexture.Size() / 2f);
			Vector2 chainDrawPosition = Projectile.Center;
			Vector2 vectorFromProjectileToPlayerArms = playerArmPosition.MoveTowards(chainDrawPosition, 3f) - chainDrawPosition;
			Vector2 unitVectorFromProjectileToPlayerArms = vectorFromProjectileToPlayerArms.SafeNormalize(Vector2.Zero);
			float chainSegmentLength = (chainSourceRectangle.HasValue ? chainSourceRectangle.Value.Height : chainTexture.Height()) + chainHeightAdjustment;
			if (chainSegmentLength == 0) {
				chainSegmentLength = 10; // When the chain texture is being loaded, the height is 0 which would cause infinite loops.
			}
			float chainRotation = unitVectorFromProjectileToPlayerArms.ToRotation() + MathHelper.PiOver2;
			int chainCount = 1;
			float chainLengthRemainingToDraw = vectorFromProjectileToPlayerArms.Length() + chainSegmentLength / 2f;

			// This while loop draws the chain texture from the projectile to the player, looping to draw the chain texture along the path
			while (chainLengthRemainingToDraw > 0f) {
				// This code gets the lighting at the current tile coordinates
				Color chainDrawColor = Lighting.GetColor((int)chainDrawPosition.X / 16, (int)(chainDrawPosition.Y / 16f));

				var chainTextureToDraw = chainTexture;
				if (chainCount >= 4) {
					// Use normal chainTexture and lighting, no changes
				}
				else if (chainCount >= 2) {
					// Near to the ball, we draw a custom chain texture and slightly make it glow if unlit.
					chainTextureToDraw = chainTextureExtra;
					byte minValue = 140;
					if (chainDrawColor.R < minValue)
						chainDrawColor.R = minValue;

					if (chainDrawColor.G < minValue)
						chainDrawColor.G = minValue;

					if (chainDrawColor.B < minValue)
						chainDrawColor.B = minValue;
				}
				else {
					// Close to the ball, we draw a custom chain texture and draw it at full brightness glow.
					chainTextureToDraw = chainTextureExtra;
					chainDrawColor = Color.White;
				}

				// Here, we draw the chain texture at the coordinates
				Main.spriteBatch.Draw(chainTextureToDraw.Value, chainDrawPosition - Main.screenPosition, chainSourceRectangle, chainDrawColor, chainRotation, chainOrigin, 1f, SpriteEffects.None, 0f);

				// chainDrawPosition is advanced along the vector back to the player by the chainSegmentLength
				chainDrawPosition += unitVectorFromProjectileToPlayerArms * chainSegmentLength;
				chainCount++;
				chainLengthRemainingToDraw -= chainSegmentLength;
			}

			// Add a motion trail when moving forward, like most flails do (don't add trail if already hit a tile)
			if (CurrentAIState == AIState.LaunchingForward)
			{
				Texture2D projectileTexture = TextureAssets.Projectile[Projectile.type].Value;
				Vector2 drawOrigin = new Vector2(projectileTexture.Width * 0.5f, Projectile.height * 0.5f);
				SpriteEffects spriteEffects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
				for (int k = 0; k < Projectile.oldPos.Length && k < StateTimer; k++) {
					Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
					Color color = Projectile.GetAlpha(lightColor) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
					Main.spriteBatch.Draw(projectileTexture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale - k / (float)Projectile.oldPos.Length / 3, spriteEffects, 0f);
				}
			}
			return true;
		}
    }

}