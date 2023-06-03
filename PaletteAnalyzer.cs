using Microsoft.Xna.Framework.Input;
using Terraria.ModLoader;

namespace PaletteAnalyzer
{
	public class PaletteAnalyzer : Mod
	{
        public override void Load()
        {
            Palette = KeybindLoader.RegisterKeybind(this, "Palette Creator", Keys.F5);
        }
        public override void Unload()
        {
            Palette = null;
        }

        public static ModKeybind Palette;
    }
}