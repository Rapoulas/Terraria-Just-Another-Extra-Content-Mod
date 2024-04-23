using System;
using System.ComponentModel.DataAnnotations;
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
		float id = 0;
		public override void SetStaticDefaults() {
			Item.staff[Type] = true; // This makes the useStyle animate as a staff instead of as a gun.
		}
        public override void SetDefaults() {
			Item.width = 50;
			Item.height = 50;
			Item.rare = ItemRarityID.Red;
			Item.autoReuse = true;
			Item.DefaultToStaff(ModContent.ProjectileType<SpiritStaffProjectile>(), 16, 25, 13);
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
			Vector2 circle = new(0f, 0f - 500f);
            float circleRotation = (float)Math.PI / 5 * id;
			if (id >= 10){
                circle.Y = -700f;
                circleRotation = (float)Math.PI / 8 * (id - 10);
            }
			Vector2 circlePositionInWorld = circle.RotatedBy(circleRotation) / 3f + player.Center;

			position = circlePositionInWorld;

			if (id >= 10) Projectile.NewProjectile(source, position, velocity, type, (int)(damage * 1.25f), knockback, player.whoAmI, 0f, 0f, id);
			else Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, 0f, 0f, id);

            id++;
			return false;
        }

        public override bool CanUseItem(Player player) => id < 26;

        public override void HoldItem(Player player)
        {
            if (!player.channel){
				id = 0;
			}
        }
    }
}