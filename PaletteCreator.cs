using System;
using System.Linq;
using System.IO;
using System.Windows;
using System.Threading;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using System.Threading.Tasks;
using Terraria.Map;
using Microsoft.Xna.Framework;
using Terraria.ID;
using tModPorter;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Build.Tasks;
using static PaletteAnalyzer.Data;
using Steamworks;
using Microsoft.CodeAnalysis.FlowAnalysis;

namespace PaletteAnalyzer
{
    internal class PaletteCreator
    {

        private const int MAX_PAINT_VALUE = 30;
        public static int BORDER_SIZE = 1;
        private const ushort DEFAULT_MAX_X = 500;
        private const ushort DEFAULT_MAX_Y = 640;
        private const ushort DEFAULT_MIN_X = 100;
        private const ushort DEFAULT_MIN_Y = 495;
        public async Task ExecuteAsync(
    ushort min_x = DEFAULT_MIN_X,
    ushort min_y = DEFAULT_MIN_Y,
    ushort max_x = DEFAULT_MAX_X,
    ushort max_y = DEFAULT_MAX_Y)
        {
            try
            {
                // Запускаем обе операции параллельно, если они независимы
                var setterTask = Task.Run(() => PaletteSetter(min_x, min_y, max_x, max_y));
                // Получение конфига
                var config = ModContent.GetInstance<PaletteAnalyzerConfig>();

                // Использование пути
                string targetDirectory = config.PaletteFilePath;
                if (string.IsNullOrEmpty(targetDirectory))
                {
                    return;
                }
                if (config.BorderMinMax)
                {
                    min_x = ushort.Parse(config.MinTileX);
                    min_y = ushort.Parse(config.MinTileY);
                    max_x = ushort.Parse(config.MaxTileX);
                    max_y = ushort.Parse(config.MaxTileY);
                    if (Math.Abs(min_x - max_x) < 100 || Math.Abs(min_y - max_y) < 100 || Math.Abs(min_x - max_x) * Math.Abs(min_y - max_y) < 10000)
                    {
                        min_x = DEFAULT_MIN_X;
                        min_y = DEFAULT_MIN_Y;
                        max_x = DEFAULT_MAX_X;
                        max_y = DEFAULT_MAX_Y;
                    }
                }
                await setterTask.ConfigureAwait(false);
                var strTask = Task.Run(() => PaletteSttr(
                    targetDirectory,
                    min_x,
                    min_y,
                    max_x,
                    max_y));
                await strTask.ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                // Логирование ошибки
                Console.WriteLine($"Error in ExecuteAsync: {ex}");
                throw; // или обработка ошибки
            }
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
        private static void SetWall(int x, int y, ushort WallId, byte paintId)
        {
            Tile tile2 = Main.tile[x, y];
            tile2.LiquidAmount = 0;
            tile2.WallType = WallId;
            tile2.WallColor = paintId;
            tile2.IsActuated = true;
            Main.tile[x, y].CopyFrom(tile2);
        }
        private static void SetTile(int x, int y, ushort TileId, byte paintId, bool isActive = true)
        {
            Tile tile1 = Main.tile[x, y];
            tile1.HasTile = true;
            tile1.BlockType = BlockType.Solid;
            tile1.LiquidAmount = 0;
            tile1.TileType = TileId;
            tile1.TileColor = paintId;
            tile1.IsActuated = isActive;
            tile1.WallType = 1;
            Main.tile[x, y].CopyFrom(tile1);
        }
        private static bool DataSet()
        {
            var config = ModContent.GetInstance<PaletteAnalyzerConfig>();
            #region Max Tiles
            Data.maxtileX = Convert.ToInt32(config.MaxTileX);
            Data.maxtileY = Convert.ToInt32(config.MaxTileY);
            #endregion
            if (!string.IsNullOrEmpty(config.TorchsPath))
            {
                File.ReadAllLines(config.TorchsPath).Any(Data.Exceptions.Torchs.Add);
            }
            if (!string.IsNullOrEmpty(config.ExceptionsPath))
            {
                if (config.ExceptionsEnabled)
                {
                    foreach (string elem in File.ReadAllLines(config.ExceptionsPath).ToHashSet())
                    {
                        var item = elem.Split(':');
                        if (item.Length != 2) continue;
                        if (item[0] == "1") { Data.Exceptions.Tiles.Add(item[1]); } else { Data.Exceptions.Walls.Add(item[1]); }
                    }
                }
                else
                {
                    Data.Exceptions.Tiles = [];
                    Data.Exceptions.Walls = [];
                    foreach (string elem in File.ReadAllLines(config.ExceptionsPath).ToHashSet())
                    {
                        var item = elem.Split(':');
                        if (item.Length != 2) continue;

                        if (item[0] == "1") { Data.Exceptions.Tiles.Add(item[1]); } else { Data.Exceptions.Walls.Add(item[1]); }
                    }
                }
            }

            return true;
        }
        private static void DrawBorderLine(int startX, int startY, int endX, int endY, bool isVertical)
        {
            // Определяем направление и количество шагов
            int steps = isVertical ?
                Math.Abs(endY - startY) :
                Math.Abs(endX - startX);

            // Определяем шаг для каждой итерации
            int direction = isVertical ?
                (endY > startY ? 1 : -1) :
                (endX > startX ? 1 : -1);

            for (int i = 0; i <= steps; i++)
            {
                // Вычисляем текущие координаты
                int x = isVertical ? startX : startX + i * direction;
                int y = isVertical ? startY + i * direction : startY;

                // Проверка границ карты
                if (x < 0 || x >= Main.maxTilesX || y < 0 || y >= Main.maxTilesY)
                    continue;

                // Очистка и установка блока
                SetAir(x, y);
                SetTile(x, y, 1, 0, false);
                //WorldGen.PlaceTile(x, y, 1);
                //Main.tile[x, y].TileType = 1;
                //WorldGen.PlaceWall(x, y, 1);
                //WorldGen.PlaceLiquid(x, y, 255, 0);
            }
        }
        public static void CreateBorder(int minx, int miny, int maxx, int maxy, int borderSize)
        {
            // Рассчитываем смещенные координаты для обводки
            int left = minx - borderSize;
            int right = maxx + borderSize - 1;
            int top = miny - borderSize;
            int bottom = maxy + borderSize - 1;

            // Рисуем 4 стороны прямоугольника
            DrawBorderLine(left, top, left, bottom, true);    // Левая граница
            DrawBorderLine(right, top, right, bottom, true);  // Правая граница
            DrawBorderLine(left, top, right, top, false);     // Верхняя граница
            DrawBorderLine(left, bottom, right, bottom, false); // Нижняя граница
        }
        public static void ProcessArea(int startX, int startY, int endX, int endY)
        {
            // Добавьте проверку на валидность координат
            startX = Math.Clamp(startX, 0, Main.maxTilesX - 1);
            startY = Math.Clamp(startY, 0, Main.maxTilesY - 1);
            endX = Math.Clamp(endX, 0, Main.maxTilesX - 1);
            endY = Math.Clamp(endY, 0, Main.maxTilesY - 1);

            // Основная логика обработки
            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    SetAir(x, y);
                }
            }
        }
        public void PaletteSetter(int minx = DEFAULT_MIN_X, int miny = DEFAULT_MIN_Y, int maxx = DEFAULT_MAX_X, int maxy = DEFAULT_MAX_Y)
        {
            int offset = 0;


            CreateBorder(minx, miny, maxx, maxy, BORDER_SIZE);
            ProcessArea(minx - BORDER_SIZE, miny - BORDER_SIZE, maxx + BORDER_SIZE, maxy + BORDER_SIZE);
            ProcessTiles(minx, maxx, miny, maxy, ref offset);
            ProcessWalls(minx, maxx, miny, maxy, ref offset);

        }

