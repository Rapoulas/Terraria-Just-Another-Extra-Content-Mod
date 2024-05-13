using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace RappleMod.Content.Acessories
{
    public class ZetaReference : ModItem
	{   
		public override void SetDefaults() {
			Item.DefaultToAccessory(26, 34);
			Item.SetShopValues(ItemRarityColor.Green2, Item.buyPrice(silver: 50));
			Item.rare = ItemRarityID.Master;
		}

		public override void UpdateAccessory(Player player, bool hideVisual) {
			player.GetModPlayer<MyPlayer>().hasZetaReference = Item;
		}

        public override void AddRecipes() {
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.Ale, 100);
			recipe.AddIngredient(ItemID.Bottle, 5);
			recipe.AddTile(TileID.TinkerersWorkbench);
			recipe.Register();
		}
	}
}