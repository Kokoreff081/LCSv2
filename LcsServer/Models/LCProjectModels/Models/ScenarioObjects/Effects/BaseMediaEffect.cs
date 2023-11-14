using LcsServer.Models.LCProjectModels.GlobalBase;
using LCSVersionControl.Interfaces;

namespace LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects;

public abstract class BaseMediaEffect : Effect
    {
        private string _fileName;
        protected string ResourcesPath;
        
        public bool FileChanged { get; set; } // имя файла сменилось

        /// <summary>
        /// Изменение полного пути файла
        /// </summary>
        public event EventHandler FileNameChanged;

        protected BaseMediaEffect(string resourcesPath)
        {
            ResourcesPath = resourcesPath;
            _fileName = string.Empty;
        }

        /// <summary>
        /// Полный путь к файлу 
        /// </summary>
        [SaveLoad]
        public string FileName
        {
            get => _fileName;
            set
            {
                string fileName = value ?? string.Empty;
                if (_fileName.Equals(fileName) && !FileChanged)
                {
                    return;
                }

                _fileName = fileName;
                OnFileNameChanged();
                FileNameChanged?.Invoke(this, EventArgs.Empty);
                base.OnNameChanged();
            }
        }

        public string FullFilePath
        {
            get
            {
                return string.IsNullOrEmpty(_fileName) ? string.Empty : Path.Combine(ResourcesPath, _fileName);
            }
        }
        /// <summary>
        /// Установить путь к папке с ресурсами эффекта
        /// </summary>
        /// <param name="resourcesPath"></param>
        public void SetResourcesPath(string resourcesPath)
        {
            ResourcesPath = resourcesPath;
            OnFileNameChanged();
        }

        public override void Save(string projectFolderPath)
        {
            if (string.IsNullOrEmpty(FileName))
                return;

            string scenarioPath = FileManager.GetScenariosFolderPath(projectFolderPath);
            var fullFileName = Path.Combine(scenarioPath, FileName);
            if (!File.Exists(fullFileName))
            {
                var tempFullFileName = Path.Combine(ResourcesPath, FileName);
                if (File.Exists(tempFullFileName))
                {
                    File.Copy(tempFullFileName, fullFileName);
                }
            }
        }

        public override void Load(List<ISaveLoad> primitives, int indexOfPrimitives, string projectFolderPath)
        {
            base.Load(primitives, indexOfPrimitives, projectFolderPath);
            ResourcesPath = FileManager.GetScenariosFolderPath(projectFolderPath);
            OnFileNameChanged();
        }
        
        public override string DisplayName => $"{Name} [{FileName}]";

        protected virtual void OnFileNameChanged() { }
    }