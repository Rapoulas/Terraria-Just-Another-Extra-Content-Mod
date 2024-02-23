using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Terraria.GameInput;
using System.IO;
using TutorialMod.Content.Acessories;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.Utilities;
using Terraria.Localization;
using TutorialMod.Content.Projectiles;

namespace TutorialMod{
    public class MyPlayer : ModPlayer {
        #region item field
        public Item ZetaReference;
        #endregion

        public override void OnHurt(Player.HurtInfo info) {
            ZetaReferenceOnHurt(info);
        }

        private void ZetaReferenceOnHurt(Player.HurtInfo info){
            if (ZetaReference == null) return;

            for (int i = 0; i < 250; i++){
                if (!Main.npc[i].active || Main.npc[i].friendly){
                    continue;
                }
                float j = (Main.npc[i].Center - Player.Center).Length();
                    
                if (j < 200){
                    Main.npc[i].AddBuff(BuffID.Ichor, 300);
                    Main.npc[i].AddBuff(BuffID.Confused, 300);
                }
			}
		}
    }
}