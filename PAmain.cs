#define debug
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria.ModLoader;
using System.IO;
using Terraria.GameInput;
using Terraria.Map;
using Terraria;
using System.Drawing;
using System.Threading;
using Terraria.ID;
using Terraria.DataStructures;
using System.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Utils;
using Terraria.IO;
using Terraria.Enums;
using ReLogic.Reflection;
using tModPorter;
using Steamworks;
using System.Threading.Tasks;
using System.CodeDom.Compiler;
using Newtonsoft.Json.Linq;
using Terraria.GameContent.Biomes.CaveHouse;
using System.Buffers.Binary;


/*               WARNING!!!
 *        
 * My eng is too bad 
 * My knowledge in c# is also very bad.
 * Please do not read the code.
 * After reading this, you can hurt yourself.
 * No one compensate for this damage.
*/
namespace PaletteAnalyzer
{
    public class PAmain : ModPlayer
    {
        private static uint artkey = 1;
        #region Cheats
        private static void Teleport_XY(int X, int Y)
        {
            Player player = Main.player[0];
            Vector2 pos = new Vector2(X, Y);
            pos = new Vector2(pos.X * 16 + 8 - player.width / 2, pos.Y * 16 - player.height);
            player.Teleport(pos, 2, 0);
        }
        private static void GodModeSet2(int X = 60)
        {
            if (GodMode.Enabled) { return; }
            Main.NewText($"GodMode is on for {X} sec");
            GodMode.Enabled = true;
            Thread.Sleep(X * 1000);
            GodMode.Enabled = false;
            Main.NewText("GodMode is off");
        }
        private static void GodModeSet()
        {
            GodModeSet2(720);
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
        #endregion
        #region Tools

        #endregion
        private bool isTaskRunning = false; // Переменная для отслеживания состояния задачи
        private bool isNewFrame = true;
        private string _filepath = $@"C:\Users\USER\OneDrive\Изображения\Output\Video\photo{artkey}.txt";

        public override async void ProcessTriggers(TriggersSet triggersSet)
        {
            var config = ModContent.GetInstance<PaletteAnalyzerConfig>();
            if (PaletteAnalyzer.MapOverView.JustPressed)
            {
                MapRevealer(Main.maxTilesX, Main.maxTilesY);
                return;
            }
            if (PaletteAnalyzer.Insert.JustPressed)
            {


                // Использование пути
                string targetDirectory = config.VideoDirectoryPath + "\\" + config.VideoFileName;
                if (!FileValidator.IsValidTextFile(targetDirectory + $"{artkey}.txt"))
                {
                    return;
                }
                // Проверяем, выполняется ли задача
                if (!isTaskRunning)
                {

                    isTaskRunning = true; // Устанавливаем флаг, что задача запущена
                    try
                    {
                        while (isTaskRunning) // Бесконечный цикл для создания артов
                        {
                            artkey++;
                            _filepath = targetDirectory + $"{artkey}.txt";
                            if (!File.Exists(_filepath))
                            {
                                artkey = artkey == (uint)1 ? (uint)1 : (uint)1;
                                _filepath = $@"C:\Users\USER\OneDrive\Изображения\Output\Video\photo{artkey}.txt";
                                // Если файл существует, выходим из цикла
                                break;
                            }



                            // Запускаем асинхронную задачу для создания арта
                            await Task.Run(() => InsertBlockAsync());

                            // Проверяем, если достигли лимита артов (например, 10)
                        }
                    }
                    finally
                    {
                        isTaskRunning = false; // Сбрасываем флаг после завершения задачи
                    }
                }
                else
                {
                    isTaskRunning = false;
                    artkey = (uint)1;
                }
                return;
            }


            if (PaletteAnalyzer.Parameters.JustPressed)
            {
                Thread thread2 = new Thread(() => GodModeSet2(1000000));
                thread2.Start();
                return;
            }
            if (!PaletteAnalyzer.Palette.JustPressed) return;

            // Использование пути
            string TilesDir = config.PaletteFilePath;
            if (string.IsNullOrEmpty(TilesDir))
            {
                return;
            }
            if (File.Exists($"{TilesDir}\\tiles.txt"))
            {
                File.Delete($"{TilesDir}\\tiles.txt");
            }
            Main.NewText("The approximate time for creating palette is 10 seconds", 191, 255, 94);
            //Main.NewText("if you saw a gray(not red) message when the game unfreeze");
            PaletteCreator paletteCreator = new PaletteCreator();
            await paletteCreator.ExecuteAsync();
            //Main.NewText("then the scipt worked correctly.");
            if (MapRevealer() == true)
            {
                Thread thread2 = new Thread(GodModeSet);
                thread2.Start();
            }
        }
        private static void SetWall(int x, int y, ushort WallId, byte paintId)
        {

            PaletteCreator.SetAir(x, y);

            Tile tile2 = Main.tile[x, y];
            tile2.LiquidAmount = 0;
            tile2.WallType = WallId;
            tile2.WallColor = paintId;
            tile2.IsActuated = true;
            if (Main.tile[x, y] == tile2)
                return;


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
            if (Main.tile[x, y] == tile1)
                return;

            Main.tile[x, y].CopyFrom(tile1);
        }
        private static bool IsInBounds(int x, int y)
        {
            // Для Terraria стандартные проверки границ мира
            return x >= 0
                && x < Main.maxTilesX
                && y >= 0
                && y < Main.maxTilesY;
        }
        private async Task InsertBlockAsync()
        {
            int x, y, i, w, h, startX, startY, size = 10;

            BinaryWorker bw = new(_filepath);
            var Photo = bw.Read();
            Main.NewText($"{artkey} - art", Microsoft.Xna.Framework.Color.AliceBlue);
            //Main.NewText($"Photo is {bw.Width}x{bw.Height}", 191, 255, 94);

            i = 0; // Обнуляем индекс для каждой новой картинки

            for (w = 0; w < bw.Width; w++)
            {
                for (h = 0; h < bw.Height; h++)
                {
                    if (!(Main.worldID >= 1) || Main.myPlayer < 0)
                    { return; }
                    x = w + 100;
                    y = h + 100;
                    if (!IsInBounds(x, y))
                    {
                        i++;
                        continue;
                    }
                    if (((WorldGen.TileType(x, y) == Photo[i].id || (Main.tile[x, y].TileType == Photo[i].id)) && Main.tile[x, y].TileColor == Photo[i].paintId))
                    {
                        i++;
                        continue;
                    }


                    int state;
                    // Определяем состояние
                    if (!Photo[i].isWall && !Photo[i].isTorch)
                    {
                        state = 0; // isWall == false && isTorch == false
                    }
                    else if (Photo[i].isTorch)
                    {
                        state = 2; // isTorch == true
                    }
                    else
                    {
                        state = 1; // isWall == true && isTorch == false
                    }

                    switch (state)
                    {
                        case 0: // isWall == false && isTorch == false
                        case 2: // isTorch == true
                            //WorldGen.PlaceTile(x, y, 1);
                            SetTile(x, y, Photo[i].id, Photo[i].paintId);
                            //WorldGen.PlaceWall(x, y, 1, true);
                            //WorldGen.PlaceTile(x, y, Photo[i].id, true);
                            //WorldGen.paintTile(x, y, Photo[i].paintId);
                            break;

                        case 1: // isWall == true && isTorch == false
                            SetWall(x, y, Photo[i].id, Photo[i].paintId);
                            //WorldGen.PlaceWall(x, y, Photo[i].id, true);
                            //WorldGen.paintWall(x, y, Photo[i].paintId);
                            break;

                        default:
                            break;
                    }
                    i++;

                }
            }
            //NetMessage.SendTileSquare(-1, 100, 100,w,h);

            MapRevealer(100 + bw.Width, 100 + bw.Height, 100, 100);


            // Задержка в 3 секунды перед переходом к следующему изображению
            //System.Threading.Thread.Sleep(300);
            //MapRevealer(250 + bw.Width, 250 + bw.Height, 50, 50);

        }
    }
    public class RectangleMerger
    {
        public class Cell
        {
            public (bool, bool, ushort, byte) Value; // Значение ячейки
            public bool Merged;

