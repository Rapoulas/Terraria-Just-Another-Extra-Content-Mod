using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace RappleMod.Content.Acessories
{
    public class Purgatory : ModItem
	{   
		public override void SetDefaults() {
			Item.DefaultToAccessory(32, 32);
			Item.SetShopValues(ItemRarityColor.Green2, Item.buyPrice(silver: 50));
			Item.rare = ItemRarityID.Master;
		}

		public override void UpdateAccessory(Player player, bool hideVisual) {
			player.GetModPlayer<MyPlayer>().hasPurgatory = Item;
		}

        public override void AddRecipes() {
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.DirtBlock, 10);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
		}
	}
}