using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaletteAnalyzerKey.Core
{
    internal class PhotoProcessor
    {
        private static string _photoDirectoryPath;
        public PhotoProcessor() 
        {
            ConfigLoader configLoader = new ConfigLoader();
            _photoDirectoryPath = configLoader.PhotoFilePath;
        }
        public async Task ProcessAsync()
        {
            if (!FileValidator.IsValidTextFile(_photoDirectoryPath))
            {
                return;
            }

            // Запускаем асинхронную задачу для создания арта
            await Task.Run(() => FrameCreator.InsertBlock(_photoDirectoryPath));

            return;
        }
    }
}
