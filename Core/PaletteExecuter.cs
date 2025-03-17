using PaletteAnalyzerKey.Export;
using System;
using PaletteAnalyzerKey.WorldGenProcessors;
using Terraria;
using System.Threading.Tasks;
using System.Threading;

namespace PaletteAnalyzerKey.Core
{
    public class PaletteExecutor
    {
        private ConfigLoader _configLoader;
        private PaletteExporter _paletteExporter;
        private PaletteCreator _paletteCreator;

        public PaletteExecutor()
        {
            _configLoader = new ConfigLoader();
            var exceptionsManager = new ExceptionsManager();
            _paletteCreator = new PaletteCreator(exceptionsManager);
            _paletteExporter = new PaletteExporter(_configLoader, exceptionsManager);
        }
        public async Task<bool> Execute()
        {
            bool result = true;
            try
            {
                result = _paletteCreator.CreatePalette(_configLoader.Bounds);
                _paletteExporter.Export(_configLoader.PalettePath);
            }
            catch (Exception)
            {
                result = false;
            }
            return result;
        }
    }
}