using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;

namespace RappleMod.Content.UI.EnergyShieldBarUI
{
	internal class EnergyShieldBarUI : UIState
	{
		// For this bar we'll be using a frame texture and then a gradient inside bar, as it's one of the more simpler approaches while still looking decent.
		// Once this is all set up make sure to go and do the required stuff for most UI's in the ModSystem class.
		private UIText text;
		private UIElement area;
		private UIImage barFrame;
		private Color gradientA;
		private Color gradientB;

		public override void OnInitialize() {
			area = new UIElement();
			area.Left.Set(-area.Width.Pixels - 400, 1f);
			area.Top.Set(10, 0f);
			area.Width.Set(24, 0f); 
			area.Height.Set(60, 0f);

			barFrame = new UIImage(ModContent.Request<Texture2D>("RappleMod/Content/UI/EnergyShieldBarUI/EnergyShieldBarFrame")); 
			barFrame.Left.Set(22, 0f);
			barFrame.Top.Set(15, 0f);
			barFrame.Width.Set(50, 0f);
			barFrame.Height.Set(29, 0f);

			text = new UIText("0/0", 0.73f);
			text.Width.Set(20, 0f);
			text.Height.Set(20, 0f);
			text.Top.Set(0, 0f);
			text.Left.Set(-area.Width.Pixels/2 + 36, 0f);

			gradientA = new Color(54, 202, 247); 
			gradientB = new Color(66, 245, 245); 

			area.Append(text);
			area.Append(barFrame);
			Append(area);
		}

		public override void Draw(SpriteBatch spriteBatch) {
            var modPlayer = Main.LocalPlayer.GetModPlayer<EnergyShieldPlayer>();
            if (modPlayer.energyShieldMax > 0)
				base.Draw(spriteBatch);
            else
			    return;
		}

        protected override void DrawSelf(SpriteBatch spriteBatch) {
			base.DrawSelf(spriteBatch);

			var modPlayer = Main.LocalPlayer.GetModPlayer<EnergyShieldPlayer>();
			float quotient = (float)modPlayer.energyShield / modPlayer.energyShieldMax; 
			quotient = Utils.Clamp(quotient, 0f, 1f);

			Rectangle hitbox = barFrame.GetInnerDimensions().ToRectangle();
			hitbox.X += 12;
			hitbox.Width -= 24;
			hitbox.Y += 8;
			hitbox.Height -= 16;

			int left = hitbox.Left;
			int right = hitbox.Right;
			int steps = (int)((right - left) * quotient);
			for (int i = 0; i < steps; i += 1) {
				float percent = (float)i / (right - left);
				spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(left + i, hitbox.Y, 1, hitbox.Height), Color.Lerp(gradientA, gradientB, percent));
			}
		}

		public override void Update(GameTime gameTime) {
            var modPlayer = Main.LocalPlayer.GetModPlayer<EnergyShieldPlayer>();
			if (modPlayer.energyShieldMax > 0){
                text.SetText(EnergyShieldBarUISystem.EnergyBarText.Format(modPlayer.energyShield, modPlayer.energyShieldMax));
                base.Update(gameTime);
            }
            else
                return;
		}
	}

	[Autoload(Side = ModSide.Client)]
	internal class EnergyShieldBarUISystem : ModSystem
	{
		private UserInterface EnergyShieldBarInterface;

		internal EnergyShieldBarUI EnergyShieldBarUI;

		public static LocalizedText EnergyBarText { get; private set; }

		public override void Load() {
			EnergyShieldBarUI = new();
			EnergyShieldBarInterface = new();
			EnergyShieldBarInterface.SetState(EnergyShieldBarUI);

			string category = "UI";
			EnergyBarText ??= Mod.GetLocalization($"{category}.EnergyShieldBar");
		}

		public override void UpdateUI(GameTime gameTime) {
			EnergyShieldBarInterface?.Update(gameTime);
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
			int resourceBarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
			if (resourceBarIndex != -1) {
				layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer(
					"Just Another Content Mod: Energy Shield Bar",
					delegate {
						EnergyShieldBarInterface.Draw(Main.spriteBatch, new GameTime());
						return true;
					},
					InterfaceScaleType.UI)
				);
			}
		}
	}
}
