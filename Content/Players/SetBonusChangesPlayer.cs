using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RappleMod.Content.Projectiles;
using RappleMod.Content.Buffs;
using RappleMod.Content.Projectiles.DryadWardCopy;

namespace RappleMod{

    public class SetBonusChangesPlayer : ModPlayer{

        public bool NecroFossilSet;
		public bool RangedCobaltPalladiumFrostSet;
		public bool MeleeCobaltPalladiumFrostSet;
		public bool MeleeOrichalcumMythrilFrostSet;
		public bool RangedOrichalcumMythrilFrostSet;
		public bool MeleeTitaniumAdamantiteFrostSet;
		public bool RangedTitaniumAdamantiteFrostSet;
		public bool MeleeHallowedChlorophyteSet;
		public bool MeleeHCSetReapply = false;
		public int maxHitCountRangedOMFSet;
		public int timer;

        public override void ResetEffects(){
			NecroFossilSet = false;
			RangedCobaltPalladiumFrostSet = false;
			MeleeCobaltPalladiumFrostSet = false;
			MeleeOrichalcumMythrilFrostSet = false;
			RangedOrichalcumMythrilFrostSet = false;
			MeleeTitaniumAdamantiteFrostSet = false;
			RangedTitaniumAdamantiteFrostSet = false;
			MeleeHallowedChlorophyteSet = false;
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
                        if (npc.Center.Distance(target.Center) < 144 && maxHitCountRangedOMFSet <= 5){
                            player.ApplyDamageToNPC(npc, damageDone, 0, 0);
							maxHitCountRangedOMFSet++;
							for (int i = 0; i < 3; i++) Dust.NewDust(npc.Center, 1, 1, DustID.IceRod, 0, 0, 0, Color.LightBlue, 1f);
                        }
                    }
				}
			}

			if (MeleeCobaltPalladiumFrostSet && Main.rand.NextBool(5) && (hit.DamageType == DamageClass.Melee || hit.DamageType == DamageClass.MeleeNoSpeed)){
				target.AddBuff(BuffID.Frostburn, 60 * Main.rand.Next(3, 6));
			}
			if (MeleeCobaltPalladiumFrostSet && Main.rand.NextBool(5) && (hit.DamageType == DamageClass.Melee || hit.DamageType == DamageClass.MeleeNoSpeed)){
				target.AddBuff(BuffID.Frostburn2, 60 * Main.rand.Next(3, 6));
			}

			if (MeleeTitaniumAdamantiteFrostSet && (hit.DamageType == DamageClass.Melee || hit.DamageType == DamageClass.MeleeNoSpeed)){
				ShardStormGenerate(target);
			}

			if (RangedTitaniumAdamantiteFrostSet && hit.DamageType == DamageClass.Ranged){
				ShardStormGenerate(target);
			}

			if (proj.type == 908 && (RangedTitaniumAdamantiteFrostSet || MeleeTitaniumAdamantiteFrostSet)){
				target.AddBuff(BuffID.Frostburn, 60 * Main.rand.Next(3, 6));
				target.AddBuff(BuffID.Frostburn2, 60 * Main.rand.Next(3, 6));
			}

			if (MeleeHallowedChlorophyteSet && (hit.DamageType == DamageClass.Melee || hit.DamageType == DamageClass.MeleeNoSpeed)){
				if (player.ownedProjectileCounts[ModContent.ProjectileType<DryadWardCopy>()] <= 0)
					Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<DryadWardCopy>(), 50, 0, player.whoAmI, 0, player.whoAmI, 1);
				else MeleeHCSetReapply = true;
			}

			if (MeleeHallowedChlorophyteSet && (hit.DamageType == DamageClass.Ranged)){
				if (player.ownedProjectileCounts[ModContent.ProjectileType<DryadWardCopy>()] <= 0)
					Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<DryadWardCopy>(), 50, 0, player.whoAmI, 0, target.whoAmI, 0);
				else MeleeHCSetReapply = true;
			}
        }

        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {	
			Player player = Main.LocalPlayer;

			if (MeleeCobaltPalladiumFrostSet && Main.rand.NextBool(5)){
				target.AddBuff(BuffID.Frostburn, 60 * Main.rand.Next(3, 6));
			}
			if (MeleeCobaltPalladiumFrostSet && Main.rand.NextBool(5)){
				target.AddBuff(BuffID.Frostburn2, 60 * Main.rand.Next(3, 6));
			}

			if (MeleeTitaniumAdamantiteFrostSet && (hit.DamageType == DamageClass.Melee || hit.DamageType == DamageClass.MeleeNoSpeed)){
				ShardStormGenerate(target);
			}

			if (MeleeHallowedChlorophyteSet && (hit.DamageType == DamageClass.Melee || hit.DamageType == DamageClass.MeleeNoSpeed)){
				if (player.ownedProjectileCounts[ModContent.ProjectileType<DryadWardCopy>()] <= 0)
					Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<DryadWardCopy>(), 50, 0, player.whoAmI, 0, player.whoAmI, player.whoAmI);
				else MeleeHCSetReapply = true;
			}
        }

        public override void PostUpdateEquips()
        {
			Player player = Main.LocalPlayer;

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

			if (RangedTitaniumAdamantiteFrostSet){
				timer++;

				if (timer % 90 == 0) {
					player.GetModPlayer<TitaniumShardOnHitPlayer>().buffStacks--;
					timer = 0;
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

		public static void ShardStormGenerate(NPC target){
			Player player = Main.LocalPlayer;

			bool flag = target is NPC && (target.type == NPCID.TargetDummy || target.SpawnedFromStatue);
			if (player.titaniumStormCooldown > 0)
			{
				flag = true;
			}
			if (target is NPC)
			{
				Main.BigBossProgressBar.TryTracking(target.whoAmI);
			}
			if (!flag)
			{
				player.titaniumStormCooldown = 20;
				player.AddBuff(306, 600);
				if (player.ownedProjectileCounts[908] < 7)
				{
					player.ownedProjectileCounts[908]++;
					Projectile.NewProjectile(player.GetSource_OnHit(target, ""), player.Center, Vector2.Zero, 908, 50, 15f, player.whoAmI);
				}
			}
			
		}

    }
}