using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.GameInput;
using RappleMod.Content.Projectiles.ThunderGauntlet;

namespace RappleMod.Content.Weapons{
    
    public class ThunderGauntlet : ModItem
    {
        public override string Texture => $"Terraria/Images/Item_{ItemID.FireGauntlet}";
        public override void SetDefaults() {
			Item.width = 64;
			Item.height = 36;
			Item.rare = ItemRarityID.Yellow;
			Item.useTime = 30; 
			Item.useAnimation = 30;
            Item.value = Item.buyPrice(0, 20, 0, 0);
			Item.DamageType = DamageClass.Melee; 
			Item.damage = 100; 
			Item.knockBack = 5f; 
			Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<ThunderGauntletHoldOut>();
            Item.shootSpeed = 10f;
            Item.channel = true;
			Item.noMelee = true;
            Item.noUseGraphic = true;
		}
    }

    public class GauntletGrapple : ModSystem
	{
		public static ModKeybind GrappleKeybind { get; private set; }

		public override void Load() {
			GrappleKeybind = KeybindLoader.RegisterKeybind(Mod, "ThunderGauntletGrappleKeybind", "Mouse2");
		}

		public override void Unload() {
			GrappleKeybind = null;
		}
	}

    public class GauntletKeybindModPlayer : ModPlayer
	{
		public override void ProcessTriggers(TriggersSet triggersSet) {
			if (GauntletGrapple.GrappleKeybind.JustPressed && Main.LocalPlayer.HeldItem.ModItem is ThunderGauntlet && Player.ownedProjectileCounts[ModContent.ProjectileType<ThunderGauntletHookProjectile>()] < 1) {
				Vector2 velocity = Player.DirectionTo(Main.MouseWorld);
				velocity *= 10f;
				Projectile proj = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), Player.Center, velocity, ModContent.ProjectileType<ThunderGauntletHookProjectile>(), 0, 0);
			}
		}

        public override void PostUpdate()
    	{
			foreach (Projectile proj in Main.projectile){
				if(proj.type == ModContent.ProjectileType<ThunderGauntletHookProjectile>() && proj.active) 
					if (Main.LocalPlayer.HeldItem.ModItem is not ThunderGauntlet)
						proj.Kill();
			}

            base.PostUpdate();
        }
    }
}