using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RappleMod.Content.SetBonusChanges
{
    public class MoltenShadow : GlobalItem
    {
        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            short S1H = ItemID.AncientShadowHelmet;
            short S1B= ItemID.AncientShadowScalemail;
            short S1L = ItemID.AncientShadowGreaves;

            short S2H = ItemID.ShadowHelmet;
            short S2B = ItemID.ShadowScalemail;
            short S2L = ItemID.ShadowGreaves;

            short S3H = ItemID.MoltenHelmet;
            short S3B = ItemID.MoltenBreastplate;
            short S3L = ItemID.MoltenGreaves;

            if (
                (head.type == S1H || head.type == S2H) && (body.type == S1B || body.type == S2B || body.type == S3B)  && (legs.type == S1L || legs.type == S2L || legs.type == S3L) &&
                !(head.type == S1H && body.type == S1B && legs.type == S1L) &&
                !(head.type == S2H && body.type == S2B && legs.type == S2L) &&
                !(head.type == S3H && body.type == S3B && legs.type == S3L) &&
                !((head.type == S1H || head.type == S2H) && (body.type == S1B || body.type == S2B)  && (legs.type == S1L || legs.type == S2L))
                ) return "MoltenShadowSet";

            return base.IsArmorSet(head, body, legs);
        }
        public override void UpdateArmorSet(Player player, string set)
        {
            if (set == "MoltenShadowSet"){
                player.setBonus = "Increases Melee crit chance based on amount of enemies set on fire";
                player.GetModPlayer<SetBonusChangesPlayer>().MoltenShadowSet = true;

                int MoltenShadowCounter = 0;
				foreach (NPC npc in Main.npc){
					if ((npc.HasBuff(BuffID.OnFire) || npc.HasBuff(BuffID.OnFire3)) && npc.type != NPCID.TargetDummy && npc.active){
						MoltenShadowCounter++;
					}
				}

                player.GetCritChance(DamageClass.Melee) += MoltenShadowCounter * 3;
            }
        }
    }
}