using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent;
using Terraria.GameContent.Achievements;
using Terraria.GameContent.Events;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;
using Terraria.Utilities;

namespace RappleMod.Content.NPCs
{
    public class MyGlobalNPCs : GlobalNPC
    {
        public override void UpdateLifeRegen(NPC npc, ref int damage){
            Player player =  Main.LocalPlayer;

            if (npc.onFrostBurn && player.GetModPlayer<SetBonusChangesPlayer>().CobaltPalladiumFrostSet == true)
            {
                
                int FrostburnBaseDot = (int)(16 * 0.5);
                npc.lifeRegen -= FrostburnBaseDot;
                if (damage < FrostburnBaseDot / 4)
                    damage = FrostburnBaseDot / 4;
            }

            if (npc.onFrostBurn2 && player.GetModPlayer<SetBonusChangesPlayer>().CobaltPalladiumFrostSet == true)
            {
                int FrostbiteBaseDot = (int)(50 * 0.5);
                npc.lifeRegen -= FrostbiteBaseDot;
                if (damage < FrostbiteBaseDot / 4)
                    damage = FrostbiteBaseDot / 4;
            }
        }
    }
}