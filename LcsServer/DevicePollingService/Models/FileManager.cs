using System.Reflection;

namespace LcsServer.DevicePollingService.Models;

public static class FileManager
    {
        public const string AppName = "LCS2";

        public static string FullVersion { get; }

        public static string ShortVersion { get; }
        public static string ApplicationDataFolderPath { get; }
        public static string Settings { get; }
        public static string LogFolderPath { get; }

        static FileManager()
        {
            ApplicationDataFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), AppName);
            Settings = Path.Combine(ApplicationDataFolderPath, "Settings.json");
            LogFolderPath = Path.Combine(ApplicationDataFolderPath, "Log");

            FullVersion = Assembly.GetEntryAssembly().GetName().Version.ToString();
            var _version = FullVersion;
/*#if !DEBUG
            var description = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyDescriptionAttribute>().Description;
            _version = $"{_version.Substring(0, _version.IndexOf('.', _version.IndexOf('.') + 1))}";
            _version = $"{_version} {description ?? string.Empty}";
#endif*/
            ShortVersion = _version.Trim();
        }

        /// <summary>
        /// Создает директорию по указанному пути, если ее не существует
        /// </summary>
        /// <param name="directory">Полный путь к директории</param>
        /// <param name="fileAttributes">Атрибут создаваемой директории</param>
        public static void CreateIfNotExist(string directory, FileAttributes fileAttributes = FileAttributes.Directory)
        {
            if (Directory.Exists(directory)) return;
            var di = Directory.CreateDirectory(directory);
            if (fileAttributes != FileAttributes.Directory)
                di.Attributes = fileAttributes;
        }

        public static void WriteAllBytes(string path, byte[] data)
        {
            string directoryPath = Path.GetDirectoryName(path);
            CreateIfNotExist(directoryPath);
            File.WriteAllBytes(path, data);
        }

    }