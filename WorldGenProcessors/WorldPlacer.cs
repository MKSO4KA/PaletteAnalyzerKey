using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;

namespace PaletteAnalyzerKey.WorldGenProcessors
{
    public class WorldPlacer
    {
        public static void SetTile(int x, int y, ushort TileId, byte paintId, bool isActive = true)
        {
            Tile tile1 = Main.tile[x, y];
            tile1.HasTile = true;
            tile1.BlockType = BlockType.Solid;
            tile1.LiquidAmount = 0;
            tile1.TileType = TileId;
            tile1.TileColor = paintId;
            tile1.IsActuated = isActive;
            tile1.WallType = 1;
            if (Main.tile[x, y] == tile1)
                return;

            Main.tile[x, y].CopyFrom(tile1);
        }
        public static void SetWall(int x, int y, ushort WallId, byte paintId)
        {
            Tile tile2 = Main.tile[x, y];
            tile2.LiquidAmount = 0;
            tile2.WallType = WallId;
            tile2.WallColor = paintId;
            tile2.IsActuated = true;
            Main.tile[x, y].CopyFrom(tile2);
        }
        public static void SetAir(int x, int y, int w = 1, int h = 1)
        {
            for (int i = x; i < x + w; i++)
            {
                for (int k = y; k < y + h; k++)
                {
                    Main.tile[x, y].ClearEverything();
                }
            }
        }
    }
}
