using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace RappleMod.Content.Projectiles.GunSummon
{
	public class GunSummonBuff : ModBuff
	{
		public override void SetStaticDefaults() {
			Main.buffNoSave[Type] = true; // This buff won't save when you exit the world
			Main.buffNoTimeDisplay[Type] = true; // The time remaining won't display on this buff
		}

		public override void Update(Player player, ref int buffIndex) {
			// If the minions exist reset the buff time, otherwise remove the buff from the player
			if (player.ownedProjectileCounts[ModContent.ProjectileType<GunSummonMinion>()] > 0) {
				player.buffTime[buffIndex] = 18000;
			}
			else {
				player.DelBuff(buffIndex);
				buffIndex--;
			}
		}
	}

	public class GunSummonMinion : ModProjectile
	{
		int timer = 0;
		public override string Texture => "RappleMod/Content/Projectiles/InvisibleProj";
		public override void SetStaticDefaults() {
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

			Main.projPet[Projectile.type] = true;
			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true; 
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
		}

		public sealed override void SetDefaults() {
			Projectile.width = 52;
			Projectile.height = 22;
			Projectile.tileCollide = false;
			Projectile.minion = true;
			Projectile.DamageType = DamageClass.Summon; 
			Projectile.minionSlots = 1f;
			Projectile.penetrate = -1;
		}

		// Here you can decide if your minion breaks things like grass or pots
		public override bool? CanCutTiles() {
			return false;
		}

		// The AI of this minion is split into multiple methods to avoid bloat. This method just passes values between calls actual parts of the AI.
		public override void AI() {
			Player owner = Main.player[Projectile.owner];

			if (!CheckActive(owner)) {
				return;
			}
			SetSpriteSize();
			SearchForTargets(owner, out bool foundTarget, out float distanceFromTarget, out NPC target);
			GeneralBehavior(owner, target, foundTarget, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition, out Vector2 idlePosition);
			Movement(foundTarget, target, distanceToIdlePosition, vectorToIdlePosition, idlePosition);
		}

		private bool CheckActive(Player owner) {
			if (owner.dead || !owner.active) {
				owner.ClearBuff(ModContent.BuffType<GunSummonBuff>());

				return false;
			}

			if (owner.HasBuff(ModContent.BuffType<GunSummonBuff>())) {
				Projectile.timeLeft = 2;
			}

			return true;
		}

		private void SetSpriteSize(){
			switch (Projectile.ai[0]){
				case 0:
					Projectile.width = 52;
					Projectile.height = 22;
					break;
				case 1:
					Projectile.width = 40;
					Projectile.height = 16;
					break;
				case 2:
					Projectile.width = 62;
					Projectile.height = 24;
					break;
				case 3:
					Projectile.width = 28;
					Projectile.height = 46;
					break;
				default:
					Projectile.width = 52;
					Projectile.height = 20;
					break;
			}
		}

		private void GeneralBehavior(Player owner, NPC target, bool foundTarget, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition, out Vector2 idlePosition) {
			idlePosition = owner.Center;
			int amountPistol = 0;
			
			if (!foundTarget){
				switch (Projectile.ai[0]){
					case 0:
						idlePosition.Y -= 160f;
						break;
					case 1:
						idlePosition.X -= 160f;
						break;
					case 2:
						idlePosition.Y += 160f;
						break;
					case 3:
						idlePosition.Y += Main.rand.NextFloat(-240f, 240f);
						idlePosition.X += Main.rand.NextFloat(-240f, 240f);
						break;
					default:
						idlePosition.X += 160f;
						float minionPositionOffsetX = 10 + (Projectile.minionPos-3) * 40;
						idlePosition.X += minionPositionOffsetX;
						break;
				}
			}

			vectorToIdlePosition = idlePosition - Projectile.Center;
			distanceToIdlePosition = vectorToIdlePosition.Length();

			if (Main.myPlayer == owner.whoAmI && distanceToIdlePosition > 2000f) {
				Projectile.position = idlePosition;
				Projectile.velocity *= 0.1f;
				Projectile.netUpdate = true;
			}

			timer++;
			
			for (int j = 0; j < Main.maxProjectiles; j++){
				if (Main.projectile[j].type == ModContent.ProjectileType<GunSummonMinion>() && Main.projectile[j].active && Main.projectile[j].ai[0] == 3){
					amountPistol++;
				}
			}

			if (foundTarget){
				switch (Projectile.ai[0]){
					case 0:
						if (timer % (150 - MathHelper.Min(amountPistol*7, 120)) == 0)
							if (owner.PickAmmo(ContentSamples.ItemsByType[ItemID.RocketLauncher], out int projToShoot, out float projSpeed, out int damage, out float knockBack, out int usedAmmoItemId)){
								ShootingHandler(target, owner, 1, projSpeed, projToShoot, knockBack, 0, 0, 0, damage);
								timer = 0;
							}
						break;
					case 1:
					if (timer % (65 - MathHelper.Min(amountPistol*3, 45)) == 0)
							if (owner.PickAmmo(ContentSamples.ItemsByType[ItemID.Shotgun], out int projToShoot, out float projSpeed, out int damage, out float knockBack, out int usedAmmoItemId)){
								ShootingHandler(target, owner, 8, projSpeed, projToShoot, knockBack, 35, 2.5f, 0, damage);
								timer = 0;
							}
						break;
					case 2:
						if (timer % (120 - MathHelper.Min(amountPistol*6, 100)) == 0)
							if (owner.PickAmmo(ContentSamples.ItemsByType[ItemID.SniperRifle], out int projToShoot, out float projSpeed, out int damage, out float knockBack, out int usedAmmoItemId)){
								ShootingHandler(target, owner, 1, projSpeed * 2f, projToShoot, knockBack, 0, 0, 2, damage);
								timer = 0;
							}
						break;
					case 3:
						if (timer % (65 - MathHelper.Min(amountPistol*3, 55)) == 0)
							if (owner.PickAmmo(ContentSamples.ItemsByType[ItemID.PulseBow], out int projToShoot, out float projSpeed, out int damage, out float knockBack, out int usedAmmoItemId)){
								ShootingHandler(target, owner, 1, projSpeed, projToShoot, knockBack, 0, 0, 0, damage);
								timer = 0;
							}
						break;
					default:
						if (timer % (45 - MathHelper.Min(amountPistol*2, 30)) == 0)
							if (owner.PickAmmo(ContentSamples.ItemsByType[ItemID.Revolver], out int projToShoot, out float projSpeed, out int damage, out float knockBack, out int usedAmmoItemId)){
								ShootingHandler(target, owner, 1, projSpeed * 1.25f, projToShoot, knockBack, 0, 0, 0, damage);
								timer = 0;
							}
						break;
				}
			}
		}

		private void ShootingHandler(NPC target, Player owner, int amountBullets, float projSpeedMultiplier, int projToShoot, float knockBack, float degreesRotation, float speedVariation, int amountPenetration, int damage){
			for (int i = 0; i < amountBullets; i++){
				Vector2 velocity = Projectile.Center.DirectionTo(target.Center).RotatedByRandom(MathHelper.ToRadians(degreesRotation));
				Projectile proj = Projectile.NewProjectileDirect(owner.GetSource_FromThis(), Projectile.position, velocity * (projSpeedMultiplier + Main.rand.NextFloat(-speedVariation, speedVariation)), projToShoot, damage, knockBack);
				proj.penetrate += amountPenetration;
				proj.tileCollide = false;
			}
		}

		private void Movement(bool foundTarget, NPC target, float distanceToIdlePosition, Vector2 vectorToIdlePosition, Vector2 idlePosition) {
			Player player = Main.player[Projectile.owner];
			float inertia = 40f;
			float speed;

			if (foundTarget) {
				Vector2 vecToTarget = target.position - Projectile.Center;
				Vector2 vecToPlayer = player.position - Projectile.Center;
				Vector2 vecSniper = player.position.DirectionTo(target.position) * 500f;
                float targetDist = vecToTarget.Length();
				float playerDist = vecToPlayer.Length();
				vecToPlayer.Normalize();
                vecToTarget.Normalize();
				
				switch (Projectile.ai[0]){
					case 0:
						speed = 12f;
						vecToPlayer *= speed;
						Projectile.velocity = (Projectile.velocity * 40f + vecToPlayer) / 41f;
						break;
					case 1: 
						if (targetDist > 120f){
							speed = 8f;
							vecToTarget *= speed;
							Projectile.velocity = (Projectile.velocity * 40f + vecToTarget) / 41f;
						}
						else{
							speed = -4f;
							vecToTarget *= speed;
							Projectile.velocity = (Projectile.velocity * 40f + vecToTarget) / 41f;
						}
						break;
					case 2:
						Projectile.position = Vector2.Lerp(Projectile.position, player.position - vecSniper, 0.05f);
						break;
					case 3:
						speed = 13f;
						Projectile.velocity = (Projectile.velocity * 40f + Projectile.Center.DirectionTo(new(target.Center.X, target.Center.Y-300f)) * speed) / 41f;
						break;
					default:
						speed = 10f;
						Projectile.velocity = (Projectile.velocity * 40f + Projectile.Center.DirectionTo(player.position-((player.position-target.position)/2)) * speed) / 41f;
						break;
				}
				Projectile.rotation = Projectile.rotation.AngleTowards(Projectile.AngleTo(target.position), 0.1f);
			}
			else {
				Projectile.rotation = Projectile.rotation.AngleTowards(Projectile.AngleTo(Main.MouseWorld), 0.1f);
				if (distanceToIdlePosition > 20f) {
					speed = 12f;
					vectorToIdlePosition.Normalize();
					vectorToIdlePosition *= speed;
					Projectile.velocity = (Projectile.velocity * (inertia - 1) + vectorToIdlePosition) / inertia;
				}
				else if (Projectile.velocity == Vector2.Zero) {
					Projectile.velocity.X = -0.15f;
					Projectile.velocity.Y = -0.05f;
				}
			}
			float piOv2 = MathHelper.PiOver2;
			Projectile.direction = Projectile.spriteDirection = (Projectile.rotation < piOv2 && Projectile.rotation > -piOv2) ? 1 : -1;

			float overlapVelocity = 2f;
			// Fix overlap with other minions
			for (int i = 0; i < Main.maxProjectiles; i++) {
				Projectile other = Main.projectile[i];

				if (i != Projectile.whoAmI && other.minion && other.active && other.owner == Projectile.owner && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width) {
					if (Projectile.position.X < other.position.X) {
						Projectile.velocity.X -= overlapVelocity;
					}
					else {
						Projectile.velocity.X += overlapVelocity;
					}

					if (Projectile.position.Y < other.position.Y) {
						Projectile.velocity.Y -= overlapVelocity;
					}
					else {
						Projectile.velocity.Y += overlapVelocity;
					}
				}
			}
		}

		private void SearchForTargets(Player owner, out bool foundTarget, out float distanceFromTarget, out NPC target) {
			distanceFromTarget = 700f;
			Vector2 targetCenter = Projectile.position;
			foundTarget = false;
			target = null;

			if (owner.HasMinionAttackTargetNPC) {
				target = Main.npc[owner.MinionAttackTargetNPC];
				float between = Vector2.Distance(target.Center, Projectile.Center);

				if (between < 2000f) {
					distanceFromTarget = between;
					targetCenter = target.Center;
					foundTarget = true;
				}
			}

			if (!foundTarget) {
				for (int i = 0; i < Main.maxNPCs; i++) {
					NPC npc = Main.npc[i];

					if (npc.CanBeChasedBy()) {
						float between = Vector2.Distance(npc.Center, Projectile.Center);
						bool closest = Vector2.Distance(Projectile.Center, targetCenter) > between;
						bool inRange = between < distanceFromTarget;

						if ((closest && inRange) || !foundTarget) {
							distanceFromTarget = between;
							targetCenter = npc.Center;
							target = npc;
							foundTarget = true;
						}
					}
				}
			}
		}

        public override bool PreDraw(ref Color lightColor)
        {
			Texture2D Texture = TextureAssets.Projectile[Projectile.type].Value;

			switch (Projectile.ai[0]){
				case 0:
					Texture = ModContent.Request<Texture2D>($"Terraria/Images/Item_{ItemID.RocketLauncher}", AssetRequestMode.ImmediateLoad).Value;
					break;
				case 1:
					Texture = ModContent.Request<Texture2D>($"Terraria/Images/Item_{ItemID.Shotgun}", AssetRequestMode.ImmediateLoad).Value;
					break;
				case 2:
					Texture = ModContent.Request<Texture2D>($"Terraria/Images/Item_{ItemID.SniperRifle}", AssetRequestMode.ImmediateLoad).Value;
					break;
				case 3:
					Texture = ModContent.Request<Texture2D>($"Terraria/Images/Item_{ItemID.PulseBow}", AssetRequestMode.ImmediateLoad).Value;
					break;
				default:
					Texture = ModContent.Request<Texture2D>($"Terraria/Images/Item_{ItemID.Revolver}", AssetRequestMode.ImmediateLoad).Value;
					break;
			}
			
			SpriteEffects spriteEffects = Projectile.spriteDirection == -1 ? SpriteEffects.FlipVertically : SpriteEffects.None;
			Vector2 drawOrigin = new Vector2(Texture.Width, Texture.Height) / 2f;
			Main.EntitySpriteDraw(Texture, Projectile.position - Main.screenPosition, null, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, spriteEffects);
			
			return false;
        }

        public override void OnKill(int timeLeft)
        {
			if (Projectile.ai[0] >= 4) return;

			Player player = Main.player[Projectile.owner];
            player.GetModPlayer<MyPlayer>().gunSummonSpawnCheck[(int)Projectile.ai[0]] = false;
        }
    }
}
