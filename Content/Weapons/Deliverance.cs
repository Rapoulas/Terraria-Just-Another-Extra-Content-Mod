using Microsoft.Xna.Framework;
using RappleMod.Content.Projectiles.DeliveranceProj;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace RappleMod.Content.Weapons{
    
    public class Deliverance : ModItem
    {
		public int ammo = 8;
		public int timer = 0;
		float projType = 0;
        public override void SetDefaults() {
			Item.width = 40;
			Item.height = 16;
			Item.scale = 1;
			Item.rare = ItemRarityID.Expert;
			Item.useTime = 35; 
			Item.useAnimation = 35;
			Item.autoReuse = true;
			Item.DamageType = DamageClass.Ranged; 
			Item.damage = 40; 
			Item.knockBack = 5f; 
			Item.noMelee = true;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.shoot = ProjectileID.PurificationPowder;
			Item.shootSpeed = 15f; 
			Item.useAmmo = AmmoID.Bullet;
			Item.UseSound = SoundID.Item36;
			Item.expert = true;
			Item.value = Item.buyPrice(0, 10, 0, 0);
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.HallowedBar, 12);
			recipe.AddIngredient(ItemID.Shotgun, 1);
			recipe.AddIngredient(ItemID.SoulofFright, 10);
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}
		
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 6; ++i){
                Vector2 newVelocity = new(velocity.X + Main.rand.Next(-50, 51) * 0.07f, velocity.Y + Main.rand.Next(-50, 51) * 0.07f);
				projType = type;
                Projectile.NewProjectile(source, position, newVelocity, type, damage, knockback, player.whoAmI);
            }
			ammo--;
            return false;
        }

		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
			Vector2 muzzleOffset = Vector2.Normalize(velocity) * 5f;
			if (Collision.CanHit(position, 0, 0, position + muzzleOffset, 0, 0))
				position += muzzleOffset;
        }

        public override bool CanUseItem(Player player)
        {
            return ammo > 0;
        }

        public override void HoldItem(Player player)
        {	
			player.GetModPlayer<MyPlayer>().deliveranceAmmo = ammo;
            if (ammo == 0){
				timer++;
				if (timer == 120) {
					ammo = 8;
					timer = 0;
				}
				if (timer == 40){
					Vector2 velocity = player.position.DirectionTo(Main.MouseWorld) * 5f;

					float baseBonus = player.GetDamage(DamageClass.Ranged).Base + player.GetDamage(DamageClass.Generic).Base;
					float addBonus = player.GetDamage(DamageClass.Ranged).Additive + player.GetDamage(DamageClass.Generic).Additive - 1;
					float multBonus = player.GetDamage(DamageClass.Ranged).Multiplicative + player.GetDamage(DamageClass.Generic).Multiplicative - 1;
					float flatBonus = player.GetDamage(DamageClass.Ranged).Flat + player.GetDamage(DamageClass.Generic).Flat;

					float damage = ((Item.damage + baseBonus) * addBonus * multBonus) + flatBonus;

					Projectile.NewProjectile(player.GetSource_FromThis(), player.position, velocity, ModContent.ProjectileType<DeliveranceProjectile>(), (int)damage, Item.knockBack, player.whoAmI, 0, projType);
					SoundStyle reload = new($"{nameof(RappleMod)}/Assets/Sounds/DeliveranceReload");
            		SoundEngine.PlaySound(reload, player.Center);
				}
			}
        }
    }
}