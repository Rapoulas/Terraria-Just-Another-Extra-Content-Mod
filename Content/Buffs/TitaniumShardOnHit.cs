using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace RappleMod.Content.Buffs
{
	public class TitaniumShardOnHit : ModBuff
	{
        public override string Texture => $"Terraria/Images/Buff_{BuffID.TitaniumStorm}";
		public override void SetStaticDefaults() {
			BuffID.Sets.LongerExpertDebuff[Type] = false;
		}

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetDamage(DamageClass.Ranged) += player.GetModPlayer<TitaniumShardOnHitPlayer>().buffStacks/25f;
            player.GetCritChance(DamageClass.Ranged) += player.GetModPlayer<TitaniumShardOnHitPlayer>().buffStacks*2f;
        }

        public override void ModifyBuffText(ref string buffName, ref string tip, ref int rare)
        {
            Player player = Main.LocalPlayer;
            tip = $"Increased ranged damage by {player.GetModPlayer<TitaniumShardOnHitPlayer>().buffStacks*4}% and ranged critical change by {player.GetModPlayer<TitaniumShardOnHitPlayer>().buffStacks*2f}%";
        }
    }

    public class TitaniumShardOnHitPlayer : ModPlayer
	{
        public int buffStacks;

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.LocalPlayer;
            if (player.GetModPlayer<SetBonusChangesPlayer>().RangedTitaniumAdamantiteFrostSet && proj.type == 908){

                    if (!Player.HasBuff(ModContent.BuffType<TitaniumShardOnHit>())) buffStacks = 0;

					player.AddBuff(ModContent.BuffType<TitaniumShardOnHit>(), 480);
					if (buffStacks < 7) buffStacks++;
				}
        }
    }
}
