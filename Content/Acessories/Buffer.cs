using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace RappleMod.Content.Acessories
{
    public class Buffer : ModItem
	{ 
        public override void SetDefaults() {
			ItemID.Sets.AnimatesAsSoul[Item.type] = true; 
			Item.DefaultToAccessory(26, 34);
			Item.SetShopValues(ItemRarityColor.Green2, Item.buyPrice(gold: 1));
			Item.rare = ItemRarityID.Green;
		}

		public override void UpdateAccessory(Player player, bool hideVisual) {
			player.GetModPlayer<MyPlayer>().hasBuffer = Item;
		}
    }
}