using RappleMod.Content.Globals;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RappleMod.Content.Buffs
{
	public class FrostburnCopy : ModBuff
	{
        public override string Texture => $"Terraria/Images/Buff_{BuffID.Frostburn}";
		public override void SetStaticDefaults() {
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true; 
			BuffID.Sets.LongerExpertDebuff[Type] = true;
		}

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<MyGlobalNPCs>().onFrostburnCopy = true;
        }
    }
}
