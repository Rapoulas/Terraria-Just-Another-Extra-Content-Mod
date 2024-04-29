using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RappleMod.Content.Acessories;
using RappleMod.Content.Buffs;
using Terraria.DataStructures;
using RappleMod.Content.Weapons;
using RappleMod.Content.Projectiles.PurgatoryProjectile;

namespace RappleMod{
    public class MyPlayer : ModPlayer {
        #region item field
        public Item hasZetaReference;
		public Item hasMeatShield;
		public Item hasBuffer;
		public Item hasOuroboros;
		public Item hasPurgatory;
        #endregion

        public bool hasAbsorbTeamDamageEffect;
		public bool defendedByAbsorbTeamDamageEffect;
		public bool isHoldingHawkEye;
		public int anarchistCookbookMode;
		public float amountEnemiesOnFire;
		public int counter;
		public float anarchistCookbookCounter = 0;
		public float hitClass;
		public int deliveranceAmmo;
		public float deliveranceUICounter;
        public override void ResetEffects(){
            hasAbsorbTeamDamageEffect = false;
			defendedByAbsorbTeamDamageEffect = false;
			isHoldingHawkEye = false;
			hasZetaReference = null;
			hasMeatShield = null;
			hasBuffer = null;
			hasOuroboros = null;
			hasPurgatory = null;
			hitClass = 0;
        }

        public override void ModifyHurt(ref Player.HurtModifiers modifiers) {
			if (defendedByAbsorbTeamDamageEffect && Player == Main.LocalPlayer && TeammateCanAbsorbDamage()) {
				modifiers.FinalDamage *= 1f - MeatShield.DamageAbsorptionMultiplier;
			}
		}
        
        public override void OnHurt(Player.HurtInfo info) {
            ZetaReferenceOnHurt(info);
			MeatShieldOnHurt(info);

            Player localPlayer = Main.LocalPlayer;
			if (defendedByAbsorbTeamDamageEffect && Player != localPlayer && IsClosestShieldWearerInRange(localPlayer, Player.Center, Player.team)) {
				float percent = MeatShield.DamageAbsorptionMultiplier;
				int damage = (int)(info.Damage * (percent / (1 - percent)));

				// Don't bother pinging the defending player and upsetting their immunity frames if the portion of damage we're taking rounds down to 0
				if (damage > 0) {
					localPlayer.Hurt(PlayerDeathReason.LegacyEmpty(), damage, 0);
				}
			}
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {	
            PurgatoryOnKill(target, hit, damageDone);
        }

        public override bool Shoot(Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			if (hasOuroboros != null){
				OuroborosEffect();
			}
            return base.Shoot(item, source, position, velocity, type, damage, knockback);
        }

		private void PurgatoryOnKill(NPC target, NPC.HitInfo hit, int damageDone){
			if (hasPurgatory == null){
				return;
			}

			if (hit.DamageType == DamageClass.Melee) hitClass = 1;
			else if (hit.DamageType == DamageClass.Magic) hitClass = 2;
			else if (hit.DamageType == DamageClass.Ranged) hitClass = 3;
			else if (hit.DamageType == DamageClass.Summon) hitClass = 4;
			else hitClass = 5;
			
			Player player = Main.LocalPlayer;
			target.GetLifeStats(out int currentHp, out int maxHp);
			
            if (!target.active && Main.rand.NextBool(5)){
				Projectile.NewProjectile(player.GetSource_FromThis(), target.Center, Vector2.Zero, ModContent.ProjectileType<PurgatoryProjectile>(), 0-(int)(currentHp*0.5f), 0, player.whoAmI, 0, hitClass, 1);
			}
		}

        private void MeatShieldOnHurt(Player.HurtInfo info){
			if (hasMeatShield == null) return;
			
			Player.AddBuff(ModContent.BuffType<MeatShieldDmgRegen>(), 240);
		}

        private void ZetaReferenceOnHurt(Player.HurtInfo info){
            if (hasZetaReference == null) return;

            for (int i = 0; i < 250; i++){
                NPC closestNPC = FindClosestNPC(112);
				if (closestNPC != null){
					closestNPC.AddBuff(BuffID.Ichor, 300);
					closestNPC.AddBuff(BuffID.Confused, 300);
				}
			}
		}
		
		private void OuroborosEffect(){
			for (int i = 0; i < Main.maxNPCs; i++){
				if (Main.npc[i].HasBuff(BuffID.OnFire) || Main.npc[i].HasBuff(BuffID.OnFire3))
					counter++;
			}
			amountEnemiesOnFire = counter;
			counter = 0;
		}

        private bool TeammateCanAbsorbDamage() {
			for (int i = 0; i < Main.maxPlayers; i++) {
				Player otherPlayer = Main.player[i];
				if (i != Main.myPlayer && IsAbleToAbsorbDamageForTeammate(otherPlayer, Player.team)) {
					return true;
				}
			}
			return false;
		}

		private static bool IsAbleToAbsorbDamageForTeammate(Player player, int team) {
			return player.active
				&& !player.dead
				&& !player.immune // This check can be removed, allowing players to take hits for team-mates in quick succession. Removing it can also help with de-syncs where the player getting hurt thinks there is no-one to tank the damage, but by the time the hit arrives on the player with the shield, they take extra damage
				&& player.GetModPlayer<MyPlayer>().hasAbsorbTeamDamageEffect
				&& player.team == team
				&& player.statLife > player.statLifeMax2 * MeatShield.DamageAbsorptionAbilityLifeThreshold;
		}

        private static bool IsClosestShieldWearerInRange(Player player, Vector2 target, int team) {
			if (!IsAbleToAbsorbDamageForTeammate(player, team)) {
				return false;
			}

			float distance = player.Distance(target);
			if (distance > MeatShield.DamageAbsorptionRange) {
				return false; // player is out of range, so can't take the hit
			}

			for (int i = 0; i < Main.maxPlayers; i++) {
				Player otherPlayer = Main.player[i];
				if (i != Main.myPlayer && IsAbleToAbsorbDamageForTeammate(otherPlayer, team)) {
					float otherPlayerDistance = otherPlayer.Distance(target);
					if (distance > otherPlayerDistance || (distance == otherPlayerDistance && i < Main.myPlayer)) {
						return false;
					}
				}
			}

			return true;
		}

		public NPC FindClosestNPC(float maxDetectDistance) {
			NPC closestNPC = null;

			float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

			for (int k = 0; k < Main.maxNPCs; k++) {
				NPC target = Main.npc[k];
				if (target.CanBeChasedBy() && !target.HasBuff(BuffID.Ichor)) {
					float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, Player.Center);

					if (sqrDistanceToTarget < sqrMaxDetectDistance) {
						sqrMaxDetectDistance = sqrDistanceToTarget;
						closestNPC = target;
					}
				}
			}
            return closestNPC;
        }
    }
}