using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RappleMod.Content.SetBonusChanges
{
    public class NecroFossil : GlobalItem
    {
        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            short S1Ha = ItemID.AncientNecroHelmet;
            short S1H = ItemID.NecroHelmet;
            short S1B= ItemID.NecroBreastplate;
            short S1L = ItemID.NecroGreaves;

            short S2H = ItemID.FossilHelm;
            short S2B= ItemID.FossilShirt;
            short S2L = ItemID.FossilPants;

            if (
                (head.type == S1H || head.type == S1Ha || head.type == S2H) && (body.type == S1B || body.type == S2B)  && (legs.type == S1L || legs.type == S2L) &&
                !((head.type == S1H || head.type == S1Ha) && body.type == S1B && legs.type == S1L) &&
                !(head.type == S2H && body.type == S2B && legs.type == S2L)
                ) return "NecroFossilSet";

            return base.IsArmorSet(head, body, legs);
        }
        public override void UpdateArmorSet(Player player, string set)
        {
            if (set == "NecroFossilSet"){
                player.setBonus = "Critical hits with ranged projectiles slow down enemies and spawn bones in random directions";
                player.GetModPlayer<SetBonusChangesPlayer>().NecroFossilSet = true;
            }
        }
    }
}