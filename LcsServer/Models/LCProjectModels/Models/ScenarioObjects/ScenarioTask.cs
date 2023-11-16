using LcsServer.Models.LCProjectModels.GlobalBase;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;

namespace LcsServer.Models.LCProjectModels.Models.ScenarioObjects;

/// <summary>
    /// Класс, описывающий последовательную коллекцию юнитов
    /// </summary>
    public class ScenarioTask : Sequence<Unit>, ISaveLoad, IClone
    {
        [SaveLoad] private int[] _unitsIds;
        [SaveLoad] private long[] _unitsStarts;

        public ScenarioTask() : base()
        {
        }

        public ScenarioTask(int id, string name, int parentId, int[] units, long[] starts, bool useParentRaster,
            int rasterId, long riseTime, long fadeTime, bool isEnabled, float dimmingLevel) : base(id, name, riseTime,
            fadeTime, dimmingLevel)
        {
            _parentId = parentId;
            _unitsIds = units;
            _unitsStarts = starts;
            UseParentRaster = useParentRaster;
            _saveRasterId = rasterId;
            IsEnabled = isEnabled;
        }

        /// <summary>
        /// Дочерние элементы
        /// </summary>
        protected override IEnumerable<PlayingEntityWithRaster> ChildrenWithRaster => Entities;

        /// <summary>
        /// Можно ли вычеслить размер по-умолчанию
        /// </summary>
        protected override bool CanCalculateDefaultSize => false;

        /// <summary>
        /// Вызывается при сохранении проекта
        /// </summary>
        /// <param name="projectFolderPath">Путь к папке проекта</param>
        public void Save(string projectFolderPath)
        {
        }

        /// <summary>
        /// Вызывается при загрузке проекта
        /// </summary>
        /// <param name="primitives">Все объекты проекта</param>
        /// <param name="indexInPrimitives">Индекс объекта в списке всех примитивов. Для ускоренного поиска родителя</param>
        /// <param name="projectFolderPath">Путь к папке проекта</param>
        public void Load(List<ISaveLoad> primitives, int indexInPrimitives, string projectFolderPath)
        {
            IEnumerable<LCObject> lcObjects = primitives.OfType<LCObject>().ToArray();
            LoadParent(lcObjects);
        }

        /// <summary>
        /// Клонирование
        /// </summary>
        /// <param name="lcObjectsManager">Ссылка на менеджер объектов LCObjectsManager (для присвоения ID)</param>
        /// <param name="parent">Родитель</param>
        public LCObject Clone(BaseLCObjectsManager lcObjectsManager, LCObject parent)
        {
            var task = new ScenarioTask();
            task.Parent = parent as IPlayingEntity;
            
            task.Id = lcObjectsManager.GetNewCurrentId();
            task.Name = Name;

            SetOwnerRastersData(task);
            SetOwnerSequence(task, lcObjectsManager);

            task.RiseTime = RiseTime;
            task.FadeTime = FadeTime;

            return task;
        }

        /// <summary>
        /// Установить Sequence при загрузке
        /// </summary>
        /// <param name="primitives">Все объекты сценария</param>
        public override void SetSequenceFromLoad(IEnumerable<PlayingEntity> primitives)
        {
            SetSequenceFromLoad(primitives, _unitsStarts, _unitsIds);
        }
    }