using LcsServer.Models.LCProjectModels.GlobalBase;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;
using LCSVersionControl;


namespace LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects;

/// <summary>
    /// Класс, описывающий градиент эффект
    /// </summary>
    public class Gradient : Effect
    {
        private FillStyles _fillStyle;
        private InOutDirection _direction;
        private Rotation _rotation;
        
        private List<CompositeColor> _colorsPalette;
        private CompositeColor[] _colors;
        
        private int _angle;
        private bool _isRandomAngle;
        private int _speed;
        private float _scale;

        private int _compositeAngle;
        private bool _isInside;
        private bool _isClockwise;

        /// <summary>
        /// Изменение типа заполнения
        /// </summary>
        public event EventHandler FillStyleChanged;

        /// <summary>
        /// Изменение направления
        /// </summary>
        public event EventHandler DirectionChanged;

        /// <summary>
        /// Изменение вращения
        /// </summary>
        public event EventHandler RotationChanged;

        /// <summary>
        /// Изменение угла поворота
        /// </summary>
        public event EventHandler AngleChanged;

        /// <summary>
        /// Изменение флага Случайный угол
        /// </summary>
        public event EventHandler IsRandomAngleChanged;

        /// <summary>
        /// Изменение цветов заполнения
        /// </summary>
        public event EventHandler ColorStackChanged;

        /// <summary>
        /// Изменение скорости заполнения
        /// </summary>
        public event EventHandler SpeedChanged;
        
        /// <summary>
        /// Изменение масштаба
        /// </summary>
        public event EventHandler ScaleChanged;
        
        //private const int GradientSize = 256 * 6;

        public Gradient()
        {
            _speed = 3;
            _scale = 1;
            _colorsPalette = new List<CompositeColor> { CompositeColor.RedColor, CompositeColor.GreenColor, CompositeColor.BlueColor };
            CalculateGradient();
            UpdateIsInside();
            UpdateIsClockwise();
            UpdateCompositeAngle();
        }

        public Gradient(int id, string name, int parentId, int angle, List<CompositeColor> colorsPalette,
            InOutDirection direction,
            long totalTicks, FillStyles fillStyle, bool isRandomAngle, Rotation rotation, int speed, float scale,
            long riseTime, long fadeTime, float dimmingLevel) : this()
        {
            Id = id;
            Name = name;
            _parentId = parentId;
            Angle = angle;
            ColorsPalette = colorsPalette;
            Direction = direction;
            TotalTicks = totalTicks;
            FillStyle = fillStyle;
            IsRandomAngle = isRandomAngle;
            Rotation = rotation;
            Speed = speed;
            Scale = scale;
            RiseTime = riseTime;
            FadeTime = fadeTime;
            DimmingLevel = dimmingLevel;
        }

        /// <summary>
        /// Категория
        /// </summary>
        public override Category Category => Category.Filling;

        /// <summary>
        /// Тип
        /// </summary>
        public override Enum Type => FillingTypes.Gradient;

        /// <summary>
        /// Тип заливки
        /// </summary>
        public FillStyles FillStyle
        {
            get => _fillStyle;
            set
            {
                if (_fillStyle == value) return;

                _fillStyle = value;
                UpdateCompositeAngle();
                FillStyleChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Направление
        /// </summary>
        public InOutDirection Direction
        {
            get => _direction;
            set
            {
                if (_direction == value) return;

                _direction = value;
                UpdateIsInside();
                DirectionChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Ориентация вращения
        /// </summary>
        public Rotation Rotation
        {
            get => _rotation;
            set
            {
                if (_rotation == value) return;

                _rotation = value;
                UpdateIsClockwise();
                RotationChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Набор цветов
        /// </summary>
        public List<CompositeColor> ColorsPalette
        {
            get => _colorsPalette;
            set
            {
                if (_colorsPalette == value) return;

                _colorsPalette = value;
                CalculateGradient();
                ColorStackChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Угол
        /// </summary>
        [SaveLoad]
        public int Angle
        {
            get => _angle;
            set
            {
                if (_angle == value) return;

                _angle = value;
                UpdateCompositeAngle();
                AngleChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Флаг Случайный угол
        /// </summary>
        [SaveLoad]
        public bool IsRandomAngle
        {
            get => _isRandomAngle;
            set
            {
                if (_isRandomAngle == value) return;

                _isRandomAngle = value;
                UpdateCompositeAngle();
                IsRandomAngleChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Скорость
        /// </summary>
        [SaveLoad]
        public int Speed
        {
            get => _speed;
            set
            {
                if (_speed == value) return;
                _speed = value;
                CalculateGradient();
                SpeedChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        
        public float Scale
        {
            get => _scale;
            set
            {
                if (_scale.NearEqual(value)) return;
                _scale = value;
                CalculateGradient();
                ScaleChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Вызывается при сохранении проекта
        /// </summary>
        /// <param name="projectFolderPath">Путь к папке проекта</param>
        public override void Save(string projectFolderPath)
        {
          
        }

        protected override Effect Clone(IPlayingEntity parent)
        {
            var clone = new Gradient
            {
                Parent = parent,
                _fillStyle = _fillStyle,
                _direction = _direction,
                _rotation = _rotation,
                _colorsPalette = _colorsPalette.ToList(),
                _angle = _angle,
                _isRandomAngle = _isRandomAngle,
                _speed = _speed,
                Name = Name,
                TotalTicks = TotalTicks,
                _colors = _colors,
                _compositeAngle = _compositeAngle,
                _isInside = _isInside,
                _isClockwise = _isClockwise,
                _scale = _scale
            };
            
            return clone;
        }

        protected override bool TryFillFrame(long ticks, EffectFrame frame, float riseOrFadeLevel, bool isDefault)
        {
            float percent = ticks * _speed / (float)TotalTicks;
            if (percent is < 0f or float.NaN)
            {
                throw new ArgumentException(null, nameof(ticks));
            }

            // while (percent > 1.0f)
            // {
            //     percent -= 1.0f;
            // }
            
            switch (_fillStyle)
            {
                case FillStyles.OneSide:
                    OneSideGradient(frame, percent, riseOrFadeLevel);
                    break;
                case FillStyles.TwoSides:
                    TwoSideGradient(frame, percent, riseOrFadeLevel);
                    break;
                case FillStyles.Circle:
                    CircleGradient(frame, percent, riseOrFadeLevel);
                    break;
                case FillStyles.Rect:
                    RectGradient(frame, percent, riseOrFadeLevel);
                    break;
                case FillStyles.Rhombus:
                    RhombusGradient(frame, percent, riseOrFadeLevel);
                    break;
                case FillStyles.Fan:
                    FanGradient(frame, percent, riseOrFadeLevel);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return true;
        }
        
        private void UpdateIsClockwise()
        {
            _isClockwise = _rotation == Rotation.Clockwise || (_rotation == Rotation.Random && LCRandom.RandomNext(2) < 1);
        }

        private void UpdateIsInside()
        {
            _isInside = _direction == InOutDirection.Inside || (_direction == InOutDirection.Random && LCRandom.RandomNext(2) < 1);
        }

        private void UpdateCompositeAngle()
        {
            _compositeAngle = _isRandomAngle ? LCRandom.RandomNext(360) : _angle + 180;
            if (_compositeAngle >= 360)
            {
                _compositeAngle %= 360;
            }
        }
        
        private void OneSideGradient(EffectFrame frame, float percent, float level) 
        {
            var x2 = frame.Width;
            var y2 = frame.Height;
            
            if (!_isInside)
            {
                percent = 1 - percent;
            }

            float locAngle = _compositeAngle;

            locAngle = (locAngle + 90) % 360;
            
            var cosTheta = MathF.Cos(-locAngle.ToRadians());
            var sinTheta = MathF.Sin(-locAngle.ToRadians());
            var centerx = x2 / 2;
            var centery = y2 / 2;

            var tmp = locAngle switch {
                >= 90 and < 180  =>  cosTheta * x2 + sinTheta * y2 - 1,
                >= 180 and < 270 =>  cosTheta * x2 - sinTheta * y2,
                >= 270 and < 360 => -cosTheta * x2 - sinTheta * y2,
                >= 0 and < 90    => -cosTheta * x2 + sinTheta * y2 - 1,
                _ => 1f
            };

            float from = tmp / 2;
            float to = - tmp / 2;

            float cb = from + percent * (to - from);

            foreach (var lp in frame.Projection)
            {
                foreach (var point in lp.Points)
                {
                    float delta = cosTheta * (centerx - point.X) - sinTheta * (centery -point.Y) - cb;

                    var index = ((int)(_scale * _colors.Length * (delta  ) / (to - from)));
                    //var index = ((int)(_scale * _colors.Length * point.Y   / y2) );
                    if (index < 0 )
                    {
                        index = Math.Abs(index);
                        index %= _colors.Length;
                        frame.Colors[point] = _colors[index] * level;
                    }
                    else
                    {
                        index %= _colors.Length;
                        frame.Colors[point] = _colors[_colors.Length-1 - index] * level;
                    }
                }
            }
            // var dimX = frame.Width;
            // var dimY = frame.Height;
            // var locAngle = (_compositeAngle + 180) % 360;
            // if(locAngle < 90)
            // {
            //     FillGradient(frame, percent, dimX, 0f, 0f, dimY, level);
            // }
            // else if(locAngle < 180)
            // {
            //     FillGradient(frame, percent, dimX, dimY, 0f, 0f, level);
            // }
            // else if(locAngle < 270)
            // {
            //     FillGradient(frame, percent, 0f, dimY, dimX, 0f, level);
            // }
            // else
            // {
            //     FillGradient(frame, percent, 0f, 0f, dimX, dimY, level);
            // }
        }

        private void TwoSideGradient(EffectFrame frame, float percent, float level) 
        {
            // if (_isInside)
            // {
            //     percent = 1 - percent;
            // }

            float locAngle = _compositeAngle;

            locAngle = (locAngle + 90) % 360;
            
            var x2 = frame.Width;
            var y2 = frame.Height;
            var cosTheta = MathF.Cos(-locAngle.ToRadians());
            var sinTheta = MathF.Sin(-locAngle.ToRadians());

            var tmp = locAngle switch {
                >= 90 and < 180  =>  cosTheta * x2 + sinTheta * y2 - 1,
                >= 180 and < 270 =>  cosTheta * x2 - sinTheta * y2,
                >= 270 and < 360 => -cosTheta * x2 - sinTheta * y2,
                >= 0 and < 90    => -cosTheta * x2 + sinTheta * y2 - 1,
                _ => 1f
            };

            float from = tmp / 2;
            
            float to = 0;
            if (!_isInside)
            {
                to = from;
                from = 0;
            }
            float cb = from + percent * (to - from);

            foreach (var lp in frame.Projection)
            {
                foreach (var point in lp.Points)
                {
                    float delta1 = cosTheta * ((x2 - point.X * 2) * 0.5f) - sinTheta * ((y2 - point.Y * 2) * 0.5f) - cb;
                    float delta2 = cosTheta * ((x2 - (x2 - point.X) * 2) * 0.5f) -
                                   sinTheta * ((y2 - (y2 - point.Y) * 2) * 0.5f) - cb;
                    
                   
                    if (delta1 >= 0 && delta2 > 0)
                    {
                        delta2 = MathF.Min(delta1, delta2);
                    }
                    else
                    {
                        delta2 = MathF.Min(delta1, delta2);
                    }
                    
                    var index = ((int)(_scale * _colors.Length * (delta2  ) / (to - from)));
                    if (index < 0)
                    {
                        index = Math.Abs(index);
                        index %= _colors.Length;
                        //frame.Colors[point] = _colors[_isInside ? index : _colors.Length-1 - index] * level;
                        frame.Colors[point] = _colors[ _colors.Length-1 - index] * level;
                    }
                    else
                    {
                        index %= _colors.Length;
                        frame.Colors[point] = _colors[ index] * level;
                    }

                    
                }
            }
            // var dimX = frame.Width;
            // var dimY = frame.Height;
            // var locAngle = (_compositeAngle + 180) % 360;
            // if(locAngle < 90 || (locAngle >= 180 && locAngle < 270))
            // {
            //     FillGradient(frame, percent, dimX, 0f, dimX / 2f, dimY / 2f, level);
            //     FillGradient(frame, percent, 0f, dimY, dimX / 2f, dimY / 2f, level);
            // }
            // else
            // {
            //     FillGradient(frame, percent, 0f, 0f, dimX / 2f, dimY / 2f, level);
            //     FillGradient(frame, percent, dimX, dimY, dimX / 2f, dimY / 2f, level);
            // }
        }

        private void RhombusGradient(EffectFrame frame, float percent, float level) 
        {
            var dimX = frame.Width;
            var dimY = frame.Height;
            var start = (int)Math.Round(((float)Math.Atan(dimY / (double)dimX)).ToDegrees());
            _compositeAngle = start;
            FillGradient(frame, percent,  dimX / 2f, dimY / 2f,dimX, 0f, level, 1 ,  - 1);//, (int)Math.Round(dimX / 2f), dimX, 0, (int)Math.Round(dimY / 2f), dimX);
            
            _compositeAngle = 180 - start;
            FillGradient(frame, percent, dimX /2f , dimY / 2f , dimX, dimY , level, 1,  1);//, (int)Math.Round(dimX / 2f), dimX, (int)Math.Round(dimY / 2f), dimY, dimX);
            
            _compositeAngle = start;
            FillGradient(frame, percent,  dimX / 2f, dimY / 2f, 0f, dimY, level, -1, 1);//, 0, (int)Math.Round(dimX / 2f), (int)Math.Round(dimY / 2f), dimY, dimX);
            
            _compositeAngle = 180 - start;
            FillGradient(frame, percent,  dimX / 2f, dimY / 2f,0f, 0f, level ,- 1, -1);//, 0, (int)Math.Round(dimX / 2f), 0, (int)Math.Round(dimY / 2f), dimX);
        }
        
        private void RectGradient(EffectFrame frame, float percent, float level) 
        {
            var dimX = frame.Width;
            var dimY = frame.Height;
            var ratio = dimX / (float)dimY;
            percent %= 1.0f;

            foreach(var lp in frame.Projection)
            {
                foreach(var point in lp.Points)
                {
                    var i = point.X;
                    var j = point.Y;

                    int quarter;
                    if(i > j * ratio)
                    {
                        quarter = dimX - i > j * ratio ? 1 : 2;
                    }
                    else
                    {
                        quarter = dimX - i > j * ratio ? 4 : 3;
                    }
                    float distance;
                    if(quarter % 2 == 1)
                    {
                        var absDist = Math.Abs(j - dimY / 2f) / (dimY / 2f);
                        distance = _isInside ? absDist + percent : 1 - absDist + percent;
                    }
                    else
                    {
                        var absDist = Math.Abs(i - dimX / 2f) / (dimX / 2f);
                        distance = _isInside ? absDist + percent : 1 - absDist + percent;
                    }
                    if(distance >= 1f)
                    {
                        distance -= 1f;
                    }
                    int index = (int)Math.Round(distance * (_colors.Length - 1));
                    frame.Colors[point] = _colors[_colors.Length-1 - index] * level;
                }
            }
        }

        private void CircleGradient(EffectFrame frame, float percent, float level) 
        {
            var dimX = frame.Width;
            var dimY = frame.Height;
            var longestDistance = Math.Sqrt(dimX * dimX + dimY * dimY) * 0.5f; // 0.5 диагонали прямоугольника 
            var needDistance = _isInside ? (1f - percent % 1.0f ) * longestDistance : percent % 1.0f * longestDistance;
            var centerX = dimX / 2f;
            var centerY = dimY / 2f;

            foreach(var lp in frame.Projection)
            {
                foreach(var point in lp.Points)
                {
                    var i = point.X;
                    var j = point.Y;

                    var distance = Math.Sqrt((i - centerX) * (i - centerX) + (j - centerY) * (j - centerY));
                    var rate = distance >= needDistance ?
                        (distance - needDistance) / longestDistance : (distance + longestDistance - needDistance) / longestDistance;

                    int index = (int)Math.Round(rate * (_colors.Length - 1));
                    frame.Colors[point] = !_isInside ? _colors[index] * level : _colors[_colors.Length-1 - index] * level;
                }
            }
        }

        private void FanGradient(EffectFrame frame, float percent, float level)
        {
            var dimX = frame.Width;
            var dimY = frame.Height;
            var needAngle = percent * 360f % 360f;
            var centerX = dimX / 2f;
            var centerY = dimY / 2f;
            foreach (var lp in frame.Projection)
            {
                foreach (var point in lp.Points)
                {
                    var i = point.X;
                    var j = point.Y;

                    var dx = i - centerX;
                    var dy = _isClockwise ? j - centerY : centerY - j;

                    var angle = ((float)Math.Atan2(dy, dx)).ToDegrees() + 180f;
                    float rate = angle >= needAngle ? (angle - needAngle) / 360f : (angle + 360f - needAngle) / 360f;
                    int index = (int)Math.Round(rate * (_colors.Length - 1));
                    //frame.SetPixel(point, _colors[index > 0? index-1 : index]);
                    index = index > 0 ? index - 1 : index;
                    frame.Colors[point] = _colors[index] * level;
                }
            }
        }


        private void FillGradient(EffectFrame frame, float percent, float x1, float y1, float x2, float y2, float level, int dx = 0, int dy = 0) 
        {
            var tanAngle = MathF.Tan(_compositeAngle.ToRadians());
            var from = y1 - tanAngle * x1;
            var to = y2 - tanAngle * x2;
            var cb = (!_isInside ? 1f - percent : percent) * (to - from);
            var min = from;
            var max = to;
            if (to < from)
            {
                min = to;
                max = from;
            }

            float delta = to - from;
            
            foreach(var lp in frame.Projection)
            {
                foreach(var point in lp.Points)
                {
                    if (dx != 0 || dy != 0)  //если заданы знаки квадрантов
                    {
                        var centerX = frame.Width * 0.5f;
                        var centerY = frame.Height * 0.5f;
                        //пропускаем не нужные
                        if (Math.Abs((point.X-centerX)) > 0.001 && Math.Sign(point.X-centerX)!=Math.Sign(dx))
                            continue;
                        if (Math.Abs((point.Y-centerY)) > 0.001 && Math.Sign(point.Y-centerY)!=Math.Sign(dy))
                            continue;
                    }
                    
                    // var index = (  _colors.Length * point.Y / frame.Height);
                    // frame.Colors[point] = _colors[index] * level;
                    var locb = point.Y - tanAngle * point.X;
                    if (locb >= min && locb <= max)
                    {
                        var cbloc = locb + cb;
                        var length = _colors.Length;//[MethodImpl(MethodImplOptions.AggressiveInlining)] 
                        var index = Math.Abs((int)(_scale * length * (cbloc - from) / delta));
                        index %= _colors.Length;//(_speed < 2) ? _colors.Length - BandWidth : _colors.Length;
                        frame.Colors[point] = _colors[_isInside? _colors.Length-1 - index : index] * level;
                    }
                }
            }
        }
        
        private const int BandWidth = 100;
        private void CalculateGradient()
        {
            int bands = _colorsPalette.Count;
            if (bands < 2)
            {
                _colors = new CompositeColor[BandWidth];
                for (int j = 0; j < BandWidth; j++)
                {
                    _colors[j] = bands == 0 ? CompositeColor.BlackColor : _colorsPalette[0];
                }
                return;
            }
            
            int gradientSize = bands * BandWidth;
            _colors = new CompositeColor[gradientSize];
            var _colorstmp = new CompositeColor[gradientSize];
            int i = 0;//(_speed < 2) ? 1 : 0;
            int ci = 0;
            var flag = true;
            while (flag)
            {
                var offset = ci * BandWidth;
                int next = i + 1;
                if (next >= bands)
                {
                    next = 0;
                }

                for (int j = 0; j < BandWidth; j++)
                {
                    if (offset + j == gradientSize)
                    {
                        flag = false;
                        break;
                    }
                    _colorstmp [offset + j] = CompositeColor.Lerp(_colorsPalette[i], _colorsPalette[next], (float)j / BandWidth);
                }

                ++ci;
                i = next == 0 ? 0 : i+1;
            }
            
            //первый цвет у нас разделён на пополам, т.е. с него начинается и заканчивается по половинке
            //это позволяет зациклить, но не очень красиво в начале эффекта
            //скопируюем половину в начало
             Array.Copy(_colorstmp, gradientSize-BandWidth/2, _colors, 0, BandWidth/2 );
             Array.Copy(_colorstmp, 0, _colors, BandWidth/2, gradientSize-BandWidth/2 );
        }
    }