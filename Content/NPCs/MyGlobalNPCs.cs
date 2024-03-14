using System;
using Microsoft.Xna.Framework;
using RappleMod.Content.Buffs;
using RappleMod.Content.SetBonusChanges;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RappleMod.Content.NPCs
{
    public class MyGlobalNPCs : GlobalNPC
    {
        readonly Player player =  Main.LocalPlayer;

        public bool onFrostburnCopy;
        public bool onFrostbiteCopy;

        public override void ResetEffects(NPC npc)
        {
            onFrostburnCopy = false;
            onFrostbiteCopy = false;
        }
        public override void UpdateLifeRegen(NPC npc, ref int damage){
            
            if (npc.HasBuff<FrostburnCopy>()){
                npc.lifeRegen -= 16;
                if (damage < 2)
                    damage = 2;
            }

            if (npc.HasBuff<FrostbiteCopy>()){
                npc.lifeRegen -= 50;
                if (damage < 10)
                    damage = 10;
            }

            if (player.GetModPlayer<SetBonusChangesPlayer>().RangedCobaltPalladiumFrostSet || player.GetModPlayer<SetBonusChangesPlayer>().MeleeCobaltPalladiumFrostSet || player.GetModPlayer<SetBonusChangesPlayer>().MeleeOrichalcumMythrilFrostSet || player.GetModPlayer<SetBonusChangesPlayer>().RangedOrichalcumMythrilFrostSet || player.GetModPlayer<SetBonusChangesPlayer>().MeleeTitaniumAdamantiteFrostSet || player.GetModPlayer<SetBonusChangesPlayer>().RangedTitaniumAdamantiteFrostSet){
                if (npc.onFrostBurn || npc.HasBuff<FrostburnCopy>())
                {
                    int FrostburnBaseDotHalf = (int)(16 * 0.5);
                    npc.lifeRegen -= FrostburnBaseDotHalf;
                    if (damage < FrostburnBaseDotHalf / 4)
                        damage = FrostburnBaseDotHalf / 4;
                }

                if (npc.onFrostBurn2 || npc.HasBuff<FrostbiteCopy>())
                {
                    int FrostbiteBaseDotHalf = (int)(50 * 0.5);
                    npc.lifeRegen -= FrostbiteBaseDotHalf;
                    if (damage < FrostbiteBaseDotHalf / 4)
                        damage = FrostbiteBaseDotHalf / 4;
                }
            }
        }

        public override bool InstancePerEntity => true;

        public override void AI(NPC npc)
        {
            if (npc.HasBuff<FrostburnCopy>() || npc.HasBuff<FrostbiteCopy>()){
                if (Main.rand.Next(4) < 3)
                {
                    Dust dust = Dust.NewDustDirect(new Vector2(npc.position.X - 2f, npc.position.Y - 2f), npc.width + 4, npc.height + 4, DustID.IceTorch, npc.velocity.X * 0.4f, npc.velocity.Y * 0.4f, 100, default, 3.5f);
                    dust.noGravity = true;
                    dust.velocity *= 1.8f;
                    dust.velocity.Y -= 0.5f;
                    if (Main.rand.NextBool(4))
                    {
                        dust.noGravity = false;
                        dust.scale *= 0.5f;
                    }
                }
                Lighting.AddLight((int)(npc.position.X / 16f), (int)(npc.position.Y / 16f + 1f), 0.1f, 0.6f, 1f);
            }

            if (npc.onFrostBurn && npc.HasBuff<FrostburnCopy>()) {
                int FrostburnIndex = npc.FindBuffIndex(ModContent.BuffType<FrostburnCopy>());
                npc.DelBuff(FrostburnIndex);
            }
            if (npc.onFrostBurn2 && npc.HasBuff<FrostbiteCopy>()) {
                int FrostbiteIndex = npc.FindBuffIndex(ModContent.BuffType<FrostbiteCopy>());
                npc.DelBuff(FrostbiteIndex);
            }

            if ((npc.onFrostBurn || npc.onFrostBurn2) && player.GetModPlayer<SetBonusChangesPlayer>().MeleeCobaltPalladiumFrostSet == true){
                NPC closestNPC = FindClosestNPC(96, npc.Center);

                if (closestNPC != null && Main.rand.NextBool(25)){
                    closestNPC.AddBuff(ModContent.BuffType<FrostburnCopy>(), 60 * Main.rand.Next(3, 6));
                }

                if (closestNPC != null && Main.rand.NextBool(25)){
                    closestNPC.AddBuff(ModContent.BuffType<FrostbiteCopy>(), 60 * Main.rand.Next(3, 6));
                }
            }
        }

        public override void OnKill(NPC npc)
        {
            if (player.GetModPlayer<SetBonusChangesPlayer>().MeleeOrichalcumMythrilFrostSet){
                if (npc.onFrostBurn || npc.HasBuff<FrostburnCopy>() || npc.onFrostBurn2 || npc.HasBuff<FrostbiteCopy>()){
                    foreach (NPC target in Main.npc){
                        if (target.Center.Distance(npc.Center) < 128){
                            player.ApplyDamageToNPC(target, npc.lifeMax/4, 3, 1);
                            if (Main.rand.NextBool(8)) target.AddBuff(BuffID.Frostburn, 60 * Main.rand.Next(1, 4));
                            if (Main.rand.NextBool(8)) target.AddBuff(BuffID.Frostburn2, 60 * Main.rand.Next(1, 4));
                        }
                    }
                    for (int i = 0; i < 50; i++){
                        Vector2 dustVelocity = new(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1));
                        dustVelocity.Normalize();
                        dustVelocity *= 10;

                        Dust.NewDust(npc.Center, 1, 1, DustID.IceRod, dustVelocity.X, dustVelocity.Y, 0, Color.LightBlue, 1f);
                    }
                }
            }
        }

        public static NPC FindClosestNPC(float maxDetectDistance, Vector2 position) {
			NPC closestNPC = null;

			float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

			for (int k = 0; k < Main.maxNPCs; k++) {
				NPC target = Main.npc[k];
				if (target.CanBeChasedBy() && !target.onFrostBurn && !target.onFrostBurn2 && (!target.HasBuff<FrostbiteCopy>() || !target.HasBuff<FrostburnCopy>() )) {
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