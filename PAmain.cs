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
        #region Cheats
        private static void Teleport_XY(int X, int Y)
        {
            Player player = Main.player[0];
            Vector2 pos = new Vector2(X, Y);
            pos = new Vector2(pos.X * 16 + 8 - player.width / 2, pos.Y * 16 - player.height);
            player.Teleport(pos, 2, 0);
        }
        private static void GodModeSet()
        {
            if (GodMode.Enabled) { return; }
            Main.NewText("GodMode is on for 10 sec");
            GodMode.Enabled = true;
            Thread.Sleep(10000);
            GodMode.Enabled = false;
            Main.NewText("GodMode is off");
        }
        private static bool MapRevealer()
        {
            int maxTilesX = Data.maxtileX;
            int maxTilesY = Data.maxtileY;

            bool result = false;
            for (int i = 0; i < maxTilesX + 10; i++)
            {
                for (int j = 0; j < maxTilesY + 10; j++)
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
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            
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
                return null;
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
                if (item == null || item == "") { Main.NewText($"Config error", 247, 83, 148); Main.NewText($"One of the required arguments is empty", 247, 83, 148); return null; }
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
            Data.torches = File.ReadAllLines(config[2]);
            string[] Exceptions = File.ReadAllLines(config[3]);
            string[] item;
            foreach (string elem in Exceptions)
            {
                item = elem.Split(':');
                if (item[0] == "1") { Data.Exceptions.Tiles.Add(item[1]); } else { Data.Exceptions.Walls.Add(item[1]); }
            }
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
            string[] torches = Data.torches;
            List<string> notsortedcol = new List<string>();
            List<string> TileNames = new List<string>();
            int index = 0;
            //string i = 
            
            for (int i = minTilesX; i < maxTilesX; i++)
            {
                for (int j = minTilesY; j < maxTilesY; j++)
                {
                    MapTile mapTile = Main.Map[i, j];
                    if (Data.Exceptions.Walls.Contains(Main.tile[i, j].WallType.ToString()) || Data.Exceptions.Tiles.Contains(Main.tile[i, j].TileType.ToString())) { goto HandledException; }
                    if (Main.tile[i, j].WallType != 0 && !torches.Contains(Convert.ToString(Main.tile[i, j].TileType)))
                    {
                        notsortedcol.Add(string.Concat(new object[] { "0-", Terraria.ID.WallID.Search.GetName(Main.tile[i, j].WallType),"-Wall-", GetPaintNameFromByte(Main.tile[i, j].WallColor), "-", Main.tile[i, j].WallType, "\t", GetPaintFromByte(Main.tile[i, j].WallColor), "\t", MapHelper.GetMapTileXnaColor(ref mapTile).Hex3().ToUpper() }));
                        //File.AppendAllText(notsortedcol, string.Concat(new object[] { "0", Environment.NewLine, Main.tile[i, j].WallType, Environment.NewLine, GetPaintFromByte(Main.tile[i, j].WallColor), Environment.NewLine, MapHelper.GetMapTileXnaColor(ref mapTile).Hex3().ToUpper(), Environment.NewLine }));
                        ;
                    }
                    else // add "	", Convert.ToString(i), "	", Convert.ToString(j), for debug
                    {
                        if (Main.tile[i, j].TileType != 0)
                        {
                            notsortedcol.Add(string.Concat(new object[] { "1-", Terraria.ID.TileID.Search.GetName(Main.tile[i, j].TileType), "-Block-", GetPaintNameFromByte(Main.tile[i, j].TileColor),"-", Main.tile[i, j].TileType, "\t", GetPaintFromByte(Main.tile[i, j].TileColor), "\t", MapHelper.GetMapTileXnaColor(ref mapTile).Hex3().ToUpper() }));
                            //File.AppendAllText(notsortedcol, string.Concat(new object[] { "1", Environment.NewLine, Main.tile[i, j].TileType, Environment.NewLine, GetPaintFromByte(Main.tile[i, j].TileColor), Environment.NewLine, MapHelper.GetMapTileXnaColor(ref mapTile).Hex3().ToUpper(), Environment.NewLine }));
                        }
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
        public static List<string> Walls = new List<string>();
		public static List<string> Tiles = new List<string>();
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
