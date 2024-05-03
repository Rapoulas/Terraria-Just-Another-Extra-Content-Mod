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
		int aaa = 0;
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

	// This minion shows a few mandatory things that make it behave properly.
	// Its attack pattern is simple: If an enemy is in range of 43 tiles, it will fly to it and deal contact damage
	// If the player targets a certain NPC with right-click, it will fly through tiles to it
	// If it isn't attacking, it will float near the player with minimal movement
	public class GunSummonMinion : ModProjectile
	{
		public override string Texture => "RappleMod/Content/Projectiles/InvisibleProj";
		public override void SetStaticDefaults() {
			ProjectileID.Sets.MinionTargettingFeature[Projectile.type] = true;

			Main.projPet[Projectile.type] = true;
			ProjectileID.Sets.MinionSacrificable[Projectile.type] = true; 
			ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
		}

		public sealed override void SetDefaults() {
			Projectile.width = 18;
			Projectile.height = 28;
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

			GeneralBehavior(owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition);
			SearchForTargets(owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
			Movement(foundTarget, distanceFromTarget, targetCenter, distanceToIdlePosition, vectorToIdlePosition);
		}

		// This is the "active check", makes sure the minion is alive while the player is alive, and despawns if not
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

		private void GeneralBehavior(Player owner, out Vector2 vectorToIdlePosition, out float distanceToIdlePosition) {
			Vector2 idlePosition = owner.Center;

			// Teleport to player if distance is too big
			vectorToIdlePosition = idlePosition - Projectile.Center;
			distanceToIdlePosition = vectorToIdlePosition.Length();

			if (Main.myPlayer == owner.whoAmI && distanceToIdlePosition > 2000f) {
				Projectile.position = idlePosition;
				Projectile.velocity *= 0.1f;
				Projectile.netUpdate = true;
			}
			
			switch (Projectile.ai[0]){
				case 0:
					idlePosition.Y -= 160f;
					Projectile.position = Vector2.Lerp(Projectile.position, idlePosition, 0.15f);
					break;
				case 1:
					idlePosition.X -= 160f;
					Projectile.position = Vector2.Lerp(Projectile.position, idlePosition, 0.15f);
					break;
				case 2:
					idlePosition.Y += 160f;
					Projectile.position = Vector2.Lerp(Projectile.position, idlePosition, 0.15f);
					break;
				default:
					idlePosition.X += 160f;

					float minionPositionOffsetX = 10 + (Projectile.minionPos-3) * 40;
					idlePosition.X += minionPositionOffsetX;

					Projectile.position = Vector2.Lerp(Projectile.position, idlePosition, 0.15f);
					break;
			}
		}

		private void SearchForTargets(Player owner, out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter) {
			distanceFromTarget = 700f;
			targetCenter = Projectile.position;
			foundTarget = false;

			if (owner.HasMinionAttackTargetNPC) {
				NPC npc = Main.npc[owner.MinionAttackTargetNPC];
				float between = Vector2.Distance(npc.Center, Projectile.Center);

				if (between < 2000f) {
					distanceFromTarget = between;
					targetCenter = npc.Center;
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
						bool lineOfSight = Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, npc.position, npc.width, npc.height);
						// Additional check for this specific minion behavior, otherwise it will stop attacking once it dashed through an enemy while flying though tiles afterwards
						// The number depends on various parameters seen in the movement code below. Test different ones out until it works alright
						bool closeThroughWall = between < 100f;

						if (((closest && inRange) || !foundTarget) && (lineOfSight || closeThroughWall)) {
							distanceFromTarget = between;
							targetCenter = npc.Center;
							foundTarget = true;
						}
					}
				}
			}
		}

		private void Movement(bool foundTarget, float distanceFromTarget, Vector2 targetCenter, float distanceToIdlePosition, Vector2 vectorToIdlePosition) {
			Player player = Main.player[Projectile.owner];
			Vector2 velocity;

			if (foundTarget)
				velocity = Projectile.Center.DirectionTo(targetCenter);
			else 
				velocity = Projectile.Center.DirectionTo(Main.MouseWorld);

			velocity.Normalize();
			Projectile.velocity = Vector2.Lerp(Projectile.velocity, velocity, 0.05f);
			Projectile.rotation = Projectile.velocity.ToRotation();
			Projectile.direction = Projectile.spriteDirection = (Projectile.velocity.X > 0f) ? 1 : -1;
		}

        public override bool PreDraw(ref Color lightColor)
        {
			Texture2D Texture = TextureAssets.Projectile[Projectile.type].Value;

			switch (Projectile.ai[0]){
				case 0:
					Texture = ModContent.Request<Texture2D>($"Terraria/Images/Item_{ItemID.RocketLauncher}", AssetRequestMode.ImmediateLoad).Value;
					break;
				case 1:
					Texture =ModContent.Request<Texture2D>($"Terraria/Images/Item_{ItemID.Shotgun}", AssetRequestMode.ImmediateLoad).Value;
					break;
				case 2:
					Texture = ModContent.Request<Texture2D>($"Terraria/Images/Item_{ItemID.SniperRifle}", AssetRequestMode.ImmediateLoad).Value;
					break;
				default:
					Texture = ModContent.Request<Texture2D>($"Terraria/Images/Item_{ItemID.Revolver}", AssetRequestMode.ImmediateLoad).Value;
					break;
			}
			
			Vector2 drawOrigin = new Vector2(Texture.Width, Texture.Height) / 2f;
			if (Projectile.direction == 1)
				Main.EntitySpriteDraw(Texture, Projectile.position - Main.screenPosition, null, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None);
			else
				Main.EntitySpriteDraw(Texture, Projectile.position - Main.screenPosition, null, lightColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.FlipVertically);
            
			return false;
        }

        public override void OnKill(int timeLeft)
        {
			if (Projectile.ai[0] >= 3) return;

			Player player = Main.player[Projectile.owner];
            player.GetModPlayer<MyPlayer>().gunSummonSpawnCheck[(int)Projectile.ai[0]] = false;
        }
    }
}
