using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RappleMod.Content.Weapons{
    
    public class HawkEye : ModItem
    {
        public override string Texture => $"Terraria/Images/Item_{ItemID.SniperRifle}";
        public override void SetDefaults() {
			Item.width = 64;
			Item.height = 36;
			Item.rare = ItemRarityID.Orange;
			Item.useTime = 60; 
			Item.useAnimation = 60;
			Item.autoReuse = true;
			Item.DamageType = DamageClass.Ranged; 
			Item.damage = 200; 
			Item.knockBack = 5f; 
			Item.noMelee = true;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.shoot = ModContent.ProjectileType<HawkEyeBullet>();
			Item.useAmmo = AmmoID.Bullet;
		}

        public override void HoldItem(Player player)
        {
            player.GetModPlayer<MyPlayer>().isHoldingHawkEye = true;

            if (!player.GetModPlayer<MyPlayer>().spawnedReticle){
                Projectile.NewProjectile(Projectile.GetSource_FromThis(), Main.MouseWorld, 0, ModContent.ProjectileType<HawkEyeReticle>(), Item.damage, Item.knockBack, player.whoAmI);
				player.GetModPlayer<MyPlayer>().spawnedReticle = true;
			}
        }

		public override void ModifyShootStats (Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback){
			position = Main.MouseWorld;
			velocity *= 0;
		}
    }
}