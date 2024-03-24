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
	internal class ChargeBar : UIState
	{
		// For this bar we'll be using a frame texture and then a gradient inside bar, as it's one of the more simpler approaches while still looking decent.
		// Once this is all set up make sure to go and do the required stuff for most UI's in the ModSystem class.
		private static Vector2 barPosition;
		private static Texture2D barFrame = ModContent.Request<Texture2D>("RappleMod/Content/UI/ChargeBarUI/ChargeBarSprite", AssetRequestMode.ImmediateLoad).Value;
		public static float timer;
		public static int frameCounter;
		public static Color color = new(255, 171, 0);

		public override void OnInitialize() {
			Player player = Main.LocalPlayer;
			barPosition = new Vector2(player.Center.X + 800, player.Center.Y + 330);
			barFrame = ModContent.Request<Texture2D>("RappleMod/Content/UI/ChargeBarUI/ChargeBarSprite", AssetRequestMode.ImmediateLoad).Value;

		}

		public override void Draw(SpriteBatch spriteBatch) {
			if (Main.LocalPlayer.HeldItem.ModItem is not Brimstone || !Main.LocalPlayer.channel){
				timer = 0;
				frameCounter = 0;
				color = new(255, 171, 0);
				return;
			}
			
			DrawChargeBar(spriteBatch);
		}

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

		private static void DrawChargeBar(SpriteBatch spriteBatch){
			timer++;
			if (timer% 23 == 0 && frameCounter < 13) frameCounter++;
			Rectangle cropRect = new(0, 32 * Math.Max(0, frameCounter-1), barFrame.Width, 32);
			if (color.G < 255) color.G++;
			else if (color.G == 255 && color.R > 0) color.R--;
			
			Vector2 drawScale = new(1f, 1f);
			drawScale *= 0.75f;
			Rectangle frame = new(0, 0, 0, 0);
			spriteBatch.Draw(barFrame, barPosition, cropRect, color, 0, frame.Size()/2f, drawScale, SpriteEffects.None, 0f);
		}
    }

	[Autoload(Side = ModSide.Client)]
	internal class ChargeBarUISystem : ModSystem
	{
		private UserInterface ChargeBarInterface;

		internal ChargeBar ChargeBar;

		public override void Load() {
			ChargeBar = new();
			ChargeBarInterface = new();
			ChargeBarInterface.SetState(ChargeBar);

		}

		public override void UpdateUI(GameTime gameTime) {
			ChargeBarInterface?.Update(gameTime);
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
			int resourceBarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
			if (resourceBarIndex != -1) {
				layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer(
					"RappleMod: Charge Bar",
					delegate {
						ChargeBarInterface.Draw(Main.spriteBatch, new GameTime());
						return true;
					},
					InterfaceScaleType.UI)
				);
			}
		}
	}
}
