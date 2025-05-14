using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace PaletteAnalyzerKey.Utilites
{
    [StructLayout(LayoutKind.Sequential)] // Опционально, но хорошая практика для простых структур данных
    public struct Point
    {
        public int X;
        public int Y;

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        // Можно добавить Equals, GetHashCode, ToString, если потребуется
        // для использования в коллекциях или для отладки.
        // Для данного сценария они не критичны для производительности самого цикла.
    }
}
