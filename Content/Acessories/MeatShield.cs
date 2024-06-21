using Terraria;
using Terraria.ID;
using Terraria.Localization;
using RappleMod.Content.Buffs;
using Terraria.ModLoader;

namespace RappleMod.Content.Acessories
{
	[AutoloadEquip(EquipType.Shield)]
	public class MeatShield : ModItem
	{
		public static readonly int DamageAbsorptionAbilityLifeThresholdPercent = 20;
		public static float DamageAbsorptionAbilityLifeThreshold => DamageAbsorptionAbilityLifeThresholdPercent / 100f;
		public static readonly int DamageAbsorptionPercent = 35;
		public static float DamageAbsorptionMultiplier => DamageAbsorptionPercent / 100f;

		// 50 tiles is 800 world units. (50 * 16 == 800)
		public static readonly int DamageAbsorptionRange = 800;

		public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DamageAbsorptionPercent, DamageAbsorptionAbilityLifeThresholdPercent);

		public override void SetDefaults() {
			Item.width = 36;
			Item.height = 38;
			Item.accessory = true;
			Item.rare = ItemRarityID.Cyan;
			Item.defense = 8;
			Item.value = Item.buyPrice(0, 30, 0, 0);
		}

		public override void UpdateAccessory(Player player, bool hideVisual) {
			player.noKnockback = true;
			player.GetModPlayer<MyPlayer>().hasAbsorbTeamDamageEffect = true;
			player.GetModPlayer<MyPlayer>().hasMeatShield = Item;
			// Remember that UpdateAccessory runs for all players on all clients. Only check every 10 ticks
			if (player.whoAmI != Main.myPlayer && player.miscCounter % 10 == 0) {
				Player localPlayer = Main.player[Main.myPlayer];
				if (localPlayer.team == player.team && player.team != 0 && player.statLife > player.statLifeMax2 * DamageAbsorptionAbilityLifeThreshold && player.Distance(localPlayer.Center) <= DamageAbsorptionRange) {
					// The buff is used to visually indicate to the player that they are defended, and is also synchronized automatically to other players, letting them know that we were defended at the time we took the hit
					localPlayer.AddBuff(ModContent.BuffType<MeatShieldAbsorb>(), 20);
				}
			}
		}
	}
}
