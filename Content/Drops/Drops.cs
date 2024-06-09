using Terraria.GameContent.ItemDropRules;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using RappleMod.Content.Weapons;

namespace CalamityMod.Items
{
    public class BossBags : GlobalItem
    {
        public override void ModifyItemLoot(Item item, ItemLoot loot)
        {
            if (item.type == ItemID.SkeletronBossBag){
                loot.Add(new CommonDrop(ModContent.ItemType<Bonetrousle>(), 15)); 
            }
            if (item.type == ItemID.GolemBossBag){
                loot.Add(new CommonDrop(ModContent.ItemType<Linoge>(), 15)); 
            }

            if (item.type == ItemID.PlanteraBossBag){
                loot.Add(new CommonDrop(ModContent.ItemType<GunSummon>(), 15));
            }
        }
    }
}