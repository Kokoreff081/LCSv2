using LcsServer.Models.LCProjectModels.GlobalBase;
using LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;
using LCSVersionControl.Interfaces;
using LightCAD.UI.Strings;

namespace LcsServer.Models.LCProjectModels.Models.ScenarioObjects;

public class Scenario : PlayingEntityWithRaster, ISaveLoad, IClone
    {
        public event EventHandler ScenarioTasksChanged;

        private readonly List<ScenarioTask> _scenarioTasks;
        private long _totalTicks;
        [SaveLoad] private int[] _scenarioTaskIds;

        public Scenario()
        {
            _scenarioTasks = new List<ScenarioTask>();
            Name = Resources.NewScenario;
        }

        public Scenario(int id, string name, int[] taskIds, int rasterId, long riseTime, long fadeTime, bool isEnabled, float dimmingLevel)
            : base(id, name, riseTime, fadeTime, dimmingLevel)
        {
            _scenarioTaskIds = taskIds;
            _saveRasterId = rasterId;
            IsEnabled = isEnabled;

            _scenarioTasks = new List<ScenarioTask>();
        }

        /// <summary>
        /// Дочерние элементы
        /// </summary>
        protected override IEnumerable<PlayingEntityWithRaster> ChildrenWithRaster => _scenarioTasks;

        /// <summary>
        /// Использовать или нет родительский растр
        /// </summary>
        [SaveLoad]
        public override bool UseParentRaster
        {
            get => false;
            set { }
        }

        /// <summary>
        /// Список задач (ScenarioTasks)
        /// </summary>
        public IReadOnlyList<ScenarioTask> ScenarioTasks => _scenarioTasks;

        /// <summary>
        /// Добавить задачи
        /// </summary>
        /// <param name="tasks">Задачи</param>
        public void AddRange(params ScenarioTask[] tasks)
        {
            if (tasks == null || tasks.Length == 0) return;

            tasks.ForEach(task =>
            {
                task.Parent = this;
                task.TotalTicksChanged += OnTaskTotalTicksChanged;
            });
            _scenarioTasks.AddRange(tasks);
            _totalTicks = _scenarioTasks.Max(a => a.TotalTicks);
            UpdateRasterInChildren();
            ScenarioTasksChanged?.Invoke(this, EventArgs.Empty);
        }

        public void InsertRange(Dictionary<int, ScenarioTask> tasks)
        {
            if (tasks == null || tasks.Count == 0)
                return;

            foreach (var pair in tasks.OrderBy(a => a.Key))
            {
                _scenarioTasks.Insert(pair.Key, pair.Value);
            }

            tasks.Values.ForEach(task => task.TotalTicksChanged += OnTaskTotalTicksChanged);
            _totalTicks = _scenarioTasks.Max(a => a.TotalTicks);
            UpdateRasterInChildren();
            ScenarioTasksChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Удалить задачи
        /// </summary>
        /// <param name="tasks">Задача</param>
        public void RemoveRange(params ScenarioTask[] tasks)
        {
            if (tasks == null || tasks.Length == 0)
                return;

            tasks.ForEach(task => task.TotalTicksChanged -= OnTaskTotalTicksChanged);
            tasks.ForEach(task => _scenarioTasks.Remove(task));
            _totalTicks = _scenarioTasks.Count > 0 ? _scenarioTasks.Max(a => a.TotalTicks) : 0L;
            UpdateRasterInChildren();
            ScenarioTasksChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Удалить задачу
        /// </summary>
        /// <param name="task">Задача</param>
        public void Remove(ScenarioTask task)
        {
            task.TotalTicksChanged -= OnTaskTotalTicksChanged;
            _scenarioTasks.Remove(task);
            _totalTicks = _scenarioTasks.Count > 0 ? _scenarioTasks.Max(a => a.TotalTicks) : 0L;
            UpdateRasterInChildren();
            ScenarioTasksChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Вернуть индекс задачи в списке дочерних элементах по Id задачи
        /// </summary>
        /// <param name="id">Id задачи</param>
        /// <returns>Индекс задачи</returns>
        public int GetCollectionIndex(int id)
        {
            return _scenarioTasks.FindIndex(a => a.Id == id);
        }

        public override bool TryGetFrame(long tick, out ScenarioFrame frame, bool calculateDefaultSize,
            float riseOrFadeLevel)
        {
            frame = null;

            if (!IsEnabled)
            {
                return false;//GetEmptyFrame(tick, out frame);
            }

            float totalRiseOrFadeLevel = GetLevelByRiseAndFade(tick) * riseOrFadeLevel;

            for (int i = 0; i < ScenarioTasks.Count; i++)
            {
                if (!ScenarioTasks[i].TryGetFrame(tick, out frame, false, totalRiseOrFadeLevel))
                {
                    continue;
                }

                for (int j = i + 1; j < ScenarioTasks.Count; j++)
                {
                    if (ScenarioTasks[j].TryGetFrame(tick, out ScenarioFrame frame2, false, totalRiseOrFadeLevel))
                    {
                        MergeToFrame1(frame, frame2);
                    }
                }

                return true;
            }

            return false;
        }


        // public override bool GetEmptyFrame(long tick, out ScenarioFrame frame)
        // {
        //     frame = null;
        //
        //     for (int i = 0; i < ScenarioTasks.Count; i++)
        //     {
        //         if (!ScenarioTasks[i].TryGetFrame(tick, out frame, false, GetLevelByRiseAndFade(tick))) continue;
        //
        //         for (int j = i + 1; j < ScenarioTasks.Count; j++)
        //         {
        //             if (ScenarioTasks[j].GetEmptyFrame(tick, out ScenarioFrame frame2))
        //             {
        //                 MergeToFrame1(frame, frame2);
        //             }
        //         }
        //
        //         return true;
        //     }
        //
        //     return false;
        // }

        private void MergeToFrame1(ScenarioFrame frame1, ScenarioFrame frame2)
        {
            var map1 = frame1.GetDictionary();
            var map2 = frame2.GetDictionary();
            foreach (var pair in map2)
            {
                map1[pair.Key] = pair.Value;
            }
        }

        #region ISaveLoad

        /// <summary>
        /// Вызывается при загрузке проекта
        /// </summary>
        /// <param name="primitives">Все объекты проекта</param>
        /// <param name="indexInPrimitives">Индекс объекта в списке всех примитивов. Для ускоренного поиска родителя</param>
        /// <param name="projectFolderPath">Путь к папке проекта</param>
        public void Load(List<ISaveLoad> primitives, int indexInPrimitives, string projectFolderPath)
        {
            IEnumerable<LCObject> lcObjects = primitives.OfType<LCObject>();
            var tasks = lcObjects.Where((x) => _scenarioTaskIds.Contains(x.Id)).OfType<ScenarioTask>().ToArray();
            AddRange(tasks);
        }

        /// <summary>
        /// Вызывается при сохранении проекта
        /// </summary>
        /// <param name="projectFolderPath">Путь к папке проекта</param>
        public void Save(string projectFolderPath)
        {
        }

        #endregion

        protected override long GetTotalTicks()
        {
            return _scenarioTasks.Count > 0
                ? _scenarioTasks.Max(a => a.TotalTicks)
                : 0;
        }

        private void OnTaskTotalTicksChanged(object sender, EventArgs e)
        {
            var newDuration = _scenarioTasks.Max(a => a.TotalTicks);
            if (newDuration == _totalTicks) return;

            _totalTicks = newDuration;
            TotalTicksNotify();
        }


        public LCObject Clone(BaseLCObjectsManager lcObjectsManager, LCObject parent)
        {
            Scenario scenario = new Scenario();
            scenario.Id = lcObjectsManager.GetNewCurrentId();
            scenario.Name = Name;

            foreach (ScenarioTask scenarioTask in ScenarioTasks)
            {
                if (scenarioTask.Clone(lcObjectsManager, scenario) is not ScenarioTask task)
                {
                    continue;
                }

                scenario._scenarioTasks.Add(task);
            }

            scenario.RiseTime = RiseTime;
            scenario.FadeTime = FadeTime;

            return scenario;
        }

        public List<LCObject> GetScenarioObjects()
        {
            var lst = new List<LCObject> {this};
            lst.AddRange(ScenarioTasks);
            foreach (var task in ScenarioTasks)
            {
                var units = task.Entities;
                lst.AddRange(units);
                foreach (Unit unit in units)
                {
                    lst.AddRange(unit.Tracks);
                    foreach (Track track in unit.Tracks)
                    {
                        lst.AddRange(track.GetSequence().Cast<LCObject>());
                    }
                }
            }

            return lst;
        }
    }