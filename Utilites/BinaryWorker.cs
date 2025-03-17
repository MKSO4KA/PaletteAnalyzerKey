using System;
using System.Buffers.Binary;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using PaletteAnalyzerKey.Core;
using System.Threading.Tasks;

namespace PaletteAnalyzerKey.Utilites
{
    

    

    public class BinaryWorker
    {
        private string _path = "";
        private ushort x, y;

        public string Path
        {
            get => _path;
            set => _path = value;
        }

        public ushort Width
        {
            get => x;
            set => x = value;
        }

        public ushort Height
        {
            get => y;
            set => y = value;
        }

        public List<PhotoData> FileValues = new List<PhotoData>();

        public BinaryWorker(string path)
        {
            Path = path;
        }

        internal PhotoData[,] ReadAs2DArray(int temp = 0)
        {
            var list = new List<PhotoData>();
            byte[] bytes = File.ReadAllBytes(Path);

            Width = (ushort)((bytes[2] & 0xff) + ((bytes[3] & 0xff) << 8));
            Height = (ushort)((bytes[4] & 0xff) + ((bytes[5] & 0xff) << 8));

            for (int i = 6; i < bytes.Length && i + 4 < bytes.Length; i += 5)
            {
                list.Add(new PhotoData(
                    isWall: Convert.ToBoolean(bytes[i]),
                    isTorch: Convert.ToBoolean(bytes[i + 1]),
                    id: (ushort)((bytes[i + 2] & 0xff) + ((bytes[i + 3] & 0xff) << 8)),
                    paintId: bytes[i + 4]
                ));
            }
            return ConvertTo2DArray(list, Width, Height);
        }

        internal PhotoData[] Read()
        {
            byte[] fileData = File.ReadAllBytes(Path);
            Span<byte> dataSpan = fileData.AsSpan();

            if (dataSpan.Length < 6)
            {
                return Array.Empty<PhotoData>();
            }

            Width = BinaryPrimitives.ReadUInt16LittleEndian(dataSpan.Slice(2, 2));
            Height = BinaryPrimitives.ReadUInt16LittleEndian(dataSpan.Slice(4, 2));

            int dataStartOffset = 6;
            Span<byte> dataSection = dataSpan.Slice(dataStartOffset);
            int elementsCount = dataSection.Length / 5;

            var result = new PhotoData[elementsCount];

            for (int i = 0; i < elementsCount; i++)
            {
                int offset = i * 5;
                result[i] = new PhotoData(
                    isWall: dataSection[offset] != 0,
                    isTorch: dataSection[offset + 1] != 0,
                    id: BinaryPrimitives.ReadUInt16LittleEndian(dataSection.Slice(offset + 2, 2)),
                    paintId: dataSection[offset + 4]
                );
            }
            return result;
        }

        private static PhotoData[,] ConvertTo2DArray(List<PhotoData> list, int width, int height)
        {
            if (list.Count != width * height)
            {
                throw new ArgumentException("Invalid array dimensions");
            }

            var array = new PhotoData[height, width];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    array[i, j] = list[i * width + j];
                }
            }
            return array;
        }

        internal void Write(ushort width, ushort height, ushort widthStart = 0, List<PhotoData> array = null)
        {
            array ??= FileValues;
            using var stream = File.Open(Path, FileMode.Create);
            using var writer = new BinaryWriter(stream, Encoding.UTF8, false);

            writer.Write(widthStart);
            writer.Write(width);
            writer.Write(height);

            foreach (var item in array)
            {
                writer.Write(item.IsWall);
                writer.Write(item.IsTorch);
                writer.Write(item.Id);
                writer.Write(item.PaintId);
            }
        }
    }
}
