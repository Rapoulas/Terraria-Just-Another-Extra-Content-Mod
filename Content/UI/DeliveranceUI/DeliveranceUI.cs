using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RappleMod.Content.Weapons;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Configuration;
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
        private UIText text;
		int ammo = 0;
		int timer = 0;
		public override void OnInitialize() {
            text = new UIText("[8/8]");

            text.Width.Set(32, 0);
            text.Height.Set(32, 0);
            // text.Top.Set(Main.screenHeight/2 + 10, 0);
            // text.Left.Set(Main.screenWidth/2 - 70, 0);
			text.Left.Set(Main.mouseX, 0f);
			text.Top.Set(Main.mouseY, 0f);

            Append(text);
        }

		public override void Draw(SpriteBatch spriteBatch) {
			if (Main.LocalPlayer.HeldItem.ModItem is not Deliverance){
				return;
			}
			
            base.Draw(spriteBatch);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch) {
			Player player = Main.LocalPlayer;
			ammo = player.GetModPlayer<MyPlayer>().deliveranceAmmo;

            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(0, 0, 32, 32), Color.White * 0);
        }

        public override void Update(GameTime gameTime) {
			if (Main.LocalPlayer.HeldItem.ModItem is not Deliverance)
				return;

			if (ammo == 0){
				if (timer == 15) 
					text.SetText("Reloading.");
				else if (timer == 30)
					text.SetText("Reloading..");
				else if (timer == 45)
					text.SetText("Reloading...");
				else if (timer == 60)
					timer = 14;
				else if (timer < 15)
					text.SetText($"  [{ammo}/8]");
					
				timer++;
			}
			else {
				text.SetText($"  [{ammo}/8]");
				timer = 0;
			}

			text.Left.Set(Main.mouseX - 25, 0f);
			text.Top.Set(Main.mouseY + 25, 0f);
			
			base.Update(gameTime);
		}
    }

    [Autoload(Side = ModSide.Client)]
	internal class DeliveranceUISystem : ModSystem
	{
		private UserInterface DeliveranceUIMag;

		internal DeliveranceUI DeliveranceUI;

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
