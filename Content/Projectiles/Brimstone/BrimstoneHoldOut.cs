using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;

namespace RappleMod.Content.Projectiles.Brimstone
{
	public class BrimstoneHoldOut : ModProjectile
	{
		public const float MaxCharge = 300f;
		public int timer = 0;
		public override string Texture => "RappleMod/Content/Projectiles/InvisibleProj";

		public override void SetStaticDefaults() {
			ProjectileID.Sets.NeedsUUID[Projectile.type] = true;
			ProjectileID.Sets.HeldProjDoesNotUsePlayerGfxOffY[Type] = true;
		}

		public override void SetDefaults() {
			Projectile.width = 18;
			Projectile.height = 18;
			Projectile.penetrate = -1;
			Projectile.tileCollide = false;
			Projectile.ownerHitCheck = true;
			Projectile.usesLocalNPCImmunity = true;
			Projectile.localNPCHitCooldown = -1;
			Projectile.timeLeft = 2;
		}

		private const float MaxManaConsumptionDelay = 20f;
		private const float MinManaConsumptionDelay = 8f;
		private bool alreadyFired = false;

		private float ChargeTime {
			get => Projectile.ai[0];
			set => Projectile.ai[0] = value;
		}

		private int ChargeLevel = 1;

		private float NextManaFrame {
			get => Projectile.ai[1];
			set => Projectile.ai[1] = value;
		}

		private float ManaConsumptionRate {
			get => Projectile.localAI[0];
			set => Projectile.localAI[0] = value;
		}

		public override void AI() {
			Player player = Main.player[Projectile.owner];
			Vector2 rrp = player.RotatedRelativePoint(player.MountedCenter, true);

			UpdatePlayerVisuals(player, rrp);
			bool hasBrimS = !Main.projectile.Any(n => n.active && n.owner == player.whoAmI && n.type == ModContent.ProjectileType<BrimstoneBeamS>());
			bool hasBrimM = !Main.projectile.Any(n => n.active && n.owner == player.whoAmI && n.type == ModContent.ProjectileType<BrimstoneBeamM>());
			bool hasBrimL = !Main.projectile.Any(n => n.active && n.owner == player.whoAmI && n.type == ModContent.ProjectileType<BrimstoneBeamL>());
			
			if (Projectile.owner == Main.myPlayer) {
				UpdateAim(rrp, player.HeldItem.shootSpeed);
				bool manaIsAvailable = !ShouldConsumeMana() || player.CheckMana(player.HeldItem.mana, true, false);

				bool stillInUse = player.channel && manaIsAvailable && !player.noItems && !player.CCed;
				
				if (!stillInUse & ChargeTime > 30 && !alreadyFired){
					FireBrimstone();
					alreadyFired = true;
				}
				else if (!stillInUse && hasBrimS && hasBrimM && hasBrimL) {
					Projectile.Kill();
				}
				else if (!stillInUse & ChargeTime <= 30) Projectile.Kill();

				if (alreadyFired) timer++;

				if (stillInUse && !alreadyFired){
					ProduceDust(Color.Red);
				}
			}
			
			Projectile.timeLeft = 2;
			
			if (!Charge(player)) {
				return;
			}
		}
		private bool Charge(Player player) {
			if (!player.channel || ChargeTime >= 600) {
				return true; // finished charging
			}

			ChargeTime++;
			if (ChargeTime == 120){
				ChargeLevel = 2;
				SoundEngine.PlaySound(SoundID.Item113 with {Pitch = -1f}, Projectile.position);
			}
			if (ChargeTime == 299){
				ChargeLevel = 3;
				SoundEngine.PlaySound(SoundID.Item113, Projectile.position);
			}

			player.itemAnimation = player.itemAnimationMax;
			player.itemTime = player.itemTimeMax;

			return false;
		}

		private void UpdateAim(Vector2 source, float speed) {
			Vector2 aim = Vector2.Normalize(Main.MouseWorld - source);
			if (aim.HasNaNs()) {
				aim = -Vector2.UnitY;
			}

			aim = Vector2.Normalize(Vector2.Lerp(Vector2.Normalize(Projectile.velocity), aim, 0.08f));
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
			player.itemTime = 2;
			player.itemAnimation = 2;
			player.itemRotation = (Projectile.velocity * Projectile.direction).ToRotation();
		}

		private bool ShouldConsumeMana() {
			// If the mana consumption timer hasn't been initialized yet, initialize it and consume mana on frame 1.
			if (ManaConsumptionRate == 0f) {
				NextManaFrame = ManaConsumptionRate = MaxManaConsumptionDelay;
				return true;
			}

			// Should mana be consumed this frame?
			bool consume = ChargeTime == NextManaFrame;

			// If mana is being consumed this frame, update the rate of mana consumption and write down the next frame mana will be consumed.
			if (consume) {
				// MathHelper.Clamp(X,A,B) guarantees that A <= X <= B. If X is outside the range, it will be set to A or B accordingly.
				ManaConsumptionRate = MathHelper.Clamp(ManaConsumptionRate - 1f, MinManaConsumptionDelay, MaxManaConsumptionDelay);
				NextManaFrame += ManaConsumptionRate;
			}
			return consume;
		}

		private void FireBrimstone() {
			Player player = Main.LocalPlayer;
			// If for some reason the beam velocity can't be correctly normalized, set it to a default value.
			Vector2 beamVelocity = Vector2.Normalize(Projectile.velocity);
			if (beamVelocity.HasNaNs()) {
				beamVelocity = -Vector2.UnitY;
			}

			// This UUID will be the same between all players in multiplayer, ensuring that the beams are properly anchored on the Prism on everyone's screen.
			int uuid = Projectile.GetByUUID(Projectile.owner, Projectile.whoAmI);

			int damage = Projectile.damage;
			float knockback = Projectile.knockBack;
			SoundStyle BrimWeak = new($"{nameof(RappleMod)}/Assets/Sounds/BrimstoneWeak{Main.rand.Next(1, 4)}");
			SoundStyle BrimMedium = new($"{nameof(RappleMod)}/Assets/Sounds/BrimstoneMedium{Main.rand.Next(1, 4)}");
			SoundStyle BrimStrong = new($"{nameof(RappleMod)}/Assets/Sounds/BrimstoneLarge{Main.rand.Next(1, 4)}");
			if (ChargeLevel == 1){
			 	SoundEngine.PlaySound(BrimWeak, player.Center);
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, beamVelocity, ModContent.ProjectileType<BrimstoneBeamS>(), (int)(damage * 0.33f), knockback, Projectile.owner, ChargeLevel, uuid);
			}
			else if (ChargeLevel == 2){
				SoundEngine.PlaySound(BrimMedium, player.Center);
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, beamVelocity, ModContent.ProjectileType<BrimstoneBeamM>(), damage, knockback, Projectile.owner, ChargeLevel, uuid);
			}
			else if (ChargeLevel == 3){
				PunchCameraModifier modifier = new PunchCameraModifier(player.Center, (Main.rand.NextFloat() * ((float)System.Math.PI * 2f)).ToRotationVector2(), 20f, 6f, 20, 1000f, FullName);
				Main.instance.CameraModifiers.Add(modifier);
				SoundEngine.PlaySound(BrimStrong, player.Center);
				Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, beamVelocity, ModContent.ProjectileType<BrimstoneBeamL>(), damage*3, knockback, Projectile.owner, ChargeLevel, uuid);
			}
			Projectile.netUpdate = true;
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
