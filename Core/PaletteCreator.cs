using PaletteAnalyzerKey.WorldGenProcessors;
using System;
using System.Collections.Generic;
using System.Linq;
using PaletteAnalyzerKey.Data;
using Terraria;
using System.Threading.Tasks;

namespace PaletteAnalyzerKey.Core
{
    public class PaletteCreator
    {
        private const byte BORDER_SIZE = 4;
        private readonly int _maxTileCount = DataManager.MaxTileCount;
        private readonly int _maxWallCount = DataManager.MaxWallCount;
        private int _maxPaintValue = DataManager.MaxPaintValue;
        private HashSet<ushort> _Tiles = new HashSet<ushort>();
        private HashSet<ushort> _Walls = new HashSet<ushort>();
        public PaletteCreator(ExceptionsManager exceptionsManager) 
        {
            foreach (var item in exceptionsManager.Tiles)
            {
                _Tiles.Add(ushort.Parse(item));
            }
            foreach (var item in exceptionsManager.Walls)
            {
                _Walls.Add(ushort.Parse(item));
            }
        }
        
        public bool CreatePalette(MapBounds bounds)
        {
            ushort minX, minY, maxX, maxY, offset = 0; ;
            minX = bounds.MinX;
            minY = bounds.MinY;
            maxX = bounds.MaxX;
            maxY = bounds.MaxY;
            bool success;
            success = AreaProcess.CreateBorder(minX, minY, maxX, maxY, BORDER_SIZE);
            if (!success)
                return false;
            success = AreaProcess.ProcessArea((ushort)(minX - BORDER_SIZE), (ushort)(minY - BORDER_SIZE), (ushort)(maxX + BORDER_SIZE), (ushort)(maxY + BORDER_SIZE));
            if (!success)
                return false;
            success = (ProcessTiles(minX, maxX, minY, maxY, ref offset) && ProcessWalls(minX, maxX, minY, maxY, ref offset));
            return success;
        }
        private bool ProcessTiles(ushort minx, ushort maxx, ushort miny, ushort maxy, ref ushort offset)
        {
            if (minx >= maxx || miny >= maxy)
                return false;

            bool success = true;
            ushort tileID = 0;
            byte paint = 0;

            for (ushort x = minx; x < maxx; x++)
            {
                for (ushort y = miny; y < maxy; y++)
                {
                    if (!WorldGen.InWorld(x, y))
                        continue;

                    try
                    {
                        WorldPlacer.SetAir(x, y);
                        if (!_Tiles.Contains(tileID) && tileID <= _maxTileCount)
                        {
                            WorldPlacer.SetTile(x, y, tileID, paint);
                        }

                        if (paint >= _maxPaintValue)
                        {
                            tileID++;
                            paint = 0;
                            if (tileID > _maxTileCount)
                                break;
                        }
                        else
                        {
                            paint++;
                        }
                    }
                    catch
                    {
                        WorldPlacer.SetAir(x, y);
                        offset = x;
                        success = false;
                        break; // Выход из внутреннего цикла
                    }

                    if (!success)
                        break;
                }

                if (!success || tileID > _maxTileCount)
                {
                    if (tileID > _maxTileCount)
                        offset = x;
                    break; // Выход из внешнего цикла
                }
            }
            NetMessage.SendTileSquare(-1, minx, miny, maxx - minx, maxy - miny);

            return success;
        }
        private bool ProcessWalls(ushort minx, ushort maxx, ushort miny, ushort maxy, ref ushort offset)
        {
            if (minx >= maxx || miny >= maxy)
                return false;

            bool success = true;
            ushort wallID = 1;
            byte paint = 0;

            for (ushort x = (ushort)(minx + offset + 1); x < maxx; x++)
            {
                for (ushort y = miny; y < maxy; y++)
                {
                    if (!WorldGen.InWorld(x, y))
                        continue;

                    try
                    {
                        WorldPlacer.SetAir(x, y);
                        if (!_Walls.Contains(wallID) && wallID <= _maxWallCount)
                        {
                            WorldPlacer.SetWall(x, y, wallID, paint);
                        }

                        if (paint >= _maxPaintValue)
                        {
                            wallID++;
                            paint = 0;
                            if (wallID > _maxWallCount)
                                break;
                        }
                        else
                        {
                            paint++;
                        }
                    }
                    catch
                    {
                        WorldPlacer.SetAir(x, y);
                        offset = x;
                        success = false;
                        break; // Выход из внутреннего цикла
                    }

                    if (!success)
                        break;
                }

                // Проверяем условия выхода после внутреннего цикла
                if (!success || wallID > _maxWallCount)
                {
                    if (wallID > _maxWallCount)
                        offset = x;
                    break; // Выход из внешнего цикла
                }
            }

            NetMessage.SendTileSquare(-1, minx, miny, maxx - minx, maxy - miny);

            return success;
        }
    }
}
