using TutorialMod.Content.Acessories;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using TutorialMod;
using Terraria.ID;

namespace TutorialMod.Content.Buffs
{
	public class MeatShieldAbsorb : ModBuff
	{
		public override string Texture => $"Terraria/Images/Buff_{BuffID.PaladinsShield}";
		public override LocalizedText Description => base.Description.WithFormatArgs(MeatShield.DamageAbsorptionPercent);

		public override void SetStaticDefaults() {
			Main.buffNoSave[Type] = true;
			Main.buffNoTimeDisplay[Type] = true;
		}

		public override void Update(Player player, ref int buffIndex) {
			player.GetModPlayer<MyPlayer>().defendedByAbsorbTeamDamageEffect = true;
		}
	}

	public class MeatShieldDmgRegen : ModBuff{

		public override string Texture => $"Terraria/Images/Buff_{BuffID.Wrath}";
        public override void Update(Player player, ref int buffIndex)
        {
            player.lifeRegen += 10;
			player.GetDamage(DamageClass.Generic) += 0.1f;
        }
    }
}
