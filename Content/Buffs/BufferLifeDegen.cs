using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RappleMod.Content.Buffs
{
	public class BufferLifeDegen : ModBuff
	{
		public override void SetStaticDefaults() {
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true; 
			BuffID.Sets.LongerExpertDebuff[Type] = false;
		}

        public override void Update(Player player, ref int buffIndex) {
			player.GetModPlayer<BufferLifeDegenPlayer>().lifeRegenDebuff = true;
		}
    }

    public class BufferLifeDegenPlayer : ModPlayer
	{
		// Flag checking when life regen debuff should be activated
		public int bufferDamageTaken;
        public bool lifeRegenDebuff;

		public override void ResetEffects() {
            lifeRegenDebuff = false;
		}
        public override void UpdateBadLifeRegen() {
			if (lifeRegenDebuff) {
                Player.lifeRegenTime = 0;
				Player.lifeRegen -= (int)(bufferDamageTaken /2.5f);
			}
		}

        public override void OnHurt(Player.HurtInfo info)
        {
            BufferOnHurt(info);
        }

        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
			if(Player.GetModPlayer<MyPlayer>().hasBuffer != null){
				modifiers.ModifyHurtInfo += (ref Player.HurtInfo info) =>
				{
					if (!Player.HasBuff(ModContent.BuffType<BufferLifeDegen>())) bufferDamageTaken = 0;
					bufferDamageTaken = Math.Min(info.Damage+bufferDamageTaken, 300);
					info.Damage = 1;
				};
			}
        }

        private void BufferOnHurt(Player.HurtInfo info){
			if (Player.GetModPlayer<MyPlayer>().hasBuffer == null) return;

			if (!Player.HasBuff(ModContent.BuffType<BufferLifeDegen>()))
				Player.AddBuff(ModContent.BuffType<BufferLifeDegen>(), 300);
			else Player.AddBuff(ModContent.BuffType<BufferLifeDegen>(), 150);
		}
    }
}
