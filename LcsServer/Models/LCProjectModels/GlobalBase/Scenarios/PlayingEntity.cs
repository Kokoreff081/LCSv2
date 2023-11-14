using LCSVersionControl;
using LCSVersionControl.Interfaces;

namespace LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;

public abstract class PlayingEntity : LCObject, IPlayingEntity
    {
        private IPlayingEntity _parent;

        private long _riseTime;
        private long _fadeTime;
        private float _dimmingLevel = 1.0f;
        
        public event EventHandler RiseTimeChanged;
        public event EventHandler FadeTimeChanged;
        public event EventHandler DimmingLevelChanged;


        public float DimmingLevel
        {
            get => _dimmingLevel;
            set
            {
                if (_dimmingLevel.NearEqual(value))
                {
                    return;
                }

                _dimmingLevel = value;
                DimmingLevelChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public long RiseTime
        {
            get => _riseTime;
            set
            {
                if (_riseTime.Equals(value))
                {
                    return;
                }

                if (value > TotalTicks)
                {
                    _riseTime = TotalTicks;
                    FadeTime = 0;
                }
                else if (TotalTicks < value + FadeTime)
                {
                    _riseTime = value;
                    FadeTime = TotalTicks - value;
                }
                else
                {
                    _riseTime = value;
                }

                RiseTimeChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public long FadeTime
        {
            get => _fadeTime;
            set
            {
                if (_fadeTime.Equals(value))
                {
                    return;
                }

                if (value > TotalTicks)
                {
                    _fadeTime = TotalTicks;
                    RiseTime = 0;
                }
                else if (TotalTicks < value + RiseTime)
                {
                    _fadeTime = value;
                    RiseTime = TotalTicks - value;
                }
                else
                {
                    _fadeTime = value;
                }

                FadeTimeChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        [SaveLoad]
        protected int _parentId; 

        /// <summary>
        /// Изменения общей длительности
        /// </summary>
        public event EventHandler TotalTicksChanged;

        /// <summary>
        /// Изменение родителя
        /// </summary>
        public event EventHandler ParentIdChanged;

        protected PlayingEntity()
        {

        }

        protected PlayingEntity(int id, string name, long riseTime, long fadeTime, float dimmingLevel)
        {
            Id = id;
            Name = name;
            _riseTime = riseTime;
            _fadeTime = fadeTime;
            _dimmingLevel = dimmingLevel;
        }

        /// <summary>
        /// Родитель
        /// </summary>
        public IPlayingEntity Parent
        {
            get => _parent;
            set
            {
                if (_parent == value)
                {
                    return;
                }

                _parent = value;
                ParentIdChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Id родителя
        /// </summary>
        [SaveLoad]
        public override int ParentId => Parent?.Id ?? 0;

        /// <summary>
        /// Длительность
        /// </summary>
        [SaveLoad]
        public long TotalTicks => GetTotalTicks();

        /// <summary>
        /// Попытка получить кадр
        /// </summary>
        /// <param name="tick"> Тик </param>
        /// <param name="frame"> Кадр </param>
        /// <param name="calculateDefaultSize"> Рассчитать изображение по умолчанию </param>
        /// <param name="riseOrFadeLevel">Уровень затухания или розжига</param>
        /// <returns> Флаг, удалось ли вычислить кадр </returns>
        public abstract bool TryGetFrame(long tick, out ScenarioFrame frame, bool calculateDefaultSize, float riseOrFadeLevel);

        //public abstract bool GetEmptyFrame(long tick, out ScenarioFrame frame);

        protected abstract long GetTotalTicks();

        public void TotalTicksNotify()
        {
            if (TotalTicks > 0 && TotalTicks < FadeTime + RiseTime)
            {
                FadeTime = TotalTicks * FadeTime / (FadeTime + RiseTime);
                RiseTime = TotalTicks * RiseTime / (FadeTime + RiseTime);
            }
            TotalTicksChanged?.Invoke(this, EventArgs.Empty);
        }
        
        protected float GetLevelByRiseAndFade(long ticks)
        {
            float level = 1.0f;
            if (RiseTime > ticks)
            {
                level = (float)ticks / RiseTime;
            }
            if (TotalTicks - FadeTime < ticks)
            {
                level = (float)(TotalTicks - ticks) / FadeTime;
            }

            return level * _dimmingLevel;
        }

        protected void LoadParent(IEnumerable<LCObject> primitives)
        {
            if (_parentId != 0)
            {
                Parent = primitives.FirstOrDefault(p => p.Id == _parentId) as IPlayingEntity;
            }
        }
    }