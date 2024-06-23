using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace RappleMod.Content.GoreAndDust
{
	public class HeartbrokenGore : ModGore
	{
        public override void OnSpawn(Gore gore, IEntitySource source)
        {
            ChildSafety.SafeGore[gore.type] = true;
            UpdateType = 331;
        }
    }
}
