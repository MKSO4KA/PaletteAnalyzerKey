using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.IO;
using System.Text.RegularExpressions;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace PaletteAnalyzer
{
    public class PaletteAnalyzerConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        // Существующие поля для путей
        [JsonProperty] private string _directoryPath = string.Empty;
        [JsonProperty] private string _fileName = string.Empty;
        [JsonProperty] private string _paletteName = string.Empty;
        [JsonProperty] private ushort _maxTileX = 0;
        [JsonProperty] private ushort _maxTileY = 0;
        [JsonProperty] private ushort _minTileX = 0;
        [JsonProperty] private ushort _minTileY = 0;
        [JsonProperty] private string _torchPath = string.Empty;
        [JsonProperty] private string _exeptionsPath = string.Empty;
        [JsonProperty] private string _tilesPath = string.Empty;
        // Новое поле для безопасного имени

        // Регулярное выражение для валидации
        private static readonly Regex SafeNameRegex = new Regex(
            @"^[a-zA-Z0-9_\-]*$",
            RegexOptions.Compiled
        );
        private bool _exceptionsEnabled;

        [DefaultValue(true)]
        [Tooltip("Отметьте, чтобы добавить ваши тайлы-исключения к уже существуующим.")]
        public bool ExceptionsEnabled
        {
            get => _exceptionsEnabled;
            set
            {
                if (_exceptionsEnabled != value)
                {
                    _exceptionsEnabled = value;
                    // Можно добавить дополнительную логику при изменении состояния
                    // Например, переинициализацию ресурсов
                }
            }
        }

        private bool _bordersMinMax;

        [DefaultValue(true)]
        [Tooltip("Отметьте, чтобы добавить ваши тайлы-исключения к уже существуующим.")]
        public bool BorderMinMax
        {
            get => _bordersMinMax;
            set
            {
                if (_bordersMinMax != value)
                {
                    _bordersMinMax = value;
                    // Можно добавить дополнительную логику при изменении состояния
                    // Например, переинициализацию ресурсов
                }
            }
        }

        [DefaultValue("")]
        [Tooltip("Директория для открытия кадров видео файлов")]
        public string VideoDirectoryPath
        {
            get => _directoryPath;
            set => _directoryPath = Directory.Exists(value) ? value : _directoryPath;
        }
        [DefaultValue("")]
        [Tooltip("Безопасное имя файла (только буквы, цифры, _ и -)")]
        public string VideoFileName
        {
            get => _fileName;
            set
            {
                if (!string.IsNullOrEmpty(value) && SafeNameRegex.IsMatch(value))
                {
                    _fileName = value;
                    SafeNameError = null;
                }
                else
                {
                    SafeNameError = "Недопустимые символы в имени файла!";
                }
            }
        }
        [DefaultValue("")]
        [Tooltip("Желаемый полный путь к создаваемой палитре")]
        public string PaletteFilePath
        {
            get => _paletteName;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (Directory.Exists(GetDirectoryFromPath(value)))
                    {
                        _paletteName = value;
                    }
                }
            }
        }
        [DefaultValue("")]
        [Tooltip("Путь к исключениям настенных элементов")]
        public string TorchsPath
        {
            get => _torchPath;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (TrySetFilePath(value))
                    {
                        _torchPath = value;
                    }
                }
            }
        }

        [DefaultValue("")]
        [Tooltip("Путь к исключениям")]
        public string ExceptionsPath
        {
            get => _exeptionsPath;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (TrySetFilePath(value))
                    {
                        _exeptionsPath = value;
                    }
                }
            }
        }
        [DefaultValue("0")]
        [Tooltip("Максимальная граница справа для размещения палитры")]
        public string MaxTileX
        {
            get => _maxTileX.ToString();
            set
            {
                // Для явного контроля переполнения
                checked
                {
                    try
                    {
                        // Автоматическая проверка диапазона для ushort
                        _maxTileX = string.IsNullOrEmpty(value) ? (ushort)0 : ushort.Parse(value);
                    }
                    catch (OverflowException)
                    {
                        // Обработка переполнения
                        _maxTileX = ushort.MaxValue;
                    }
                }
            }
        }
        [DefaultValue("0")]
        [Tooltip("Максимальная граница снизу для размещения палитры")]
        public string MaxTileY
        {
            get => _maxTileY.ToString();
            set
            {
                // Для явного контроля переполнения
                checked
                {
                    try
                    {
                        // Автоматическая проверка диапазона для ushort
                        _maxTileY = string.IsNullOrEmpty(value) ? (ushort)0 : ushort.Parse(value);
                    }
                    catch (OverflowException)
                    {
                        // Обработка переполнения
                        _maxTileY = ushort.MaxValue;
                    }
                }
            }
        }
        [DefaultValue("0")]
        [Tooltip("Минимальная граница справа для размещения палитры")]
        public string MinTileX
        {
            get => _minTileX.ToString();
            set
            {
                // Для явного контроля переполнения
                checked
                {
                    try
                    {
                        // Автоматическая проверка диапазона для ushort
                        _minTileX = string.IsNullOrEmpty(value) ? (ushort)0 : ushort.Parse(value);
                    }
                    catch (OverflowException)
                    {
                        // Обработка переполнения
                        _minTileX = ushort.MinValue;
                    }
                }
            }
        }
        [DefaultValue("0")]
        [Tooltip("Минимальная граница снизу для размещения палитры")]
        public string MinTileY
        {
            get => _minTileY.ToString();
            set
            {
                // Для явного контроля переполнения
                checked
                {
                    try
                    {
                        // Автоматическая проверка диапазона для ushort
                        _minTileY = string.IsNullOrEmpty(value) ? (ushort)0 : ushort.Parse(value);
                    }
                    catch (OverflowException)
                    {
                        // Обработка переполнения
                        _minTileY = ushort.MinValue;
                    }
                }
            }
        }




        [JsonIgnore] public string DirectoryError { get; private set; }
        [JsonIgnore] public string FileError { get; private set; }
        [JsonIgnore] public string SafeNameError { get; private set; }
        [JsonIgnore] public string LastFileError { get; private set; }

        public bool TrySetFilePath(string fullPath)
        {
            LastFileError = null;

            try
            {
                // Базовые проверки
                if (string.IsNullOrWhiteSpace(fullPath))
                {
                    LastFileError = "Путь не может быть пустым";
                    return false;
                }

                // Проверка существования файла
                if (!File.Exists(fullPath))
                {
                    LastFileError = $"Файл не существует: {fullPath}";
                    return false;
                }

                // Попытка открыть файл для чтения
                using (var fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    if (!fs.CanRead)
                    {
                        LastFileError = "Нет прав на чтение файла";
                        return false;
                    }
                }

                // Если все проверки пройдены
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                LastFileError = "Отказано в доступе к файлу";
                return false;
            }
            catch (PathTooLongException)
            {
                LastFileError = "Слишком длинный путь к файлу";
                return false;
            }
            catch (IOException ex)
            {
                LastFileError = $"Ошибка ввода-вывода: {ex.Message}";
                return false;
            }
            catch (Exception ex)
            {
                LastFileError = $"Неизвестная ошибка: {ex.Message}";
                return false;
            }
        }
        // Метод для генерации полного пути

        public bool ValidateDirectory(string directory)
        {
            bool valid = Directory.Exists(directory);
            DirectoryError = valid ? null : "Директория не существует";
            return valid;
        }

        public bool ValidateFileName(string filename)
        {
            bool valid = SafeNameRegex.IsMatch(filename);
            SafeNameError = valid ? null : "Некорректное имя файла";
            return valid;
        }
        public static string GetDirectoryFromPath(string path)
        {
            // Normalize the path by trimming trailing separators
            path = path.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            // Split the path into parts
            string[] parts = path.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            // If the last part has an extension, treat it as a file
            bool isFile = parts.Length > 0 && Path.HasExtension(parts[^1]);

            return isFile
                ? Path.GetDirectoryName(path)
                : path;
        }

        public override void OnChanged()
        {
            // Автоматическая проверка при изменении конфига
            ValidateDirectory(VideoDirectoryPath);
            ValidateFileName(VideoFileName);
        }
    }
}