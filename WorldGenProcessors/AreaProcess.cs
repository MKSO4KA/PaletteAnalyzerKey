using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace PaletteAnalyzerKey.WorldGenProcessors
{
    internal static class AreaProcess
    {
        private static bool DrawBorderLine(ushort startX, ushort startY, ushort endX, ushort endY, bool isVertical)
        {
            try
            {
                // Проверка координат
                if (!WorldGen.InWorld(startX, startY) || !WorldGen.InWorld(endX, endY))
                    return false;

                // Определяем направление и шаги
                int steps = isVertical
                    ? endY - startY
                    : endX - startX;

                if (steps == 0) return true; // Нулевая длина линии

                sbyte direction = (sbyte)(steps > 0 ? 1 : -1);
                steps = Math.Abs(steps);

                for (int i = 0; i <= steps; i++)
                {
                    ushort x = (ushort)(isVertical ? startX : startX + i * direction);
                    ushort y = (ushort)(isVertical ? startY + i * direction : startY);

                    if (!WorldGen.InWorld(x, y))
                        continue;

                    WorldPlacer.SetAir(x, y);
                    WorldPlacer.SetTile(x, y, 1, 0, false);
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool CreateBorder(ushort minx, ushort miny, ushort maxx, ushort maxy, ushort borderSize)
        {
            // Валидация основных параметров
            if (minx >= maxx || miny >= maxy ||
                borderSize == 0 ||
                !WorldGen.InWorld(minx, miny) ||
                !WorldGen.InWorld(maxx, maxy))
                return false;

            // Рассчет границ с проверкой переполнения
            try
            {
                checked
                {
                    ushort left = (ushort)(minx - borderSize);
                    ushort right = (ushort)(maxx + borderSize - 1);
                    ushort top = (ushort)(miny - borderSize);
                    ushort bottom = (ushort)(maxy + borderSize - 1);

                    if (!WorldGen.InWorld(left, top) ||
                        !WorldGen.InWorld(right, bottom))
                        return false;

                    return DrawBorderLine(left, top, left, bottom, true)   // Левая
                        & DrawBorderLine(right, top, right, bottom, true)  // Правая
                        & DrawBorderLine(left, top, right, top, false)     // Верх
                        & DrawBorderLine(left, bottom, right, bottom, false); // Низ
                }
            }
            catch (OverflowException)
            {
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool ProcessArea(ushort startX, ushort startY, ushort endX, ushort endY)
        {
            // Валидация координат
            if (startX >= Main.maxTilesX || endX >= Main.maxTilesX ||
                startY >= Main.maxTilesY || endY >= Main.maxTilesY)
                return false;

            // Корректировка координат
            startX = Math.Clamp(startX, (ushort)0, (ushort)(Main.maxTilesX - 1));
            startY = Math.Clamp(startY, (ushort)0, (ushort)(Main.maxTilesY - 1));
            endX = Math.Clamp(endX, (ushort)0, (ushort)(Main.maxTilesX - 1));
            endY = Math.Clamp(endY, (ushort)0, (ushort)(Main.maxTilesY - 1));

            // Проверка области
            if (startX > endX || startY > endY)
                return false;

            try
            {
                for (int x = startX; x <= endX; x++)
                {
                    for (int y = startY; y <= endY; y++)
                    {
                        WorldPlacer.SetAir(x, y);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
