using System;
using Humanizer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace RappleMod{
    public class EnergyShieldPlayer : ModPlayer {
        public int energyShield = 0;
        public int energyShieldMax = 100;
        public int energyShieldRate = 6;
        public int energyShieldRecharge = 300;
        public int timeSinceLastHit = 0;
        public bool dodgeHitAbsorbed = false;

        public override void ResetEffects()
        {
            energyShieldMax = 100;
            energyShieldRate = 6; 
            energyShieldRecharge = 300;
        }

        public override void UpdateBadLifeRegen()
        {
            if (energyShieldMax > 0){
                timeSinceLastHit++;
            }
            if (energyShieldMax == 0){
                timeSinceLastHit = 0;
            }

            if (timeSinceLastHit >= energyShieldRecharge){
                if (energyShield < energyShieldMax){
                    int rateInt = energyShieldRate / 60;

					energyShield += Math.Min(rateInt, energyShieldMax - energyShield);

					if (energyShieldRate % 60 != 0){
						int rateSubDelay = 60 / (energyShieldRate % 60);

						if (timeSinceLastHit % rateSubDelay == 0 && energyShield < energyShieldMax)
							energyShield++;
					}
                }
            }
        }

        public override bool FreeDodge(Player.HurtInfo info){
            if (dodgeHitAbsorbed){
                dodgeHitAbsorbed = false;
                return true;
            }
            return base.FreeDodge(info);
        }

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (energyShield > 0){
                modifiers.ModifyHurtInfo += ReduceBarrier;
            }
            
            timeSinceLastHit = 0;
        }

        public void ReduceBarrier(ref Player.HurtInfo info){
            if (energyShield > 0){
                if (energyShield > info.Damage){
                    CombatText.NewText(Player.Hitbox, Color.Cyan, info.Damage);
                    energyShield -= info.Damage;
                    dodgeHitAbsorbed = true;

                    if (info.Knockback != 0f && info.HitDirection != 0 && (!Player.mount.Active || !Player.mount.Cart)){
                        Player.velocity.X = info.Knockback * (float)info.HitDirection;
                        Player.velocity.Y = info.Knockback * -7f / 9f;
                        Player.fallStart = (int)(Player.position.Y / 16f);
                    }
                }
                else {
                    CombatText.NewText(Player.Hitbox, Color.Cyan, energyShield);
                    info.Damage -= energyShield;
                    energyShield = 0;
                }

                GiveInvincibility(Player, Player.longInvince ? 100 : 60, true);
            }
        }

        public static bool GiveInvincibility(Player player, int frames, bool blink = false){
            //Thanks to Calamity Mod for this

            bool anyIFramesWouldBeGiven = false;
            for (int i = 0; i < player.hurtCooldowns.Length; ++i)
                if (player.hurtCooldowns[i] < frames)
                    anyIFramesWouldBeGiven = true;

            if (!anyIFramesWouldBeGiven)
                return false;

            player.immune = true;
            player.immuneNoBlink = !blink;
            player.immuneTime = frames;
            for (int i = 0; i < player.hurtCooldowns.Length; ++i)
                if (player.hurtCooldowns[i] < frames)
                    player.hurtCooldowns[i] = frames;

            return true;
        }
    }
}