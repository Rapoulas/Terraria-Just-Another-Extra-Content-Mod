using RappleMod.Content.Buffs;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace RappleMod.Content.GlobalNPCandProj
{
	public class MyGlobalProjectiles : GlobalProjectile
	{
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            Player player = Main.player[projectile.owner];

            if (source is EntitySource_Parent parent && parent.Entity is NPC npc && npc.HasBuff(ModContent.BuffType<HeartbrokenDebuff>())){
				projectile.damage = (int)(projectile.damage * 0.85f);
			}
        }
    }
}