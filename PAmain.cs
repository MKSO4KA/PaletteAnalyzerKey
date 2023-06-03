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
        
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            
            if (!PaletteAnalyzer.Palette.JustPressed) return;
            if (File.Exists("C:\\ARTs\\colors\\tiles.txt"))
            {
                File.Delete("C:\\ARTs\\colors\\tiles.txt");
            }
            Main.NewText("The game freezes for 10 seconds.");
            Main.NewText("if you saw a gray(not red) message when the game unfreeze");
            Main.NewText("then the scipt worked correctly.");
            PaletteSttr();

            
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
            int maxTilesX;
            int maxTilesY;
            if (!Directory.Exists("C:\\ARTs\\colors"))
            {
                Directory.CreateDirectory("C:\\ARTs\\colors");
            }
            string notsortedcol = "C:\\ARTs\\colors\\nscolors.txt";
            string maxtile = "C:\\ARTs\\colors\\maxtileXndY.txt";
            string sortedcol = "C:\\ARTs\\colors\\sortedcolors.txt";
            if (File.Exists(maxtile))
            {
                using (StreamReader f = new StreamReader(maxtile))
                {
                    maxTilesX = Convert.ToInt32(f.ReadLine());
                    maxTilesY = Convert.ToInt32(f.ReadLine());
                }
            }
            else
            {
                // if file not exist
                maxTilesX = 200;
                maxTilesY = 1100;
                File.AppendAllText(maxtile, maxTilesX + Environment.NewLine + maxTilesY);
            }
            
            // ---------------------------------
            int minTilesY = 100;
            int minTilesX = 100;
            //int maxTilesY = Main.maxTilesY - 100;

            //Player player; 

            //RunTeleport(player, new Vector2(Main.spawnTileX, Main.spawnTileY), syncData, true);
            //File.Create(notsortedcol);
            if (File.Exists(notsortedcol))
            {
                File.Delete(notsortedcol);
            }
            if (File.Exists(sortedcol))
            {
                File.Delete(sortedcol);
            }
            for (int i = minTilesX; i < maxTilesX; i++)
            {
                for (int j = minTilesY; j < maxTilesY; j++)
                {

                    try
                    {
                        MapTile mapTile = Main.Map[i, j];
                        if (Main.tile[i, j].WallType != 0)
                        {
                            File.AppendAllText(notsortedcol, string.Concat(new object[] { "0-", Main.tile[i, j].WallType, "\t", GetPaintFromByte(Main.tile[i, j].WallColor), "\t", MapHelper.GetMapTileXnaColor(ref mapTile).Hex3().ToUpper(), Environment.NewLine }));
                            //File.AppendAllText(notsortedcol, string.Concat(new object[] { "0", Environment.NewLine, Main.tile[i, j].WallType, Environment.NewLine, GetPaintFromByte(Main.tile[i, j].WallColor), Environment.NewLine, MapHelper.GetMapTileXnaColor(ref mapTile).Hex3().ToUpper(), Environment.NewLine }));

                        }
                        else // add "	", Convert.ToString(i), "	", Convert.ToString(j), for debug
                        {
                            if (Main.tile[i, j].TileType != 0)
                            {
                                File.AppendAllText(notsortedcol, string.Concat(new object[] { "1-", Main.tile[i, j].TileType, "\t", GetPaintFromByte(Main.tile[i, j].TileColor), "\t", MapHelper.GetMapTileXnaColor(ref mapTile).Hex3().ToUpper(), Environment.NewLine }));
                                //File.AppendAllText(notsortedcol, string.Concat(new object[] { "1", Environment.NewLine, Main.tile[i, j].TileType, Environment.NewLine, GetPaintFromByte(Main.tile[i, j].TileColor), Environment.NewLine, MapHelper.GetMapTileXnaColor(ref mapTile).Hex3().ToUpper(), Environment.NewLine }));
                            }
                        }
                    }
                    catch (Exception)

                    {
                        //MessageBox.Show("Error.");
                        //string path2 = @"c:\temp\MyTest.txt";
                        //using (FileStream fs = File.Create(path2))
                        //{
                        //    byte[] info = new UTF8Encoding(true).GetBytes("This is some text in the file.");
                        //    // Add some information to the file.
                        // //    fs.Write(info, 0, info.Length);
                        // }
                    }
                }
            }
            Thread.Sleep(1000);
            ColorChose(notsortedcol,sortedcol);
        }

        private static void ColorChose(string path, string path_sorted)
        {
            //string path = "C:\\colors\\colors.txt";
            List<Tuple<string, string, Color>> tiledata = new List<Tuple<string, string, Color>>();

            foreach (string line in File.ReadLines(path))
            {
                string[] array10 = line.Split(new char[] { '	' });
                string tile = array10[0];
                string paint = array10[1];
                string color = array10[2];

                tiledata.Add(new Tuple<string, string, Color>(tile, paint, System.Drawing.ColorTranslator.FromHtml("#" + color)));
            }

            // Create An IOrderEnumerable List
            var tiledatalist = tiledata.OrderBy(color => color.Item3.GetHue()).ThenBy(o => o.Item3.R * 3 + o.Item3.G * 2 + o.Item3.B * 1);

            // Expand Each Item Of The List
            foreach (var tiledatainfo in tiledatalist)
            {
                // Define info from the list item
                string tile = tiledatainfo.Item1;
                string paint = tiledatainfo.Item2;
                Color color = tiledatainfo.Item3;

                
                // Output the data
                File.AppendAllText(path_sorted,tile + "\t" + paint + "\t" + ColorConverterExtensions.ToHexString(color).Replace("#", "") + Environment.NewLine);
            }

            // End Sub
            Thread.Sleep(1000);
            RemDuple(path_sorted);
            return;
        }

        private static void RemDuple(string path_sort)
        {
            
            //Thread.Sleep(2000);

            //string path = "C:\\ARTs\\nscolors.txt";
            //string path2 = "C:\\ARTs\\tiles.txt";
            string[] lines = File.ReadAllLines(path_sort);
            File.Delete(path_sort);
            lines = lines.Take(lines.Length - 1).ToArray();
            try
            {
                foreach (var line in lines.Where(x => x.Split('\t')[2] != null).GroupBy(x => x.Split('\t')[2]).Select(y => y.FirstOrDefault()))
                {
                    // Add value.
                    File.AppendAllText(path_sort, line + Environment.NewLine);
                }
            }
            catch(Exception)
            {
                //File.Create(path2);
            }
            Thread.Sleep(1000);
            Create_tile_File(path_sort);
            
        }

        private static void Create_tile_File(string path)
        {
            //string path = "C:\\ARTs\\nscolors.txt";
            //string path2 = "C:\\ARTs\\tiles.txt";
            string[] lines = File.ReadAllLines(path);
            File.Delete(path);
            char[] separators = new char[] { '\t', '-' };
            foreach (var line in lines)
            {
                string[] subs = line.Split(separators, StringSplitOptions.RemoveEmptyEntries);
                foreach (var sub in subs)
                {
                    File.AppendAllText(path, sub + Environment.NewLine );
                }
            }
            File.Delete("C:\\ARTs\\colors\\nscolors.txt");
            File.Move("C:\\ARTs\\colors\\sortedcolors.txt", "C:\\ARTs\\colors\\tiles.txt");
            Main.NewText("palette file for py-script created on C:ARTs\\colors\\", 230, 230, 230);
        }

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


    }


}
