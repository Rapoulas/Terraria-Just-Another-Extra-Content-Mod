using Microsoft.Xna.Framework;
using RappleMod.Content.Projectiles.GunSummon;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace RappleMod.Content.Weapons
{
	public class GunSummon : ModItem
	{
		public override void SetStaticDefaults() {
			ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true; 
			ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
			ItemID.Sets.StaffMinionSlotsRequired[Type] = 1f;
		}

		public override void SetDefaults() {
			Item.damage = 60;
			Item.knockBack = 3f;
			Item.mana = 10;
			Item.width = 32;
			Item.height = 32;
			Item.useTime = 36;
			Item.useAnimation = 36;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.value = Item.sellPrice(gold: 15);
			Item.rare = ItemRarityID.Lime;
			Item.UseSound = SoundID.Item44;
			Item.noMelee = true;
			Item.DamageType = DamageClass.Summon; 
			Item.buffType = ModContent.BuffType<GunSummonBuff>();
			Item.shoot = ModContent.ProjectileType<GunSummonMinion>();
		}

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback) {
			position = Main.MouseWorld;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
			player.AddBuff(Item.buffType, 2);
			int counter = 0;

			for (int i = 0; i < player.GetModPlayer<MyPlayer>().gunSummonSpawnCheck.Length; i++){
				if (!player.GetModPlayer<MyPlayer>().gunSummonSpawnCheck[i]) {
					var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer, i);
					projectile.originalDamage = Item.damage;
					player.GetModPlayer<MyPlayer>().gunSummonSpawnCheck[i] = true;
					
					continue;
				}
				counter++;
			}
			
			if (counter == 4){
				var p = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer, counter);
				p.originalDamage = Item.damage;
			}
		    return false;
		}
    }
}
