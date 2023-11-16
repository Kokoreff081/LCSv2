using LcsServer.Models.LCProjectModels.GlobalBase;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;
using LcsServer.Models.LCProjectModels.Models.Rasters;
using LCSVersionControl;


namespace LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects;

public class Flash : BaseCyclicEffect
    {
        private bool _isRandomColor;
        private CompositeColor _compColor;
        private CompositeColor _backgroundColor;
        private CompositeColor[] _flashColors;

        public event EventHandler IsRandomColorChanged;
        public event EventHandler CompColorChanged;
        public event EventHandler BackgroundColorChanged;

        public Flash()
        {
            _compColor = CompositeColor.RedColor;
            _backgroundColor = CompositeColor.BlackColor;

            float stepTime = TotalTicks / (float)Repeat;
            CyclicFadeTime = (long) (stepTime * 0.1); // Затухание - 10% 
            CyclicRiseTime = (long) (stepTime * 0.8f); // Возгорание - 80%
            //_onTime = StepTime - FadeTime - RiseTime; // Период максимальной яркости
            UpdateFlashColors();
        }

        public Flash(int id, string name, int parentId, CompositeColor background, CompositeColor compColor,
            long fadeTime, bool isRandomColor, int repeat, long riseTime, long totalTicks, long cycleRiseTime, long cycleFadeTime, float dimmingLevel) : this()
        {
            Id = id;
            Name = name;
            _parentId = parentId;
            BackgroundColor = background;
            CompColor = compColor;
            IsRandomColor = isRandomColor;
            Repeat = repeat;
            TotalTicks = totalTicks;
            FadeTime = fadeTime;
            RiseTime = riseTime;
            CyclicFadeTime = cycleFadeTime;
            CyclicRiseTime = cycleRiseTime;
            DimmingLevel = dimmingLevel;
        }

        public override Category Category => Category.Filling;

        public override Enum Type => FillingTypes.Flash;

        [SaveLoad]
        public bool IsRandomColor
        {
            get { return _isRandomColor; }
            set
            {
                if (_isRandomColor == value) return;

                _isRandomColor = value;
                UpdateFlashColors();
                IsRandomColorChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public CompositeColor CompColor
        {
            get { return _compColor; }
            set
            {
                if (_compColor == value) return;

                _compColor = value;
                UpdateFlashColors();
                CompColorChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public CompositeColor BackgroundColor
        {
            get { return _backgroundColor; }
            set
            {
                if (_backgroundColor == value)
                    return;

                _backgroundColor = value;
                BackgroundColorChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        protected override Effect Clone(IPlayingEntity parent)
        {
            Flash cloneFlash = new Flash
            {
                Parent = parent,
                RiseTime = RiseTime,
                FadeTime = FadeTime,
                TotalTicks = TotalTicks,
                _compColor = _compColor,
                _isRandomColor = _isRandomColor,
                Repeat = Repeat,
                Name = Name,
                _flashColors = _flashColors,
                _backgroundColor = BackgroundColor,
                CyclicFadeTime = CyclicFadeTime,
                CyclicRiseTime = CyclicRiseTime,
            };

            return cloneFlash;
        }

        public override void Save(string projectFolderPath)
        {
        }

        protected override void ChangeTotalTicks()
        {
            base.ChangeTotalTicks();
            UpdateFlashColors();
        }

        private void UpdateFlashColors()
        {
            _flashColors = new CompositeColor[Repeat];

            if (_isRandomColor)
            {
                for (int i = 0; i < Repeat; i++)
                {
                    _flashColors[i] = new CompositeColor(LCRandom.RandomNextByte(), LCRandom.RandomNextByte(), LCRandom.RandomNextByte());
                }
            }
            else
            {
                for (int i = 0; i < Repeat; i++)
                {
                    _flashColors[i] = _compColor;
                }
            }
        }

        protected override bool TryFillFrame(long ticks, EffectFrame frame, float riseOrFadeLevel, bool isDefault)
        {
            CompositeColor color = GetColor(ticks, riseOrFadeLevel);

            foreach (LampProjection lp in frame.Projection)
            {
                foreach (IntPoint point in lp.Points)
                {
                    frame.Colors[point] = color;
                }
            }

            return true;
        }

        private CompositeColor GetColor(long ticks, float riseOrFadeLevel)
        {
            float cyclicRiseFadeLevel = GetCycleRiseOrFadeLevel(ticks);

            float stepTime = TotalTicks / (float)Repeat;

            int cycle = (int)Math.Floor(ticks / stepTime);
            cycle = Math.Min(cycle, _flashColors.Length - 1);

            CompositeColor color = _flashColors[cycle];

            float level2 = 1f - cyclicRiseFadeLevel;

            byte r = (byte)((color.Red * cyclicRiseFadeLevel + _backgroundColor.Red * level2) * riseOrFadeLevel);
            byte g = (byte)((color.Green * cyclicRiseFadeLevel+ _backgroundColor.Green * level2) * riseOrFadeLevel);
            byte b = (byte)((color.Blue * cyclicRiseFadeLevel + _backgroundColor.Blue * level2) * riseOrFadeLevel);

            byte a = (byte)((color.Amber * cyclicRiseFadeLevel + _backgroundColor.Amber * level2) * riseOrFadeLevel);
            float cct = (color.CCT * cyclicRiseFadeLevel + _backgroundColor.CCT * level2)  * riseOrFadeLevel;
            byte w = (byte)((color.White * cyclicRiseFadeLevel + _backgroundColor.White * level2) * riseOrFadeLevel);

            return new CompositeColor(r, g, b, a, cct, w);
        }

    }