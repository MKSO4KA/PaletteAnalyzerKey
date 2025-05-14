// MapRevealer.cs
using Mono.Cecil.Cil;
using ReLogic.Content;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Terraria;

namespace PaletteAnalyzerKey.Utilites
{
    public static class MapRevealer
    {
        private const sbyte OFFSET = 10;
        public static bool RevealMapArea(int minTilesX, int minTilesY, int wideTiles, int heightTiles)
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
        public static bool RevealMapArea((int x, int y)[] coordinates)
        {
            var map = Main.Map;
            int worldWidth = Main.maxTilesX;
            int worldHeight = Main.maxTilesY;

            // Используем Span для быстрого доступа
            var coordsSpan = coordinates.AsSpan();
            for (int i = 0; i < coordsSpan.Length; i++)
            {
                var coord = coordsSpan[i];
                if (coord.x == 0 || coord.y == 0)
                {
                    break;
                }
                if ((uint)coord.x < worldWidth && (uint)coord.y < worldHeight)
                {
                    map.Update(coord.x, coord.y, 255);
                }
            }

            Main.refreshMap = true;
            return true;
        }
        public static bool RevealMapArea(ReadOnlyMemory<Point> coordinatesMemory)
        {

            var map = Main.Map; // Захватываем ссылку на карту

            // Если количество координат небольшое, накладные расходы на параллелизм могут быть больше выгоды.
            const int parallelThreshold = 1024; // Например, больше 1024 элементов

            if (coordinatesMemory.Length > parallelThreshold) // Используем Length от ReadOnlyMemory
            {
                // 'coordinatesMemory' захватывается лямбда-выражением, что допустимо,
                // так как ReadOnlyMemory<T> не является ref struct.
                Parallel.For(0, coordinatesMemory.Length, i =>
                {
                    // Внутри лямбды мы получаем Span из захваченной 'coordinatesMemory'.
                    // Этот Span является локальным для данной итерации и существует в стеке потока.
                    var coord = coordinatesMemory.Span[i];
                    map.Update(coord.X, coord.Y, 255);
                });
            }
            else
            {
                // Для последовательного выполнения можно получить Span один раз.
                ReadOnlySpan<Point> coordsSpan = coordinatesMemory.Span;
                for (int i = 0; i < coordsSpan.Length; i++)
                {
                    var coord = coordsSpan[i];
                    map.Update(coord.X, coord.Y, 255);
                }
            }

            Main.refreshMap = true;
            return true;
        }
    }

}