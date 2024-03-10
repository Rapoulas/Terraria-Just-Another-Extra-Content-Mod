using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RappleMod.Content.NPCs
{
    public class MyGlobalNPCs : GlobalNPC
    {
        readonly Player player =  Main.LocalPlayer;
        public override void UpdateLifeRegen(NPC npc, ref int damage){
            

            if (npc.onFrostBurn && (player.GetModPlayer<SetBonusChangesPlayer>().RangedCobaltPalladiumFrostSet == true || player.GetModPlayer<SetBonusChangesPlayer>().MeleeCobaltPalladiumFrostSet == true))
            {
                
                int FrostburnBaseDot = (int)(16 * 0.5);
                npc.lifeRegen -= FrostburnBaseDot;
                if (damage < FrostburnBaseDot / 4)
                    damage = FrostburnBaseDot / 4;
            }

            if (npc.onFrostBurn2 && (player.GetModPlayer<SetBonusChangesPlayer>().RangedCobaltPalladiumFrostSet == true || player.GetModPlayer<SetBonusChangesPlayer>().MeleeCobaltPalladiumFrostSet == true))
            {
                int FrostbiteBaseDot = (int)(50 * 0.5);
                npc.lifeRegen -= FrostbiteBaseDot;
                if (damage < FrostbiteBaseDot / 4)
                    damage = FrostbiteBaseDot / 4;
            }
        }

        public override bool InstancePerEntity => true;

        public override void AI(NPC npc)
        {
            if ((npc.onFrostBurn || npc.onFrostBurn2) && player.GetModPlayer<SetBonusChangesPlayer>().MeleeCobaltPalladiumFrostSet == true){
                NPC closestNPC = FindClosestNPC(64, npc.Center);
                if (closestNPC != null && Main.rand.Next(0, 100) <= 2){
                    closestNPC.AddBuff(BuffID.Frostburn, 60);
                }

                if (closestNPC != null && Main.rand.Next(0, 100) <= 2){
                    closestNPC.AddBuff(BuffID.Frostburn2, 60);
                }
            }
        }

        public static NPC FindClosestNPC(float maxDetectDistance, Vector2 position) {
			NPC closestNPC = null;

			float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

			for (int k = 0; k < Main.maxNPCs; k++) {
				NPC target = Main.npc[k];
				if (target.CanBeChasedBy() && (!target.onFrostBurn || !target.onFrostBurn2)) {
					float sqrDistanceToTarget = Vector2.DistanceSquared(target.Center, position);

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