using LcsServer.Models.LCProjectModels.GlobalBase;
using LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;
using LcsServer.Models.LCProjectModels.Models.Rasters;
using LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects;
using LCSVersionControl;
using LCSVersionControl.Interfaces;

namespace LcsServer.Models.LCProjectModels.Models.ScenarioObjects;

public class Track : Sequence<Effect>, ISaveLoad, IClone
    {
        [SaveLoad] private readonly long[] _starts;
        [SaveLoad] private readonly int[] _effectIds;

        public Track()
        {
        }

        public Track(int id, string name, int parentId, int[] effects, long[] starts, bool useParentRaster,
            int rasterId, long riseTime, long fadeTime, bool isEnabled, float dimmingLevel, float opacity) : base(id, name, riseTime,
            fadeTime, dimmingLevel)
        {
            _parentId = parentId;
            _effectIds = effects;
            _starts = starts;
            UseParentRaster = useParentRaster;
            _saveRasterId = rasterId;
            IsEnabled = isEnabled;
            Opacity = opacity;
        }

        /// <summary>
        /// Дочерние элементы
        /// </summary>
        protected override IEnumerable<PlayingEntityWithRaster> ChildrenWithRaster =>
            Array.Empty<PlayingEntityWithRaster>();

        /// <summary>
        /// Ссылка на объект растр
        /// </summary>
        public override Raster Raster
        {
            get => _raster;
            set
            {
                if (_raster == value)
                {
                    return;
                }

                RasterUnSubscription();
                _raster = value;
                RasterSubscription();
                UpdateRasterInChildren();
                NotifyAboutRasterChanged();
            }
        }
        
        public event EventHandler OpacityChanged;
        
        private float _opacity = 1.0f;

        /// <summary>
        /// Прозрачность для слоя - учитывается при режиме смешивания Normal
        /// </summary>
        public float Opacity
        {
            get => _opacity;
            set
            {
                if (_opacity.NearEqual(value))
                {
                    return;
                }

                _opacity = value;
                OpacityChanged?.Invoke(this, EventArgs.Empty);
            }
        }

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
        /// Установить Sequence при загрузке
        /// </summary>
        /// <param name="primitives">Все объекты сценария</param>
        public override void SetSequenceFromLoad(IEnumerable<PlayingEntity> primitives)
        {
            SetSequenceFromLoad(primitives, _starts, _effectIds);
        }

        /// <summary>
        /// Клонирование
        /// </summary>
        /// <param name="lcObjectsManager">Ссылка на менеджер объектов LCObjectsManager (для присвоения ID)</param>
        /// <param name="parent">Родитель</param>
        public LCObject Clone(BaseLCObjectsManager lcObjectsManager, LCObject parent)
        {
            var track = new Track();
            track.Parent = parent as IPlayingEntity;
            track.Id = lcObjectsManager.GetNewCurrentId();

            SetOwnerRastersData(track);
            SetOwnerSequence(track, lcObjectsManager);

            track.RiseTime = RiseTime;
            track.FadeTime = FadeTime;

            return track;
        }

        protected override bool CanCalculateDefaultSize => true;

        protected override void UpdateRasterInChildren()
        {
            if (_raster != null)
            {
                UpdateRastersDataInEffects(_raster.RotatedWidth, _raster.RotatedHeight,
                    _raster.RotatedProjectionMapping);
            }
            else
            {
                UpdateRastersDataInEffects(1, 1, new List<LampProjection>());
            }
        }

        public override List<T> GetAllChildren<T>()
        {
            List<LCObject> result = new List<LCObject>();
            result.AddRange(Entities);
            result.AddRange(Transitions);
            return result.OfType<T>().ToList();
        }

        private void RasterSubscription()
        {
            if (_raster == null) return;

            _raster.DimensionChanged += OnRastersDataChanged;
            _raster.ProjectionChanged += OnRastersDataChanged;
            _raster.RotationChanged += OnRastersDataChanged;
        }

        private void RasterUnSubscription()
        {
            if (_raster != null)
            {
                _raster.DimensionChanged -= OnRastersDataChanged;
                _raster.ProjectionChanged -= OnRastersDataChanged;
                _raster.RotationChanged -= OnRastersDataChanged;
            }
        }

        private void OnRastersDataChanged(object sender, EventArgs e)
        {
            UpdateRastersDataInEffects(_raster.RotatedWidth, _raster.RotatedHeight, _raster.RotatedProjectionMapping);
        }

        private void UpdateRastersDataInEffects(int width, int height, List<LampProjection> map)
        {
            foreach (var effect in Entities)
            {
                effect.SetRasterParam(width, height, map);
            }
            //Entities.ForEach(a => a.SetRasterParam(width, height, map));
        }

        private void ClearRastersInEffects()
        {
            Entities.ForEach(a => a.SetRasterParam(1, 1, null));
        }
    }