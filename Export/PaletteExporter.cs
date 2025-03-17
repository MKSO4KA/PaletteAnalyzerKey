using PaletteAnalyzerKey.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using Terraria;
using PaletteAnalyzerKey.Utilites;
using Terraria.GameContent.UI.States;
using Terraria.ID;
using Terraria.Map;
using Terraria.ModLoader;
using PaletteAnalyzerKey.Data;
using System.Threading.Tasks;

namespace PaletteAnalyzerKey.Export
{
    public class PaletteExporter
    {
        private const ushort DEFAULT_MIN_X = 100;
        private const ushort DEFAULT_MIN_Y = 495;
        private const ushort DEFAULT_MAX_X = 500;
        private const ushort DEFAULT_MAX_Y = 640;
        private const ushort MAP_REVEAL_DELAY = 3000;

        private readonly ConfigLoader _configLoader;
        private readonly ExceptionsManager _exceptions;

        public PaletteExporter(ConfigLoader configLoader, ExceptionsManager exceptions)
        {
            _configLoader = configLoader;
            _exceptions = exceptions;
        }

        public void Export(string tilesPath, ushort minTilesX = DEFAULT_MIN_X, ushort minTilesY = DEFAULT_MIN_Y,
                         ushort maxTilesX = DEFAULT_MAX_X, ushort maxTilesY = DEFAULT_MAX_Y)
        {
            PrepareEnvironment(minTilesX, minTilesY, (ushort)(maxTilesX - minTilesX), (ushort)(maxTilesY - minTilesY));
            var rawData = CollectTileData(minTilesX, minTilesY, maxTilesX, maxTilesY);
            var processedData = ProcessCollectedData(rawData);
            SaveResults(tilesPath, processedData);
        }

        private void PrepareEnvironment(ushort x, ushort y, ushort width, ushort height)
        {
            Thread.Sleep(MAP_REVEAL_DELAY);
            MapRevealer.RevealMapArea(x, y, width, height);
            Thread.Sleep(MAP_REVEAL_DELAY);
        }

        private List<string> CollectTileData(int minX, int minY, int maxX, int maxY)
        {
            var rawData = new List<string>();
            var torchIds = _exceptions.Torchs.ToArray();

            for (int x = minX; x < maxX; x++)
            {
                for (int y = minY; y < maxY; y++)
                {
                    ProcessTile(x, y, rawData);
                }
            }
            return rawData;
        }

        private bool ProcessTile(int x, int y, List<string> output)
        {
            var tile = Main.tile[x, y];
            var mapTile = Main.Map[x, y];
            if (tile == null || tile.TileType < 0 || tile.TileType > DataManager.MaxTileCount || tile.WallType > DataManager.MaxWallCount || tile.WallType < 0)
                return false;
            if (tile.WallType != 0 && tile.TileType != 0)
            {
                output.Add(CreateTileEntry(tile, mapTile));
            }
            else if (tile.WallType > 1)
            {
                output.Add(CreateWallEntry(tile, mapTile));
            }
            return true;
        }

        private string CreateTileEntry(Tile tile, MapTile mapTile)
        {
            return string.Join("-", "1",
                TileID.Search.GetName(tile.TileType),
                "Block",
                GetPaintName(tile.TileColor),
                tile.TileType) + FormatColorInfo(tile.TileColor, mapTile);
        }

        private string CreateWallEntry(Tile tile, MapTile mapTile)
        {
            return string.Join("-", "0",
                WallID.Search.GetName(tile.WallType),
                "Wall",
                GetPaintName(tile.WallColor),
                tile.WallType) + FormatColorInfo(tile.WallColor, mapTile);
        }

        private string FormatColorInfo(byte paintColor, MapTile mapTile)
        {
            return $"\t{(int)paintColor}\t{MapHelper.GetMapTileXnaColor(ref mapTile).Hex3().ToUpper()}";
        }

        private List<string> ProcessCollectedData(List<string> rawData)
        {
            var sorted = SortByColor(rawData);
            var deduplicated = RemoveDuplicates(sorted);
            return FormatFinalOutput(deduplicated);
        }

        private List<string> SortByColor(List<string> data)
        {
            return data.Select(line =>
            {
                var parts = line.Split('\t');
                return new
                {
                    Line = line,
                    Color = ColorTranslator.FromHtml($"#{parts[2]}")
                };
            })
            .OrderBy(x => x.Color.GetHue())
            .ThenBy(x => x.Color.R * 3 + x.Color.G * 2 + x.Color.B)
            .Select(x => x.Line)
            .ToList();
        }

        private List<string> RemoveDuplicates(List<string> sortedData)
        {
            return sortedData
                .GroupBy(line => line.Split('\t')[2])
                .Select(group => group.First())
                .ToList();
        }

        private List<string> FormatFinalOutput(List<string> processedData)
        {
            return processedData.Select(line =>
            {
                var parts = line.Split(new[] { '-', '\t' });
                return $"{parts[0]}:{parts[4]}:{parts[5]}:{parts[6]}:{parts[2]}:{parts[1]}:{parts[3]}";
            }).ToList();
        }

        private void SaveResults(string path, List<string> data)
        {
            var outputPath = Path.Combine(path, "tiles.txt");

            if (!File.Exists(outputPath))
            {
                File.WriteAllLines(outputPath, data);
                NotifySuccess(outputPath);
            }
        }

        private void NotifySuccess(string path)
        {
            Main.NewText($"Palette file created: {path}", new Microsoft.Xna.Framework.Color(230, 230, 230));
        }

        private static string GetPaintName(byte color) => color switch
        {
            0 => "NonePaint",
            1 => "RedPaint",
            2 => "OrangePaint",
            3 => "YellowPaint",
            4 => "LimePaint",
            5 => "GreenPaint",
            6 => "TealPaint",
            7 => "CyanPaint",
            8 => "SkyBluePaint",
            9 => "BluePaint",
            10 => "PurplePaint",
            11 => "VioletPaint",
            12 => "PinkPaint",
            13 => "DeepRedPaint",
            14 => "DeepOrangePaint",
            15 => "DeepYellowPaint",
            16 => "DeepLimePaint",
            17 => "DeepGreenPaint",
            18 => "DeepTealPaint",
            19 => "DeepCyanPaint",
            20 => "DeepSkyBluePaint",
            21 => "DeepBluePaint",
            22 => "DeepPurplePaint",
            23 => "DeepVioletPaint",
            24 => "DeepPinkPaint",
            25 => "BlackPaint",
            26 => "WhitePaint",
            27 => "GrayPaint",
            28 => "BrownPaint",
            29 => "ShadowPaint",
            30 => "NegativePaint",
            31 => "IlluminantPaint",
            _ => throw new Exception($"GetPaintNameFromByte | Paint not in 0 - 31 range. Recieved Id = '{color}'"),
        };
    }
}