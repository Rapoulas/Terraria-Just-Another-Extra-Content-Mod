using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RappleMod.Content.Weapons{
    
    public class HawkEye : ModItem
    {
        bool spawnedReticle = false;
        public override string Texture => $"Terraria/Images/Item_{ItemID.SniperRifle}";
        public override void SetDefaults() {
			Item.width = 64;
			Item.height = 36;
			Item.rare = ItemRarityID.Orange;
			Item.useTime = 60; 
			Item.useAnimation = 60;
			Item.autoReuse = true;
			Item.DamageType = DamageClass.Ranged; 
			Item.damage = 20; 
			Item.knockBack = 5f; 
			Item.noMelee = true;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.shoot = ProjectileID.PurificationPowder;
			Item.shootSpeed = 5f; 
			Item.useAmmo = AmmoID.Bullet;
		}

        public override void HoldItem(Player player)
        {
            player.GetModPlayer<MyPlayer>().isHoldingHawkEye = true;

            
        }
    }
}