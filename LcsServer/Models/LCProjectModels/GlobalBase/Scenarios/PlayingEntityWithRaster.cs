using LcsServer.Models.LCProjectModels.Models.Rasters;

namespace LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;

public abstract class PlayingEntityWithRaster : PlayingEntity
    {
        [SaveLoad] protected int _saveRasterId;
        private bool _useParentRaster;

        protected Raster _raster;

        /// <summary>
        /// Изменени флага Использовать родительский растр
        /// </summary>
        public event EventHandler UseParentRasterChanged;

        /// <summary>
        /// Изменение растра
        /// </summary>
        public event EventHandler RasterChanged;

        protected PlayingEntityWithRaster() { }

        protected PlayingEntityWithRaster(int id, string name, long riseTime, long fadeTime, float dimmingLevel) : base(
            id, name, riseTime, fadeTime, dimmingLevel)
        {
        }

        /// <summary>
        /// Флаг Использовать родительский растр
        /// </summary>
        [SaveLoad]
        public virtual bool UseParentRaster
        {
            get => _useParentRaster;
            set
            {
                if (_useParentRaster == value) return;

                _useParentRaster = value;
                UseParentRasterChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Растр
        /// </summary>
        public virtual Raster Raster
        {
            get => _raster;
            set
            {
                if (_raster != value)
                {
                    _raster = value;
                    UpdateRasterInChildren();
                    NotifyAboutRasterChanged();
                }
            }
        }

        public bool IsEnabled { get; set; } = true;


        /// <summary>
        /// Hack!!! Так как сценарии всегда меняют свой id при загрузке (файлы сценарии можно копировать в разные проекты) поэтому тут храним id который был до загрузки файла
        /// </summary>
        public int SavedId { get; protected set; }

        /// <summary>
        /// Получить всех детей
        /// </summary>
        /// <typeparam name="T"> Тип детей </typeparam>
        /// <returns> Коллекция детей </returns>
        public override List<T> GetAllChildren<T>()
        {
            List<LCObject> result = new List<LCObject>();
            foreach (var child in ChildrenWithRaster)
            {
                result.AddRange(child.GetAllLCObjects<LCObject>());
            }

            return result.OfType<T>().ToList();
        }

        protected abstract IEnumerable<PlayingEntityWithRaster> ChildrenWithRaster { get; }

        protected void NotifyAboutRasterChanged()
        {
            RasterChanged?.Invoke(this, EventArgs.Empty);
        }

        protected void SaveRaster()
        {
            _saveRasterId = _raster?.Id ?? -1;
        }

        public void LoadRaster(IEnumerable<LCObject> primitives)
        {
            if (_saveRasterId <= 0) return;

            Raster = primitives.FirstOrDefault(a => a.Id == _saveRasterId) as Raster;
            UpdateRasterInChildren();
        }

        protected virtual void UpdateRasterInChildren()
        {
            ChildrenWithRaster.Where(a => a.UseParentRaster).ForEach(a => a.Raster = _raster);
        }

        protected void SetOwnerRastersData(PlayingEntityWithRaster entity)
        {
            entity.UseParentRaster = UseParentRaster;
            entity.Raster = Raster;
        }
    }