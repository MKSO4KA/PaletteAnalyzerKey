using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace PaletteAnalyzer
{
    public class PaletteAnalyzer : Mod
    {
        public override void Load()
        {
            MapOverView = KeybindLoader.RegisterKeybind(this, "MapOverView", Keys.F7);
            Palette = KeybindLoader.RegisterKeybind(this, "Palette Creator", Keys.F5);
            Parameters = KeybindLoader.RegisterKeybind(this, "Godmode&Tp", Keys.F3);
            Insert = KeybindLoader.RegisterKeybind(this, "ArtInsert", Keys.F10);
        }
        public override void Unload()
        {
            Palette = null;
            Insert = null;
            MapOverView = null;
            Parameters = null;

        }

        public static ModKeybind Palette, MapOverView, Insert, Parameters;
    }
}