            public Cell((bool, bool, ushort, byte) value)
            {
                Value = value;
                Merged = false;
            }

            public bool IsSameValue(Cell other)
            {
                return this.Value == other.Value;
            }
        }

        public class Rectangle
        {
            public int X, Y, Width, Height;
            public (bool, bool, ushort, byte) Value;

            public Rectangle(int x, int y, int width, int height, (bool, bool, ushort, byte) value)
            {
                X = x;
                Y = y;
                Width = width;
                Height = height;
                Value = value;
            }
        }

        private Cell[,] grid;
        private List<Rectangle> rectangles;

        public RectangleMerger(int n, int m, (bool, bool, ushort, byte)[,] values)
        {
            grid = new Cell[n, m];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    grid[i, j] = new Cell(values[i, j]);
                }
            }
            rectangles = new List<Rectangle>();
        }

        public void MergeRectangles()
        {
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int j = 0; j < grid.GetLength(1); j++)
                {
                    if (!grid[i, j].Merged)
                    {
                        Merge(i, j);
                    }
                }
            }
        }

        private void Merge(int i, int j)
        {
            if (i < 0 || i >= grid.GetLength(0) || j < 0 || j >= grid.GetLength(1) || grid[i, j].Merged)
            {
                return;
            }

            (bool, bool, ushort, byte) value = grid[i, j].Value;
            int width = 1;
            int height = 1;

            // Проверка вправо
            for (int k = 1; k < grid.GetLength(1); k++)
            {
                if (j + k < grid.GetLength(1) && grid[i, j + k].IsSameValue(grid[i, j]) && !grid[i, j + k].Merged)
                {
                    width++;
                }
                else
                {
                    break;
                }
            }

            // Проверка вниз
            for (int k = 1; k < grid.GetLength(0); k++)
            {
                bool canMerge = true;
                for (int l = 0; l < width; l++)
                {
                    if (i + k >= grid.GetLength(0) || !grid[i + k, j + l].IsSameValue(grid[i, j]) || grid[i + k, j + l].Merged)
                    {
                        canMerge = false;
                        break;
                    }
                }
                if (canMerge)
                {
                    height++;
                }
                else
                {
                    break;
                }
            }

            // Помечаем ячейки как объединенные
            for (int x = i; x < i + height; x++)
            {
                for (int y = j; y < j + width; y++)
                {
                    grid[x, y].Merged = true;
                }
            }

            rectangles.Add(new Rectangle(j, i, width, height, value));
        }

        public List<Rectangle> GetMergedRectangles()
        {
            return rectangles;
        }
    }
    public partial class Data
    {
        public static string path = String.Empty;
        public static int maxtileX = 200;
        public static int maxtileY = 1100;
        public static int minTileX, minTileY = 100;
        public static string[] notsortedcol, sortedcol, torches;
        public const int MAX_TILE_COUNT = 692, MAX_WALL_COUNT = 346;
    }
}
public class BinaryWorker
{
    /// <summary>
    /// Путь к файлу результата
    /// </summary>
    private string _path = ""; // Приватное поле для хранения пути к файлу
    private ushort x, y;
    public string Path
    {
        get { return _path; } // Возвращает значение из приватного поля
        set { _path = value; } // Устанавливает значение в приватное поле
    }
    public ushort Width
    {
        get { return x; }
        set { x = value; }
    }
    public ushort Height
    {
        get { return y; }
        set { y = value; }
    }
    // Конструктор класса, принимает путь к файлу, по умолчанию установлен путь к TET.txt
    public BinaryWorker(string path)
    {
        Path = path; // Инициализация пути
    }


