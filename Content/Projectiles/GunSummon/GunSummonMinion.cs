using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RappleMod.Content.Weapons;
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
		public static Item FalsePistol = null;
		public static Item FalseLauncher = null;
		public static Item FalseBow = null;
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

		public override bool? CanCutTiles() {
			return false;
		}

        public override bool? CanDamage()
        {
            return false;
        }
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
					Projectile.width = 60;
					Projectile.height = 30;
					break;
				case 1:
					Projectile.width = 44;
					Projectile.height = 20;
					break;
				case 2:
					Projectile.width = 70;
					Projectile.height = 22;
					break;
				case 3:
					Projectile.width = 26;
					Projectile.height = 42;
					break;
				default:
					Projectile.width = 48;
					Projectile.height = 24;
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

            FalsePistol = new Item();
            FalseLauncher = new Item();
			FalseBow = new Item();

            FalsePistol.SetDefaults(ItemID.Revolver, true);
            FalseLauncher.SetDefaults(ItemID.SnowmanCannon, true);
            FalseBow.SetDefaults(ItemID.WoodenBow, true);

            FalsePistol.damage = 0;
            FalsePistol.knockBack = 4;

            FalseLauncher.damage = 0;
			FalseLauncher.knockBack = 4;

            FalseBow.damage = 0;
			FalseBow.knockBack = 4;

			if (foundTarget){
				switch (Projectile.ai[0]){
					case 0:
						if (timer % (150 - MathHelper.Min(amountPistol*2, 120)) == 0)
							if (owner.PickAmmo(FalseLauncher, out int projToShoot, out float projSpeed, out int damage, out float knockBack, out int usedAmmoItemId)){
								// Launcher
								int newDamage = damage + 65;
								float mult = 1;
								if (owner.head == 105) mult = 1.15f;
								ShootingHandler(target, owner, 1, projSpeed, projToShoot, knockBack, 0, 0, 0, (int)(newDamage * mult));
								timer = 0;
							}
						break;
					case 1:
					if (timer % (65 - MathHelper.Min(amountPistol*2, 45)) == 0)
							if (owner.PickAmmo(FalsePistol, out int projToShoot, out float projSpeed, out int damage, out float knockBack, out int usedAmmoItemId)){
								// Shotgun
								int newDamage = damage + 25;
								float mult = 1;
								if (owner.head == 104) mult = 1.15f;
								ShootingHandler(target, owner, 8, projSpeed, projToShoot, knockBack, 35, 2.5f, 0, (int)(newDamage * mult));
								timer = 0;
							}
						break;
					case 2:
						if (timer % (120 - MathHelper.Min(amountPistol*2, 100)) == 0)
							if (owner.PickAmmo(FalsePistol, out int projToShoot, out float projSpeed, out int damage, out float knockBack, out int usedAmmoItemId)){
								// Sniper
								int newDamage = damage + 135;
								float mult = 1;
								if (owner.head == 104) mult = 1.15f;
								ShootingHandler(target, owner, 1, projSpeed * 3f, projToShoot, knockBack, 0, 0, 2, (int)(newDamage * mult));
								timer = 0;
							}
						break;
					case 3:
						if (timer % (65 - MathHelper.Min(amountPistol*2, 45)) == 0)
							if (owner.PickAmmo(FalseBow, out int projToShoot, out float projSpeed, out int damage, out float knockBack, out int usedAmmoItemId)){
								// Bow
								int newDamage = damage + 40;
								float mult = 1;
								if (owner.head == 103) mult = 1.15f;
								ShootingHandler(target, owner, 1, projSpeed, projToShoot, knockBack, 0, 0, 0, (int)(newDamage * mult));
								timer = 0;
							}
						break;
					default:
						if (timer % (45 - MathHelper.Min(amountPistol, 30)) == 0)
							if (owner.PickAmmo(FalsePistol, out int projToShoot, out float projSpeed, out int damage, out float knockBack, out int usedAmmoItemId)){
								// Revolver
								int newDamage = damage + 20;
								float mult = 1;
								if (owner.head == 104) mult = 1.15f;
								ShootingHandler(target, owner, 1, projSpeed * 1.25f, projToShoot, knockBack, 0, 0, 0, (int)(newDamage * mult));
								timer = 0;
							}
						break;
				}
			}
		}

		private void ShootingHandler(NPC target, Player owner, int amountBullets, float projSpeedMultiplier, int projToShoot, float knockBack, float degreesRotation, float speedVariation, int amountPenetration, int damage){
			float baseBonus = owner.GetDamage(DamageClass.Summon).Base;
			float addBonus = owner.GetDamage(DamageClass.Summon).Additive;
			float multBonus = owner.GetDamage(DamageClass.Summon).Multiplicative;
			float flatBonus = owner.GetDamage(DamageClass.Summon).Flat;

			float finalDamage = ((damage + baseBonus) * addBonus * multBonus) + flatBonus;
			
			for (int i = 0; i < amountBullets; i++){
				Vector2 velocity = Projectile.Center.DirectionTo(target.Center).RotatedByRandom(MathHelper.ToRadians(degreesRotation));
				Projectile proj = Projectile.NewProjectileDirect(owner.GetSource_FromThis(), Projectile.position, velocity * (projSpeedMultiplier + Main.rand.NextFloat(-speedVariation, speedVariation)), projToShoot, (int)finalDamage, knockBack);
				proj.penetrate += amountPenetration;
				proj.tileCollide = false;
				proj.minion = true;
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
			for (int i = 0; i < Main.maxProjectiles; i++) {
				Projectile other = Main.projectile[i];

				if (i != Projectile.whoAmI && other.minion && other.active && other.owner == Projectile.owner && Math.Abs(Projectile.position.X - other.position.X) + Math.Abs(Projectile.position.Y - other.position.Y) < Projectile.width && other.DamageType == DamageClass.Summon) {
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

            Texture = Projectile.ai[0] switch
            {
                0 => ModContent.Request<Texture2D>("RappleMod/Content/Projectiles/GunSummon/GunSummonLauncher", AssetRequestMode.ImmediateLoad).Value,
                1 => ModContent.Request<Texture2D>("RappleMod/Content/Projectiles/GunSummon/GunSummonShotgun", AssetRequestMode.ImmediateLoad).Value,
                2 => ModContent.Request<Texture2D>("RappleMod/Content/Projectiles/GunSummon/GunSummonSniper", AssetRequestMode.ImmediateLoad).Value,
                3 => ModContent.Request<Texture2D>("RappleMod/Content/Projectiles/GunSummon/GunSummonBow", AssetRequestMode.ImmediateLoad).Value,
                _ => ModContent.Request<Texture2D>("RappleMod/Content/Projectiles/GunSummon/GunSummonRevolver", AssetRequestMode.ImmediateLoad).Value,
            };
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
