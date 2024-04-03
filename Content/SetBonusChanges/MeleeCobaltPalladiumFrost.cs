using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RappleMod.Content.SetBonusChanges
{
    public class MeleeCobaltPalladiumFrost : GlobalItem
    {
        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            short S1H = ItemID.CobaltHelmet;
            short S1B= ItemID.CobaltBreastplate;
            short S1L = ItemID.CobaltLeggings;

            short S2H = ItemID.PalladiumMask;
            short S2B = ItemID.PalladiumBreastplate;
            short S2L = ItemID.PalladiumLeggings;

            short S3H = ItemID.FrostHelmet;
            short S3B = ItemID.FrostBreastplate;
            short S3L = ItemID.FrostLeggings;

            if (
                (head.type == S1H || head.type == S2H) && (body.type == S1B || body.type == S2B || body.type == S3B)  && (legs.type == S1L || legs.type == S2L || legs.type == S3L) &&
                !(head.type == S1H && body.type == S1B && legs.type == S1L) &&
                !(head.type == S2H && body.type == S2B && legs.type == S2L) &&
                !(head.type == S3H && body.type == S3B && legs.type == S3L)
                ) return "MeleeCobaltPalladiumFrostSet";

            return base.IsArmorSet(head, body, legs);
        }
        public override void UpdateArmorSet(Player player, string set)
        {
            if (set == "MeleeCobaltPalladiumFrostSet"){
                player.setBonus = "Melee attacks can apply Frostburn and Frostbite\nEnemies have a chance to spread frost debuffs to nearby enemies\nIncreases the damage of frost debuffs by 50%";
                player.GetModPlayer<SetBonusChangesPlayer>().MeleeCobaltPalladiumFrostSet = true;
            }
        }
    }
}