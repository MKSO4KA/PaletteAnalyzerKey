// ExceptionsManager.cs
using PaletteAnalyzerKey.Utilites;
using System.Collections.Generic;
using System.IO;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace PaletteAnalyzerKey.Core
{
    public class ExceptionsManager
    {
        public HashSet<string> Torchs { get; } = new();
        public HashSet<string> Tiles { get; } = new();
        public HashSet<string> Walls { get; } = new();

        public void LoadFromConfig(PaletteAnalyzerConfig config)
        {
            LoadTorchs(config.TorchsPath);
            LoadTileWallsExceptions(config.ExceptionsPath, config.ExceptionsEnabled);
        }

        private void LoadTorchs(string path)
        {
            if (!string.IsNullOrEmpty(path) && File.Exists(path))
            {
                Torchs.UnionWith(File.ReadAllLines(path));
            }
        }

        private void LoadTileWallsExceptions(string path, bool exceptionsEnabled)
        {
            if (string.IsNullOrEmpty(path) || !File.Exists(path)) return;

            Tiles.Clear();
            Walls.Clear();

            foreach (var line in File.ReadAllLines(path))
            {
                var parts = line.Split(':');
                if (parts.Length != 2) continue;

                if (exceptionsEnabled)
                {
                    if (parts[0] == "1") Tiles.Add(parts[1]);
                    else if (parts[0] == "0") Walls.Add(parts[1]);
                }
            }
        }
    }
}