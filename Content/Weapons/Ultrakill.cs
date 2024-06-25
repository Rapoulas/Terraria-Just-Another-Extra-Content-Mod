using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using RappleMod.Content.Projectiles.Ultrakill;

namespace RappleMod.Content.Weapons{
    
    public class Ultrakill : ModItem
    {
        public override void SetDefaults() {
			Item.width = 64;
			Item.height = 36;
			Item.rare = ItemRarityID.Yellow;
			Item.useTime = 25; 
			Item.useAnimation = 25;
			Item.autoReuse = true;
            Item.value = Item.buyPrice(0, 20, 0, 0);
			Item.DamageType = DamageClass.Ranged; 
			Item.damage = 100; 
			Item.knockBack = 5f; 
			Item.noMelee = true;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.shoot = ModContent.ProjectileType<UltrakillBullet>();
			Item.shootSpeed = 5f;
		}

        public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.SniperRifle, 1)
				.AddIngredient(ItemID.PlatinumCoin, 20)
				.AddIngredient(ItemID.SoulofNight, 15)
				.AddIngredient(ItemID.SoulofSight, 15)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}

        public override bool AltFunctionUse(Player player)
        {
            if (player.ownedProjectileCounts[ModContent.ProjectileType<UltrakillCoin>()] < 4) return true;
            else return false;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2){
                SoundStyle coinToss = new($"{nameof(RappleMod)}/Assets/Sounds/CoinToss") {
				Volume = 0.9f,
				PitchVariance = 0.2f
			    };
                SoundEngine.PlaySound(coinToss, player.Center);
            } else SoundEngine.PlaySound(SoundID.Item41, player.Center);
            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (player.altFunctionUse == 2){
                type = ModContent.ProjectileType<UltrakillCoin>();
                damage = 0;
            }
            else {
                type = ModContent.ProjectileType<UltrakillBullet>();
            }
            
            base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
        }
    }
}