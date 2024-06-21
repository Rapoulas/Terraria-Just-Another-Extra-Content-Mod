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
        public override void SetDefaults() {
			Item.width = 50;
			Item.height = 18;
			Item.rare = ItemRarityID.Red;
			Item.useTime = 6; 
			Item.useAnimation = 30;
			Item.autoReuse = true;
			Item.DamageType = DamageClass.Ranged; 
			Item.damage = 75; 
			Item.knockBack = 0.45f; 
			Item.noMelee = true;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.shoot = ModContent.ProjectileType<OuroborosProjectile>();
			Item.shootSpeed = 8f; 
			Item.useAmmo = AmmoID.Gel;
			Item.UseSound = SoundID.Item34;
			Item.consumeAmmoOnFirstShotOnly = true;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.FragmentVortex, 18)
				.AddIngredient(ItemID.LunarBar, 12)
				.AddIngredient(ItemID.Flamethrower, 1)
				.AddTile(TileID.LunarCraftingStation)
				.Register();
		}

		public override void HoldItem(Player player)
        {
            player.GetModPlayer<MyPlayer>().hasOuroboros = Item;
		}

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            Vector2 muzzleOffset = Vector2.Normalize(velocity) * 40f;
			
			if (Collision.CanHit(position, 0, 0, position - muzzleOffset, 0, 0)) {
				position += muzzleOffset;
			}
        }
    }
}