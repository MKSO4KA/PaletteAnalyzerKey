using PaletteAnalyzerKey.Utilites;
using PaletteAnalyzerKey.WorldGenProcessors;
using System;
using System.Buffers;
using Terraria;

namespace PaletteAnalyzerKey.Core
{
    internal class FrameCreator
    {
        // Структура для улучшения читаемости
        

        public static void InsertBlock(string filepath) // Убрали async, т.к. нет асинхронных операций
        {
            var bw = new BinaryWorker(filepath);
            var photo = bw.Read();
            
            // Предварительная проверка условий
            if (Main.worldID < 1 || Main.myPlayer < 0)
                return;

            int width = bw.Width;
            int height = bw.Height;
            int i = 0;
            int replacedCount = 0;
            Point[] replacedCoordinatesArray = ArrayPool<Point>.Shared.Rent(width * height);
            // Предвычисление границ
            try
            {
                int maxX = 100 + width;
                int maxY = 100 + height;

                for (int w = 0; w < width; w++)
                {
                    int x = w + 100;
                    bool inWorldX = x >= 0 && x < Main.maxTilesX;

                    for (int h = 0; h < height; h++)
                    {
                        int y = h + 100;

                        // Проверка границ один раз
                        if (!inWorldX || y < 0 || y >= Main.maxTilesY)
                        {
                            i++;
                            continue;
                        }

                        // Кэширование тайла
                        var tile = Main.tile[x, y];
                        var currentPhoto = photo[i];

                        // Проверка необходимости изменения тайла
                        if (ShouldSkipTile(tile, currentPhoto, x, y))
                        {
                            i++;
                            continue;
                        }
                        replacedCoordinatesArray[replacedCount++] = new Point(x, y);
                        ProcessTile(x, y, currentPhoto);
                        i++;
                    }
                }

                if (replacedCount > 0)
                {
                    ReadOnlyMemory<Point> relevantCoordinatesMemory = new ReadOnlyMemory<Point>(replacedCoordinatesArray, 0, replacedCount);
                    MapRevealer.RevealMapArea(relevantCoordinatesMemory);
                }
            }
            finally
            {
                ArrayPool<Point>.Shared.Return(replacedCoordinatesArray);
            }
            
        }

        private static bool ShouldSkipTile(Tile tile, PhotoData photo, int x, int y)
        {

            return (tile.TileType == photo.Id)
                   && tile.TileColor == photo.PaintId;
        }

        private static void ProcessTile(int x, int y, PhotoData photo)
        {
            WorldPlacer.SetAir(x, y);

            switch (DetermineState(photo))
            {
                case 0:
                case 2:
                    WorldPlacer.SetTile(x, y, photo.Id, photo.PaintId);
                    break;
                case 1:
                    WorldPlacer.SetWall(x, y, photo.Id, photo.PaintId);
                    break;
                default:
                    // Логирование неожиданного состояния
                    Main.NewText($"Unexpected state for tile at {x},{y}", 255, 0, 0);
                    break;
            }
        }

        private static int DetermineState(PhotoData photo)
        {
            if (!photo.IsWall && !photo.IsTorch) return 0;
            return photo.IsTorch ? 2 : 1;
        }
    }
}