    // Список для хранения значений из файла
    public List<(bool, bool, ushort, byte)> FileValues = new List<(bool, bool, ushort, byte)>();

    // Метод для чтения данных из файла
    /// <summary>
    /// Читает данные из файла и возвращает список кортежей, содержащих информацию о блоках.
    /// </summary>
    /// <returns>
    /// Список кортежей, где каждый кортеж содержит:
    ///   <list type="bullet">
    ///     <item><description>bool isWall - указывает, является ли блок стеной</description></item>
    ///     <item><description>bool isTorch - указывает, является ли блок факелом</description></item>
    ///     <item><description>ushort id - идентификатор блока</description></item>
    ///     <item><description>byte paintId - идентификатор краски блока</description></item>
    ///   </list>
    /// </returns>
    internal (bool isWall, bool isTorch, ushort id, byte paintId)[,] Read(int temp = 0)
    {
        List<(bool, bool, ushort, byte)> Array = new List<(bool, bool, ushort, byte)>(); // Создаем новый список для хранения прочитанных данных
        byte[] bytes = File.ReadAllBytes(Path); // Читаем все байты из файла

        // Извлечение ширины и высоты из первых байтов
        //ushort WidthStart = (ushort)((bytes[0] & 0xff) + ((bytes[1] & 0xff) << 8));
        Width = (ushort)((bytes[2] & 0xff) + ((bytes[3] & 0xff) << 8));
        Height = (ushort)((bytes[4] & 0xff) + ((bytes[5] & 0xff) << 8));

        // Чтение данных из файла и добавление их в список
        for (int i = 6; i < bytes.Length && i + 4 < bytes.Length; i += 5)
        {
            Array.Add((
                Convert.ToBoolean(bytes[i]), // Признак стены
                Convert.ToBoolean(bytes[i + 1]), // Признак факела
                (ushort)((bytes[i + 2] & 0xff) + ((bytes[i + 3] & 0xff) << 8)), // ID
                bytes[i + 4] // Цвет
            ));
        }
        return ConvertTo2DArray(Array, Width, Height); // Возвращаем список прочитанных значений
    }
    internal (bool isWall, bool isTorch, ushort id, byte paintId)[] Read()
    {
        byte[] fileData = File.ReadAllBytes(Path);
        Span<byte> dataSpan = fileData.AsSpan();

        // Минимальный размер: 6 байт (заголовок)
        if (dataSpan.Length < 6)
        {
            return Array.Empty<(bool, bool, ushort, byte)>();
        }

        // Чтение ширины и высоты (байты 2-5)
        Width = BinaryPrimitives.ReadUInt16LittleEndian(dataSpan.Slice(2, 2));
        Height = BinaryPrimitives.ReadUInt16LittleEndian(dataSpan.Slice(4, 2));

        // Расчет элементов данных (каждый по 5 байт)
        int dataStartOffset = 6;
        Span<byte> dataSection = dataSpan.Slice(dataStartOffset);
        int elementsCount = dataSection.Length / 5;

        var result = new (bool, bool, ushort, byte)[elementsCount];

        // Быстрое чтение через Span
        for (int i = 0; i < elementsCount; i++)
        {
            int offset = i * 5;

            // Экстракция данных за 5 операций
            result[i] = (
                dataSection[offset] != 0,                          // isWall
                dataSection[offset + 1] != 0,                      // isTorch
                BinaryPrimitives.ReadUInt16LittleEndian(           // id
                    dataSection.Slice(offset + 2, 2)),
                dataSection[offset + 4]                            // paintId
            );
        }

        return result;
    }
    static (bool, bool, ushort, byte)[,] ConvertTo2DArray(List<(bool, bool, ushort, byte)> oneDimensionalArray, int width, int height)
    {
        // Проверяем, что размер одномерного массива соответствует ожидаемому размеру
        if (oneDimensionalArray.Count != width * height)
        {
            throw new ArgumentException("Размер одномерного массива не соответствует заданным ширине и высоте.");
        }

        // Создаем двумерный массив
        (bool, bool, ushort, byte)[,] twoDimensionalArray = new (bool, bool, ushort, byte)[height, width];

        // Заполняем двумерный массив
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                // Индекс в одномерном массиве
                int index = i * width + j;

                // Заполняем двумерный массив значениями из одномерного массива
                twoDimensionalArray[i, j] = oneDimensionalArray[index];
            }
        }

        return twoDimensionalArray;
    }

    /// <summary>
    /// Конвертирует число в формате ushort в строку заданной длины (до 8)
    /// </summary>
    /// <param name="value">Число для конвертации</param>
    /// <param name="length">Длина результирующей строки</param>
    /// <returns>Строка в двоичном формате</returns>
    private static string ConvertToBinary(ushort value, byte length)
    {
        string tmp = String.Empty; // Временная строка для хранения нулей
        string result = Convert.ToString(value, 2); // Конвертируем число в двоичную строку

        // Добавляем нули в начало строки до нужной длины
        for (int i = 0; i < (length - result.Length); i++)
        {
            tmp += "0"; // Добавление нуля
        }
        return tmp + result; // Возвращаем строку с нулями и результатом
    }

    // Метод для записи данных в файл
    internal void Write(ushort Width, ushort Height, ushort WidthStart = 0, List<(bool, bool, ushort, byte)> Array = null)
    {
        Array = Array ?? FileValues; // Если массив не передан, используем значения по умолчанию
        using (var stream = File.Open(Path, FileMode.Create)) // Открываем файл для записи
        {
            using (var binaryWriter = new BinaryWriter(stream, Encoding.UTF8, false)) // Создаем бинарный писатель
            {
                // Записываем ширину, высоту и значения в файл
                binaryWriter.Write(WidthStart); // Начальная ширина
                binaryWriter.Write(Width); // Ширина
                binaryWriter.Write(Height); // Высота
                for (int index = 0; index < Array.Count; index++)
                {
                    binaryWriter.Write(Array[index].Item1); // Стена?
                    binaryWriter.Write(Array[index].Item2); // Факел?
                    binaryWriter.Write(Array[index].Item3); // ID
                    binaryWriter.Write(Array[index].Item4); // Цвет
                }
            }
        }
    }
}


class GodMode
{
    public static bool Enabled = false;
}

class GodModeModPlayer : ModPlayer
{
    public override bool FreeDodge(Player.HurtInfo info)
    {
        if (GodMode.Enabled)
            return true;
        return base.FreeDodge(info);
    }

    public override void PreUpdate()
    {
        if (GodMode.Enabled)
        {
            Player.statLife = Player.statLifeMax2;
            Player.statMana = Player.statManaMax2;
            Player.wingTime = Player.wingTimeMax;
        }
    }
}
