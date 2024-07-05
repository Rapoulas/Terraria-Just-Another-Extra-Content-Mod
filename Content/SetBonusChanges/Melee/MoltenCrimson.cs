using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RappleMod.Content.SetBonusChanges
{
    public class MoltenCrimson : GlobalItem
    {
        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            short S1H = ItemID.CrimsonHelmet;
            short S1B= ItemID.CrimsonScalemail;
            short S1L = ItemID.CrimsonGreaves;

            short S2H = ItemID.MoltenHelmet;
            short S2B= ItemID.MoltenBreastplate;
            short S2L = ItemID.MoltenGreaves;

            if (
                (head.type == S1H|| head.type == S2H) && (body.type == S1B || body.type == S2B)  && (legs.type == S1L || legs.type == S2L) &&
                !(head.type == S1H && body.type == S1B && legs.type == S1L) &&
                !(head.type == S2H && body.type == S2B && legs.type == S2L)
                ) return "MoltenCrimsonSet";

            return base.IsArmorSet(head, body, legs);
        }
        public override void UpdateArmorSet(Player player, string set)
        {
            if (set == "MoltenCrimsonSet"){
                player.setBonus = "Increases health regen based on amount of enemies set on fire";
                player.GetModPlayer<SetBonusChangesPlayer>().MoltenCrimsonSet = true;

                int MoltenCrimsonCounter = 0;
				foreach (NPC npc in Main.npc){
					if ((npc.HasBuff(BuffID.OnFire) || npc.HasBuff(BuffID.OnFire3)) && npc.type != NPCID.TargetDummy && npc.active){
						MoltenCrimsonCounter++;
					}
				}

                player.lifeRegen += MoltenCrimsonCounter * 2;
            }
        }
    }
}