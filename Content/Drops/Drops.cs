using Terraria.GameContent.ItemDropRules;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RappleMod.Content.Weapons;

namespace RappleMod.Content.Drops
{
    public class BossBagsDrops : GlobalItem
    {
        public override void ModifyItemLoot(Item item, ItemLoot loot)
        {
            if (item.type == ItemID.SkeletronBossBag){
                loot.Add(ItemDropRule.Common(ModContent.ItemType<Bonetrousle>(), 5)); 
            }
            if (item.type == ItemID.GolemBossBag){
                loot.Add(ItemDropRule.Common(ModContent.ItemType<Linoge>(), 5));
            }

            if (item.type == ItemID.PlanteraBossBag){
                loot.Add(ItemDropRule.Common(ModContent.ItemType<GunSummon>(), 5));
            }
        }
    }

    public class GloabLNPCDrops : GlobalNPC
	{
		public override void ModifyNPCLoot(NPC npc, NPCLoot npcLoot) {
			if (npc.type == NPCID.Plantera) {
				npcLoot.Add(ItemDropRule.ByCondition(new Conditions.NotExpert(), ModContent.ItemType<GunSummon>(), 5));
			}
		}
	}
}