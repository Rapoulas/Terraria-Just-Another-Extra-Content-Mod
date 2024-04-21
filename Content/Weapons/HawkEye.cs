using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RappleMod.Content.Projectiles.HawkEye;
using Terraria.DataStructures;
using Terraria.Audio;

namespace RappleMod.Content.Weapons{
    
    public class HawkEye : ModItem
    {
		int reticleCounter = 0;
        public override void SetDefaults() {
			Item.width = 64;
			Item.height = 36;
			Item.rare = ItemRarityID.Red;
			Item.useTime = 90; 
			Item.useAnimation = 90;
			Item.autoReuse = true;
			Item.DamageType = DamageClass.Ranged; 
			Item.damage = 320; 
			Item.knockBack = 5f; 
			Item.noMelee = true;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.shoot = ModContent.ProjectileType<HawkEyeBullet>();
			Item.useAmmo = AmmoID.Bullet;
		}

        public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.LunarBar, 15);
			recipe.AddIngredient(ItemID.FragmentVortex, 10);
			recipe.AddIngredient(ItemID.SniperRifle, 1);
			recipe.AddTile(TileID.LunarCraftingStation);
			recipe.Register();
		}

        public override Vector2? HoldoutOffset()
        {
            Vector2 offset = new(-4, 0);
    		return offset;
        }

        public override void HoldItem(Player player)
        {
            player.GetModPlayer<MyPlayer>().isHoldingHawkEye = true;

			for (int i=0; i < Main.maxProjectiles; i++){
                if (Main.projectile[i].type == ModContent.ProjectileType<HawkEyeReticle>() && Main.projectile[i].active){
					reticleCounter++;
				}
            }

			if (reticleCounter < 1) Projectile.NewProjectile(Item.GetSource_FromThis(), Main.MouseWorld.X, Main.MouseWorld.Y,0 , 0, ModContent.ProjectileType<HawkEyeReticle>(), Item.damage, Item.knockBack, player.whoAmI);

			reticleCounter = 0;

			player.scope = true;
		}

		public override void ModifyShootStats (Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback){
			position = Main.MouseWorld;
			velocity *= 0;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			SoundStyle HawkEyeShoot = new($"{nameof(RappleMod)}/Assets/Sounds/HawkEyeShoot");
            SoundEngine.PlaySound(HawkEyeShoot, player.Center);
			
            Projectile.NewProjectile(source, Main.MouseWorld, velocity*0, ModContent.ProjectileType<HawkEyeBullet>(), damage, knockback, player.whoAmI);
            return false;
        }
    }
}