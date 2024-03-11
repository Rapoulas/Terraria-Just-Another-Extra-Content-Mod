using RappleMod.Content.Projectiles.ProminenceRevolt;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using System.Linq;


namespace RappleMod.Content.Weapons
{
	public class ProminenceRevolt : ModItem
	{
		public override void SetStaticDefaults() {
			ItemID.Sets.SkipsInitialUseSound[Item.type] = true; // This skips use animation-tied sound playback, so that we're able to make it be tied to use time instead in the UseItem() hook.
			ItemID.Sets.Spears[Item.type] = true; // This allows the game to recognize our new item as a spear.
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults() {
			// Common Properties
			Item.rare = ItemRarityID.Expert; // Assign this item a rarity level of Pink
			Item.value = Item.sellPrice(silver: 10); // The number and type of coins item can be sold for to an NPC

			// Use Properties
			Item.useStyle = ItemUseStyleID.Shoot; // How you use the item (swinging, holding out, etc.)
			Item.useAnimation = 12; // The length of the item's use animation in ticks (60 ticks == 1 second.)
			Item.useTime = 18; // The length of the item's use time in ticks (60 ticks == 1 second.)
			Item.UseSound = SoundID.Item71; // The sound that this item plays when used.
			Item.autoReuse = true; // Allows the player to hold click to automatically use the item again. Most spears don't autoReuse, but it's possible when used in conjunction with CanUseItem()

			// Weapon Properties
			Item.damage = 50;
			Item.knockBack = 6.5f;
			Item.noUseGraphic = true; // When true, the item's sprite will not be visible while the item is in use. This is true because the spear projectile is what's shown so we do not want to show the spear sprite as well.
			Item.DamageType = DamageClass.Melee;
			Item.noMelee = true; // Allows the item's animation to do damage. This is important because the spear is actually a projectile instead of an item. This prevents the melee hitbox of this item.

			// Projectile Properties
			Item.shootSpeed = 10f; // The speed of the projectile measured in pixels per frame.
			Item.shoot = ModContent.ProjectileType<ProminenceRevoltProjectile>();
		}

		public override bool CanUseItem(Player player) {
			// Ensures no more than one spear can be thrown out, use this when using autoReuse
			return !Main.projectile.Any(n => n.active && n.owner == player.whoAmI && n.type == ModContent.ProjectileType<ProminenceRevoltProjectileAlt>());
		}
		public override bool AltFunctionUse(Player player) {
			return true;
		}

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
    	{	
			if (player.altFunctionUse == 2) {
				type = ModContent.ProjectileType<ProminenceRevoltProjectileAlt>();
				Item.useStyle = ItemUseStyleID.Swing;
				damage = 25;
			} 
			else{
				type = ModContent.ProjectileType<ProminenceRevoltProjectile>();
				Item.useStyle = ItemUseStyleID.Shoot;
				damage = 50;
			}
            base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
        }
        public override bool? UseItem(Player player) {
			if (!Main.dedServ && Item.UseSound.HasValue) {
				SoundEngine.PlaySound(Item.UseSound.Value, player.Center);
			}

			return true;
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.DirtBlock, 10);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}
}
