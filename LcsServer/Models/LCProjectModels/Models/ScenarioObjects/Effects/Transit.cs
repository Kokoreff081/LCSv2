using LcsServer.Models.LCProjectModels.GlobalBase;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;


namespace LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects;

public class Transit : Effect
    {
        private List<CompositeColor> _colorStack;
        private int _speed;
        private int _repeat;

        public event EventHandler ColorStackChanged;
        public event EventHandler SpeedChanged;
        public event EventHandler RepeatChanged;

        public Transit()
        {
            _speed = 1;
            _repeat = 1;
            _colorStack = new List<CompositeColor> {CompositeColor.RedColor, CompositeColor.BlueColor};
        }

        public Transit(int id, string name, int parentId, int repeat, List<CompositeColor> colorStack, int speed,
            long totalTicks, long riseTime, long fadeTime, float dimmingLevel)
        {
            Id = id;
            Name = name;
            _parentId = parentId;
            _repeat = 1;
            _colorStack = colorStack;
            _speed = speed * repeat;
            TotalTicks = totalTicks;
            RiseTime = riseTime;
            FadeTime = fadeTime;
            DimmingLevel = dimmingLevel;
        }

        public override Category Category => Category.Filling;
        public override Enum Type => FillingTypes.Transition;

        [SaveLoad]
        public int Repeat
        {
            get { return _repeat; }
            set
            {
                if (_repeat == value) return;

                _repeat = Math.Max(value, 1);
                RepeatChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public List<CompositeColor> ColorStack
        {
            get { return _colorStack; }
            set
            {
                if (_colorStack == value) return;

                _colorStack = value;
                ColorStackChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        [SaveLoad]
        public int Speed
        {
            get { return _speed; }
            set
            {
                if (_speed == value) return;

                _speed = value;
                SpeedChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public override void Load(List<ISaveLoad> primitives, int indexOfPrimitives, string projectFolderPath)
        {
            base.Load(primitives, indexOfPrimitives, projectFolderPath);
        }

        protected override Effect Clone(IPlayingEntity parent)
        {
            var clone = new Transit {
                Name = Name,
                Parent = parent,
                Speed = Speed,
                Repeat = Repeat,
                TotalTicks = TotalTicks,
                ColorStack = ColorStack.ToList()
            };


            return clone;
        }

        protected override bool TryFillFrame(long ticks, EffectFrame frame, float riseOrFadeLevel, bool isDefault)
        {
            var color = GetColor(ticks);

            foreach (var lp in frame.Projection)
            {
                foreach (var point in lp.Points)
                {
                    frame.Colors[point] = color * riseOrFadeLevel;
                }
            }

            return true;
        }

        private CompositeColor GetColor(long ticks)
        {
            int numofcolors = ColorStack.Count;
            switch (numofcolors)
            {
                case 0:
                    return CompositeColor.BlackColor;  //цветов нет
                case 1:
                    return ColorStack[0];              //один цвет
            }

            int periods = numofcolors * _speed - 1;

            long stepcolor = TotalTicks / periods;
            int colorIndexStart = (int)(ticks / stepcolor) % numofcolors;
            int colorIndexEnd = colorIndexStart + 1;
            if (colorIndexEnd > (numofcolors - 1))
            {
                colorIndexEnd = 0; //виртуальный +1 цвет для плавного перехода
            }

            long cnum1 = ticks % stepcolor;
            CompositeColor color = CompositeColor.Lerp(ColorStack[colorIndexStart], ColorStack[colorIndexEnd], (float)cnum1 / stepcolor);
            return color;
        }
    }