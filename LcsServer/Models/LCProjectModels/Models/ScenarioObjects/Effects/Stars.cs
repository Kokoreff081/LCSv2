using LcsServer.Models.LCProjectModels.GlobalBase;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;
using LCSVersionControl;


namespace LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects;

public partial class Stars : Effect
    {
        public const int MIN_FILLING = 1;
        public const int MAX_FILLING = 100;
        public const int MIN_LIFETIME = 1;
        public const int MAX_LIFETIME = 60;

        private const int MAX_APPEAR_SPEED = 8;
        private const int MAX_FADE_SPEED = 8;
        private const int MAX_FADE = 10;

        private List<CompositeColor> _colorStack;
        private CompositeColor _background;
        private bool _isColorMix;
        private bool _modulation;
        private int _lifeTime;
        private int _filing;

        private readonly Canvas[] _params;

        public event EventHandler ColorStackChanged;
        public event EventHandler FillingChanged;
        public event EventHandler LifeTimeChanged;
        public event EventHandler ModulationChanged;
        public event EventHandler IsColorMixChanged;
        public event EventHandler BackgroundChanged;

        public Stars()
        {
            _filing = 1;
            _lifeTime = 10;

            _background = CompositeColor.BlackColor;

            _colorStack = new List<CompositeColor>()
            {
                CompositeColor.RedColor,
                CompositeColor.WhiteColor,
                CompositeColor.GreenColor,
                CompositeColor.WhiteColor,
                CompositeColor.BlueColor,
                CompositeColor.WhiteColor,
            };

            _params = new Canvas[]
            {
                new Canvas(),
                new Canvas(),
            };
        }

        public Stars(int id, string name, int parentId, CompositeColor background, List<CompositeColor> colorStack,
            int filling, bool isColorMix, int lifeTime, bool modulation, long totalTicks, long riseTime, long fadeTime
            , float dimmingLevel) : this()
        {
            Id = id;
            Name = name;
            _parentId = parentId;
            Background = background;
            ColorStack = colorStack;
            Filling = filling;
            IsColorMix = isColorMix;
            LifeTime = lifeTime;
            Modulation = modulation;
            TotalTicks = totalTicks;
            RiseTime = riseTime;
            FadeTime = fadeTime;
            DimmingLevel = dimmingLevel;
        }

        public override Category Category => Category.Particle;
        public override Enum Type => NatureTypes.Stars;

        [SaveLoad]
        public int Filling
        {
            get { return _filing; }
            set
            {
                if (_filing == value) return;

                _filing = value;
                ResetStars();
                FillingChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        [SaveLoad]
        public int LifeTime
        {
            get { return _lifeTime; }
            set
            {
                if (_lifeTime == value) return;

                _lifeTime = value;
                ResetStars();
                LifeTimeChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        [SaveLoad]
        public bool Modulation
        {
            get { return _modulation; }
            set
            {
                if (_modulation == value) return;

                _modulation = value;
                ResetStars();
                ModulationChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        [SaveLoad]
        public bool IsColorMix
        {
            get { return _isColorMix; }
            set
            {
                if (_isColorMix == value) return;

                _isColorMix = value;
                ResetStars();
                IsColorMixChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public CompositeColor Background
        {
            get { return _background; }
            set
            {
                if (_background == value) return;

                _background = value;
                BackgroundChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public List<CompositeColor> ColorStack
        {
            get { return _colorStack; }
            set
            {
                if (_colorStack == value) return;

                _colorStack = value;
                ResetStars();
                ColorStackChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        protected override Effect Clone(IPlayingEntity parent)
        {
            var effect = new Stars
            {
                Parent = parent,
                Background = Background,
                Filling = Filling,
                TotalTicks = TotalTicks,
                LifeTime = LifeTime,
                Modulation = Modulation,
                IsColorMix = IsColorMix,
                ColorStack = ColorStack.ToList()
            };

            return effect;
        }

        public override void Save(string projectFolderPath)
        {
        }

        protected override bool TryFillFrame(long ticks, EffectFrame frame, float riseOrFadeLevel, bool isDefault)
        {
            var canvas = GetCanvas(frame);

            if (canvas.ResetStars)
            {
                canvas.Reset(_filing, _modulation, _isColorMix, _colorStack);
            }

            int framesCount = _lifeTime * 1000 / LCRandom.RandomNext(20, 40); // 40ms _frameTime

            canvas.Tick(framesCount);
            
            frame.FillByColor(_background * riseOrFadeLevel);
            canvas.FillFrameByStars(frame, riseOrFadeLevel);

            return true;
        }

        private void ResetStars()
        {
            _params.ForEach(a => a.ResetStars = true);
        }

        private Canvas GetCanvas(EffectFrame frame)
        {
            var param = Array.Find(_params, a => Equals(a.Projection, frame.Projection));
            if (param != null)
            {
                return param;
            }

            _params[0] = _params[1];
            _params[1] = new Canvas(frame.Projection);
            return _params[1];
        }
    }