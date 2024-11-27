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
        private static bool MapRevealer(int maxTilesX = -1, int maxTilesY = -1,int minTilesX = 0, int minTilesY = 0)
        {
            maxTilesX = maxTilesX == -1 ? Data.maxtileX : maxTilesX;
            maxTilesY = maxTilesY == -1 ? Data.maxtileY : maxTilesY;
            if (maxTilesX >= Main.maxTilesX)
            {
                maxTilesX = Main.maxTilesX-1;
            } else if (maxTilesY >= Main.maxTilesY)
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
        private static string GetPaintFromByte(byte color)
        {
            string result = "None";
            if (color == 0)
            {
                result = "0";
            }
            else if (color == 1)
            {
                result = "1";
            }
            else if (color == 2)
            {
                result = "2";
            }
            else if (color == 3)
            {
                result = "3";
            }
            else if (color == 4)
            {
                result = "4";
            }
            else if (color == 5)
            {
                result = "5";
            }
            else if (color == 6)
            {
                result = "6";
            }
            else if (color == 7)
            {
                result = "7";
            }
            else if (color == 8)
            {
                result = "8";
            }
            else if (color == 9)
            {
                result = "9";
            }
            else if (color == 10)
            {
                result = "10";
            }
            else if (color == 11)
            {
                result = "11";
            }
            else if (color == 12)
            {
                result = "12";
            }
            else if (color == 13)
            {
                result = "13";
            }
            else if (color == 14)
            {
                result = "14";
            }
            else if (color == 15)
            {
                result = "15";
            }
            else if (color == 16)
            {
                result = "16";
            }
            else if (color == 17)
            {
                result = "17";
            }
            else if (color == 18)
            {
                result = "18";
            }
            else if (color == 19)
            {
                result = "19";
            }
            else if (color == 20)
            {
                result = "20";
            }
            else if (color == 21)
            {
                result = "21";
            }
            else if (color == 22)
            {
                result = "22";
            }
            else if (color == 23)
            {
                result = "23";
            }
            else if (color == 24)
            {
                result = "24";
            }
            else if (color == 25)
            {
                result = "25";
            }
            else if (color == 26)
            {
                result = "26";
            }
            else if (color == 27)
            {
                result = "27";
            }
            else if (color == 28)
            {
                result = "28";
            }
            else if (color == 29)
            {
                result = "29";
            }
            else if (color == 30)
            {
                result = "30";
            }
            else if (color == 31)
            {
                result = "31";
            }

            return result;
        }
        private static string GetPaintNameFromByte(byte color)
        {
            string result = color.ToString();
            if (color == 0)
            {
                result = "NonePaint";
            }
            else if (color == 1)
            {
                result = "RedPaint";
            }
            else if (color == 2)
            {
                result = "OrangePaint";
            }
            else if (color == 3)
            {
                result = "YellowPaint";
            }
            else if (color == 4)
            {
                result = "LimePaint";
            }
            else if (color == 5)
            {
                result = "GreenPaint";
            }
            else if (color == 6)
            {
                result = "TealPaint";
            }
            else if (color == 7)
            {
                result = "CyanPaint";
            }
            else if (color == 8)
            {
                result = "SkyBluePaint";
            }
            else if (color == 9)
            {
                result = "BluePaint";
            }
            else if (color == 10)
            {
                result = "PurplePaint";
            }
            else if (color == 11)
            {
                result = "VioletPaint";
            }
            else if (color == 12)
            {
                result = "PinkPaint";
            }
            else if (color == 13)
            {
                result = "DeepRedPaint";
            }
            else if (color == 14)
            {
                result = "DeepOrangePaint";
            }
            else if (color == 15)
            {
                result = "DeepYellowPaint";
            }
            else if (color == 16)
            {
                result = "DeepLimePaint";
            }
            else if (color == 17)
            {
                result = "DeepGreenPaint";
            }
            else if (color == 18)
            {
                result = "DeepTealPaint";
            }
            else if (color == 19)
            {
                result = "DeepCyanPaint";
            }
            else if (color == 20)
            {
                result = "DeepSkyBluePaint";
            }
            else if (color == 21)
            {
                result = "DeepBluePaint";
            }
            else if (color == 22)
            {
                result = "DeepPurplePaint";
            }
            else if (color == 23)
            {
                result = "DeepVioletPaint";
            }
            else if (color == 24)
            {
                result = "DeepPinkPaint";
            }
            else if (color == 25)
            {
                result = "BlackPaint";
            }
            else if (color == 26)
            {
                result = "WhitePaint";
            }
            else if (color == 27)
            {
                result = "GrayPaint";
            }
            else if (color == 28)
            {
                result = "BrownPaint";
            }
            else if (color == 29)
            {
                result = "ShadowPaint";
            }
            else if (color == 30)
            {
                result = "NegativePaint";
            }
            else if (color == 31)
            {
                result = "IlluminantPaint";
            }
            return result;
        }
        
        private static void ColorChose()
        {
            List<string> sortedcol = new List<string>();
            List<Tuple<string, string, System.Drawing.Color>> tiledata = new List<Tuple<string, string, System.Drawing.Color>>();
            string[] notsortedcol = Data.notsortedcol;
            foreach (string line in notsortedcol)
            {
                string[] array10 = line.Split(new char[] { '	' });
                string tile = array10[0];
                string paint = array10[1];
                string color = array10[2];

                tiledata.Add(new Tuple<string, string, System.Drawing.Color>(tile, paint, System.Drawing.ColorTranslator.FromHtml("#" + color)));
            }

            // Create An IOrderEnumerable List
            var tiledatalist = tiledata.OrderBy(color => color.Item3.GetHue()).ThenBy(o => o.Item3.R * 3 + o.Item3.G * 2 + o.Item3.B * 1);

            // Expand Each Item Of The List
            foreach (var tiledatainfo in tiledatalist)
            {
                // Define info from the list item
                string tile = tiledatainfo.Item1;
                string paint = tiledatainfo.Item2;
                System.Drawing.Color color = tiledatainfo.Item3;


                // Output the data
                sortedcol.Add(tile + "\t" + paint + "\t" + ColorConverterExtensions.ToHexString(color).Replace("#", ""));
            }
            RemDuple(sortedcol.ToArray());
            return;
        }
        private static void RemDuple(string[] array)
        {

            List<string> sortedcol = new List<string>();
            array = array.Take(array.Length - 1).ToArray();
            try
            {
                foreach (var line in array.Where(x => x.Split('\t')[2] != null).GroupBy(x => x.Split('\t')[2]).Select(y => y.FirstOrDefault()))
                {
                    // Add value.
                    sortedcol.Add(line);
                }
            }
            catch (Exception)
            {
            }
            Create_tile_File(sortedcol);

        }
        private static void Create_tile_File(List<string> lines)
        {
            List<string> sortedcol = new List<string>();
            char[] separators = new char[] { '\t', '-', '\n' };
            foreach (var line in lines)
            {
                string[] subs = line.Split(separators);
                List<string> subs2 = new List<string>();
                foreach (var sub in subs)
                {
                    subs2.Add(sub);
                }
                // subs[0] = wall or tile \ bool-int value(1 or 0)
                // subs[1] = Name of the Pixel
                // subs[2] = Block or Wall
                // subs[3] = Paint name of the Pixel
                // subs[4] = Pixel's id
                // subs[5] = Pixel's paint id
                // subs[6] = hex value of Pixel
                sortedcol.Add($"{subs2[0]}:{subs2[4]}:{subs2[5]}:{subs2[6]}:{subs2[2]}:{subs2[1]}:{subs[3]}");
                //Main.NewText(subs[0] + ":" + subs[1] + ":" + subs[2] + ":" + subs[3], 230, 230, 230);
            }
            File.WriteAllLines($"{Data.path}\\tiles.txt", sortedcol);
            Main.NewText($"palette file for script created on {Data.path}", 230, 230, 230);
        }
        #endregion
        private bool isTaskRunning = false; // Переменная для отслеживания состояния задачи
        private bool isNewFrame = true;
        public override async void ProcessTriggers(TriggersSet triggersSet)
        {
            if (PaletteAnalyzer.MapOverView.JustPressed)
            {
                MapRevealer(Main.maxTilesX, Main.maxTilesY);
                return;
            }
            if (PaletteAnalyzer.Insert.JustPressed)
            {
                // Проверяем, выполняется ли задача
                if (!isTaskRunning)
                {
                    isTaskRunning = true; // Устанавливаем флаг, что задача запущена
                    try
                    {
                        if (!(File.Exists($@"C:\Users\USER\OneDrive\Desktop\video\Exif\photo{artkey}.txt")))
                        {
                            Main.NewText("File doesnt Exists", Microsoft.Xna.Framework.Color.Coral);
                            return;
                        }
                        await Task.Run(() => InsertBlockAsync(artkey));
                    }
                    finally
                    {
                        isTaskRunning = false; // Сбрасываем флаг после завершения задачи
                    }

                    artkey++;

                }
                else
                {
                    Main.NewText("Task already started", Microsoft.Xna.Framework.Color.Snow);
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
            if (File.Exists($"{Data.path}\\tiles.txt"))
            {
                File.Delete($"{Data.path}\\tiles.txt");
            }
            Main.NewText("The approximate time for creating palette is 10 seconds",191,255,94);
            //Main.NewText("if you saw a gray(not red) message when the game unfreeze");
            //Main.NewText("then the scipt worked correctly.");
            if (MapRevealer() == true)
            {
                Thread thread2 = new Thread(GodModeSet);
                thread2.Start();
                Thread thread1 = new Thread(PaletteSttr);
                thread1.Start();
            }
        }
        private async Task InsertBlockAsync(uint j=0)
        {
            int x, y, i, w, h, startX, startY, size = 10;
            
            BinaryWorker bw = new($@"C:\Users\USER\OneDrive\Desktop\video\Exif\photo{j}.txt");
            var Photo = bw.Read();
            //Main.NewText($"Photo is {bw.Width}x{bw.Height}", 191, 255, 94);
            startX = 110 + bw.Width; // Начальная координата по X
            startY = 90; // Начальная координата по Y
            
            i = 0; // Обнуляем индекс для каждой новой картинки
            for (w = 0; w < bw.Width; w++)
            {
                for (h = 0; h < bw.Height; h++)
                {
                    if (!(Main.worldID >= 1) || Main.myPlayer < 0)
                    { return; }
                    x = w + 100;
                    y = h + 100;
                    if ((WorldGen.TileType(x, y) == Photo[i].id || (Main.tile[x, y].TileType == Photo[i].id)) && Main.tile[x, y].TileColor == Photo[i].paintId)
                    {
                        i++;
                        continue;
                    }
                    WorldGen.KillTile(x, y, false, false, true);
                    NetMessage.SendTileSquare(-1, x, y);
                    //WorldGen.KillWall(x, y);
                    Main.tile[x, y].WallType = 0;

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
                            WorldGen.PlaceTile(x, y, 1);
                            Main.tile[x, y].TileType = Photo[i].id;
                            WorldGen.PlaceWall(x, y, 1, true);
                            WorldGen.PlaceTile(x, y, Photo[i].id, true);
                            WorldGen.paintTile(x, y, Photo[i].paintId);
                            break;

                        case 1: // isWall == true && isTorch == false
                            WorldGen.PlaceWall(x, y, Photo[i].id, true);
                            WorldGen.paintWall(x, y, Photo[i].paintId);
                            break;

                        default:
                            break;
                    }
                    i++;
                    
                }
            }
            NetMessage.SendTileSquare(-1, 100, 100,bw.Width,bw.Height);
            if (isNewFrame)
            {
                for (x = 0; x < size; x++)
                {
                    for (y = 0; y < size; y++)
                    {
                        WorldGen.KillTile(startX + x, startY + y);
                        WorldGen.PlaceTile(startX + x, startY + y, 1, true);
                    }
                }
                isNewFrame = false;
            }
            else
            {
                 // Размер квадрата (3x3)

                for (x = 0; x < size; x++)
                {
                    for (y = 0; y < size; y++)
                    {
                        WorldGen.KillTile(startX + x, startY + y);
                        WorldGen.PlaceTile(startX + x, startY + y, 0, true);
                    }
                }
                isNewFrame = true;
            }
            MapRevealer(250 + bw.Width, 250 + bw.Height, 50, 50);
            //System.Threading.Thread.Sleep(100);
            MapRevealer(250 + bw.Width, 250 + bw.Height, 50, 50);
            MapRevealer(250 + bw.Width, 250 + bw.Height, 50, 50);
            
            
            // Задержка в 3 секунды перед переходом к следующему изображению
            System.Threading.Thread.Sleep(300);
            //MapRevealer(250 + bw.Width, 250 + bw.Height, 50, 50);

        }
        private static string[] GetConfig()
        {
            string path = Data.path = System.Environment.GetEnvironmentVariable("USERPROFILE") + "\\Documents" + "\\PixelArtCreatorByMixailka";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            if (!File.Exists(path + "\\config.cfg"))
            {
                //File.Create(path + "\\config.cfg");
                Main.NewText("config file not found", 247, 83, 148);
                Main.NewText($"config file was created on {path}", 247, 83, 148);
                File.WriteAllLines(path + "\\config.cfg", new string[] {"MaxTile X> ","MaxTile Y> ","Torchs path> ","Exceptions path> "});
            }
            string[] config = File.ReadAllLines(path + "\\config.cfg");
            int index = 0;
            config[index] = config[index].Replace("MaxTile X>","").Trim();
            index++;
            config[index] = config[index].Replace("MaxTile Y>", "").Trim();
            index++;
            config[index] = config[index].Replace("Torchs path>", "").Trim();
            index++;
            config[index] = config[index].Replace("Exceptions path>", "").Trim();
            foreach (var item in config)
            {
                if (item == null || item == "") { Main.NewText($"Config error", 247, 83, 148); Main.NewText($"One of the required arguments is empty", 247, 83, 148); }
            }
            for (int i = 0; i < index; i++)
            {
                if (config[i] == null || config[i] == "") { Main.NewText($"Config error", 247, 83, 148); Main.NewText($"One of the required arguments is empty", 247, 83, 148); }
                switch (i)
                {
                    case 0:
                        config[i] = "228"; //maxtile x
                        break;
                    case 1:
                        config[i] = "1100"; //maxtile y
                        break;
                }
            }
            return config;
        }
        private static bool DataSet()
        {
            string[] config = GetConfig();
            if (config == null) return false;
            #region Max Tiles
            Data.maxtileX = Convert.ToInt32(config[0]);
            Data.maxtileY = Convert.ToInt32(config[1]);
            #endregion
            //Data.torches = File.ReadAllLines(config[2]);
            //string[] Exceptions = File.ReadAllLines(config[3]);
            string[] item;
            /*foreach (string elem in Exceptions)
            {
                item = elem.Split(':');
                if (item[0] == "1") { Data.Exceptions.Tiles.Add(item[1]); } else { Data.Exceptions.Walls.Add(item[1]); }
            }*/
            return true;
        }
        private static void PaletteSttr()
        {
            /*
             * get maxtile (X and Y) from a file maxtileXndY
             * who located on the notsortedcol (c:\ART)
             * 
             * 
             *
             */
            if (!DataSet()) { return; }
            Teleport_XY(Data.maxtileX + 200, Data.minTileY + 20);
            int maxTilesX = Data.maxtileX;
            int maxTilesY = Data.maxtileY;
            int minTilesX = Data.minTileX;
            int minTilesY = Data.minTileY;
            string[] torches = Data.Exceptions.Torchs.ToArray() ?? new string[] {"4"}; // torchs path replace with torches stroke
            List<string> notsortedcol = new List<string>();
            List<string> TileNames = new List<string>();
            int index = 0;
            //string i = 
            
            for (int i = minTilesX; i < maxTilesX; i++)
            {
                for (int j = minTilesY; j < maxTilesY; j++)
                {
                    MapTile mapTile = Main.Map[i, j];
                    /* LATER
                     * if (Data.Exceptions.Walls.Contains(Main.tile[i, j].WallType.ToString()) || Data.Exceptions.Tiles.Contains(Main.tile[i, j].TileType.ToString())) { goto HandledException; }
                    if (Main.tile[i, j].WallType != 0 && !torches.Contains(Convert.ToString(Main.tile[i, j].TileType)))
                     * 
                     * 
                     * 

                    */
                    //if (Data.Exceptions.Walls.Contains(Main.tile[i, j].WallType.ToString()) || Data.Exceptions.Tiles.Contains(Main.tile[i, j].TileType.ToString())) { goto HandledException; }
                    if (Main.tile[i, j].WallType != 0 && Main.tile[i, j].TileType != 0)
                    {
                        notsortedcol.Add(string.Concat(new object[] { "1-", Terraria.ID.TileID.Search.GetName(Main.tile[i, j].TileType), "-Block-", GetPaintNameFromByte(Main.tile[i, j].TileColor), "-", Main.tile[i, j].TileType, "\t", GetPaintFromByte(Main.tile[i, j].TileColor), "\t", MapHelper.GetMapTileXnaColor(ref mapTile).Hex3().ToUpper() }));
                    }
                    else if (Main.tile[i, j].WallType != 0)
                    {
                        notsortedcol.Add(string.Concat(new object[] { "0-", Terraria.ID.WallID.Search.GetName(Main.tile[i, j].WallType),"-Wall-", GetPaintNameFromByte(Main.tile[i, j].WallColor), "-", Main.tile[i, j].WallType, "\t", GetPaintFromByte(Main.tile[i, j].WallColor), "\t", MapHelper.GetMapTileXnaColor(ref mapTile).Hex3().ToUpper() }));
                        //File.AppendAllText(notsortedcol, string.Concat(new object[] { "0", Environment.NewLine, Main.tile[i, j].WallType, Environment.NewLine, GetPaintFromByte(Main.tile[i, j].WallColor), Environment.NewLine, MapHelper.GetMapTileXnaColor(ref mapTile).Hex3().ToUpper(), Environment.NewLine }));
                        ;
                    }
                    else if (Main.tile[i, j].TileType != 0) // add "	", Convert.ToString(i), "	", Convert.ToString(j), for debug
                    {


                        notsortedcol.Add(string.Concat(new object[] { "1-", Terraria.ID.TileID.Search.GetName(Main.tile[i, j].TileType), "-Block-", GetPaintNameFromByte(Main.tile[i, j].TileColor), "-", Main.tile[i, j].TileType, "\t", GetPaintFromByte(Main.tile[i, j].TileColor), "\t", MapHelper.GetMapTileXnaColor(ref mapTile).Hex3().ToUpper() }));
                        //File.AppendAllText(notsortedcol, string.Concat(new object[] { "1", Environment.NewLine, Main.tile[i, j].TileType, Environment.NewLine, GetPaintFromByte(Main.tile[i, j].TileColor), Environment.NewLine, MapHelper.GetMapTileXnaColor(ref mapTile).Hex3().ToUpper(), Environment.NewLine }));
                    }
                HandledException:
                    index++;
                }
            }
            Data.notsortedcol = notsortedcol.ToArray();
            ColorChose();
        }



        

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
    internal List<(bool isWall, bool isTorch, ushort id, byte paintId)> Read()
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
        return Array; // Возвращаем список прочитанных значений
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
class Data
{
    public static string path = String.Empty;
    public static int maxtileX = 200;
    public static int maxtileY = 1100;
    public static int minTileX, minTileY = 100;
    public static string[] notsortedcol, sortedcol, torches;
    public static class Exceptions
    {
        public static List<string> Torchs { get; private set; } = new List<string> { "4", "136", "557", "429", "424", "423", "420" };
        public static List<string> Tiles { get; private set; } = new List<string> {"3", "5", "10", "11",
                                                                                "12", "13", "14", "15",
                                                                                "16", "17", "18", "20",
                                                                                "21", "24", "26", "27",
                                                                                "28", "29", "31", "33",
                                                                                "34", "35", "36", "42",
                                                                                "49", "50", "55", "61",
                                                                                "71", "72", "73", "74",
                                                                                "77", "78", "79", "81",
                                                                                "82", "83", "84", "85",
                                                                                "86", "87", "88", "89",
                                                                                "90", "91", "92", "93",
                                                                                "94", "95", "96", "97",
                                                                                "98", "99", "100", "191",
                                                                                "102", "103", "104", "105",
                                                                                "106", "110", "113", "114",
                                                                                "125", "126", "128", "129",
                                                                                "132", "133", "134", "135",
                                                                                "137", "138", "139", "141",
                                                                                "142", "143", "144", "149",
                                                                                "165", "171", "172", "173",
                                                                                "174", "178", "184", "185",
                                                                                "186", "187", "201", "207",
                                                                                "209", "210", "212", "215",
                                                                                "216", "217", "218", "219",
                                                                                "220", "227", "228", "231",
                                                                                "233", "235", "236", "237",
                                                                                "238", "239", "240", "241",
                                                                                "242", "243", "244", "245",
                                                                                "246", "247", "254", "269",
                                                                                "270", "271", "275", "276",
                                                                                "277", "278", "279", "280",
                                                                                "281", "282", "283", "285",
                                                                                "286", "287", "288", "289",
                                                                                "290", "291", "292", "293",
                                                                                "294", "295", "296", "297",
                                                                                "298", "299", "300", "301",
                                                                                "302", "303", "304", "305",
                                                                                "306", "307", "308", "309",
                                                                                "310", "314", "316", "317",
                                                                                "318", "319", "320", "323",
                                                                                "324", "334", "335", "337",
                                                                                "338", "339", "349", "354",
                                                                                "355", "356", "358", "359",
                                                                                "360", "361", "362", "363",
                                                                                "364", "372", "373", "374",
                                                                                "375", "376", "377", "378",
                                                                                "380", "386", "387", "388",
                                                                                "389", "390", "391", "392",
                                                                                "393", "394", "395", "405",
                                                                                "406", "410", "411", "412",
                                                                                "413", "414", "419", "425",
                                                                                "527", "428", "440", "441",
                                                                                "442", "443", "444", "452",
                                                                                "453", "454", "455", "456",
                                                                                "457", "461", "462", "463",
                                                                                "464", "465", "466", "467",
                                                                                "468", "469", "470", "471",
                                                                                "475", "476", "480", "484",
                                                                                "485", "486", "487", "488",
                                                                                "489", "490", "491", "493",
                                                                                "494", "497", "499", "505",
                                                                                "506", "509", "510", "511",
                                                                                "518", "519", "520", "521",
                                                                                "522", "523", "524", "525",
                                                                                "526", "527", "529", "530",
                                                                                "531", "532", "533", "538",
                                                                                "542", "543", "544", "545",
                                                                                "547", "548", "549", "550",
                                                                                "551", "552", "553", "554",
                                                                                "555", "556", "558", "559",
                                                                                "560", "564", "565", "567",
                                                                                "568", "569", "570", "571",
                                                                                "572", "573", "579", "580",
                                                                                "581", "582", "583", "584",
                                                                                "585", "586", "587", "588",
                                                                                "589", "590", "591", "592",
                                                                                "593", "594", "595", "596",
                                                                                "597", "598", "599", "600",
                                                                                "601", "602", "603", "604",
                                                                                "605", "606", "607", "608",
                                                                                "609", "610", "611", "612",
                                                                                "613", "614", "615", "616",
                                                                                "617", "619", "620", "621",
                                                                                "622", "623", "624", "629",
                                                                                "630", "631", "632", "634",
                                                                                "637", "639", "640", "642",
                                                                                "643", "644", "645", "646",
                                                                                "647", "648", "649", "650",
                                                                                "651", "652", "653", "654",
                                                                                "656", "657", "658", "660",
                                                                                "663", "664", "665", "127",
                                                                                "52", "53", "112", "116",
                                                                                "234", "224", "123", "330",
                                                                                "331", "332", "333", "51",
                                                                                "52", "62", "115", "205",
                                                                                "382", "528", "636", "638",
                                                                                "32", "69", "352", "655",
                                                                                "80", "101", "124", "179",
                                                                                "180", "181", "182", "183",
                                                                                "366", "381", "449", "450",
                                                                                "451", "481", "482", "483",
                                                                                "504", "512", "513", "514",
                                                                                "515", "516", "517", "546",
                                                                                "574", "575", "576", "577",
                                                                                "578", "56", "495", "692",
                                                                                "160", "627", "628", "541" };
        public static List<string> Walls { get; private set; } = new List<string> { "168", "169" };

    }
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
