using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace RappleMod.Content.Acessories
{
    public class Antagonist : ModItem
	{ 
        public override void SetDefaults() {
			Item.DefaultToAccessory(26, 34);
			Item.SetShopValues(ItemRarityColor.Purple11, Item.buyPrice(gold: 1));
			Item.rare = ItemRarityID.Purple;
		}

		public override void UpdateAccessory(Player player, bool hideVisual) {
			player.GetModPlayer<MyPlayer>().hasAntagonist = Item;
			player.GetModPlayer<EnergyShieldPlayer>().energyShieldMax += 200;
		}
    }
}