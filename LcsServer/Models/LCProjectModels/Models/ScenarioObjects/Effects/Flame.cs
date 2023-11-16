using LcsServer.Models.LCProjectModels.GlobalBase;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;
using LCSVersionControl;


namespace LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects;

public partial class Flame : Effect
    {
        public const int MIN_INTENSE = 20;
        public const int MAX_INTENSE = 100;
        public const int MIN_SPEED = 1;
        public const int MAX_SPEED = 5;
        public const int MIN_STABILITY = 1;
        public const int MAX_STABILITY = 10;

        private const int MAX_HOTS = 4;
        private const int MAX_HOT_VAL = 32767;

        private int _heightPerсent;
        private int _speed;
        private int _stability;
        private bool _floating;
        private CompositeColorList _colorStack;

        private readonly Canvas[] _flameParam;

        public event EventHandler HeightChanged;
        public event EventHandler SpeedChanged;
        public event EventHandler StabilityChanged;
        public event EventHandler ColorStackChanged;
        public event EventHandler FloatingChanged;

        public Flame()
        {
            _heightPerсent = MIN_INTENSE;
            _speed = MIN_SPEED;
            _stability = MIN_STABILITY;

            _colorStack = new CompositeColorList(new[]{CompositeColor.BlackColor,
                                                      CompositeColor.BlueColor,
                                                      CompositeColor.RedColor,
                                                      CompositeColor.YellowColor,
                                                      CompositeColor.WhiteColor });

            _flameParam = new Canvas[2];
        }

        public Flame(int id, string name, int parentId, CompositeColorList colorStack,
            bool floating, int height, int speed, int stability, long totalTicks, long riseTime, long fadeTime, float dimmingLevel) : this()
        {
            Id = id;
            Name = name;
            _parentId = parentId;
            ColorStack = colorStack;
            Floating = floating;
            Height = height;
            Speed = speed;
            Stability = stability;
            TotalTicks = totalTicks;
            RiseTime = riseTime;
            FadeTime = fadeTime;
            DimmingLevel = dimmingLevel;
        }

        public override Category Category => Category.Nature;
        public override Enum Type => NatureTypes.Flame;

        [SaveLoad]
        public int Height
        {
            get { return _heightPerсent; }
            set
            {
                if (_heightPerсent == value) return;

                _heightPerсent = value;
                HeightChanged?.Invoke(this, EventArgs.Empty);
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

        [SaveLoad]
        public int Stability
        {
            get { return _stability; }
            set
            {
                if (_stability == value) return;

                _stability = value;
                StabilityChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        [SaveLoad]
        public bool Floating
        {
            get { return _floating; }
            set
            {
                if (_floating == value) return;

                _floating = value;
                FloatingChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public CompositeColorList ColorStack
        {
            get { return _colorStack; }
            set
            {
                if (_colorStack == value) return;

                _colorStack = value;
                ColorStackChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        protected override Effect Clone(IPlayingEntity parent)
        {
            var effect = new Flame
            {
                Parent = parent,
                Height = Height,
                TotalTicks = TotalTicks,
                Speed = Speed,
                Stability = Stability,
                Floating = Floating,
                ColorStack = ColorStack.Clone()
            };


            return effect;
        }

        public override void Save(string projectFolderPath)
        {
        }

        protected override bool TryFillFrame(long ticks, EffectFrame frame, float riseOrFadeLevel, bool isDefault)
        {
            Canvas param = GetFlameParam(frame.Width, frame.Height);

            if (_floating)
            {
                int d = LCRandom.RandomNext(50);
                if (d < 3)
                {
                    d -= 1;      //т.е. -1, 0, 1
                    param.Intense += d * _heightPerсent / 10;
                    param.Intense = param.Intense.Clamped(MIN_INTENSE, _heightPerсent);
                }
            }
            else
            {
                param.Intense = _heightPerсent;
            }

            int shift = _speed;
            if (shift > param.Height)
            {
                shift = param.Height - 1;
            }

            int fade = MAX_HOT_VAL * 100 / param.Height / param.Intense;
            fade = fade * shift / 2;  //уменьшаем на 25%, чтобы при макс интенсивности достигать
                                          //почти полной высоты с учетом черного вверху

            //shift up
            for (int i = param.Height - 1; i > shift; i--)
            {
                Buffer.BlockCopy(param.Flame1[i - shift], 0, param.Flame1[i], 0, param.Width * sizeof(int));
            }

            for (int n = 0; n < shift; n++)
            {
                param.RecalculateHottestSpots(_stability);

                for (int i = 0; i < param.Width; i++)
                {
                    int result = param.BotLine[i];
                    int count = 1;
                    if (i > 0)
                    {
                        result += param.BotLine[i - 1];
                        count++;
                    }

                    if (i < param.Width - 1)
                    {
                        result += param.BotLine[i + 1];
                        count++;
                    }

                    param.Flame1[n][i] = result / count;
                }
            }

            //усредняем flame1 с помещением результата во flame2
            for (int i = 0; i < param.Height; i++)
            {
                for (int j = 0; j < param.Width; j++)
                {
                    int result = param.Flame1[i][j];
                    int count = 1;
                    if (j > 0)
                    {
                        result += param.Flame1[i][j - 1];
                        count++;
                    }

                    if (j < param.Width - 1)
                    {
                        result += param.Flame1[i][j + 1];
                        count++;
                    }

                    if (j > 0 && i > 0)
                    {
                        result += param.Flame1[i - 1][j - 1];
                        count++;
                    }

                    if (i > 0)
                    {
                        result += param.Flame1[i - 1][j];
                        count++;
                    }

                    if (i > 0 && j < param.Width - 1)
                    {
                        result += param.Flame1[i - 1][j + 1];
                        count++;
                    }

                    if (i < param.Height - 1 && j > 0 )
                    {
                        result += param.Flame1[i + 1][j - 1];
                        count++;
                    }

                    if (i < param.Height - 1)
                    {
                        result += param.Flame1[i + 1][j];
                        count++;
                    }

                    if (i < param.Height - 1 && j < param.Width - 1)
                    {
                        result += param.Flame1[i + 1][j + 1];
                        count++;
                    }

                    param.Flame2[i][j] = result / count;
                }
            }

            //переносим в flame1 и выводим
            for (int i = 0; i < param.Height; i++)
            {
                for (int j = 0; j < param.Width; j++)
                {
                    int val = param.Flame2[i][j];
                    if (val > fade)
                    {
                        val -= fade;
                        param.Flame1[i][j] = val;
                    }
                }
            }

            foreach (var lp in frame.Projection)
            {
                foreach (var point in lp.Points)
                {
                    var i = param.Width - 1 - point.X;
                    var j = param.Height - 1 - point.Y;

                    int val = param.Flame2[j][i];
                    if (val > fade)
                        val -= fade;

                    var color = _colorStack.Pallete[val >> 7];
                    frame.Colors[point] = color * riseOrFadeLevel;
                }
            }

            return true;
        }

        private Canvas GetFlameParam(int width, int height)
        {
            if (_flameParam.Any(a => a.Width == width && a.Height == height))
            {
                return _flameParam.First(a => a.Width == width && a.Height == height);
            }

            var index = _flameParam.ToList().FindIndex(a => a.Width == 0 && a.Height == 0);
            if (index > 0)
            {
                _flameParam[index] = new Canvas(width, height, _heightPerсent);
                return _flameParam[index];
            }

            _flameParam[0] = _flameParam[1];
            _flameParam[1] = new Canvas(width, height, _heightPerсent);
            return _flameParam[1];
        }
    }