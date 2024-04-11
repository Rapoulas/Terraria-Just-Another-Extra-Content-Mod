using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using RappleMod.Content.Projectiles.Ouroboros;

namespace RappleMod.Content.Weapons{
    
    public class Ouroboros : ModItem
    {
        public override string Texture => $"Terraria/Images/Item_{ItemID.Flamethrower}";
        public override void SetDefaults() {
			Item.width = 50;
			Item.height = 18;
			Item.rare = ItemRarityID.Lime;
			Item.useTime = 6; 
			Item.useAnimation = 30;
			Item.autoReuse = true;
			Item.DamageType = DamageClass.Ranged; 
			Item.damage = 35; 
			Item.knockBack = 0.45f; 
			Item.noMelee = true;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.shoot = ModContent.ProjectileType<OuroborosProjectile>();
			Item.shootSpeed = 8f; 
			Item.useAmmo = AmmoID.Gel;
			Item.UseSound = SoundID.Item34;
			Item.consumeAmmoOnFirstShotOnly = true;
		}
    }
}