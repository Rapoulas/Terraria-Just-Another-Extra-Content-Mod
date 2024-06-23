using Microsoft.Xna.Framework;
using RappleMod.Content.GlobalNPCandProj;
using RappleMod.Content.GoreAndDust;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RappleMod.Content.Buffs
{
	public class HeartbrokenDebuff : ModBuff
	{
		public override void SetStaticDefaults() {
			Main.debuff[Type] = true;
			Main.pvpBuff[Type] = true;
			Main.buffNoSave[Type] = true; 
			BuffID.Sets.LongerExpertDebuff[Type] = true;
		}

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<MyGlobalNPCs>().heartbroken = true;

        }

        internal static void DrawEffects(NPC npc, ref Color drawColor)
        {
            if (Main.rand.NextBool(5))
            {
                Vector2 vector2 = new(Main.rand.Next(-10, 11), Main.rand.Next(-10, 11));
				vector2.Normalize();
				vector2.X *= 0.66f;
				int num2 = Gore.NewGore(npc.GetSource_FromThis(), npc.position + new Vector2(Main.rand.Next(npc.width + 1), Main.rand.Next(npc.height + 1)), vector2 * Main.rand.Next(3, 6) * 0.33f, ModContent.GoreType<HeartbrokenGore>(), (float)Main.rand.Next(40, 121) * 0.01f);
				Main.gore[num2].sticky = false;
				Main.gore[num2].velocity *= 0.4f;
				Main.gore[num2].velocity.Y -= 0.6f;
            }
        }
    }
}
