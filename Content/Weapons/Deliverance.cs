using Microsoft.Xna.Framework;
using RappleMod.Content.Projectiles.Deliverance;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace RappleMod.Content.Weapons{
    
    public class Deliverance : ModItem
    {
		public int ammo = 8;
		public int timer = 0;
		float projType = 0;
        public override string Texture => $"Terraria/Images/Item_{ItemID.Shotgun}";
        public override void SetDefaults() {
			Item.width = 40;
			Item.height = 16;
			Item.scale = 1;
			Item.rare = ItemRarityID.Expert;
			Item.useTime = 35; 
			Item.useAnimation = 35;
			Item.autoReuse = true;
			Item.DamageType = DamageClass.Ranged; 
			Item.damage = 40; 
			Item.knockBack = 5f; 
			Item.noMelee = true;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.shoot = ProjectileID.PurificationPowder;
			Item.shootSpeed = 15f; 
			Item.useAmmo = AmmoID.Bullet;
			Item.UseSound = SoundID.Item36;
		}
		
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            for (int i = 0; i < 6; ++i){
                Vector2 newVelocity = new(velocity.X + Main.rand.Next(-50, 51) * 0.07f, velocity.Y + Main.rand.Next(-50, 51) * 0.07f);
				projType = type;
                Projectile.NewProjectile(source, position, newVelocity, type, damage, knockback, player.whoAmI);
            }
			ammo--;
            return false;
        }

        public override bool CanUseItem(Player player)
        {
            return ammo > 0;
        }

        public override void HoldItem(Player player)
        {
            if (ammo == 0){
				timer++;
				if (timer == 120) {
					ammo = 8;
					timer = 0;
				}
				if (timer == 40){
					Vector2 velocity = player.position.DirectionTo(Main.MouseWorld) * 5f;

					Projectile.NewProjectile(Item.GetSource_FromThis(), player.position, velocity, ModContent.ProjectileType<DeliveranceProjectile>(), Item.damage, Item.knockBack, player.whoAmI, 0, projType);
					SoundStyle reload = new($"{nameof(RappleMod)}/Assets/Sounds/DeliveranceReload");
            		SoundEngine.PlaySound(reload, player.Center);
				}
			}
        }
    }
}