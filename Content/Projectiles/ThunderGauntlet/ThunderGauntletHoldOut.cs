using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace RappleMod.Content.Projectiles.ThunderGauntlet
{
	public class ThunderGauntletHoldOut : ModProjectile
	{
		SlotId soundSlot;
		int timer = 0;
		private float ChargeTime {
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}
		public override void SetStaticDefaults() {
			ProjectileID.Sets.NeedsUUID[Projectile.type] = true;
			ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
		}

		public override void SetDefaults() {
			Projectile.width = 26;
			Projectile.height = 30;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.localNPCHitCooldown = -1;
			Projectile.timeLeft = 2;
			Projectile.ownerHitCheck = true;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.extraUpdates = 4;
		}

		public override void AI() {
			timer++;
			SoundStyle loopChargeSound = new($"{nameof(RappleMod)}/Assets/Sounds/ThunderGauntletChargeLoop");
			SoundStyle chargeSoundRelease = new($"{nameof(RappleMod)}/Assets/Sounds/ThunderGauntletChargeRelease");
			Player player = Main.player[Projectile.owner];
			Vector2 rrp = player.RotatedRelativePoint(player.MountedCenter, true);
			player.heldProj = Projectile.whoAmI;
			DrawHeldProjInFrontOfHeldItemAndArms = true;
		
			UpdatePlayerVisuals(player, rrp);
			if (Projectile.owner == Main.myPlayer && timer % 5 == 0) {
				UpdateAim(rrp, player.HeldItem.shootSpeed);
			}

			if (!SoundEngine.TryGetActiveSound(soundSlot, out var _)) {
				var tracker = new ProjectileAudioTracker(Projectile);
				soundSlot = SoundEngine.PlaySound(loopChargeSound, player.Center, soundInstance => {
					soundInstance.Position = player.Center;
					return tracker.IsActiveAndInGame();
				});
				loopChargeSound.IsLooped = true;
				loopChargeSound.MaxInstances = 1;
			}
			
			if (!Charge(player)) {			
				if (ChargeTime > 0){
					float speedBonus = 0.1f + player.velocity.Length() / 7f * 0.9f;
					Vector2 velocityLaunch = Projectile.velocity;
					velocityLaunch.Normalize();

					Projectile.damage = (int)(Projectile.damage * (1 + ChargeTime/300f) * (1 + speedBonus));
					Projectile.knockBack = Projectile.knockBack * player.velocity.Length() / 7f;

					player.velocity += velocityLaunch * 10f * (ChargeTime/300f);

					if (SoundEngine.TryGetActiveSound(soundSlot, out var activeSound)) {
						activeSound.Stop();
					}	
					chargeSoundRelease.MaxInstances = 1;
					soundSlot = SoundEngine.PlaySound(chargeSoundRelease, player.Center);
				}
				player.noKnockback = true;
				ChargeTime = 0;
				Projectile.friendly = true;
				player.immune = true;
				player.immuneNoBlink = true;
				player.immuneTime = 10;
				return;
			}
			else Projectile.timeLeft = 300;
		}

        public override void OnSpawn(IEntitySource source)
        {
			Player player = Main.player[Projectile.owner];
            SoundStyle chargeSound = new($"{nameof(RappleMod)}/Assets/Sounds/ThunderGauntletCharge"){
                MaxInstances = 1
            };

			var tracker = new ProjectileAudioTracker(Projectile);
            soundSlot = SoundEngine.PlaySound(chargeSound, player.Center, soundInstance => {
				soundInstance.Position = player.Center;
				return tracker.IsActiveAndInGame();
			});
        }

        public override bool PreDraw(ref Color lightColor) => false;

        public override void PostDraw(Color lightColor) {
			Player player = Main.player[Projectile.owner];

			Texture2D projectileTexture = ModContent.Request<Texture2D>("RappleMod/Content/Projectiles/ThunderGauntlet/ThunderGauntletHoldOut").Value;
			Vector2 drawOrigin = new Vector2(projectileTexture.Width, projectileTexture.Height) / 2f;
			Vector2 drawPosition = Projectile.Center - Main.screenPosition;

			Main.EntitySpriteDraw(projectileTexture, drawPosition, null, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None);
		}
	
        private bool Charge(Player player) {
			if (!player.channel) {
				return false;
			}

			Projectile.friendly = false;
			if (ChargeTime < 1050) {
				ChargeTime++;
			}

			player.itemAnimation = player.itemAnimationMax;
			player.itemTime = player.itemTimeMax;

			return true;
		}

		private void UpdateAim(Vector2 source, float speed) {
			Vector2 aim = Vector2.Normalize(Main.MouseWorld - source);
			if (aim.HasNaNs()) {
				aim = -Vector2.UnitY;
			}

			aim = Vector2.Normalize(Vector2.Lerp(Vector2.Normalize(Projectile.velocity), aim, 0.12f));
			aim *= speed;

			if (aim != Projectile.velocity) {
				Projectile.netUpdate = true;
			}
			Projectile.velocity = aim;
		}

		private void UpdatePlayerVisuals(Player player, Vector2 playerHandPos) {
			Projectile.Center = playerHandPos;
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
			Projectile.spriteDirection = Projectile.direction;

			player.ChangeDir(Projectile.direction);
			player.heldProj = Projectile.whoAmI;
			player.itemTime = 11;
			player.itemAnimation = 11;
			player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
		}

		private void ProduceDust(Color dustColor) {
			// Create one dust per frame a small distance from where the beam ends.
			const int type = 15;

			Vector2 centerOffset = new(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
			centerOffset.Normalize();
			centerOffset *= 30f;
			Vector2 startDistance = centerOffset + Projectile.Center;
			float scale = Main.rand.NextFloat(0.7f, 1.1f);
			Vector2 velocity = startDistance.DirectionTo(Projectile.Center) * 10;
			Dust dust = Dust.NewDustDirect(startDistance, 0, 0, type, velocity.X, velocity.Y, 0, dustColor, scale);
			dust.color = dustColor;
			dust.noGravity = true;

		}
	}
}
