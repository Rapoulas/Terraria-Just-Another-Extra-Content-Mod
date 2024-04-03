using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RappleMod.Content.SetBonusChanges
{
    public class MeleeHallowedChlorophyte : GlobalItem
    {
        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            short S1H = ItemID.HallowedMask;
            short S1B= ItemID.HallowedPlateMail;
            short S1L = ItemID.HallowedGreaves;

            short S2H = ItemID.AncientHallowedMask;
            short S2B = ItemID.AncientHallowedPlateMail;
            short S2L = ItemID.AncientHallowedGreaves;

            short S3H = ItemID.ChlorophyteMask;
            short S3B = ItemID.ChlorophytePlateMail;
            short S3L = ItemID.ChlorophyteGreaves;

            if (
                (head.type == S1H || head.type == S2H || head.type == S3H) && (body.type == S1B || body.type == S2B || body.type == S3B)  && (legs.type == S1L || legs.type == S2L || legs.type == S3L) &&
                !(head.type == S1H && body.type == S1B && legs.type == S1L) &&
                !(head.type == S2H && body.type == S2B && legs.type == S2L) &&
                !(head.type == S3H && body.type == S3B && legs.type == S3L)
                ) return "MeleeHallowedChlorophyteSet";

            return base.IsArmorSet(head, body, legs);
        }
        public override void UpdateArmorSet(Player player, string set)
        {
            if (set == "MeleeHallowedChlorophyteSet"){
                player.setBonus = "Melee attacks surrounds the player in a buffing aura that damages nearby enemies\n";
                player.GetModPlayer<SetBonusChangesPlayer>().MeleeHallowedChlorophyteSet = true;
            }
        }
    }
}