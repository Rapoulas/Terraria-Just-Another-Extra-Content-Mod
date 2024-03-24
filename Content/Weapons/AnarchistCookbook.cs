using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RappleMod.Content.Projectiles.Brimstone;
using Terraria.DataStructures;

namespace RappleMod.Content.Weapons
{
	public class AnarchistCookbook: ModItem
	{
        public int mode;
		public override void SetDefaults() {
            Item.width = 28;
            Item.height = 30;
			Item.mana = 0;
			Item.shootSpeed = 1f;
            Item.noMelee = true;
            Item.damage = 50;
            Item.useTime = 5;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.useAnimation = 180;
            Item.rare = ItemRarityID.Red;
            Item.DamageType = DamageClass.Magic;
			Item.useStyle = ItemUseStyleID.Shoot;
            Item.useLimitPerAnimation = 6;
		}

		public override void AddRecipes() {
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.LastPrism, 1);
			recipe.AddIngredient(ItemID.LunarFlareBook, 1);
			recipe.AddIngredient(ItemID.NebulaBlaze, 1);
			recipe.AddTile(TileID.LunarCraftingStation);
			recipe.Register();
		}
        
        public override bool CanUseItem(Player player) {
			if (player.altFunctionUse == 2) {
				Item.useTime = 15;
				Item.useAnimation = 15;
                Item.useLimitPerAnimation = 1;
                Item.mana = 0;
			}
			else {
                Item.useTime = 5;
				Item.useAnimation = 180;
                Item.useLimitPerAnimation = 6;
                Item.mana = 40;
			}
			return base.CanUseItem(player);
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2){
                if (mode == 0){
                    mode = 1;
                    Main.NewText("Sticky grenade mode");
                    return false;
                }
                else if (mode == 1){
                    mode = 2;
                    Main.NewText("Bouncy grenade mode");
                    return false;
                }
                else if (mode == 2){
                    mode = 0;
                    Main.NewText("Normal grenade mode");
                    return false;
                }
            }
            velocity = player.DirectionTo(Main.MouseWorld);
            velocity.Normalize();
            velocity *= Main.rand.NextFloat(4, 7);
            if (mode == 0) type = ProjectileID.Grenade;
            else if (mode == 1) type = ProjectileID.StickyGrenade;
            else if (mode == 2) type = ProjectileID.BouncyGrenade;
            int a = Projectile.NewProjectile(source, position, velocity.RotatedByRandom(MathHelper.ToRadians(135)/2), type, damage, 1);
            Main.projectile[a].friendly = true;
            Main.projectile[a].hostile = false;
            return false;
        }

        public override bool AltFunctionUse(Player player) => true;
    }
}