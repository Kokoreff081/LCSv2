using System.Numerics;
using LcsServer.Models.LCProjectModels.GlobalBase;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;


namespace LcsServer.Models.LCProjectModels.Models.Rasters;

public class Raster : LCObject, ISaveLoad
    {
        private const float Tolerance = 0.0001f;

        /// <summary>
        /// Изменен Dimension
        /// </summary>
        public event EventHandler DimensionChanged;
        /// <summary>
        /// Изменен Projection
        /// </summary>
        public event EventHandler ProjectionChanged;
        /// <summary>
        /// Изменен Rotation
        /// </summary>
        public event EventHandler RotationChanged;
        /// <summary>
        /// Изменен ManualSet
        /// </summary>
        public event EventHandler ManualSetChanged;

        private int _dimensionX;
        private int _dimensionY;
        private bool _manualSet;
        private IReadOnlyList<LampProjection> _originalProjection;
        private List<LampProjection> _oldProjection;
        private int _angle;
        private bool _isFlipVertical;
        private bool _isFlipHorizontal;

        [SaveLoad]
        private int[] _idMap;
        [SaveLoad]
        private long[][] _projectionMap;

        public Raster()
        {
            _originalProjection = new List<LampProjection>();
            _oldProjection = new List<LampProjection>();
            RotatedProjectionMapping = new List<LampProjection>();
        }

        public Raster(int id, string name, int dimensionX, int dimensionY, int angle, bool isFlipHorizontal,
            bool isFlipVertical, int[] idMap, long[][] projectionMap, bool manualSet, Quaternion? rotation) : this()
        {
            Id = id;
            Name = name;
            DimensionX = dimensionX;
            DimensionY = dimensionY;
            _angle = angle;
            IsFlipHorizontal = isFlipHorizontal;
            IsFlipVertical = isFlipVertical;
            _idMap = idMap;
            _projectionMap = projectionMap;
            ManualSet = manualSet;
            Rotation = rotation;
        }

        /// <summary>
        /// Размерность по оси X
        /// </summary>
        [SaveLoad]
        public int DimensionX
        {
            get => _dimensionX;
            set
            {
                if (_dimensionX == value)
                {
                    return;
                }

                _dimensionX = value;
                IsDimensionChanged = true;
                DimensionChanged?.Invoke(this, EventArgs.Empty);
                OnNameChanged();
                
            }
        }

        /// <summary>
        /// Размерность по оси Y
        /// </summary>
        [SaveLoad]
        public int DimensionY
        {
            get => _dimensionY;
            set
            {
                if (_dimensionY == value)
                {
                    return;
                }

                _dimensionY = value;
                IsDimensionChanged = true;
                DimensionChanged?.Invoke(this, EventArgs.Empty);
                OnNameChanged();
            }
        }
        
        public bool IsDimensionChanged { get; set; }

        /// <summary>
        /// Ручная или автоматическая установка размерности
        /// </summary>
        [SaveLoad]
        public bool ManualSet
        {
            get => _manualSet;
            set
            {
                if (_manualSet == value) return;

                _manualSet = value;
                ManualSetChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        
        /// <summary>
        /// Карта расположения светильников
        /// </summary>
        public IReadOnlyList<LampProjection> ProjectionMapping
        {
            get => _originalProjection;
            set
            {
                if (_originalProjection.Equals(value)) return;

                _originalProjection = value;
                
                RotateProjectionMapping();
                DimensionChanged?.Invoke(this, EventArgs.Empty);
                ProjectionChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        public List<LampProjection> OldProjection { get { return _oldProjection; } set { _oldProjection = value; } }
        public List<LampProjection> Projection
        {
            get { return (List<LampProjection>)_originalProjection; }
            set
            {
                //if (_originalProjection.Equals(value)) return;

                _originalProjection = value;
                RotateProjectionMapping();
                DimensionChanged?.Invoke(this, EventArgs.Empty);
                ProjectionChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        /// <summary>
        /// Угол поворота
        /// </summary>
        [SaveLoad]
        public int Angle
        {
            get => _angle;
            set
            {
                if (_angle == value) return;

                _angle = value;
                RotateProjectionMapping();
                RotationChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Развернут по горизонтали
        /// </summary>
        [SaveLoad]
        public bool IsFlipHorizontal
        {
            get => _isFlipHorizontal;
            set
            {
                if (_isFlipHorizontal == value) return;

                _isFlipHorizontal = value;
                RotateProjectionMapping();
                RotationChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Развернут по вертикали
        /// </summary>
        [SaveLoad]
        public bool IsFlipVertical
        {
            get => _isFlipVertical;
            set
            {
                if (_isFlipVertical == value) return;

                _isFlipVertical = value;
                RotateProjectionMapping();
                RotationChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public Quaternion? Rotation { get; set; }

        /// <summary>
        /// Повернутая карта расположения светильников
        /// </summary>
        public List<LampProjection> RotatedProjectionMapping { get; private set; }

        /// <summary>
        /// Ширина развернутого растра
        /// </summary>
        public int RotatedWidth => _angle == 90 || _angle == 270 ? _dimensionY : _dimensionX;

        /// <summary>
        /// Высота развернутого растра
        /// </summary>
        public int RotatedHeight => _angle == 90 || _angle == 270 ? _dimensionX : _dimensionY;

        public bool IsEditable { get; set; }
        

        /// <summary>
        /// Вызывается при сохранении проекта
        /// </summary>
        /// <param name="projectFolderPath">Путь к папке проекта</param>
        public void Save(string projectFolderPath)
        {
        }

        /// <summary>
        /// Вызывается при загрузке проекта
        /// </summary>
        /// <param name="primitives">Все объекты проекта</param>
        /// <param name="indexInPrimitives">Индекс объекта в списке всех примитивов. Для ускоренного поиска родителя</param>
        /// <param name="projectFolderPath">Путь к папке проекта</param>
        public void Load(List<ISaveLoad> primitives, int indexInPrimitives, string projectFolderPath)
        {
            if (_idMap == null || _idMap.Length == 0) return;

            var projections = new List<LampProjection>(_idMap.Length);
            for (int i = 0; i < _idMap.Length; i++)
            {
                var lpr = new LampProjection(_idMap[i]);
                for (int j = 0; j < _projectionMap[i].Length; j++)
                {
                    lpr.AddPoint(new IntPoint(_projectionMap[i][j]));
                }

                projections.Add(lpr);
            }

            _originalProjection = projections;
            RotateProjectionMapping();
        }

        /// <summary>
        /// Сравнение значений растров на идентичность
        /// </summary>
        /// <param name="raster">Растр для сравнения</param>
        /// <returns>Идентичны или нет все значения растров</returns>
        public bool IsEquals(Raster raster)
        {
            if (raster == null)
                return false;

            if (raster.Angle != Angle ||
                raster.DimensionX != DimensionX ||
                raster.DimensionY != DimensionY ||
                raster.IsFlipHorizontal != IsFlipHorizontal ||
                raster.IsFlipVertical != IsFlipVertical ||
                !raster.Name.Equals(Name, StringComparison.InvariantCulture) ||
                raster.RotatedHeight != RotatedHeight ||
                raster.RotatedWidth != RotatedWidth)
            {
                return false;
            }

            if (raster._originalProjection.Count != _originalProjection.Count)
                return false;

            var map1 = raster._originalProjection.ToDictionary(a => a.LampId, b => b.Points);
            var map2 = _originalProjection.ToDictionary(a => a.LampId, b => b.Points);

            foreach (var (id, positions) in map1)
            {
                if (!map2.TryGetValue(id, out var destPositions)) return false;

                if (positions.Count != destPositions.Count) return false;

                for (int i = 0; i < positions.Count; i++)
                {
                    if (positions[i].Equals(destPositions[i])) return false;
                }
            }

            return true;
        }

        private void RotateProjectionMapping()
        {
            RotatedProjectionMapping = GetRotatedProjections(_originalProjection);
        }

        private int GetValue(float val, float minVal, float maxVal, int dimension)
        {
            var res = (int)((val - minVal) / (maxVal - minVal) * (dimension - Tolerance));
            res = Math.Min(dimension - 1, res);
            return res;
        }

        private List<LampProjection> GetRotatedProjections(IReadOnlyList<LampProjection> projections)
        {
            Func<IntPoint, IntPoint> func;
            if (_isFlipHorizontal)
            {
                if (_isFlipVertical)
                {
                    switch (_angle)
                    {
                        case 90:
                            func = a => Rotate90(FlipVertical(FlipHorizontal(a)));
                            break;
                        case 180:
                            func = a => Rotate180(FlipVertical(FlipHorizontal(a)));
                            break;
                        case 270:
                            func = a => Rotate270(FlipVertical(FlipHorizontal(a)));
                            break;
                        default:
                            func = a => FlipVertical(FlipHorizontal(a));
                            break;
                    }
                }
                else
                {
                    switch (_angle)
                    {
                        case 90:
                            func = a => Rotate90(FlipHorizontal(a));
                            break;
                        case 180:
                            func = a => Rotate180(FlipHorizontal(a));
                            break;
                        case 270:
                            func = a => Rotate270(FlipHorizontal(a));
                            break;
                        default:
                            func = a => FlipHorizontal(a);
                            break;
                    }
                }
            }
            else if (_isFlipVertical)
            {
                switch (_angle)
                {
                    case 90:
                        func = a => Rotate90(FlipVertical(a));
                        break;
                    case 180:
                        func = a => Rotate180(FlipVertical(a));
                        break;
                    case 270:
                        func = a => Rotate270(FlipVertical(a));
                        break;
                    default:
                        func = a => FlipVertical(a);
                        break;
                }
            }
            else
            {
                switch (_angle)
                {
                    case 90:
                        func = a => Rotate90(a);
                        break;
                    case 180:
                        func = a => Rotate180(a);
                        break;
                    case 270:
                        func = a => Rotate270(a);
                        break;
                    default:
                        func = a => a;
                        break;
                }
            }

            var result = new List<LampProjection>();

            foreach (var pr in projections)
            {
                var lpr = new LampProjection(pr.LampId, pr.PixelsCount, pr.ColorsCount);
                pr.Points.ForEach(point => lpr.AddPoint(func(point)));
                result.Add(lpr);
            }

            return result;
        }

        private IntPoint FlipHorizontal(in IntPoint a) => new IntPoint(_dimensionX - a.X - 1, a.Y);
        private IntPoint FlipVertical(in IntPoint a) => new IntPoint(a.X, _dimensionY - a.Y - 1);
        private IntPoint Rotate90(in IntPoint a) => new IntPoint(_dimensionY - a.Y - 1, a.X);
        private IntPoint Rotate180(in IntPoint a) => new IntPoint(_dimensionX - a.X - 1, _dimensionY - a.Y - 1);
        private IntPoint Rotate270(in IntPoint a) => new IntPoint(a.Y, _dimensionX - a.X - 1);

        public override string DisplayName => $"{Name} [{DimensionX}x{DimensionY}]";

        public void Clear()
        {
            RotatedProjectionMapping?.Clear();
            _originalProjection = null;
        }
    }