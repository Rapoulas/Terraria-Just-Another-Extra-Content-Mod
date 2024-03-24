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
		public override void SetDefaults() {
            Item.width = 28;
            Item.height = 30;
			Item.mana = 0;
			Item.shootSpeed = 1f;
            Item.noMelee = true;
            Item.damage = 75;
            Item.useTime = 5;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.useAnimation = 180;
            Item.rare = ItemRarityID.LightRed;
            Item.DamageType = DamageClass.Magic;
			Item.useStyle = ItemUseStyleID.Shoot;
            Item.useLimitPerAnimation = 6;
		}

		public override void AddRecipes() {
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.SpellTome, 1);
			recipe.AddIngredient(ItemID.Grenade, 50);
			recipe.AddIngredient(ItemID.BouncyGrenade, 50);
			recipe.AddIngredient(ItemID.StickyGrenade, 50);
			recipe.AddTile(TileID.Bookcases);
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
                player.GetModPlayer<MyPlayer>().anarchistCookbookCounter = 100;
                if (player.GetModPlayer<MyPlayer>().anarchistCookbookMode == 0){
                    player.GetModPlayer<MyPlayer>().anarchistCookbookMode = 1;
                    return false;
                }
                else if (player.GetModPlayer<MyPlayer>().anarchistCookbookMode == 1){
                    player.GetModPlayer<MyPlayer>().anarchistCookbookMode = 2;
                    return false;
                }
                else if (player.GetModPlayer<MyPlayer>().anarchistCookbookMode == 2){
                    player.GetModPlayer<MyPlayer>().anarchistCookbookMode = 3;
                    return false;
                }
                else if (player.GetModPlayer<MyPlayer>().anarchistCookbookMode == 3){
                    player.GetModPlayer<MyPlayer>().anarchistCookbookMode = 0;
                    return false;
                }
            }
            velocity = player.DirectionTo(Main.MouseWorld);
            velocity.Normalize();
            velocity *= Main.rand.NextFloat(4, 7);
            if (player.GetModPlayer<MyPlayer>().anarchistCookbookMode == 0) type = ProjectileID.Grenade;
            else if (player.GetModPlayer<MyPlayer>().anarchistCookbookMode == 1) type = ProjectileID.StickyGrenade;
            else if (player.GetModPlayer<MyPlayer>().anarchistCookbookMode == 2) type = ProjectileID.BouncyGrenade;
            else if (player.GetModPlayer<MyPlayer>().anarchistCookbookMode == 3) {
                int random = Main.rand.Next(1, 4);
                if (random == 1 ) type = ProjectileID.Grenade;
                else if (random == 2) type = ProjectileID.StickyGrenade;
                else if (random == 3) type = ProjectileID.BouncyGrenade;
            }
            Projectile.NewProjectile(source, position, velocity.RotatedByRandom(MathHelper.ToRadians(135)/2), type, damage, 1);
            return false;
        }

        public override bool AltFunctionUse(Player player) => true;
    }
}