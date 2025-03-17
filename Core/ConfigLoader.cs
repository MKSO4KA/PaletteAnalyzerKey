using Newtonsoft.Json;
using System.IO;
using Terraria.ModLoader;

namespace PaletteAnalyzerKey.Core
{
    public class ConfigLoader
    {
        public PaletteAnalyzerConfig Config { get; }
        public ExceptionsManager Exceptions { get; } = new();
        public MapBounds Bounds { get; private set; }
        public string PalettePath { get; private set; }
        public string VideoDirectory { get; private set; }
        public string FileName { get; private set; }
        public string PhotoFilePath { get; private set; }
        public ConfigLoader()
        {
            Config = ModContent.GetInstance<PaletteAnalyzerConfig>();
            LoadAll();
        }

        private void LoadAll()
        {
            Exceptions.LoadFromConfig(Config);
            Bounds = new MapBounds(
                ushort.Parse(Config.MinTileX),
                ushort.Parse(Config.MinTileY),
                ushort.Parse(Config.MaxTileX),
                ushort.Parse(Config.MaxTileY)
            );
            FileName = Config.VideoFileName;
            VideoDirectory = Config.VideoDirectoryPath;
            PalettePath = Config.PaletteFilePath;
            PhotoFilePath = Config.PhotoFilePath;
        }
    }

    public record MapBounds(ushort MinX, ushort MinY, ushort MaxX, ushort MaxY);
}