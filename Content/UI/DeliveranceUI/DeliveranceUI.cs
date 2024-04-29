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

namespace RappleMod.Content.UI.DeliveranceUI
{
	internal class DeliveranceUI : UIState
	{
		private Player player = Main.LocalPlayer;
        private UIText text;
        private UIElement area;
        private int ammo = 0;

		public override void OnInitialize() {
            text = new UIText("[8/8]");
            area = new UIElement();

            text.Width.Set(32, 0);
            text.Height.Set(32, 0);
            text.Top.Set(400, 0);
            text.Left.Set(400, 0);

            text.Width.Set(32, 0);
            text.Height.Set(32, 0);
            text.Top.Set(400, 0);
            text.Left.Set(400, 0);

            area.Append(text);
            Append(area);
        }

		public override void Draw(SpriteBatch spriteBatch) {
			if (Main.LocalPlayer.HeldItem.ModItem is not Deliverance){
				return;
			}

            base.Draw(spriteBatch);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch) {
            base.DrawSelf(spriteBatch);

            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(0, 0, 32, 32), Color.White);
        }

        public override void Update(GameTime gameTime) {
			if (Main.LocalPlayer.HeldItem.ModItem is not Deliverance)
				return;

			int ammo = player.GetModPlayer<MyPlayer>().deliveranceAmmo;

			text.SetText($"[{ammo}/8]");
			base.Update(gameTime);
		}
    }

    [Autoload(Side = ModSide.Client)]
	internal class DeliveranceUISystem : ModSystem
	{
		private UserInterface DeliveranceUIMag;

		internal DeliveranceUI DeliveranceUI;
        public static LocalizedText AmmoText { get; private set; }

		public override void Load() {
			DeliveranceUI = new();
			DeliveranceUIMag = new();
			DeliveranceUIMag.SetState(DeliveranceUI);
		}

		public override void UpdateUI(GameTime gameTime) {
			DeliveranceUIMag?.Update(gameTime);
		}

		public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers) {
			int resourceBarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
			if (resourceBarIndex != -1) {
				layers.Insert(resourceBarIndex, new LegacyGameInterfaceLayer(
					"RappleMod: Deliverance UI",
					delegate {
						DeliveranceUIMag.Draw(Main.spriteBatch, new GameTime());
						return true;
					},
					InterfaceScaleType.UI)
				);
			}
		}
	}
}
