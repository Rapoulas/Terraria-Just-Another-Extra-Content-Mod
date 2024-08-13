using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.ModLoader;


namespace RappleMod{
    public static partial class TooltipKeybindUtil
    {
        // Credits to Calamity Mod for this
		public static string TooltipHotkeyString(this ModKeybind mhk)
        {
            if (Main.dedServ || mhk is null)
                return "";

            List<string> keys = mhk.GetAssignedKeys();
            if (keys.Count == 0)
                return "[NONE]";
			else
            {
                StringBuilder sb = new StringBuilder(16);
                if (keys[0] == "Mouse2"){
                    sb.Append("Right Click");
                }
                else if (keys[0] == "Mouse1"){
                    sb.Append("Left Click");
                }
                else {
                    sb.Append(keys[0]);
                }

                // In almost all cases, this code won't run, because there won't be multiple bindings for the hotkey. But just in case...
                for (int i = 1; i < keys.Count; ++i)
                    sb.Append(" / ").Append(keys[i]);
                return sb.ToString();
            }
		}

		public static void FindAndReplace(this List<TooltipLine> tooltips, string replacedKey, string newKey)
        {
            TooltipLine line = tooltips.FirstOrDefault(x => x.Mod == "Terraria" && x.Text.Contains(replacedKey));
            if (line != null)
                line.Text = line.Text.Replace(replacedKey, newKey);
        }

        public static void IntegrateHotkey(this List<TooltipLine> tooltips, ModKeybind mhk)
        {
            if (Main.dedServ || mhk is null)
                return;
            
            string finalKey = mhk.TooltipHotkeyString();
            tooltips.FindAndReplace("[KEY]", finalKey);
        }
	}
}