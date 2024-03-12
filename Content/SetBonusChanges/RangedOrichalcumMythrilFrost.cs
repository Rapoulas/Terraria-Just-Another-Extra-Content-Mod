using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RappleMod.Content.SetBonusChanges
{
    public class RangedOrichalcumMythrilFrost : GlobalItem
    {
        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            short S1H = ItemID.MythrilHat;
            short S1B= ItemID.MythrilChainmail;
            short S1L = ItemID.MythrilGreaves;

            short S2H = ItemID.OrichalcumHelmet;
            short S2B = ItemID.OrichalcumBreastplate;
            short S2L = ItemID.OrichalcumLeggings;

            short S3H = ItemID.FrostHelmet;
            short S3B = ItemID.FrostBreastplate;
            short S3L = ItemID.FrostLeggings;

            if (
                (head.type == S1H || head.type == S2H) && (body.type == S1B || body.type == S2B || body.type == S3B)  && (legs.type == S1L || legs.type == S2L || legs.type == S3L) &&
                !(head.type == S1H && head.type == S1B && legs.type == S1L) &&
                !(head.type == S2H && head.type == S2B && legs.type == S2L) &&
                !(head.type == S3H && head.type == S3B && legs.type == S3L)
                ) return "RangedOrichalcumMythrilSet";

            return base.IsArmorSet(head, body, legs);
        }
        public override void UpdateArmorSet(Player player, string set)
        {
            player.setBonus = "Chance to chain damage to nearby enemies when hitting a\nFrostburnt or Frostbitten enemy with a projectile\nIncreases the damage of frost debuffs by 50%";
            player.GetModPlayer<SetBonusChangesPlayer>().RangedOrichalcumMythrilFrostSet = true;
        }
    }
}