using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using tModPorter;

namespace PaletteAnalyzerKey.Core
{
    internal class VideoProcessor
    {
        private static bool isTaskRunning = false;
        public static void StopProcessing() { isTaskRunning = true; }
        private static string _videoDirectoryPath;
        private static string _frameName;
        public VideoProcessor()
        {
            ConfigLoader configLoader = new ConfigLoader();
            _videoDirectoryPath = configLoader.VideoDirectory;
            _frameName = configLoader.FileName;
        }
        public async Task ProcessAsync()
        {
            long artKey = 1;
            string targetDirectory = Path.Combine(_videoDirectoryPath);

            // Проверяем, выполняется ли задача
            if (!isTaskRunning)
            {
                isTaskRunning = true; // Устанавливаем флаг, что задача запущена
                try
                {
                    while (isTaskRunning) // Бесконечный цикл для создания артов
                    {
                        string framePath = Path.Combine(targetDirectory, $"{_frameName}{artKey}.txt");

                        // Проверяем, существует ли файл
                        if (FileValidator.IsValidTextFile(framePath))
                        {
                            artKey++;
                            // Запускаем асинхронную задачу для создания арта
                            FrameCreator.InsertBlock(framePath);
                        }
                        else
                        {
                            // Если файл существует, выходим из цикла
                            break;
                        }
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
            }
        }
    }
}
