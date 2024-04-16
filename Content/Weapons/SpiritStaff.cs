using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using RappleMod.Content.Projectiles.Bonetrousle;
using RappleMod.Content.Projectiles.SpiritStaff;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace RappleMod.Content.Weapons{
    
    public class SpiritStaff : ModItem
    {
		public override void SetStaticDefaults() {
			Item.staff[Type] = true; // This makes the useStyle animate as a staff instead of as a gun.
		}
        public override void SetDefaults() {
			Item.width = 50;
			Item.height = 50;
			Item.rare = ItemRarityID.Red;
			Item.autoReuse = true;
			Item.DefaultToStaff(ModContent.ProjectileType<SpiritStaffProjectile>(), 16, 25, 20);
			Item.DamageType = DamageClass.Magic; 
			Item.damage = 70; 
			Item.knockBack = 0.45f;
			Item.noMelee = true;
			Item.channel = true;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.UseSound = SoundID.Item20;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			velocity *= 0;

			position = new(Main.rand.NextFloat(-200, 200), Main.rand.NextFloat(-200, 200));
			position += Main.screenPosition;
			position.Y += Main.screenHeight/2;
			position.X += Main.screenWidth/2;

			int proj = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0f);
            return false;
        }
    }
}