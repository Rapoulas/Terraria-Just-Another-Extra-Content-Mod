using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TutorialMod.Content.Acessories;
using TutorialMod.Content.Weapons;
using TutorialMod.Content.Projectiles;
using TutorialMod.Content.Buffs;
using Terraria.DataStructures;

namespace TutorialMod{
    public class MyPlayer : ModPlayer {
        #region item field
        public Item hasZetaReference;
		public Item hasMeatShield;
        #endregion

        public bool hasAbsorbTeamDamageEffect;
		public bool defendedByAbsorbTeamDamageEffect;

        public override void ResetEffects(){
            hasAbsorbTeamDamageEffect = false;
			defendedByAbsorbTeamDamageEffect = false;
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

		private void MeatShieldOnHurt(Player.HurtInfo info){
			if (hasMeatShield == null) return;
			
			Player.AddBuff(ModContent.BuffType<MeatShieldDmgRegen>(), 240);
		}

        private void ZetaReferenceOnHurt(Player.HurtInfo info){
            if (hasZetaReference == null) return;

            for (int i = 0; i < 250; i++){
                if (!Main.npc[i].active || Main.npc[i].friendly){
                    continue;
                }
                float j = (Main.npc[i].Center - Player.Center).Length();
                    
                if (j < 200){
                    Main.npc[i].AddBuff(BuffID.Ichor, 300);
                    Main.npc[i].AddBuff(BuffID.Confused, 300);
                }
			}
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
    }
}