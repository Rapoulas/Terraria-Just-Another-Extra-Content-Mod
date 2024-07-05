using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RappleMod.Content.Projectiles;
using RappleMod.Content.Buffs;
using RappleMod.Content.Projectiles.DryadWardCopy;
using RappleMod.Content.Projectiles.ShroomiteVortexProj;
using Terraria.DataStructures;

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
		public bool RangedHallowedChlorophyteSet;
		public bool ChlorophyteShroomiteSet;
		public bool ShroomiteVortexSet;
		public bool MoltenShadowSet;
		public bool MeleeHCSetReapply = false;
		public int maxHitCountRangedOMFSet;
		public int timer;
		public float ChlorophyteShroomiteDamageBonus;

        public override void ResetEffects(){
			NecroFossilSet = false;
			RangedCobaltPalladiumFrostSet = false;
			MeleeCobaltPalladiumFrostSet = false;
			MeleeOrichalcumMythrilFrostSet = false;
			RangedOrichalcumMythrilFrostSet = false;
			MeleeTitaniumAdamantiteFrostSet = false;
			RangedTitaniumAdamantiteFrostSet = false;
			MeleeHallowedChlorophyteSet = false;
			RangedHallowedChlorophyteSet = false;
			ShroomiteVortexSet = false;
			ChlorophyteShroomiteSet = false;
			MoltenShadowSet = false;
			maxHitCountRangedOMFSet = 0;
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
			NecroFossilSetBonus(proj, target, hit);
			RangedOrichalcumMythrilFrostSetBonus(proj, target, damageDone);
			MeleeCobaltPalladiumFrostSetBonus(hit, target);
			TitaniumAdamantiteFrostSetBonus(target, hit, proj);
			HallowedChlorophyteSetBonus(target, hit);
			ShroomiteVortexSetBonus(target, proj, hit, damageDone);
        }

        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
			MeleeCobaltPalladiumFrostSetBonus(hit, target);
			TitaniumAdamantiteFrostSetBonus(target, hit);
			HallowedChlorophyteSetBonus(target, hit);
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

        public override bool Shoot(Item item, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {	
			ChlorophyteShroomiteSetBonus(item, source);
            return true;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (ChlorophyteShroomiteSet && modifiers.DamageType == DamageClass.Ranged){
				modifiers.FinalDamage *= ChlorophyteShroomiteDamageBonus;
			}
        }

        private static NPC FindClosestNPC(float maxDetectDistance, Vector2 position) {
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

		private static void ShardStormGenerate(NPC target){
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

		private void NecroFossilSetBonus(Projectile proj, NPC target, NPC.HitInfo hit){
			if (!(proj.DamageType == DamageClass.Ranged && hit.Crit && NecroFossilSet)) return;
			Player player = Main.LocalPlayer;

			for (int i = 0; i < 3; i++){
				Vector2 velocity = new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 0));
				velocity.Normalize();
				velocity *= Main.rand.NextFloat(10, 15);
				float newDamage = hit.Damage/5;
				
				Projectile.NewProjectile(proj.GetSource_FromThis(), target.Center, velocity, ModContent.ProjectileType<NecroFossilBone>(), (int)newDamage, hit.Knockback, player.whoAmI, 0, target.whoAmI, target.whoAmI);
			}
			target.AddBuff(BuffID.Slow, 120);
		}

		private void RangedOrichalcumMythrilFrostSetBonus(Projectile proj, NPC target, int damageDone){
			Player player = Main.LocalPlayer;
			if (!(proj.DamageType == DamageClass.Ranged && RangedOrichalcumMythrilFrostSet && Main.rand.NextBool(4))) return;

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

		private void MeleeCobaltPalladiumFrostSetBonus(NPC.HitInfo hit, NPC target){
			if (!(MeleeCobaltPalladiumFrostSet && (hit.DamageType == DamageClass.Melee || hit.DamageType == DamageClass.MeleeNoSpeed))) return;
			
			if (Main.rand.NextBool(5)) target.AddBuff(BuffID.Frostburn, 60 * Main.rand.Next(3, 6));
			if (Main.rand.NextBool(5)) target.AddBuff(BuffID.Frostburn2, 60 * Main.rand.Next(3, 6));
		}

		private void TitaniumAdamantiteFrostSetBonus(NPC target, NPC.HitInfo hit, Projectile proj = null){
			if (!(MeleeTitaniumAdamantiteFrostSet || RangedTitaniumAdamantiteFrostSet)) return;

			if ((MeleeTitaniumAdamantiteFrostSet && (hit.DamageType == DamageClass.Melee || hit.DamageType == DamageClass.MeleeNoSpeed)) || (RangedTitaniumAdamantiteFrostSet && hit.DamageType == DamageClass.Ranged))
				ShardStormGenerate(target);

			if (proj.type == 908){
				target.AddBuff(BuffID.Frostburn, 60 * Main.rand.Next(3, 6));
				target.AddBuff(BuffID.Frostburn2, 60 * Main.rand.Next(3, 6));
			}
		}

		private void HallowedChlorophyteSetBonus(NPC target, NPC.HitInfo hit){
			if (!(MeleeHallowedChlorophyteSet || RangedHallowedChlorophyteSet)) return;
			Player player = Main.LocalPlayer;

			if (MeleeHallowedChlorophyteSet && (hit.DamageType == DamageClass.Melee || hit.DamageType == DamageClass.MeleeNoSpeed)){
				if (player.ownedProjectileCounts[ModContent.ProjectileType<DryadWardCopy>()] <= 0)
					Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<DryadWardCopy>(), 75, 0, player.whoAmI, 0, player.whoAmI, 1);
				else MeleeHCSetReapply = true;
			}

			if (RangedHallowedChlorophyteSet && (hit.DamageType == DamageClass.Ranged)){
				if (player.ownedProjectileCounts[ModContent.ProjectileType<DryadWardCopy>()] <= 0)
					Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, Vector2.Zero, ModContent.ProjectileType<DryadWardCopy>(), 75, 0, player.whoAmI, 0, target.whoAmI, 0);
				else MeleeHCSetReapply = true;
			}
		}

		private void ShroomiteVortexSetBonus(NPC target, Projectile proj, NPC.HitInfo hit, int damageDone){
			if (!ShroomiteVortexSet) return;
			Player player = Main.LocalPlayer;

			if (hit.DamageType == DamageClass.Ranged && Main.rand.NextBool(10)){
				Projectile.NewProjectile(player.GetSource_FromThis(), new Vector2(player.Top.X, player.Top.Y - 25), new Vector2(0, -17), ModContent.ProjectileType<ShroomiteVortexProj>(), damageDone*3, 7f, player.whoAmI, target.whoAmI);
			}
		}

		private void ChlorophyteShroomiteSetBonus(Item item, EntitySource_ItemUse_WithAmmo source){
			if (!ChlorophyteShroomiteSet) return;
			
			if (item.DamageType == DamageClass.Ranged){
				int manaCost = item.useTime;
				if (source.Player.CheckMana(manaCost, true)){
					source.Player.manaRegenDelay = (1f - (float)source.Player.statMana / (float)source.Player.statManaMax2) * 60f * 4f + 45f;
					source.Player.manaRegenDelay *= 0.7f;
					ChlorophyteShroomiteDamageBonus = 1.5f - (source.Player.manaSickReduction/1.3f);
				}
				else ChlorophyteShroomiteDamageBonus = 1f;
			}
		}
    }
}