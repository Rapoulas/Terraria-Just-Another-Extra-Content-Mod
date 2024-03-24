using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RappleMod.Content.Weapons;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ModLoader.UI;
using Terraria.UI;

namespace RappleMod.Content.UI.ChargeBarUI
{
	// This custom UI will show whenever the player is holding the ExampleCustomResourceWeapon item and will display the player's custom resource amounts that are tracked in ExampleResourcePlayer
	internal class GrenadeIconUI : UIState
	{
		// For this bar we'll be using a frame texture and then a gradient inside bar, as it's one of the more simpler approaches while still looking decent.
		// Once this is all set up make sure to go and do the required stuff for most UI's in the ModSystem class.
		private static Vector2 barPosition;
		private static Texture2D grenIcon;
		public Player player = Main.LocalPlayer;

		public override void OnInitialize() {
			barPosition = new Vector2(player.Center.X + 775, player.Center.Y + 330);
			grenIcon = ModContent.Request<Texture2D>("RappleMod/Content/UI/AnarchistCookBookUI/NormalGrenadeIcon", AssetRequestMode.ImmediateLoad).Value;
		}

		public override void Draw(SpriteBatch spriteBatch) {
			if (Main.LocalPlayer.HeldItem.ModItem is not AnarchistCookbook){
				return;
			}
			
			DrawIcon(spriteBatch);
		}

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

		private static void DrawIcon(SpriteBatch spriteBatch){
            Player player = Main.LocalPlayer;
			player.GetModPlayer<MyPlayer>().anarchistCookbookCounter--;
			
			Vector2 drawScale = new(1f, 1f);
			drawScale *= 1f;
			Rectangle frame = new(0, 0, 0, 0);
			switch (player.GetModPlayer<MyPlayer>().anarchistCookbookMode) {
				case 0:
					grenIcon = ModContent.Request<Texture2D>("RappleMod/Content/UI/AnarchistCookBookUI/NormalGrenadeIcon", AssetRequestMode.ImmediateLoad).Value;
					break;
				case 1:
					grenIcon = ModContent.Request<Texture2D>("RappleMod/Content/UI/AnarchistCookBookUI/StickyGrenadeIcon", AssetRequestMode.ImmediateLoad).Value;
					break;
				case 2:
					grenIcon = ModContent.Request<Texture2D>("RappleMod/Content/UI/AnarchistCookBookUI/BouncyGrenadeIcon", AssetRequestMode.ImmediateLoad).Value;
					break;
				case 3:
					grenIcon = ModContent.Request<Texture2D>("RappleMod/Content/UI/AnarchistCookBookUI/RandomGrenadeIcon", AssetRequestMode.ImmediateLoad).Value;
					break;
			}
			Rectangle cropRect = new(0, 0, 14, 20);
			spriteBatch.Draw(grenIcon, barPosition, cropRect, Color.White * (player.GetModPlayer<MyPlayer>().anarchistCookbookCounter/100f), 0, frame.Size()/2f, drawScale, SpriteEffects.None, 0f);
		}
    }

    [Autoload(Side = ModSide.Client)]
	internal class AnarchistCookBookUI : ModSystem
	{
		private UserInterface GrenadeIconInterface;

		internal GrenadeIconUI GrenadeIcon;

		public override void Load() {
			GrenadeIcon = new();
			GrenadeIconInterface = new();
			GrenadeIconInterface.SetState(GrenadeIcon);

		}

		public override void UpdateUI(GameTime gameTime) {
			GrenadeIconInterface?.Update(gameTime);
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
			int resourceBarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
			if (resourceBarIndex != -1) {
				layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer(
					"RappleMod: Grenade Icon",
					delegate {
						GrenadeIconInterface.Draw(Main.spriteBatch, new GameTime());
						return true;
					},
					InterfaceScaleType.UI)
				);
			}
		}
	}
}
