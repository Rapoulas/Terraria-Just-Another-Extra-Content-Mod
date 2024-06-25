using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;

namespace RappleMod.Content.Acessories
{
    public class WaxQuail : ModItem
	{   
        bool canMultSpeed = true;
		public override void SetDefaults() {
			Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(1, 30));
			ItemID.Sets.AnimatesAsSoul[Item.type] = true; 
			Item.DefaultToAccessory(34, 29);
			Item.SetShopValues(ItemRarityColor.Green2, Item.buyPrice(gold: 5));
		}

		public override void UpdateAccessory(Player player, bool hideVisual) {
			player.GetJumpState<WaxExtraJump>().Enable();
            if (player.controlJump == true){
                if (canMultSpeed && player.jump > 0){
                    player.velocity.X *= 1.5f;
                    if (player.velocity.X > 40){
                        player.velocity.X = 40;
                    }
                    if (player.velocity.X < -40){
                        player.velocity.X = -40;
                    }
                    canMultSpeed = false;
                }
            } else {
                canMultSpeed = true;
            }
		}

		public override void AddRecipes() {
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.CloudinaBottle, 1);
			recipe.AddIngredient(ItemID.SwiftnessPotion, 10);
			recipe.AddTile(TileID.TinkerersWorkbench);
			recipe.Register();
		}
	}

    public class WaxExtraJump : ExtraJump
	{
		public override Position GetDefaultPosition() => new After(BlizzardInABottle);

		public override float GetDurationMultiplier(Player player) {
			// Use this hook to set the duration of the extra jump
			// The XML summary for this hook mentions the values used by the vanilla extra jumps
			return 0.5f;
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