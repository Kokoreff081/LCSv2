namespace LcsServer.Models.LCProjectModels.GlobalBase;

public static class FileManager
{
    public const string AssetsFolderName = "Assets";
        private const string TempBackupFolderName = "TempBackup";
        private const string ResourcesFolderName = "Resources";
        private const string DataBaseFolderName = "DB";
        public const string SettingsFolderName = "Settings";
        public const string CacheFolderName = "Cache";
        public const string TempFolderName = "Temp";
        public const string TempUnZipFolderName = "TempUnZip";
        public const string TexturesFolderName = "Textures";
        public const string LayoutsFolderName = "Layouts";
        public const string ScenariosFolderName = "Scenarios";
        public const string RastersFolderName = "Rasters";
        public const string AddressingFolderName = "Addressing";
        public const string SchedulerFolderName = "Scheduler";
        public const string DocumentationFolderName = "Documentation";
        public const string VisualizationFolderName = "Visualization";
        public const string ObjectTypeLampsFolderName = "Lamps";
        private const string ObjectTypeMeshesFolderName = "Meshes";
        public const string ObjectTypeEntitiesFolderName = "Entities";
        public const string ObjectTypeMotionSensorsFolderName = "MotionSensors";
        public const string ObjectTypeLightSensorsFolderName = "LightSensors";
        public const string CustomLampsFolderName = "CustomLamp";
        public const string DefaultProjectName = "New project";
        private const string PhotometryFolderName = "Phootometry";

        public const string SceneEntityFileName = "SceneEntity.lce";
        public const string SceneNodeFileName = "SceneEntity.lcn";
        private const string RasterFileName = "Rasters.lcr";
        private const string AddressingFileName = "Addressing.lca";
        public const string SchedulerFileNameNew = "lc.schedule";
        private const string InputPlayingEntityLinkFileName = "Links.lca";
        private const string RegisteredAddressingFileName = "RegisteredMap.lca";
        private const string DocumentationFileName = "Documentation.lct";
        public const string SchedulerFileName = "new-schedule.lcsked";
        public const string SceneEntityFileExtension = ".lcez";
        public const string ProjectFileExtension = ".lcd";
        public const string ZippedProjectFileExtension = ".lcdz";
        public const string SchedulerFileExtension = ".lcsked";
        public const string ScenariosFileExtension = ".lcs";
        public const string ProjectLCSFileName = "LCSProject";
        public const string ProjectLCSFilenameExtension = ".lcdw";
        public const string LCSProjectFolderName = "ProjectToLcs";
        
        public const string IesFileExtension = ".ies";

        public const string SkyboxesFolderName = "Skyboxes";

        private const string ProjectsFolderName = "Projects";
        private const string LczFileExtension = ".lcz";
        private const string SamplesFolderName = "Samples";
        private const string ImagesFolderName = "Images";
        private const string VideosFolderName = "Videos";
        private const string ModelsFolderName = "Models";
        private const string HydraFolderName = "Hydra";

        private static readonly string _applicationProgramDataFolderPath;

        //private static readonly Logger //_logger = //LogManager.GetCurrentClassLogger();

