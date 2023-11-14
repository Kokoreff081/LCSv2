using System.Diagnostics;
using LcsServer.Models.LCProjectModels.GlobalBase;
using LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;
using LcsServer.Models.LCProjectModels.Models.Rasters;
using LCSVersionControl;
using LCSVersionControl.Interfaces;

namespace LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects;

    /// <summary>
    /// Класс, описывающий заполняющий эффект
    /// </summary>
    public class ModbusFill : BaseModbusEffect
    {
        private FillStyles _fillStyle;
        private InOutDirection _direction;
        private Rotation _rotation;
        private int _angle;
        private CompositeColor _backgroundColor;
        private CompositeColor _compColor;
        private int _blurFactor;

        private CompositeColor _currCompositeColor;
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
        /// Изменение цвета фона
        /// </summary>
        public event EventHandler BackgroundColorChanged;

        /// <summary>
        /// Изменение цвета заливки
        /// </summary>
        public event EventHandler CompColorChanged;

        /// <summary>
        /// Изменение флага Случайный цвет
        /// </summary>
        public event EventHandler IsRandomColorChanged;

        public event EventHandler BlurFactorChanged;

        public ModbusFill()
        {
            _compColor = CompositeColor.RedColor;
            _backgroundColor = CompositeColor.BlackColor;
            
            UpdateIsInside();
            UpdateCompositeClockwise();
            UpdateCompositeAngle();
            UpdateCompositeColor();
        }

        public ModbusFill(int id, string name, int parentId, FillStyles fillStyle, int angle, 
            CompositeColor backgroundColor, CompositeColor compColor, InOutDirection direction, Rotation rotation, 
            long totalTicks, long riseTime, long fadeTime, float dimmingLevel, int blurFactor,
            string ipAddress, ushort port, byte unitId, ushort register, int interval, float min, float max,
            SensorUnits sensorUnits) : this()
        {
            Id = id;
            Name = name;
            _parentId = parentId;
            FillStyle = fillStyle;
            Angle = angle;
            BackgroundColor = backgroundColor;
            CompColor = compColor;
            Direction = direction;
            Rotation = rotation;
            TotalTicks = totalTicks;
            RiseTime = riseTime;
            FadeTime = fadeTime;
            DimmingLevel = dimmingLevel;
            BlurFactor = blurFactor;
            
            IpAddress = ipAddress;
            Port = port;
            UnitId = unitId;
            Register = register;
            Interval = interval;
            Minimum = min;
            Maximum = max;
            SensorUnits = sensorUnits;
        }

        /// <summary>
        /// Категория
        /// </summary>
        public override Category Category => Category.Modbus;

        /// <summary>
        /// Тип
        /// </summary>
        public override Enum Type => ModbusTypes.ModbusFill;

        /// <summary>
        /// Тип заливки
        /// </summary>
        public FillStyles FillStyle
        {
            get => _fillStyle;
            set
            {
                if (_fillStyle == value)
                {
                    return;
                }

                _fillStyle = value;
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
                if (_direction == value)
                {
                    return;
                }

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
                UpdateCompositeClockwise();
                RotationChanged?.Invoke(this, EventArgs.Empty);
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
                if (_angle == value)
                {
                    return;
                }

                _angle = value;
                UpdateCompositeAngle();
                AngleChanged?.Invoke(this, EventArgs.Empty);
            }
        }


        /// <summary>
        /// Цвет фона
        /// </summary>
        public CompositeColor BackgroundColor
        {
            get => _backgroundColor;
            set
            {
                if (_backgroundColor == value)
                {
                    return;
                }

                _backgroundColor = value;
                BackgroundColorChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Цвет заливки
        /// </summary>
        public CompositeColor CompColor
        {
            get => _compColor;
            set
            {
                if (_compColor == value)
                {
                    return;
                }

                _compColor = value;
                UpdateCompositeColor();
                CompColorChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public int BlurFactor
        {
            get => _blurFactor;
            set
            {
                if (_blurFactor == value)
                {
                    return;
                }

                _blurFactor = value;
                BlurFactorChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        
           

        /// <summary>
        /// Вызывается при сохранении проекта
        /// </summary>
        /// <param name="projectFolderPath">Путь к папке проекта</param>
        public override void Save(string projectFolderPath)
        {
        }

        /// <summary>
        /// Вызывается при загрузке проекта
        /// </summary>
        /// <param name="primitives">Все объекты проекта</param>
        /// <param name="indexInPrimitives">Индекс объекта в списке всех примитивов. Для ускоренного поиска родителя</param>
        /// <param name="projectFolderPath">Путь к папке проекта</param>
        public override void Load(List<ISaveLoad> primitives, int indexInPrimitives, string projectFolderPath)
        {
            base.Load(primitives, indexInPrimitives, projectFolderPath);

            UpdateIsInside();
            UpdateCompositeClockwise();
            UpdateCompositeAngle();
            UpdateCompositeColor();
        }

        protected override Effect Clone(IPlayingEntity parent)
        {
            var clone = new ModbusFill
            {
                Parent = parent,
                FillStyle = FillStyle,
                Direction = _direction,
                Rotation = _rotation,
                Angle = _angle,
                CompColor = _compColor,
                BackgroundColor = _backgroundColor,
                TotalTicks = TotalTicks,
                _compositeAngle = _compositeAngle,
                _isInside = _isInside,
                _isClockwise = _isClockwise,
                BlurFactor = BlurFactor,
            };

            return clone;
        }

        protected override bool TryFillFrame(long ticks, EffectFrame frame, float riseOrFadeLevel, bool isDefault)
        {
            ReadValue();

            float percent = (FloatValue - Minimum) / (Maximum - Minimum);
            
            Debug.WriteLine(percent);

            switch (_fillStyle)
            {
                case FillStyles.OneSide:
                    OneSide(frame, percent, riseOrFadeLevel);
                    break;
                case FillStyles.TwoSides:
                    TwoSides(frame, percent, riseOrFadeLevel);
                    break;
                case FillStyles.Rect:
                    Rect(frame, percent, riseOrFadeLevel);
                    break;
                case FillStyles.Rhombus:
                    Rhombus(frame, percent, riseOrFadeLevel);
                    break;
                case FillStyles.Circle:
                    Circle(frame, percent, riseOrFadeLevel);
                    break;
                case FillStyles.Fan:
                    Fan(frame, percent, riseOrFadeLevel);
                    break;
                default:
                    return false;
            }

            return true;
        }

        private CompositeColor GetColor(bool condition, float percent, float level)
        {
            if (condition)
            {
                return CompositeColor.Lerp( _currCompositeColor, _backgroundColor,percent) * level;
                //return _backgroundColor * level;
            }
            return CompositeColor.Lerp(_backgroundColor, _currCompositeColor, percent) * level;
        }

     

        private void OneSide(EffectFrame frame, float percent, float level)
        {
            var x2 = frame.Width;
            var y2 = frame.Height;

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

            float from = tmp / 2 - BlurFactor;
            float to = - tmp / 2;

            float cb = from + percent * (to - from);

            foreach (LampProjection lp in frame.Projection)
            {
                foreach (IntPoint point in lp.Points)
                {
                    float delta = cosTheta * (centerx - point.X) - sinTheta * (centery -point.Y) - cb;
                    float per = 1f;
                    if (delta >= 0 && BlurFactor > 0 )
                    {
                        per = delta / BlurFactor;
                        per = per <= 1 ? per : 1;
                    }
                    frame.Colors[point] = GetColor(delta >= 0,  per, level);
                }
            }
        }

        private void TwoSides(EffectFrame frame, float percent, float level)
        {
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

            float from = tmp / 2 - BlurFactor;
            
            float to = 0;
            if (!_isInside)
            {
                to = from;
                from = 0;
            }
            float cb = from + percent * (to - from);

            foreach (LampProjection lp in frame.Projection)
            {
                foreach (IntPoint point in lp.Points)
                {
                    float delta1 = cosTheta * ((x2 - point.X * 2) * 0.5f) - sinTheta * ((y2 - point.Y * 2) * 0.5f) - cb;
                    float delta2 = cosTheta * ((x2 - (x2 - point.X) * 2) * 0.5f) -
                                   sinTheta * ((y2 - (y2 - point.Y) * 2) * 0.5f) - cb;
                    
                    float per = 0f;
                    float per1 = delta1 / (BlurFactor + 0.01f);
                    per1 = per1 <= 1 ? per1 : 1;
                    float per2 = delta2 / (BlurFactor + 0.01f);
                    per2 = per2 <= 1 ? per2 : 1;

                    if (delta1 >= 0 && delta2 > 0)
                    {
                        per = MathF.Min(per1, per2);
                    }

                    frame.Colors[point] = GetColor((delta1 >= 0 || delta2 > 0) ^ !_isInside, per, level);
                }
            }
        }

        private void Rect(EffectFrame frame, float percent, float level)
        {
            int dimX = frame.Width;
            int dimY = frame.Height;
            int dimXadd = dimX + (BlurFactor+1) * 2;
            int dimYadd = dimY + (BlurFactor+1) * 2;
            float x1, x2, y1, y2;
            //double tolerance;
            float tolerance;
            
            percent = !_isInside ? 1f - percent : percent;

            x1 = percent * dimXadd * 0.5f;
            x2 = dimX - percent * dimXadd * 0.5f;
            y1 = percent * dimYadd * 0.5f;
            y2 = dimY - percent * dimYadd * 0.5f;
            tolerance = BlurFactor +0.1f;


            foreach (var lp in frame.Projection)
            {
                foreach (var point in lp.Points)
                {
                    var i = point.X;
                    var j = point.Y;
                    var x1tol = x1 - tolerance;
                    var x2tol = x2 + tolerance;
                    var y1tol = y1 - tolerance;
                    var y2tol = y2 + tolerance;
                    bool condition;

                    condition = i >= x1tol &&
                                i <= x2tol &&
                                j >= y1tol &&
                                j <= y2tol;
                    
                    float per = 1;
                    float perx1 = 1;
                    float perx2 = 1;
                    float pery1 = 1;
                    float pery2 = 1;
                    if (condition)
                    {
                        var x1dif = i - x1tol;
                        var x2dif = x2tol - i;
                        var y1dif = j - y1tol;
                        var y2dif = y2tol - j;
                        
                        if (y2dif < tolerance)
                        {
                           pery2 = y2dif / tolerance;
                        }
                        if (y1dif < tolerance)
                        {
                            pery1 = y1dif / tolerance;
                        }
                        if (x1dif < tolerance)
                        {
                            perx1 = x1dif / tolerance;
                        }
                        //if (y2dif < BlurFactor)
                        {
                            perx2 = x2dif / tolerance;
                        }

                        var tmp1 = MathF.Min(perx1, perx2);
                        var tmp2 = MathF.Min(pery1, pery2);
                        per = MathF.Min(tmp1, tmp2);
                    }

                    // var color = condition ^ _isInside ? _currCompositeColor * level : _backgroundColor * level;
                    // frame.Colors[point] = color;
                    frame.Colors[point] = GetColor( !(condition ^ _isInside), per, level);
                }
            }
        }

        private void Rhombus(EffectFrame frame, float percent, float level)
        {
            int dimX = frame.Width;
            int dimY = frame.Height;
            float dimXadd = dimX + 1 + BlurFactor;
            float dimYadd = dimY + 1 + BlurFactor;
            var k = _isInside ? 1f - percent : percent;
            float koeff = dimYadd / dimXadd;
                // ? Math.Max(dimYadd, dimXadd) / (float)Math.Min(dimYadd, dimXadd)
                // : Math.Min(dimYadd, dimXadd) / (float)Math.Max(dimYadd, dimXadd);
            var distanceX = koeff * k * dimYadd;    //расстояние от центра до границы ромба
            var distanceY = koeff * k * dimXadd;

            var centerX = frame.Width * 0.5f;
            var centerY = frame.Height * 0.5f;

            //float level2 = 1 - level;

            foreach (var lp in frame.Projection)
            {
                foreach (var point in lp.Points)
                {
                    var i = point.X;
                    var j = point.Y;

                    var distantX = i < centerX ? centerX - distanceX : centerX + distanceX;
                    var distantY = j < centerY ? centerY - distanceY : centerY + distanceY;

                    var shortDistance =  (i - centerX) * (i - centerX) + (j - distantY) * (j - centerY);
                    var longDistance = (i - centerX) * (i - distantX) + (j - distantY) * (j - distantY);

                    var delta = shortDistance - longDistance;
                    float per = 1f;
                    if (delta < 0)
                    {
                        per = MathF.Sqrt(-delta) / (BlurFactor+0.1f); 
                    }
                    
                    per = per < 1 ? per : 1;
                    frame.Colors[point] = GetColor(!(delta < 0  ^ _isInside), per, level);
                }
            }
        }

        private void Circle(EffectFrame frame, float percent, float level)
        {
            //BlurFactor = 5;
            int dimX = frame.Width;
            int dimY = frame.Height;
            var radius = MathF.Sqrt(dimX * dimX + dimY * dimY) * 0.5f +0.1f + MathF.Sqrt(BlurFactor) ; // 0.5 диагонали прямоугольника 
            var needDistance = _isInside ? (1f - percent) * radius : percent * radius;
            var centerX = dimX * 0.5f;
            var centerY = dimY * 0.5f;

            //float level2 = 1 - level;

            foreach (var lp in frame.Projection)
            {
                foreach (var point in lp.Points)
                {
                    var i = point.X;
                    var j = point.Y;

                    var squareDistance = (i - centerX) * (i - centerX) + (j - centerY) * (j - centerY);
                    var delta = needDistance * needDistance - squareDistance;
                    float per = 1;
                    bool condition = true;
                    if (delta > 0)
                    {
                        var deltas = MathF.Sqrt(delta);
                        per = deltas / (float) (BlurFactor+0.1); 
                        per = per < 1 ? per : 1;
                        condition = false;
                    }
                    
                    
                    //var color = condition ^ _isInside ? _currCompositeColor * level : _backgroundColor * level;
                    //frame.Colors[point] = color;
                    frame.Colors[point] = GetColor(condition ^ _isInside, per, level);
                }
            }
        }

        private void Fan(EffectFrame frame, float percent, float level)
        {
            var blurangle = 3.6f * BlurFactor;
            var percentangle = percent * 360f;
            float tmp = 0;
            if (percentangle < blurangle)
            {
                tmp = blurangle - percentangle;  //если текущий угол меньше угла блюра - нужно внести поправку, чтоб эфект начинался с нуля
            }

            int dimX = frame.Width;
            int dimY = frame.Height;
            var needAngle = percent * 370f + blurangle - tmp;
            
            float cAngle = _compositeAngle;

            float locAngle = (-cAngle % 360f).ToRadians();

            var centerX = dimX * 0.5f;
            var centerY = dimY * 0.5f;

            float pX = 1f;
            float pY = -1f;

            if (_isClockwise)
            {
                pX = -1f;
                pY = 1f;
            }

            //float level2 = 1 - level;

            foreach (var lp in frame.Projection)
            {
                foreach (var point in lp.Points)
                {
                    var i = point.X;
                    var j = point.Y;

                    var dx = i - centerX;
                    var dy = (j - centerY) * pY;

                    var xt = dx * MathF.Cos(locAngle) + dy * pY * MathF.Sin(locAngle);
                    var yt = dx * pX * MathF.Sin(locAngle) + dy * MathF.Cos(locAngle);

                    var newAngle = MathF.Atan2(yt, xt).ToDegrees() + 180f;

                    var delta = newAngle - needAngle;
                    float per = 1;
                    if (delta < 0)
                    {
                        per = MathF.Abs(delta / (blurangle + 0.1f));
                        per = per < 1 ? per : 1;
                    }

                    bool condition = delta<0;
                    frame.Colors[point] = GetColor(!(condition ^ _isInside), per, level);
                }
            }
        }

        private void UpdateCompositeColor()
        {
            _currCompositeColor = _compColor;
        }

        private void UpdateCompositeAngle()
        {
            _compositeAngle = _angle;
        }

        private void UpdateIsInside()
        {
            _isInside = _direction == InOutDirection.Inside || _direction == InOutDirection.Random && LCRandom.RandomNext(2) < 1;
        }

        private void UpdateCompositeClockwise()
        {
            _isClockwise = _rotation == Rotation.Clockwise || _rotation == Rotation.Random && LCRandom.RandomNext(2) < 1;
        }
    }