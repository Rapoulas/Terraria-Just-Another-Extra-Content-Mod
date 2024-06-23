using System;
using Microsoft.Xna.Framework;
using RappleMod.Content.Buffs;
using RappleMod.Content.SetBonusChanges;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace RappleMod.Content.GlobalNPCandProj
{
	public class MyGlobalProjectiles : GlobalProjectile
	{
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            if (source is EntitySource_Parent parent && parent.Entity is NPC npc && npc.HasBuff(ModContent.BuffType<HeartbrokenDebuff>())){
				projectile.damage = (int)(projectile.damage * 0.85f);
			}
        }
    }
}