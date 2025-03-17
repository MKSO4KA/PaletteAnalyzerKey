using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace PaletteAnalyzerKey
{
    public class PaletteAnalyzer : Mod
    {
        public override void Load()
        {
            MapOverView = KeybindLoader.RegisterKeybind(this, "MapOverView", Keys.F7);
            Palette = KeybindLoader.RegisterKeybind(this, "Palette Creator", Keys.F5);
            Parameters = KeybindLoader.RegisterKeybind(this, "Godmode", Keys.F3);
            InsertArt = KeybindLoader.RegisterKeybind(this, "ArtInsert", Keys.F10);
            StartVideoProcess = KeybindLoader.RegisterKeybind(this, "Start Video Paste", Keys.RightControl);
        }
        public override void Unload()
        {
            StartVideoProcess = null;
            Palette = null;
            InsertArt = null;
            MapOverView = null;
            Parameters = null;

        }

        public static ModKeybind Palette, MapOverView, InsertArt, Parameters, StartVideoProcess;
    }
}