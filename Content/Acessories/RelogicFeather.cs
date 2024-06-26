using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace RappleMod.Content.Acessories
{
    public class RelogicFeather : ModItem
	{   
		public override void SetDefaults() {
			Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 11));
			ItemID.Sets.AnimatesAsSoul[Item.type] = true; 
			Item.DefaultToAccessory(26, 34);
			Item.SetShopValues(ItemRarityColor.Green2, Item.buyPrice(silver: 50));
		}

		public override void UpdateAccessory(Player player, bool hideVisual) {
			player.GetJumpState<RelogicFeatherExtraJump>().Enable();
			player.AddBuff(BuffID.Featherfall, 2);
		}
	}

	public class RelogicFeatherExtraJump : ExtraJump
	{
		public override Position GetDefaultPosition() => new After(BlizzardInABottle);

		public override float GetDurationMultiplier(Player player) {
			// Use this hook to set the duration of the extra jump
			// The XML summary for this hook mentions the values used by the vanilla extra jumps
			return 1f;
		}

		public override void UpdateHorizontalSpeeds(Player player) {
			// Use this hook to modify "player.runAcceleration" and "player.maxRunSpeed"
			// The XML summary for this hook mentions the values used by the vanilla extra jumps
			player.runAcceleration *= 3.75f;
			player.maxRunSpeed *= 2f;
		}

		public override void OnStarted(Player player, ref bool playSound) {
			// Use this hook to trigger effects that should appear at the start of the extra jump
			// This example mimics the logic for spawning the puff of smoke from the Cloud in a Bottle
			int offsetY = player.height;
			if (player.gravDir == -1f)
				offsetY = 0;

			offsetY -= 16;

			SpawnCloudPoof(player, player.Top + new Vector2(-16f, offsetY));
			SpawnCloudPoof(player, player.position + new Vector2(-36f, offsetY));
			SpawnCloudPoof(player, player.TopRight + new Vector2(4f, offsetY));
		}

		private static void SpawnCloudPoof(Player player, Vector2 position) {
			Gore gore = Gore.NewGoreDirect(player.GetSource_FromThis(), position, -player.velocity, Main.rand.Next(11, 14));
			gore.velocity.X = gore.velocity.X * 0.1f - player.velocity.X * 0.1f;
			gore.velocity.Y = gore.velocity.Y * 0.1f - player.velocity.Y * 0.05f;
		}
	}
}