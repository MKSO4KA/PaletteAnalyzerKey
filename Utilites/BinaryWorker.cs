using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Buffers.Binary;
using System.IO.MemoryMappedFiles;

namespace PaletteAnalyzerKey.Utilites
{

    public readonly struct PhotoData
    {
        public readonly bool IsWall;
        public readonly bool IsTorch;
        public readonly ushort Id;
        public readonly byte PaintId;

        public PhotoData(bool isWall, bool isTorch, ushort id, byte paintId)
        {
            IsWall = isWall;
            IsTorch = isTorch;
            Id = id;
            PaintId = paintId;
        }
    }

    public class BinaryWorker
    {
        private const int BufferSize = 6 * 1024 * 1024; // 6 МБ
        private string _path;
        private ushort _width, _height;

        public string Path
        {
            get => _path;
            set => _path = value;
        }

        public ushort Width
        {
            get => _width;
            set => _width = value;
        }

        public ushort Height
        {
            get => _height;
            set => _height = value;
        }

        public List<(bool isWall, bool isTorch, ushort id, byte paintId)> FileValues = new List<(bool isWall, bool isTorch, ushort id, byte paintId)>();

        public BinaryWorker(string path) => _path = path;

        internal PhotoData[,] ReadAs2DArray()
        {
            var (data, width, height) = ReadData();
            return ConvertTo2DArray(data, width, height);
        }

        internal PhotoData[] Read()
        {
            var (data, _, _) = ReadData();
            return data;
        }

        private (PhotoData[] data, ushort width, ushort height) ReadData()
        {
            using var mmFile = MemoryMappedFile.CreateFromFile(_path, FileMode.Open, null, 0, MemoryMappedFileAccess.Read);
            using var accessor = mmFile.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read);

            long fileLength = accessor.Capacity;
            if (fileLength < 6) throw new InvalidDataException("File too small");

            byte[] headerBytes = new byte[6];
            accessor.ReadArray(0, headerBytes, 0, 6);
            Span<byte> header = headerBytes.AsSpan();

            ushort width = BinaryPrimitives.ReadUInt16LittleEndian(header.Slice(2, 2));
            ushort height = BinaryPrimitives.ReadUInt16LittleEndian(header.Slice(4, 2));

            int dataLength = (int)(fileLength - 6);
            if (dataLength % 5 != 0) throw new InvalidDataException("Invalid data section");

            int count = dataLength / 5;
            PhotoData[] result = new PhotoData[count];
            byte[] dataBytes = new byte[dataLength];
            accessor.ReadArray(6, dataBytes, 0, dataLength);
            Span<byte> data = dataBytes.AsSpan();

            for (int i = 0; i < count; i++)
            {
                int offset = i * 5;
                result[i] = new PhotoData(
                    isWall: data[offset] != 0,
                    isTorch: data[offset + 1] != 0,
                    id: BinaryPrimitives.ReadUInt16LittleEndian(data.Slice(offset + 2, 2)),
                    paintId: data[offset + 4]
                );
            }

            _width = width;
            _height = height;
            return (result, width, height);
        }

        private static PhotoData[,] ConvertTo2DArray(PhotoData[] data, int width, int height)
        {
            if (data.Length != width * height)
                throw new ArgumentException("Dimensions mismatch");

            var array = new PhotoData[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    array[i, j] = data[i * width + j];
                }
            }
            return array;
        }

        internal void Write(ushort width, ushort height, ushort widthStart = 0, List<(bool isWall, bool isTorch, ushort id, byte paintId)> array = null)
        {
            array ??= FileValues;

            using var fs = new FileStream(_path, FileMode.Create, FileAccess.Write, FileShare.None, BufferSize, FileOptions.SequentialScan);

            Span<byte> header = stackalloc byte[6];
            BinaryPrimitives.WriteUInt16LittleEndian(header.Slice(0, 2), widthStart);
            BinaryPrimitives.WriteUInt16LittleEndian(header.Slice(2, 2), width);
            BinaryPrimitives.WriteUInt16LittleEndian(header.Slice(4, 2), height);
            fs.Write(header);

            const int elementSize = 5;
            byte[] buffer = new byte[array.Count * elementSize];
            for (int i = 0; i < array.Count; i++)
            {
                int offset = i * elementSize;
                var item = array[i];
                buffer[offset] = item.isWall ? (byte)1 : (byte)0;
                buffer[offset + 1] = item.isTorch ? (byte)1 : (byte)0;
                BinaryPrimitives.WriteUInt16LittleEndian(buffer.AsSpan(offset + 2, 2), item.id);
                buffer[offset + 4] = item.paintId;
            }
            fs.Write(buffer, 0, buffer.Length);
        }
    }
}
