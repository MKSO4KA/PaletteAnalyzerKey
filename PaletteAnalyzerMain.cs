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
using PaletteAnalyzerKey.Utilites;
using PaletteAnalyzerKey.Core;


/*               WARNING!!!
 *        
 * My eng is too bad 
 * My knowledge in c# is also very bad.
 * Please do not read the code.
 * After reading this, you can hurt yourself.
 * No one compensate for this damage.
*/
namespace PaletteAnalyzerKey
{
    public class PaletteAnalyzerKey : ModPlayer
    {
        private static uint artkey = 1;
        
        private static void GodModeSet()
        {
            GodMode.Enabled = !GodMode.Enabled;
            string text = $"GodMode is " + (GodMode.Enabled == true ? "on" : "off");
            Main.NewText(text);
        }
        

        public override async void ProcessTriggers(TriggersSet triggersSet)
        {
            var config = new ConfigLoader();
            if (PaletteAnalyzer.MapOverView.JustPressed)
            {
                await Task.Run(() => MapRevealer.RevealMapArea(0,0,Main.maxTilesX, Main.maxTilesY));
                return;
            }
            if (PaletteAnalyzer.InsertArt.JustPressed)
            {
                PhotoProcessor processor = new PhotoProcessor();
                await Task.Run(() => processor.ProcessAsync());

                
            }
            if (PaletteAnalyzer.StartVideoProcess.JustPressed)
            {
                VideoProcessor processor = new VideoProcessor();
                await Task.Run(() => processor.ProcessAsync());
            }

            if (PaletteAnalyzer.Parameters.JustPressed)
            {
                Thread thread2 = new Thread(() => GodModeSet());
                thread2.Start();
                return;
            }
            if (!PaletteAnalyzer.Palette.JustPressed) return;

            // Использование пути
            string TilesDir = config.PalettePath;
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

            PaletteExecutor paletteCreator = new PaletteExecutor();
            await Task.Run(() => paletteCreator.Execute());
            //Main.NewText("then the scipt worked correctly.");
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




