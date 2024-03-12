using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RappleMod.Content.Projectiles;
using RappleMod.Content.Buffs;

namespace RappleMod{

    public class SetBonusChangesPlayer : ModPlayer{

        public bool NecroFossilSet;
		public bool RangedCobaltPalladiumFrostSet;
		public bool MeleeCobaltPalladiumFrostSet;
		public bool MeleeOrichalcumMythrilFrostSet;
		public bool RangedOrichalcumMythrilFrostSet;
		public int maxHitCountRangedOMFSet;

        public override void ResetEffects(){
			NecroFossilSet = false;
			RangedCobaltPalladiumFrostSet = false;
			MeleeCobaltPalladiumFrostSet = false;
			MeleeOrichalcumMythrilFrostSet = false;
			RangedOrichalcumMythrilFrostSet = false;
			maxHitCountRangedOMFSet = 0;
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
			Player player = Main.LocalPlayer;
			
			if (proj.DamageType == DamageClass.Ranged && hit.Crit && NecroFossilSet && proj.type != ModContent.ProjectileType<NecroFossilBone>()){
				for (int i = 0; i < 3; i++){
					Vector2 velocity = new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 0));
					velocity.Normalize();
					velocity *= Main.rand.NextFloat(10, 15);
					float newDamage = hit.Damage/5;
					
					Projectile.NewProjectile(proj.GetSource_FromThis(), target.Center, velocity, ModContent.ProjectileType<NecroFossilBone>(), (int)newDamage, hit.Knockback, player.whoAmI, 0, target.whoAmI, target.whoAmI);
				}
				target.AddBuff(BuffID.Slow, 120);
			}

			if (proj.DamageType == DamageClass.Ranged && RangedOrichalcumMythrilFrostSet && Main.rand.NextBool(4)){
				if (target.onFrostBurn || target.onFrostBurn2 || target.HasBuff<FrostburnCopy>() || target.HasBuff<FrostbiteCopy>()){
					foreach (NPC npc in Main.npc){
                        if (npc.Center.Distance(target.Center) < 96 && maxHitCountRangedOMFSet <= 5){
                            player.ApplyDamageToNPC(npc, damageDone, 0, 0);
							maxHitCountRangedOMFSet++;
                        }
                    }
				}
			}
        }

        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {	
			if (MeleeCobaltPalladiumFrostSet && Main.rand.NextBool(5)){
				target.AddBuff(BuffID.Frostburn, 60 * Main.rand.Next(3, 6));
			}
			if (MeleeCobaltPalladiumFrostSet && Main.rand.NextBool(5)){
				target.AddBuff(BuffID.Frostburn2, 60 * Main.rand.Next(3, 6));
			}
        }

        public override void PostUpdateEquips()
        {
			if (RangedCobaltPalladiumFrostSet){
				for (int i = 0; i < Main.maxProjectiles; i++){
					if (Main.projectile[i].owner == Main.myPlayer){
						NPC closestNPC = FindClosestNPC(96, Main.projectile[i].Center);
						if (closestNPC != null){
							closestNPC.AddBuff(BuffID.Frostburn, 120);
							closestNPC.AddBuff(BuffID.Frostburn2, 120);
						}
					}
				}
			}
        }

        public static NPC FindClosestNPC(float maxDetectDistance, Vector2 position) {
			NPC closestNPC = null;

			float sqrMaxDetectDistance = maxDetectDistance * maxDetectDistance;

			for (int k = 0; k < Main.maxNPCs; k++) {
				NPC target = Main.npc[k];
				if (target.CanBeChasedBy()) {
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