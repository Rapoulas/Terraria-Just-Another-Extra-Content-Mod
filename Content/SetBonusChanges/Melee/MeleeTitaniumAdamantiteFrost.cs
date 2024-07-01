using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RappleMod.Content.SetBonusChanges
{
    public class MeleeTitaniumAdamantiteFrost : GlobalItem
    {
        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            short S1H = ItemID.AdamantiteHelmet;
            short S1B= ItemID.AdamantiteBreastplate;
            short S1L = ItemID.AdamantiteLeggings;

            short S2H = ItemID.TitaniumMask;
            short S2B = ItemID.TitaniumBreastplate;
            short S2L = ItemID.TitaniumLeggings;

            short S3H = ItemID.FrostHelmet;
            short S3B = ItemID.FrostBreastplate;
            short S3L = ItemID.FrostLeggings;

            if (
                (head.type == S1H || head.type == S2H) && (body.type == S1B || body.type == S2B || body.type == S3B)  && (legs.type == S1L || legs.type == S2L || legs.type == S3L) &&
                !(head.type == S1H && body.type == S1B && legs.type == S1L) &&
                !(head.type == S2H && body.type == S2B && legs.type == S2L) &&
                !(head.type == S3H && body.type == S3B && legs.type == S3L) &&
                !((head.type == S1H || head.type == S2H) && (body.type == S1B || body.type == S2B)  && (legs.type == S1L || legs.type == S2L))
                ) return "MeleeTitaniumAdamantiteFrostSet";

            return base.IsArmorSet(head, body, legs);
        }
        public override void UpdateArmorSet(Player player, string set)
        {
            if (set == "MeleeTitaniumAdamantiteFrostSet"){
                player.setBonus = "Melee hits generate shards\nIncreases melee abilities based on amount of shards\nIncreases the damage of frost debuffs by 50%";
                player.GetModPlayer<SetBonusChangesPlayer>().MeleeTitaniumAdamantiteFrostSet = true;
                player.GetDamage(DamageClass.Melee) += player.ownedProjectileCounts[908]/20f;
                player.GetCritChance(DamageClass.Melee) += player.ownedProjectileCounts[908];
            }
        }
    }
}