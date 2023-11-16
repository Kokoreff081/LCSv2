using System.Diagnostics;
using LcsServer.Models.LCProjectModels.GlobalBase;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;
using LcsServer.Models.LCProjectModels.Models.Rasters;
using NLog;

namespace LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects;

public abstract class Effect : PlayingEntity, ISaveLoad, IClone
    {
        /// <summary>
        /// Категория по-умолчанию
        /// </summary>
        public const Category DefaultCategory = Category.Filling;

        /// <summary>
        /// Тип эффекта по-умолчанию
        /// </summary>
        public const FillingTypes DefaultType = FillingTypes.Static;

        private long _totalTicks;

        //protected int _quant;
        //private long _lastTick;

        private volatile bool _isBusy;

        private List<LampProjection> _rasterProjection;
        private int _width = 1;
        private int _height = 1;

        private static readonly List<LampProjection> _defaultProjection;
        //private static readonly IntSize _defaultSize;

        static Effect()
        {
            var pr = new LampProjection(0, 1, 1);

            for (int i = 0; i < Helper.DefaultWidth; i++)
            {
                for (int j = 0; j < Helper.DefaultHeight; j++)
                {
                    pr.AddPoint(new IntPoint(i, j));
                }
            }
            _defaultProjection = new List<LampProjection> { pr };
        }

        protected Effect()
        {
            _rasterProjection = new List<LampProjection>();
            //_rasterSize = new IntSize(1, 1);

            _totalTicks = Helper.DefaultEffectDuration;
            //_frameTime = 40;
        }

        /// <summary>
        /// Категория эффекта
        /// </summary>
        public abstract Category Category { get; }

        /// <summary>
        /// Тип эффекта
        /// </summary>
        public abstract Enum Type { get; }

        /// <summary>
        /// Количество тиков
        /// </summary>
        [SaveLoad]
        public new long TotalTicks
        {
            get => base.TotalTicks;
            set
            {
                if (_totalTicks == value) return;

                _totalTicks = value;
                ChangeTotalTicks();
                TotalTicksNotify();
            }
        }

        public Raster GetRaster()
        {
            IPlayingEntity entity = this;
            while (entity.Parent != null)
            {
                if (entity.Parent is PlayingEntityWithRaster {UseParentRaster: false} playingEntityWithRaster)
                {
                    return playingEntityWithRaster.Raster;
                }
                entity = entity.Parent;
            }

            return null;
        }

        /// <summary>
        /// Задать параметры растра
        /// </summary>
        /// <param name="width"> Ширина растра </param>
        /// <param name="height"> Высота растра </param>
        /// <param name="rastersMapping"> Проекция </param>
        public void SetRasterParam(int width, int height, List<LampProjection> rastersMapping)
        {
            if (rastersMapping is null)
            {
                _height = 0;
                _width = 0;
                _rasterProjection = EmptyList;
            }
            else
            {
                _width = width; 
                _height = height;
                _rasterProjection = rastersMapping;
            }
        }

        //private static readonly IntSize EmptySize = new IntSize(0, 0);
        private static readonly List<LampProjection> EmptyList = new List<LampProjection>();

        public override bool TryGetFrame(long tick, out ScenarioFrame frame, bool calculateDefaultSize, float riseOrFadeLevel)
        {
            frame = null;
            if (_isBusy)
            {
                return false;
            }

            if (tick > TotalTicks || tick < 0)
            {
                frame = GetDefaultFrame();
                return true;
            }

            if (tick == TotalTicks)
            {
                Debug.WriteLine("tick == TotalTicks!!!");
            }

            //FixateQuant(tick);

            var rasterFrame = new EffectFrame(_width, _height, _rasterProjection);

            float totalRiseOrFadeLevel = riseOrFadeLevel * GetLevelByRiseAndFade(tick);

            var fillRaster = FillFrame(tick, rasterFrame, totalRiseOrFadeLevel, false);

            if (!fillRaster) return false;

            if (calculateDefaultSize)
            {
                var defaultFrame = new EffectFrame(Helper.DefaultWidth, Helper.DefaultHeight, _defaultProjection);
                FillFrame(tick, defaultFrame, totalRiseOrFadeLevel, true);
                frame = new ScenarioFrame(rasterFrame, defaultFrame);
            }
            else
            {
                frame = new ScenarioFrame(rasterFrame);
            }
            return true;
        }

        // public override bool GetEmptyFrame(long tick, out ScenarioFrame frame)
        // {
        //     frame = null;
        //     if (_isBusy)
        //         return false;
        //
        //     frame = GetDefaultFrame();
        //     return true;
        // }

        private ScenarioFrame GetDefaultFrame()
        {
            var rasterFrame = new EffectFrame(_width, _height, _rasterProjection);
            return new ScenarioFrame(rasterFrame);
        }

        /// <summary>
        /// Клонирование
        /// </summary>
        /// <param name="lcObjectsManager">Ссылка на менеджер объектов LCObjectsManager (для присвоения ID)</param>
        /// <param name="parent"> Родитель </param>
        /// <returns> Клон </returns>
        public LCObject Clone(BaseLCObjectsManager lcObjectsManager, LCObject parent)
        {
            var effect = Clone(parent as IPlayingEntity);
            effect.Id = lcObjectsManager.GetNewCurrentId();
            effect.RiseTime = RiseTime;
            effect.FadeTime = FadeTime;
            effect.DimmingLevel = DimmingLevel;

            return effect;
        }

        /// <summary>
        /// Вызывается при сохранении проекта
        /// </summary>
        /// <param name="projectFolderPath">Путь к папке проекта</param>
        public virtual void Save(string projectFolderPath) { }

        /// <summary>
        /// Вызывается при загрузке проекта
        /// </summary>
        /// <param name="primitives">Все объекты проекта</param>
        /// <param name="indexInPrimitives">Индекс объекта в списке всех примитивов. Для ускоренного поиска родителя</param>
        /// <param name="projectFolderPath">Путь к папке проекта</param>
        public virtual void Load(List<ISaveLoad> primitives, int indexInPrimitives, string projectFolderPath)
        {
            LoadParent(primitives.OfType<LCObject>());
        }

        protected abstract bool TryFillFrame(long ticks, EffectFrame frame, float riseOrFadeLevel, bool isDefault);

        protected abstract Effect Clone(IPlayingEntity parent);

        protected virtual void ChangeTotalTicks()
        {
        }

        protected override long GetTotalTicks()
        {
            return _totalTicks;
        }

        //public bool Active => (DateTime.Now - _lastFrameTime).Milliseconds < 100;
        
        //private DateTime _lastFrameTime = DateTime.Now;
        private bool FillFrame(long tick, EffectFrame frame, float riseOrFadeLevel, bool isDefault)
        {
            if (_isBusy || frame.Width == 0 || frame.Height == 0)
            {
                return false;
            }

            //_lastFrameTime = DateTime.Now;
            bool result = false;
            _isBusy = true;
            try
            {
                result = TryFillFrame(tick, frame, riseOrFadeLevel, isDefault);
            }
            catch (Exception e)
            {
                LogManager.GetCurrentClassLogger().Error($"{e.Message}\n{e.StackTrace}");
            }
            finally
            {
                _isBusy = false;
            }
            return result;
        }

        // private void FixateQuant(long ticks)
        // {
        //     _quant = (int)(ticks - _lastTick);
        //     if (_quant < 0)
        //     {
        //         _quant = 0;
        //     }
        //     _lastTick = ticks;
        // }

    }