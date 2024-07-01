using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RappleMod.Content.SetBonusChanges
{
    public class ChlorophyteShroomite : GlobalItem
    {
        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            short S1Ha = ItemID.ShroomiteHeadgear;
            short S1Hb = ItemID.ShroomiteHelmet;
            short S1Hc = ItemID.ShroomiteMask;
            short S1B= ItemID.ShroomiteBreastplate;
            short S1L = ItemID.ShroomiteLeggings;

            short S2H = ItemID.ChlorophyteHelmet;
            short S2B= ItemID.ChlorophytePlateMail;
            short S2L = ItemID.ChlorophyteGreaves;

            if (
                (head.type == S1Ha || head.type == S1Hb || head.type == S1Hc || head.type == S2H) && (body.type == S1B || body.type == S2B)  && (legs.type == S1L || legs.type == S2L) &&
                !((head.type == S1Ha || head.type == S1Hb || head.type == S1Hc) && body.type == S1B && legs.type == S1L) &&
                !(head.type == S2H && body.type == S2B && legs.type == S2L)
                ) return "ChlorophyteShroomiteSet";

            return base.IsArmorSet(head, body, legs);
        }
        public override void UpdateArmorSet(Player player, string set)
        {
            if (set == "ChlorophyteShroomiteSet"){
                player.setBonus = "Ranged weapons consume mana to increase their power by 1.5x\nMana consumed based on weapon fire rate";
                player.GetModPlayer<SetBonusChangesPlayer>().ChlorophyteShroomiteSet = true;
            }
        }
    }
}