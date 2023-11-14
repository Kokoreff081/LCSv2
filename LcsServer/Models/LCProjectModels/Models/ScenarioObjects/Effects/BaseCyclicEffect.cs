using LcsServer.Models.LCProjectModels.GlobalBase;

namespace LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects;

public abstract class BaseCyclicEffect : Effect
    {
        private int _repeat = 1;
        private float _stepTime;
        /// <summary>
        /// Изменение количества повторений
        /// </summary>
        public event EventHandler RepeatChanged;

        /// <summary>
        /// Количество повторов
        /// </summary>
        [SaveLoad]
        public int Repeat
        {
            get => _repeat;
            set
            {
                if (_repeat == value)
                {
                    return;
                }

                _repeat = value;
                ChangeTotalTicks();
                RepeatChanged?.Invoke(this, EventArgs.Empty);
            }
        }


        private long _cyclicRiseTime;
        private long _cyclicFadeTime;

        public event EventHandler CyclicRiseTimeChanged;
        public event EventHandler CyclicFadeTimeChanged;

        public long CyclicRiseTime
        {
            get => _cyclicRiseTime;
            set
            {
                if (_cyclicRiseTime.Equals(value))
                    return;

                //float stepTime = TotalTicks / (float)Repeat;

                if (value > _stepTime)
                {
                    _cyclicRiseTime = (long)_stepTime;
                    CyclicFadeTime = 0;
                }
                else if (_stepTime < value + CyclicFadeTime)
                {
                    _cyclicRiseTime = value;
                    CyclicFadeTime = (long)(_stepTime - value);
                }
                else
                {
                    _cyclicRiseTime = value;
                }

                CyclicRiseTimeChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public long CyclicFadeTime
        {
            get => _cyclicFadeTime;
            set
            {
                if (_cyclicFadeTime.Equals(value))
                    return;

                //float stepTime = TotalTicks / (float)Repeat;

                if (value > _stepTime)
                {
                    _cyclicFadeTime = (long)_stepTime;
                    CyclicRiseTime = 0;
                }
                else if (_stepTime < value + CyclicRiseTime)
                {
                    _cyclicFadeTime = value;
                    CyclicRiseTime = (long)(_stepTime - value);
                }
                else
                {
                    _cyclicFadeTime = value;
                }

                CyclicFadeTimeChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        protected override void ChangeTotalTicks()
        {
            base.ChangeTotalTicks();

            _stepTime = TotalTicks / (float)_repeat;

            if (_stepTime < CyclicFadeTime + CyclicRiseTime)
            {
                CyclicFadeTime = 0;
                CyclicRiseTime = 0;
            }
        }

        protected float GetCycleRiseOrFadeLevel(long ticks)
        {
            //float stepTime = TotalTicks / (float)Repeat;

            float inCycleTime = ticks % _stepTime;

            float _dim = 1.0f;

            if (inCycleTime < CyclicRiseTime)
            {
                _dim = CyclicRiseTime <= 0 ? 1 : inCycleTime / CyclicRiseTime;
            }
            else if (inCycleTime > _stepTime - CyclicFadeTime)
            {
                _dim = CyclicFadeTime <= 0 ? 1 : (_stepTime - inCycleTime) / CyclicFadeTime;
            }

            return DimmingLevel * _dim;
        }
    }