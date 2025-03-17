// MapRevealer.cs
using ReLogic.Content;
using System;
using System.Threading.Tasks;
using Terraria;

namespace PaletteAnalyzerKey.Utilites
{
    public static class MapRevealer
    {
        private const sbyte OFFSET = 10;
        public static async Task<bool> RevealMapArea(int minTilesX, int minTilesY, int wideTiles, int heightTiles)
        {
            if (wideTiles <= 0 || heightTiles <= 0) { return false; }
            minTilesX -= OFFSET;
            minTilesY -= OFFSET;
            if (minTilesX < 0) { minTilesX = 0; }
            if (minTilesY < 0) { minTilesY = 0; }
            int maxTilesX = minTilesX + wideTiles + 2 * OFFSET;
            int maxTilesY = minTilesY + heightTiles + 2 * OFFSET;

            if (maxTilesX >= Main.maxTilesX)
            {
                maxTilesX = Main.maxTilesX - 1;
            }
            if (maxTilesY >= Main.maxTilesY)
            {
                maxTilesY = Main.maxTilesY - 1;
            }

            for (int i = minTilesX; i < maxTilesX; i++)
            {
                for (int j = minTilesY; j < maxTilesY; j++)
                {

                    if (Terraria.WorldGen.InWorld(i, j))
                        Main.Map.Update(i, j, 255);
                }
            }
            Main.refreshMap = true;
            return true;
        }
    }

}