        private void ProcessTiles(int minx, int maxx, int miny, int maxy, ref int offset)
        {
            int tileID = 0;
            int paint = 0;
            int breakPointX = minx;

            for (int x = minx; x < maxx; x++)
            {
                for (int y = miny; y < maxy; y++)
                {
                    if (!WorldGen.InWorld(x, y)) continue;

                    try
                    {
                        SetAir(x, y);
                        if (!Data.Exceptions.Tiles.Contains(tileID.ToString()) && tileID <= Data.MAX_TILE_COUNT)
                        {

                            SetTile(x, y, (ushort)tileID, (byte)paint);
                        }

                        if (paint >= MAX_PAINT_VALUE)
                        {
                            tileID++;
                            paint = 0;
                            if (tileID > Data.MAX_TILE_COUNT) break;
                        }
                        else
                        {
                            paint++;
                        }
                    }
                    catch
                    {
                        SetAir(x, y);
                        offset = x;
                        break;
                    }
                }
                if (tileID > Data.MAX_TILE_COUNT)
                {
                    offset = x;
                    break;
                }
            }
            NetMessage.SendTileSquare(-1, minx, miny, maxx - minx, maxy - miny);
        }

        private void ProcessWalls(int minx, int maxx, int miny, int maxy, ref int offset)
        {
            int wallID = 1;
            int paint = 0;
            for (int x = minx + offset + 1; x < maxx; x++)
            {
                for (int y = miny; y < maxy; y++)
                {
                    if (!WorldGen.InWorld(x, y)) continue;

                    SetAir(x, y);
                    if (!Data.Exceptions.Walls.Contains(wallID.ToString()) && wallID <= Data.MAX_WALL_COUNT)
                    {
                        SetWall(x, y, (ushort)wallID, (byte)paint);
                    }

                    if (paint >= MAX_PAINT_VALUE)
                    {
                        wallID++;
                        paint = 0;
                        if (wallID > Data.MAX_WALL_COUNT)
                        {
                            break;
                        }
                    }
                    else
                    {
                        paint++;
                    }
                }
                if (wallID > Data.MAX_WALL_COUNT)
                {
                    offset = x;
                    break;
                }
            }

            NetMessage.SendTileSquare(-1, minx, miny, maxx - minx, maxy - miny);
        }
        private static void Teleport_XY(int X, int Y)
        {
            Player player = Main.player[0];
            Vector2 pos = new Vector2(X, Y);
            pos = new Vector2(pos.X * 16 + 8 - player.width / 2, pos.Y * 16 - player.height);
            player.Teleport(pos, 2, 0);
        }
        private static bool MapRevealer(int maxTilesX = -1, int maxTilesY = -1, int minTilesX = 0, int minTilesY = 0)
        {
            maxTilesX = maxTilesX == -1 ? Data.maxtileX : maxTilesX;
            maxTilesY = maxTilesY == -1 ? Data.maxtileY : maxTilesY;
            if (maxTilesX >= Main.maxTilesX)
            {
                maxTilesX = Main.maxTilesX - 1;
            }
            else if (maxTilesY >= Main.maxTilesY)
            {
                maxTilesY = Main.maxTilesY - 1;
            }
            bool result = false;
            for (int i = minTilesX; i < maxTilesX + 10; i++)
            {
                for (int j = minTilesY; j < maxTilesY + 10; j++)
                {
                    if (WorldGen.InWorld(i, j))
                        Main.Map.Update(i, j, 255);
                }
            }
            Main.refreshMap = true;
            result = true;
            return result;
        }
        private static string GetPaintFromByte(byte color)
        {
            if (color >= 0 && color <= 31)
            {
                return color.ToString();
            }
            throw new Exception($"GetPaintFromByte | Paint not in 0 - 31 range. Recieved Id = '{color}'"); // Для значений, не входящих в диапазон 0-31
        }
        private static string GetPaintNameFromByte(byte color)
        {
            switch (color)
            {
                case 0: return "NonePaint";
                case 1: return "RedPaint";
                case 2: return "OrangePaint";
                case 3: return "YellowPaint";
                case 4: return "LimePaint";
                case 5: return "GreenPaint";
                case 6: return "TealPaint";
                case 7: return "CyanPaint";
                case 8: return "SkyBluePaint";
                case 9: return "BluePaint";
                case 10: return "PurplePaint";
                case 11: return "VioletPaint";
                case 12: return "PinkPaint";
                case 13: return "DeepRedPaint";
                case 14: return "DeepOrangePaint";
                case 15: return "DeepYellowPaint";
                case 16: return "DeepLimePaint";
                case 17: return "DeepGreenPaint";
                case 18: return "DeepTealPaint";
                case 19: return "DeepCyanPaint";
                case 20: return "DeepSkyBluePaint";
                case 21: return "DeepBluePaint";
                case 22: return "DeepPurplePaint";
                case 23: return "DeepVioletPaint";
                case 24: return "DeepPinkPaint";
                case 25: return "BlackPaint";
                case 26: return "WhitePaint";
                case 27: return "GrayPaint";
                case 28: return "BrownPaint";
                case 29: return "ShadowPaint";
                case 30: return "NegativePaint";
                case 31: return "IlluminantPaint";
                default: throw new Exception($"GetPaintNameFromByte | Paint not in 0 - 31 range. Recieved Id = '{color}'");
            }
        }
        private static void PaletteSttr(string Tilespath, int minTilesX = DEFAULT_MIN_X, int minTilesY = DEFAULT_MIN_Y, int maxTilesX = DEFAULT_MAX_X, int maxTilesY = DEFAULT_MAX_Y)
        {
            var config = ModContent.GetInstance<PaletteAnalyzerConfig>();
            DataSet();
            //Teleport_XY(Data.maxtileX + 200, Data.minTileY + 20);
            Thread.Sleep(3000);
            MapRevealer(Main.maxTilesX, Main.maxTilesY);
            Thread.Sleep(3000);
            string[] torches = Data.Exceptions.Torchs.ToArray() ?? new[] { "4" };
            List<string> notsortedcol = new List<string>();
            HashSet<ushort> exIds = new HashSet<ushort>();
            for (int i = minTilesX; i < maxTilesX; i++)
            {
                for (int j = minTilesY; j < maxTilesY; j++)
                {
                    MapTile mapTile = Main.Map[i, j];
                    ProcessTile(i, j, mapTile, torches, notsortedcol);
                }
            }

            ColorChose(notsortedcol.ToArray());

            // Вложенная функция для обработки плитки
            void ProcessTile(int i, int j, MapTile mapTile, string[] torches, List<string> notsortedcol)
            {
                if (Main.tile[i, j].WallType != 0 && Main.tile[i, j].TileType != 0)
                {
                    notsortedcol.Add(CreateTileEntry("1", Main.tile[i, j], mapTile));
                }
                else if (Main.tile[i, j].WallType > 1)
                {
                    notsortedcol.Add(CreateWallEntry(Main.tile[i, j], mapTile));
                }
            }

            // Вложенная функция для создания записи плитки
            string CreateTileEntry(string type, Tile tile, MapTile mapTile)
            {
                return string.Concat(new object[] {
            type,
            "-",
            Terraria.ID.TileID.Search.GetName(tile.TileType),
            "-Block-",
            GetPaintNameFromByte(tile.TileColor),
            "-",
            tile.TileType,
            "\t",
            GetPaintFromByte(tile.TileColor),
            "\t",
            MapHelper.GetMapTileXnaColor(ref mapTile).Hex3().ToUpper()
        });
            }

            // Вложенная функция для создания записи стены
            string CreateWallEntry(Tile tile, MapTile mapTile)
            {
                return string.Concat(new object[] {
            "0-",
            Terraria.ID.WallID.Search.GetName(tile.WallType),
            "-Wall-",
            GetPaintNameFromByte(tile.WallColor),
            "-",
            tile.WallType,
            "\t",
            GetPaintFromByte(tile.WallColor),
            "\t",
            MapHelper.GetMapTileXnaColor(ref mapTile).Hex3().ToUpper()
        });
            }

            // Вложенная функция для выбора цвета
            List<string> ColorChose(string[] notsortedcol)
            {
                List<string> sortedcol = new List<string>();
                List<Tuple<string, string, System.Drawing.Color>> tiledata = new List<Tuple<string, string, System.Drawing.Color>>();

                foreach (string line in notsortedcol)
                {
                    string[] array10 = line.Split(new char[] { '\t' });
                    string tile = array10[0];
                    string paint = array10[1];
                    string color = array10[2];
                    tiledata.Add(new Tuple<string, string, System.Drawing.Color>(tile, paint, System.Drawing.ColorTranslator.FromHtml("#" + color)));
                }

                var tiledatalist = tiledata.OrderBy(color => color.Item3.GetHue())
                                            .ThenBy(o => o.Item3.R * 3 + o.Item3.G * 2 + o.Item3.B * 1);

                foreach (var tiledatainfo in tiledatalist)
                {
                    string tile = tiledatainfo.Item1;
                    string paint = tiledatainfo.Item2;
                    System.Drawing.Color color = tiledatainfo.Item3;
                    sortedcol.Add(tile + "\t" + paint + "\t" + ColorConverterExtensions.ToHexString(color).Replace("#", ""));
                }

                return RemDuple(sortedcol.ToArray());
            }

            // Вложенная функция для удаления дубликатов
            List<string> RemDuple(string[] array)
            {
                List<string> sortedcol = new List<string>();
                array = array.Take(array.Length - 1).ToArray();

                foreach (var line in array.Where(x => x.Split('\t')[2] != null).GroupBy(x => x.Split('\t')[2]).Select(y => y.FirstOrDefault()))
                {
                    sortedcol.Add(line);
                }

                return Create_tile_File(sortedcol);
            }

            // Вложенная функция для создания файла плитки
            List<string> Create_tile_File(List<string> lines)
            {
                List<string> sortedcol = new List<string>();
                char[] separators = new char[] { '\t', '-', '\n' };

                foreach (var line in lines)
                {
                    string[] subs = line.Split(separators);
                    sortedcol.Add($"{subs[0]}:{subs[4]}:{subs[5]}:{subs[6]}:{subs[2]}:{subs[1]}:{subs[3]}");
                }
                if (File.Exists($"{Tilespath}\\tiles.txt"))
                {
                    return sortedcol;
                }
                File.WriteAllLines(Tilespath + "\\tiles.txt", sortedcol);
                Main.NewText($"palette file for script created on {Tilespath}", 230, 230, 230);

                return sortedcol;
            }
        }
    }
    public static class FileValidator
    {
        public static bool IsValidTextFile(string filePath)
        {
            if (!File.Exists(filePath))
                return false;

            if (!Path.GetExtension(filePath).Equals(".txt", StringComparison.OrdinalIgnoreCase))
                return false;

            try
            {
                using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