        static FileManager()
        {
            string appName = $"LightControlService";
            ExePath = AppDomain.CurrentDomain.BaseDirectory;
            AppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), appName);
            _applicationProgramDataFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), appName);
            
            ApplicationSettingsFolderPath = Path.Combine(AppDataPath, SettingsFolderName);
            ApplicationLayoutsFolderPath = Path.Combine(AppDataPath, LayoutsFolderName);
            TempUnZipFolderPath = Path.Combine(AppDataPath, TempUnZipFolderName);
            DocumentsFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), appName);

            var commonDocumentsFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments), appName);
            DefaultTempFolderProjectPath = Path.Combine(AppDataPath, TempFolderName);
            DefaultTempProjectFilePath = Path.Combine(DefaultTempFolderProjectPath, $"{DefaultProjectName}{ZippedProjectFileExtension}");
            DefaultTempProjectResourcesFolderPath = Path.Combine(DefaultTempFolderProjectPath, ResourcesFolderName);

            DefaultProjectsPath = Path.Combine(DocumentsFolderPath, ProjectsFolderName);
            SamplesProjectsPath = Path.Combine(commonDocumentsFolderPath, SamplesFolderName, ProjectsFolderName);
            DefaultImagesResourcesFolderPath = Path.Combine(commonDocumentsFolderPath, SamplesFolderName, ImagesFolderName);
            DefaultVideosResourcesFolderPath = Path.Combine(commonDocumentsFolderPath, SamplesFolderName, VideosFolderName);
            DefaultModelsResourcesFolderPath = Path.Combine(commonDocumentsFolderPath, SamplesFolderName, ModelsFolderName);

            
            LogFolder = Path.Combine(AppDataPath, "Logs");

            Directory.CreateDirectory(LogFolder);
            
        }

        /// <summary>
        /// Путь к папке  приложения в системе
        /// </summary>
        private static string ExePath { get; }
        private static string AppDataPath { get; }

        /// <summary>
        /// Путь к папке  с логами
        /// </summary>
        public static string LogFolder { get; }


        /// <summary>
        /// Путь к папке настроек приложения
        /// </summary>
        public static string ApplicationSettingsFolderPath { get; }

        /// <summary>
        /// Путь к папке с настройками интерфейса (раположение блоков в комнатах)
        /// </summary>
        public static string ApplicationLayoutsFolderPath { get; }

        /// <summary>
        /// Путь к папке приложения в "Моих документах"
        /// </summary>
        public static string DocumentsFolderPath { get; }

        /// <summary>
        /// Папка c примерами проектов
        /// </summary>
        public static string SamplesProjectsPath { get; }

        /// <summary>
        /// Папка нового проекта по умолчанию
        /// </summary>
        public static string DefaultProjectsPath { get; }

        /// <summary>
        /// Путь к временной папке, куда распаковывается архив проекта
        /// </summary>
        public static string TempUnZipFolderPath { get; }

        /// <summary>
        /// Путь к временной папке проекта в системной папке приложения
        /// </summary>
        public static string DefaultTempFolderProjectPath { get; }

        /// <summary>
        /// Полное имя файла нового проекта (включает путь)
        /// </summary>
        public static string DefaultTempProjectFilePath { get; }

        /// <summary>
        /// Путь к папке по-умолчанию откуда брать картинки
        /// </summary>
        public static string DefaultImagesResourcesFolderPath { get; }

        /// <summary>
        /// Путь к папке по-умолчанию откуда брать видео
        /// </summary>
        public static string DefaultVideosResourcesFolderPath { get; }

        /// <summary>
        /// Путь к папке по-умолчанию откуда брать файлы моделей (.fbx, .3ds, ...)
        /// </summary>
        public static string DefaultModelsResourcesFolderPath { get; }


        /// <summary>
        /// Путь к временной папке ресурсов в системной папке приложения
        /// </summary>
        public static string DefaultTempProjectResourcesFolderPath { get; }


        /// <summary>
        /// Возвращает путь к файлу базы данных проекта
        /// </summary>
        /// <param name="projectFolderPath">Путь к папке проекта</param>
        /// <param name="dbFileName">Имя файла базы данных</param>
        /// <returns>Путь к файлу базы данных проекта</returns>
        public static string GetProjectDataBaseFilePath(string projectFolderPath, string dbFileName)
        {
            return Path.Combine(projectFolderPath, ResourcesFolderName, DataBaseFolderName, dbFileName);
        }

        /// <summary>
        /// Возвращает путь к папке ресурсов проекта
        /// </summary>
        /// <param name="projectFolderPath">Путь к папке проекта</param>
        /// <returns>Путь к папке ресурсов проекта</returns>
        public static string GetResourceFolderPath(string projectFolderPath)
        {
            return Path.Combine(projectFolderPath, ResourcesFolderName);
        }

        /// <summary>
        /// Возвращает путь к папке настроек проекта
        /// </summary>
        /// <param name="projectFolderPath">Путь к папке проекта</param>
        /// <returns>Путь к папке настроек проекта</returns>
        public static string GetSettingsFolderPath(string projectFolderPath)
        {
            return Path.Combine(projectFolderPath, ResourcesFolderName, SettingsFolderName);
        }

        /// <summary>
        /// Возвращает путь к папке с картинками визуализации проекта
        /// </summary>
        /// <param name="projectFolderPath">Путь к папке проекта</param>
        /// <returns>Путь к папке с картинками визуализации</returns>
        public static string GetVisualizationFolderPath(string projectFolderPath)
        {
            return Path.Combine(projectFolderPath, ResourcesFolderName, VisualizationFolderName);
        }

        /// <summary>
        /// Возвращает путь к папке кеша проекта
        /// </summary>
        /// <param name="projectFolderPath">Путь к папке проекта</param>
        /// <returns>Путь к папке кеша проекта</returns>
        public static string GetCacheFolderPath(string projectFolderPath)
        {
            return Path.Combine(projectFolderPath, ResourcesFolderName, CacheFolderName);
        }


        /// <summary>
        /// Возвращает путь к временной папке проекта
        /// </summary>
        /// <param name="projectFolderPath">Путь к папке проекта</param>
        /// <returns>Путь к временной папке проекта</returns>
        public static string GetTempProjectPath(string projectFolderPath)
        {
            return Path.Combine(projectFolderPath, TempFolderName);
        }

        /// <summary>
        /// Возвращает путь к папке сценариев проекта
        /// </summary>
        /// <param name="projectFolderPath">Путь к папке проекта</param>
        /// <returns>Путь к папке сценариев проекта</returns>
        public static string GetScenariosFolderPath(string projectFolderPath)
        {
            return Path.Combine(projectFolderPath, ResourcesFolderName, ScenariosFolderName);
        }

        /// <summary>
        /// Возвращает путь к папке где лежит файл растров
        /// </summary>
        /// <param name="projectFolderPath"></param>
        /// <returns></returns>
        public static string GetRastersFolderPath(string projectFolderPath)
        {
            return Path.Combine(projectFolderPath, ResourcesFolderName, RastersFolderName);
        }

        /// <summary>
        /// Возвращает путь к файлу растров
        /// </summary>
        /// <param name="projectFolderPath">Путь к папке проекта</param>
        /// <returns></returns>
        public static string GetRastersFilePath(string projectFolderPath)
        {
            return Path.Combine(GetRastersFolderPath(projectFolderPath), RasterFileName);
        }

        /// <summary>
        /// Возвращает путь к папке где лежит файл адресации
        /// </summary>
        /// <param name="projectFolderPath"></param>
        /// <returns></returns>
        public static string GetAddressingFolderPath(string projectFolderPath)
        {
            return Path.Combine(projectFolderPath, ResourcesFolderName, AddressingFolderName);
        }

        /// <summary>
        /// Возвращает путь к файлу адресации
        /// </summary>
        /// <param name="projectFolderPath"></param>
        /// <returns></returns>
        public static string GetAddressingFilePath(string projectFolderPath)
        {
            return Path.Combine(GetAddressingFolderPath(projectFolderPath), AddressingFileName);
        }

        /// <summary>
        /// Возвращает путь к файлу связи между modbus устройствами и эл-ми сценариев
        /// </summary>
        /// <param name="projectFolderPath"></param>
        /// <returns></returns>
        public static string GetInputPlayingEntityLinkFilePath(string projectFolderPath)
        {
            return Path.Combine(GetAddressingFolderPath(projectFolderPath), InputPlayingEntityLinkFileName);
        }

        /// <summary>
        /// Возвращает путь к файлу зарегестрированной адресации
        /// </summary>
        /// <param name="projectFolderPath"></param>
        /// <returns></returns>
        public static string GetRegisteredAddressingFilePath(string projectFolderPath)
        {
            return Path.Combine(GetAddressingFolderPath(projectFolderPath), RegisteredAddressingFileName);
        }

        /// <summary>
        /// Возвращает путь к папке где лежит файл планировщика
        /// </summary>
        /// <param name="projectFolderPath"></param>
        /// <returns></returns>
        public static string GetScheduleFolderPath(string projectFolderPath)
        {
            return Path.Combine(projectFolderPath, ResourcesFolderName, SchedulerFolderName);
        }
        /// <summary>
        /// Возвращает путь к папке где лежит проект для LCS
        /// </summary>
        /// <param name="projectFolderPath"></param>
        /// <returns></returns>
        public static string GetLCSProjectFolderPath(string projectFolderPath)
        {
            return Path.Combine(projectFolderPath, ResourcesFolderName, LCSProjectFolderName);
        }
        /// <summary>
        /// Возвращает путь к файлу планировщика
        /// </summary>
        /// <param name="projectFolderPath"></param>
        /// <returns></returns>
        public static string GetSchedulerFilePath(string projectFolderPath)
        {
            return Path.Combine(GetScheduleFolderPath(projectFolderPath), SchedulerFileName);
        }

        /// <summary>
        /// Возвращает путь к папке где лежит файл адресации
        /// </summary>
        /// <param name="projectFolderPath"></param>
        /// <returns></returns>
        public static string GetDocumentationFolderPath(string projectFolderPath)
        {
            return Path.Combine(projectFolderPath, ResourcesFolderName, DocumentationFolderName);
        }

        /// <summary>
        /// Возвращает путь к файлу адресации
        /// </summary>
        /// <param name="projectFolderPath"></param>
        /// <returns></returns>
        public static string GetDocumentationFilePath(string projectFolderPath)
        {
            return Path.Combine(GetDocumentationFolderPath(projectFolderPath), DocumentationFileName);
        }

        /// <summary>
        /// Возвращает путь к папке ассетов проекта
        /// </summary>
        /// <param name="projectFolderPath">Путь к папке проекта</param>
        /// <returns>Путь к папке ассетов проекта</returns>
        public static string GetAssetsFolderPath(string projectFolderPath)
        {
            return Path.Combine(projectFolderPath, ResourcesFolderName, AssetsFolderName);
        }

        /// <summary>
        /// Возвращает путь к папке ассетов проекта
        /// </summary>
        /// <param name="projectFolderPath">Путь к файлу проекта</param>
        /// <returns>Путь к папке ассетов проекта</returns>
        public static string GetAssetsFolderPathByFile(string projectFilePath)
        {
            return Path.Combine(Path.GetDirectoryName(projectFilePath), ResourcesFolderName, AssetsFolderName);
        }

        /// <summary>
        /// Возвращает путь к папке базы данных проекта
        /// </summary>
        /// <param name="projectFolderPath">Путь к папке проекта</param>
        /// <returns>Путь к папке базы данных проекта</returns>
        public static string GetDataBaseFolderPath(string projectFolderPath)
        {
            return Path.Combine(projectFolderPath, ResourcesFolderName, DataBaseFolderName);
        }

        /// <summary>
        /// Возвращает путь к временной папке проекта
        /// </summary>
        /// <param name="projectFilePath">Путь к файлу проекта</param>
        /// <returns>Путь к временной папке проекта</returns>
        public static string GetTempProjectFilePath(string projectFilePath)
        {
            return Path.Combine(Path.GetDirectoryName(projectFilePath), TempFolderName, Path.GetFileName(projectFilePath));
        }

        /// <summary>
        /// Возвращает путь к папке моделей проекта
        /// </summary>
        /// <param name="projectFolderPath">Путь к папке проекта</param>
        /// <returns>Путь к папке моделей проекта</returns>
        public static string GetMeshesFolderPath(string projectFolderPath)
        {
            return Path.Combine(projectFolderPath, ResourcesFolderName, AssetsFolderName, ObjectTypeMeshesFolderName);
        }

        /// <summary>
        /// Возвращает путь к временной папке моделей проекта
        /// </summary>
        /// <param name="projectFolderPath">Путь к папке проекта</param>
        /// <returns>Путь к папке моделей проекта</returns>
        public static string GetMeshesTempFolderPath(string projectFolderPath)
        {
            return Path.Combine(projectFolderPath, ResourcesFolderName, AssetsFolderName, ObjectTypeMeshesFolderName, TempFolderName);
        }

        /// <summary>
        /// Возвращает путь к папке конкретной модели на сцене
        /// </summary>
        /// <param name="projectFolderPath">Путь к папке проекта</param>
        /// <param name="meshId">Идентификатор объекта</param>
        /// <returns>Путь к папке конкретной модели на сцене</returns>
        public static string GetMeshInstanceFolderPath(string projectFolderPath, string meshId)
        {
            return Path.Combine(projectFolderPath, ResourcesFolderName, AssetsFolderName, ObjectTypeMeshesFolderName, meshId);
        }

        /// <summary>
        /// Возвращает путь к файлу модели 
        /// </summary>
        /// <param name="projectFolderPath">Путь к папке проекта</param>
        /// <returns>Путь к файлу модели</returns>
        public static string GetMeshesDataFilePath(string projectFolderPath)
        {
            return Path.Combine(GetMeshesFolderPath(projectFolderPath), $"MeshesData{SceneEntityFileExtension}");
        }

        /// <summary>
        /// Возвращает путь к файлу модели 
        /// </summary>
        /// <param name="projectFolderPath">Путь к папке проекта</param>
        /// <param name="index">Индекс добавляемый к имени файла</param>
        /// <returns>Путь к файлу модели</returns>
        public static string GetTempMeshesDataFilePath(string projectFolderPath, int index)
        {
            if (index == 0)
                return Path.Combine(GetMeshesTempFolderPath(projectFolderPath), $"MeshesData{SceneEntityFileExtension}");

            return Path.Combine(GetMeshesTempFolderPath(projectFolderPath), $"MeshesData ({index}){SceneEntityFileExtension}");
        }

        /// <summary>
        /// Возвращает путь к папке с данными источников в проекте
        /// </summary>
        /// <param name="projectFolderPath">Путь к папке проекта</param>
        /// <returns>Путь к папке с данными источников в проекте</returns>
        public static string GetLampsFolderPath(string projectFolderPath)
        {
            return Path.Combine(projectFolderPath, ResourcesFolderName, AssetsFolderName, ObjectTypeLampsFolderName);
        }

        /// <summary>
        /// Возвращает путь к папке с фотометричексими файлами (.ies)
        /// </summary>
        /// <param name="projectFolderPath">>Путь к папке проекта</param>
        /// <returns>Путь к папке с фотометричексими файлами (.ies)</returns>
        public static string GetLampPhotometryFolderPath(string projectFolderPath)
        {
            return Path.Combine(projectFolderPath, ResourcesFolderName, AssetsFolderName, ObjectTypeLampsFolderName, PhotometryFolderName);
        }

        /// <summary>
        /// Возвращает путь к папке с изображениями светильника
        /// </summary>
        /// <param name="projectFolderPath">>Путь к папке проекта</param>
        /// <returns>Путь к папке с изображениями светильника</returns>
        public static string GetLampImagesFolderPath(string projectFolderPath)
        {
            return Path.Combine(projectFolderPath, ResourcesFolderName, AssetsFolderName, ObjectTypeLampsFolderName, ImagesFolderName);
        }

        /// <summary>
        /// Возвращает путь к папке с текстурами 
        /// </summary>
        /// <param name="projectFolderPath">Путь к папке проекта</param>
        /// <returns>Путь к папке с текстурами</returns>
        public static string GetTexturesFolderPath(string projectFolderPath)
        {
            return Path.Combine(projectFolderPath, ResourcesFolderName, AssetsFolderName, ObjectTypeMeshesFolderName, TexturesFolderName);
        }

        /// <summary>
        /// Возвращает путь к файлу материалов
        /// </summary>
        /// <param name="projectFolderPath">Путь к папке проекта</param>
        /// <returns>Путь к файлу материалов</returns>
        public static string GetMaterialsDataFilePath(string projectFolderPath)
        {
            return Path.Combine(GetMeshesFolderPath(projectFolderPath), $"MaterialsData{SceneEntityFileExtension}");
        }

        /// <summary>
        /// Возвращает путь к папке бэкапа сохранения. Используется при сохранеии проека в новую локацию.
        /// </summary>
        /// <param name="projectFolderPath">Путь к папке проекта</param>
        /// <returns>Путь к папке бэкапа сохранения</returns>
        public static string GetTempBackupFolderPath(string projectFolderPath)
        {
            return Path.Combine(projectFolderPath, TempBackupFolderName);
        }

        /// <summary>
        /// Создает директорию по указанному пути, если ее не существует
        /// </summary>
        /// <param name="directory">Полный путь к директории</param>
        /// <param name="fileAttributes">Атрибут создаваемой директории</param>
        public static void CreateIfNotExist(string directory, FileAttributes fileAttributes = FileAttributes.Directory)
        {
            if (directory == null || Directory.Exists(directory))
            {
                return;
            }
            //TODO разобраться с каталогоми и их созданием почём зря
            try
            {
                _ = Directory.CreateDirectory(directory);
            }
            catch (Exception e)
            {
                ////LogManager.GetCurrentClassLogger().Error($"Cant create directory {directory} \n {e.Message}");
            }

            // var di = Directory.CreateDirectory(directory);
            // if (fileAttributes != FileAttributes.Directory)
            // {
            //     di.Attributes = fileAttributes;
            // }
        }

        /// <summary>
        /// Проверяет является ли файл файлом объекта сцены путем проверки расширения файла
        /// </summary>
        /// <param name="filePath">Полный путь к файлу</param>
        /// <returns>true - проверяемый файл имеет расширение файла объекта сцены
        ///          false - проверяемый файл имеет другое расширение
        /// </returns>
        public static bool IsPackedLcSceneObjectFile(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            return fileInfo.Extension.Equals(LczFileExtension, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Проверяет является ли файл файлом объекта сцены путем проверки расширения файла
        /// </summary>
        /// <param name="filePath">Полный путь к файлу</param>
        /// <returns>true - проверяемый файл имеет расширение файла объекта сцены
        ///          false - проверяемый файл имеет другое расширение
        /// </returns>
        public static bool IsZippedProjectFile(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            return fileInfo.Extension.Equals(ZippedProjectFileExtension, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Копирует файл
        /// </summary>
        /// <param name="sourceFilePath">Путь к копируемому файлк</param>
        /// <param name="outputFolderPath">Путь назначения</param>
        /// <returns>string.Empty - если исходный файл не неайден. 
        /// Новый путь к фай
        /// </returns>
        public static string CopyFile(string sourceFilePath, string outputFolderPath)
        {
            if (!File.Exists(sourceFilePath))
            {
                // TODO: Semyon return throw Exception
                //throw new FileNotFoundException(sourceFilePath);
                return string.Empty;
            }

            CreateIfNotExist(outputFolderPath);

            string fileName = Path.GetFileName(sourceFilePath);
            string destFilePath = Path.Combine(outputFolderPath, fileName);
            if (File.Exists(sourceFilePath) && !File.Exists(destFilePath))
            {
                File.Copy(sourceFilePath, destFilePath, true);
            }

            return destFilePath;
        }

        /// <summary>
        /// Устанавливает файлу атрибут Normal и удаляет его
        /// </summary>
        /// <param name="filePath">Полный путь к файлу</param>
        public static void DeleteFile(string filePath)
        {
            File.SetAttributes(filePath, FileAttributes.Normal);
            File.Delete(filePath);
        }

        /// <summary>
        /// Удаляет файлы по маске
        /// </summary>
        /// <param name="folderPath">Путь к папке</param>
        /// <param name="mask">Маска, по которой производтся поиск файлов</param>
        public static void DeleteFilesByMask(string folderPath, string mask)
        {
            foreach (string f in Directory.EnumerateFiles(folderPath, mask))
            {
                File.Delete(f);
            }
        }

        /// <summary>
        /// Очищает дирекорию 
        /// </summary>
        /// <param name="usedResourceDirectories">Созданные папки </param>
        /// <param name="directoryPath">Путь к проекту</param>
        public static void RemoveUnusedDirectories(List<string> usedResourceDirectories, string directoryPath)
        {
            DirectoryInfo directory = new DirectoryInfo(directoryPath);

            if (!directory.Exists)
                return;

            foreach (DirectoryInfo subDirectory in directory.GetDirectories())
            {
                //if (subDirectory.FullName.Equals(GetTexturesFolderPath(projectPath), StringComparison.InvariantCultureIgnoreCase))
                //    continue;

                if (usedResourceDirectories.Contains(subDirectory.FullName)) continue;
                try
                {
                    subDirectory.Delete(true);
                }
                catch (Exception ex)
                {
                    //_logger.Error(ex);
                }
            }
        }

        /// <summary>
        /// Очищает дирекорию от файлов которые не в списке нужных файлов
        /// </summary>
        /// <param name="usedFiles">Нужные файлы (их не удаляем)</param>
        /// <param name="directoryPath">Путь к проекту</param>
        public static void RemoveUnusedFiles(HashSet<string> usedFiles, string directoryPath)
        {
            DirectoryInfo directory = new DirectoryInfo(directoryPath);

            if (!directory.Exists)
                return;

            foreach (FileInfo fileInfo in directory.GetFiles())
            {
                if (usedFiles.Contains(fileInfo.FullName)) continue;
                try
                {
                    fileInfo.Delete();
                }
                catch (Exception ex)
                {
                    //_logger.Error(ex);
                }
            }
        }

        /// <summary>
        /// Возвращает уникальное имя файла. Если файл с указанным именем уже существует, к имени добавляется порядковый номер.
        /// </summary>
        /// <param name="folderPath">Путь к папке</param>
        /// <param name="fileName">Имя файла</param>
        /// <returns></returns>
        public static string GetUniqueFileName(string folderPath, string fileName)
        {
            string fullFileName = Path.Combine(folderPath, fileName);
            if (!File.Exists(fullFileName))
                return fullFileName;

            string fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
            string ext = Path.GetExtension(fileName);

            for (int i = 1; i < 100; i++)
            {
                string filePath = Path.Combine(folderPath, $"{fileNameWithoutExt}({i}){ext}");
                if (!File.Exists(filePath))
                {
                    return filePath;
                }
            }

            return fullFileName;
        }

        /// <summary>
        /// Сравнение размеров файлов
        /// </summary>
        /// <returns>true - файлы равны по размеру
        /// false - в обратном случае
        /// </returns>
        public static bool IsFilesSizeAreEquals(string filePath1, string filePath2)
        {
            var fi1 = new FileInfo(filePath1);
            var fi2 = new FileInfo(filePath2);

            if (!fi1.Exists || !fi2.Exists)
                return false;

            return fi1.Length == fi2.Length;
        }

        /// <summary>
        /// Возвращает список файлов из директории по списку расширений
        /// </summary>
        /// <param name="directoryPath">Объект описания директории</param>
        /// <param name="extensions">Список расширений файлов</param>
        /// <returns>Коллекция найденых файлов</returns>
        public static IEnumerable<FileInfo> GetFilesByExtensions(this DirectoryInfo dir, params string[] extensions)
        {
            if (!dir.Exists)
            {
                return new List<FileInfo>();
            }

            IEnumerable<FileInfo> files = dir.EnumerateFiles();
            if (extensions != null)
            {
                return files.Where(f => extensions.Contains(f.Extension, StringComparer.InvariantCultureIgnoreCase));
            }

            return files;
        }

        /// <summary>
        /// Возвращает список файлов из директории по списку расширений
        /// </summary>
        /// <param name="directoryPath">Путь к папке</param>
        /// <param name="extensions">Список расширений файлов</param>
        /// <returns>Коллекция найденых файлов</returns>
        public static IEnumerable<string> GetFilesByExtensions(string directoryPath, params string[] extensions)
        {
            var directoryInfo = new DirectoryInfo(directoryPath);
            return directoryInfo.GetFilesByExtensions(extensions).Select(x => x.FullName);
        }

        /// <summary>
        /// Удаление директории
        /// </summary>
        /// <param name="path">Путь к удаляемой папке</param>
        public static void DeleteDirectory(string path)
        {
            try
            {
                if (!Directory.Exists(path)) return;

                Directory.Delete(path, true);
                //if (!Directory.Exists(path)) return;

                //string[] files = Directory.GetFiles(path);
                //string[] dirs = Directory.GetDirectories(path);

                //foreach (string file in files)
                //{
                //    DeleteFile(file);
                //}

                //foreach (string dir in dirs)
                //{
                //    DeleteDirectory(dir);
                //}

                //Directory.Delete(path, false);
            }
            catch (Exception ex)
            {
                //_logger.Warn(ex);
            }
        }

        /// <summary>
        /// Проверяет имя файла на корректность
        /// </summary>
        /// <param name="fileName">Имя файла</param>
        /// <returns>Результат проверки</returns>
        public static bool IsFileNameValid(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return false;

            foreach (char ch in Path.GetInvalidFileNameChars())
            {
                if (fileName.Contains(ch))
                    return false;
            }

            FileInfo fi = null;
            try
            {
                fi = new FileInfo(fileName);
            }
            catch (ArgumentException)
            {
            }
            catch (PathTooLongException)
            {
            }
            catch (NotSupportedException)
            {
            }

            return fi != null;
        }

        /// <summary>
        /// Проверяет, является 
        /// </summary>
        /// <param name="candidate">Путь к проверяемой папке</param>
        /// <param name="posibleParent">Путь к папке, которую просматриваем</param>
        /// <returns>Результат проверки. 
        ///          true - проверяемая папка является поддиректорией папки,которую просматриваем
        ///          false - в обратном случае.
        /// </returns>
        public static bool IsSubDirectoryOf(this string candidate, string posibleParent)
        {
            if (candidate == null || posibleParent == null)
                return false;

            try
            {
                var candidateInfo = new DirectoryInfo(candidate);
                var otherInfo = new DirectoryInfo(posibleParent);
                while (candidateInfo.Parent != null)
                {
                    if (candidateInfo.Parent.FullName == otherInfo.FullName)
                        return true;

                    candidateInfo = candidateInfo.Parent;
                }
            }
            catch (Exception ex)
            {
                //_logger.Error(ex);
                throw;
            }

            return false;
        }

        /// <summary>
        /// Копирует директорию
        /// </summary>
        /// <param name="sourceDirName">Путь к копируемой директрии</param>
        /// <param name="destDirName">Путь куда копируем</param>
        /// <param name="copySubDirs">Флаг: копировать ли поддиректории</param>
        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            var dir = new DirectoryInfo(sourceDirName);

            if (destDirName.IsSubDirectoryOf(sourceDirName))
                throw new  Exception("Can not copy a directory to its child directory");

            if (!dir.Exists) return;

            DirectoryInfo[] dirs = dir.GetDirectories();

            if (!Directory.Exists(destDirName))
                Directory.CreateDirectory(destDirName);

            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                try
                {
                    file.CopyTo(temppath, true);
                }
                catch (Exception ex)
                {
                    //_logger.Error(ex);
                }
            }

            if (!copySubDirs) return;

            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, temppath, copySubDirs);
            }
        }

        /// <summary>
        /// Creates a relative path from one file or folder to another.
        /// </summary>
        /// <param name="fromPath">Contains the directory that defines the start of the relative path.</param>
        /// <param name="toPath">Contains the path that defines the endpoint of the relative path.</param>
        /// <returns>The relative path from the start directory to the end path or <c>toPath</c> if the paths are not related.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="UriFormatException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static string MakeRelativePath(string fromPath, string toPath)
        {
            if (string.IsNullOrEmpty(fromPath)) throw new ArgumentNullException(nameof(fromPath));
            if (string.IsNullOrEmpty(toPath)) throw new ArgumentNullException(nameof(toPath));

            var fromUri = new Uri(fromPath);
            var toUri = new Uri(toPath);

            if (fromUri.Scheme != toUri.Scheme)
            {
                return toPath;
            } // path can't be made relative.

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            String relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (toUri.Scheme.Equals("file", StringComparison.InvariantCultureIgnoreCase))
            {
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }

            return relativePath;
        }

        /// <summary>
        /// Проверяет имя файла, что заканчивается на расширение
        /// </summary>
        /// <param name="fileName">Имя файла</param>
        /// <param name="fileExtensions">Расширения файлов</param>
        /// <returns></returns>
        public static bool FileNameExtensionMatch(this string fileName, IEnumerable<string> fileExtensions)
        {
            return fileExtensions.Any(fileExtension => fileName.EndsWith(fileExtension, StringComparison.OrdinalIgnoreCase));
        }

        public static IEnumerable<string> GetFilesByExtension(string path, string filter)
        {
            var exts = filter.Split(';');
            try
            {
                return
                    Directory.EnumerateFiles(path, "*.*")
                        .Where(file => exts.Any(x => file.EndsWith(x, StringComparison.OrdinalIgnoreCase)));
            }
            catch (UnauthorizedAccessException)
            {
                return Array.Empty<string>();
            }
        }

        public static bool IsDirectoryEmpty(string path)
        {
            return !Directory.EnumerateFileSystemEntries(path).Any();
        }
}