using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LcsServer.Models.LCProjectModels.GlobalBase;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;
using LcsServer.Models.LCProjectModels.Models.Rasters;
using LcsServer.Models.LCProjectModels.Models.ScenarioObjects;
using LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects;
using LCSVersionControl;
using NLog;

namespace LcsServer.Models.LCProjectModels.Managers;

public class ScenarioManager : BaseLCObjectsManager
    {
        //private readonly ISerializationManager _serializer;
        //private readonly ISoundController _soundController;
        private readonly VersionControlManager _versionControlManagerEx;

        private Dictionary<int, Raster> _oldIdNewRaster = new Dictionary<int, Raster>();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public event EventHandler<UpdateObjectsEventArgs> ObjectsUpdated;

        private readonly ModbusMastersManager _modbus;

        public ScenarioManager(VersionControlManager versionControlManagerEx)
        {
            /*_serializer = serializer;
            _soundController = soundController;*/
            _versionControlManagerEx = versionControlManagerEx;
            _modbus = new ModbusMastersManager();
        }

        /// <summary>
        /// Включенные сценарии
        /// </summary>
        public List<string> EnabledScenarios => GetPrimitives<Scenario>().Select(x => x.Name).ToList();

        /// <summary>
        /// Доступные сценарии
        /// </summary>
        public IEnumerable<string> AvailableScenarios => GetPrimitives<Scenario>().Select(x => x.Name).ToList();

        public override void AddObjects(params LCObject[] objects)
        {
            var result = AddRangeToLcObjectsList(objects);
            //TODO включить звуковые эффекты в версии 2.0
            //UpdateSoundBuffer();
            ObjectsUpdated?.Invoke(this, new UpdateObjectsEventArgs(result, InformAction.Add));
            UpdateDuration();
            UpdateModbusEffects();
        }

        private void UpdateModbusEffects()
        {
            IReadOnlyCollection<BaseModbusEffect> effects = GetPrimitives<BaseModbusEffect>();
            foreach (BaseModbusEffect effect in effects)
            {
                //_soundController.Enabled = true;
                effect.ModbusManager = _modbus;
            }
        }

        private void UpdateDuration()
        {
            foreach (PlayingEntity playingEntity in GetPrimitives<PlayingEntity>())
            {
                playingEntity.TotalTicksNotify();
            }
        }

        private void UpdateSoundBuffer()
        {
            IReadOnlyCollection<BaseAudioEffect> effects = GetPrimitives<BaseAudioEffect>();
            foreach (BaseAudioEffect effect in effects)
            {
                //_soundController.Enabled = true;
                //effect.Buffer = _soundController.GetBuffer();
            }
        }

        public override void RemoveObjects(LCObject[] objects)
        {
            Dictionary<int, LCObject> itemsForRemove = new Dictionary<int, LCObject>();
            foreach (LCObject lcObject in objects)
            {
                foreach (LCObject child in lcObject.GetAllLCObjects<LCObject>())
                {
                    itemsForRemove.TryAdd(child.Id, child);
                }
            }

            RemoveRangeFromLcObjectsList(itemsForRemove.Values.ToArray());
            ObjectsUpdated?.Invoke(this, new UpdateObjectsEventArgs(itemsForRemove.Values.ToList(), InformAction.Remove));
            UpdateDuration();
        }

        public override void RemoveAllObjects()
        {
            var effects = GetPrimitives<Effect>();
            foreach (var effect in effects)
            {
                if (effect is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
            ClearLcObjectsList();
            _oldIdNewRaster.Clear();
            //_soundController.Enabled = false;
            ObjectsUpdated?.Invoke(this, new UpdateObjectsEventArgs(null, InformAction.RemoveAll));
            UpdateDuration();
        }

        /// <summary>
        /// Переименовать сценарий
        /// </summary>
        /// <param name="scenario">Сценарий</param>
        /// <param name="newScenarioName">Новое имя сценария</param>
        public void RenameScenario(Scenario scenario, string newScenarioName)
        {
            if (!IsScenarioNameCorrect(newScenarioName))
            {
                return;
            }
            scenario.Name = newScenarioName;
        }

        /// <summary>
        /// Проверяет имя сценария на корректность
        /// </summary>
        /// <param name="scenarioName">Имя сценария</param>
        /// <returns>Корректное имя сценария или нет</returns>
        public bool IsScenarioNameCorrect(string scenarioName)
        {
            if (!FileManager.IsFileNameValid(scenarioName))
                return false;

            return !AvailableScenarios.Contains(scenarioName);
        }

        /// <summary>
        /// Создать сценарий по-умолчанию
        /// </summary>
        /// <returns>Сценарий по-умолчанию</returns>
        public Scenario CreateDefaultScenario()
        {
            var defaultScenario = new Scenario();

            defaultScenario.Id = GetNewCurrentId();
            defaultScenario.Name = GetNewName(defaultScenario.Name);

            return defaultScenario;
        }

        #region Save/Load

        /// <summary>
        /// Сохранить сценарии
        /// </summary>
        /// <param name="projectPath">Путь до папки проекта</param>
        /// <param name="projectName">Имя проекта</param>
        /// <param name="autoSave">Автосохранение</param>
        public void SaveScenarios(string projectPath, string projectName, bool autoSave)
        {
            List<Scenario> scenarios = GetPrimitives<Scenario>().ToList();
            foreach (var scenario in scenarios)
            {
                if (string.IsNullOrEmpty(scenario.Name))
                {
                    scenario.Name = projectName;
                }
                string fileName = $"{scenario.Name}.lcs";
                string folderPath = FileManager.GetScenariosFolderPath(projectPath);
                var scenarioFile = Path.Combine(folderPath, fileName);
                FileManager.CreateIfNotExist(folderPath);

                List<LCObject> lst = scenario.GetScenarioObjects();

                if (!autoSave)
                {
                    string scenarioPath = FileManager.GetScenariosFolderPath(projectPath);
                    lst.OfType<BaseMediaEffect>().ForEach(x => x.SetResourcesPath(scenarioPath));
                }

                _versionControlManagerEx.ConvertToVCAndSave(lst.OfType<ISaveLoad>(), projectPath, scenarioFile, true);
            }

            DeleteRemovedScenarios(projectPath);
        }

        /// <summary>
        /// Загрузить сценарии
        /// </summary>
        /// <param name="projectPath">Путь до папки проекта</param>
        /// <param name="enabledScenarios">Включенные сценарии</param>
        /// <param name="rasters">Список растров</param>
        /// <param name="isNewProjectFormat"></param>
        /// <returns>Список объектов в сценарии</returns>
        public void LoadScenarios(string scenariosPath, List<Raster> rasters)
        {
            _oldIdNewRaster = rasters.ToDictionary(x => x.Id, x => x);

            var scenarioItems = new List<LCObject>();

            if (!string.IsNullOrEmpty(scenariosPath))
            {
                /*string scenariosPath = FileManager.GetScenariosFolderPath(projectPath);
                FileManager.CreateIfNotExist(scenariosPath);

                IEnumerable<string> lcsFiles = Directory.GetFiles(scenariosPath).Where(s =>
                    ".lcs".Equals(Path.GetExtension(s), StringComparison.InvariantCultureIgnoreCase));

                foreach (string lcsFile in lcsFiles)
                {*/
                    var vcItems = _versionControlManagerEx.LoadAndConvertFromVC(scenariosPath, false);
                    //string scenarioName = Path.GetFileNameWithoutExtension(scenariosPath);

                    RestoreScenario(vcItems, scenariosPath);//, scenarioName);

                    IEnumerable<LCObject> deserializedItems = vcItems.Cast<LCObject>();


                    scenarioItems.AddRange(deserializedItems);
                //}
            }

            if (!scenarioItems.OfType<Scenario>().Any())
            {
                Scenario scenario = CreateDefaultScenario();
                scenarioItems.Add(scenario);
            }

            AddObjects(scenarioItems.ToArray());
        }

        public void ClearProject(string projectFolderPath)
        {
            var mediaEffects = GetPrimitives<BaseMediaEffect>(x=>!string.IsNullOrEmpty( x.FileName));
            string scenarioFolderPath = FileManager.GetScenariosFolderPath(projectFolderPath);
            HashSet<string> usedFiles = new HashSet<string>();

            foreach (BaseMediaEffect mediaEffect in mediaEffects)
            {
                usedFiles.Add(Path.Combine(scenarioFolderPath, mediaEffect.FileName));
            }

            foreach (string lcsFilePath in FileManager.GetFilesByExtensions(scenarioFolderPath, FileManager.ScenariosFileExtension))
            {
                usedFiles.Add(lcsFilePath);
            }

            FileManager.RemoveUnusedFiles(usedFiles, scenarioFolderPath);
        }

        private void DeleteRemovedScenarios(string projectPath)
        {
            string scenariosPath = FileManager.GetScenariosFolderPath(projectPath);
            FileManager.CreateIfNotExist(scenariosPath);
            IEnumerable<string> lcsFiles = Directory.GetFiles(scenariosPath).Where(s => ".lcs".Equals(Path.GetExtension(s), StringComparison.InvariantCultureIgnoreCase));

            foreach (string lcsFile in lcsFiles)
            {
                string scenarioName = Path.GetFileNameWithoutExtension(lcsFile);
                if (AvailableScenarios.Contains(scenarioName)) continue;

                try
                {
                    File.Delete(lcsFile);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }
        }

        private void RestoreScenario(List<ISaveLoad> scenarioItems, string projectFolderPath)//, string scenarioName)
        {
            for (int i = 0; i < scenarioItems.Count; i++)
            {
                ISaveLoad saveLoadObj = scenarioItems[i];
                saveLoadObj.Load(scenarioItems, i, projectFolderPath);
            }

            List<PlayingEntity> playingItems = scenarioItems.OfType<PlayingEntity>().ToList();

            foreach (ISaveLoad scenarioItem in scenarioItems)
            {
                switch (scenarioItem)
                {
                    case ScenarioTask task:
                        task.SetSequenceFromLoad(playingItems);
                        break;
                    case Track track:
                        track.SetSequenceFromLoad(playingItems);
                        break;
                }
            }

            var ignoredRaster = new List<Raster>();
            scenarioItems.OfType<Raster>().ForEach(x =>
            {
                string rasterName = x.Name;
                if (_oldIdNewRaster.ContainsKey(x.Id) && _oldIdNewRaster[x.Id].IsEquals(x))
                {
                    // Если уже был растер с таким же ID значит присваиваем ему ID предыдущего растра и добавляем его в ignore list
                    x.Id = _oldIdNewRaster[x.Id].Id;
                    ignoredRaster.Add(x);
                }
                else
                {
                    _oldIdNewRaster.TryAdd(x.Id, x);
                }
                x.Name = rasterName;
            });
            // Удаляем все растры которые попали в ignore list
            ignoredRaster.ForEach(x => scenarioItems.Remove(x));

            Scenario scenario = scenarioItems.OfType<Scenario>().FirstOrDefault();
            if (scenario == null)
            {
                return;
            }

            scenario.Id = GetNewCurrentId();
            RestoreRaster(scenario);

            // Обновляем ID у элементов сценариев
            foreach (var task in scenario.ScenarioTasks)
            {
                task.Parent = scenario;
                RestoreRaster(task);
                task.Id = GetNewCurrentId();
                foreach (var taskChild in task.GetSequence())
                {
                    taskChild.Parent = task;
                    ((PlayingEntity)taskChild).Id = GetNewCurrentId();
                    if (taskChild is not Unit unit)
                    {
                        continue;
                    }

                    RestoreRaster(unit);
                    foreach (Track track in unit.Tracks)
                    {
                        track.Parent = unit;
                        RestoreRaster(track);
                        track.Id = GetNewCurrentId();
                        foreach (IPlayingEntity itree in track.GetSequence())
                        {
                            itree.Parent = track;
                            ((PlayingEntity)itree).Id = GetNewCurrentId();
                        }
                    }
                }
            }

            //scenario.Name = scenarioName;
        }

        private void RestoreRaster(PlayingEntityWithRaster obj)
        {
            obj.LoadRaster(_oldIdNewRaster.Values);
            if (obj.UseParentRaster || obj.Raster == null) return;

            Raster value = _oldIdNewRaster.Values.FirstOrDefault(x => x.Id == obj.Raster.Id);
            obj.Raster = value;
        }

        #endregion

    }