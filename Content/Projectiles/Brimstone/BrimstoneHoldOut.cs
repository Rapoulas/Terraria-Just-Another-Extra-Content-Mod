using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace RappleMod.Content.Projectiles.Brimstone
{
	public class BrimstoneHoldOut : ModProjectile
	{
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

		private float ChargeTime {
			get => Projectile.ai[1];
			set => Projectile.ai[1] = value;
		}

		private float ChargeLevel {
			get => Projectile.ai[1];
			set => Projectile.ai[1] = 1;
		}

		public override void AI() {
			Player player = Main.player[Projectile.owner];
			Vector2 rrp = player.RotatedRelativePoint(player.MountedCenter, true);

			UpdatePlayerVisuals(player, rrp);
			
			if (Projectile.owner == Main.myPlayer) {
				UpdateAim(rrp, player.HeldItem.shootSpeed);
				bool stillInUse = player.channel && !player.noItems && !player.CCed;
				if (!stillInUse) {
					Projectile.Kill();
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

			if (ChargeTime % 90 == 0)
				ChargeLevel = 2;
			if (ChargeTime % 300 == 0)
				ChargeLevel = 3;

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
	}
}
