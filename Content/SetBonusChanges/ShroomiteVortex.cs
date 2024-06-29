using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RappleMod.Content.SetBonusChanges
{
    public class ShroomiteVortex : GlobalItem
    {
        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            short S1Ha = ItemID.ShroomiteHeadgear;
            short S1Hb = ItemID.ShroomiteHelmet;
            short S1Hc = ItemID.ShroomiteMask;
            short S1B= ItemID.ShroomiteBreastplate;
            short S1L = ItemID.ShroomiteLeggings;

            short S2H = ItemID.VortexHelmet;
            short S2B= ItemID.VortexBreastplate;
            short S2L = ItemID.VortexLeggings;

            if (
                (head.type == S1Ha || head.type == S1Hb || head.type == S1Hc || head.type == S2H) && (body.type == S1B || body.type == S2B)  && (legs.type == S1L || legs.type == S2L) &&
                !((head.type == S1Ha || head.type == S1Hb || head.type == S1Hc) && body.type == S1B && legs.type == S1L) &&
                !(head.type == S2H && body.type == S2B && legs.type == S2L)
                ) return "ShroomiteVortexSet";

            return base.IsArmorSet(head, body, legs);
        }
        public override void UpdateArmorSet(Player player, string set)
        {
            if (set == "ShroomiteVortexSet"){
                player.setBonus = "Ranged projectiles have a 10% chance to spawn a homing missile on hit that deals 3x damage dealt";
                player.GetModPlayer<SetBonusChangesPlayer>().ShroomiteVortexSet = true;
            }
        }
    }
}