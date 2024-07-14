using System;
using Humanizer;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace RappleMod{
    public class EnergyShieldPlayer : ModPlayer {
        public int energyShield = 0;
        public int energyShieldMax = 1000;
        public int energyShieldRate = 200;
        public int energyShieldRecharge = 300;
        public int timeSinceLastHit = 0;
        public int idleTimer = 180;

        public override void ResetEffects()
        {
            energyShieldMax = 1000;
            energyShieldRate = 200; 
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
                    info.Damage = 0;
                }
                else {
                    CombatText.NewText(Player.Hitbox, Color.Cyan, energyShield);
                    info.Damage = info.Damage - energyShield;
                    energyShield = 0;
                    
                }
            }
        }
    }
}