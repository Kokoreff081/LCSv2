using System.Diagnostics;
using LcsServer.Models.LCProjectModels.GlobalBase;
using LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;
using LCSVersionControl.Interfaces;

namespace LcsServer.Models.LCProjectModels.Models.ScenarioObjects;

/// <summary>
    /// Класс, описывающий наложение наложение 2-х эффектов
    /// </summary>
    public class Transition : PlayingEntity, ISaveLoad
    {
        private TransitionType _transitionType;
        private long _intersectionPoint;

        private IPlayingEntity _first;
        private IPlayingEntity _second;

        [SaveLoad] private int _saveFirstEntityId;
        [SaveLoad] private int _saveSecondEntityId;

        /// <summary>
        /// Изменение типа перехода
        /// </summary>
        public event EventHandler TransitionTypeChanged;

        public Transition() { }

        public Transition(IPlayingEntity first, IPlayingEntity second, long intersectionPoint)
        {
            _first = first;
            _second = second;
            _intersectionPoint = intersectionPoint;

            Subscription();
        }

        public Transition(int id, string name, int parentId, long intersectionPoint, int firstEntityId,
            int secondEntityId, TransitionType transitionType, float dimmingLevel)
        {
            Id = id;
            Name = name;
            _parentId = parentId;
            _intersectionPoint = intersectionPoint;
            _saveFirstEntityId = firstEntityId;
            _saveSecondEntityId = secondEntityId;
            TransitionType = transitionType;
            DimmingLevel = dimmingLevel;
        }

        ~Transition()
        {
            UnSubscription();
        }

        /// <summary>
        /// Тип перехода
        /// </summary>
        public TransitionType TransitionType
        {
            get => _transitionType;
            set
            {
                if (_transitionType == value)
                {
                    return;
                }

                _transitionType = value;
                TransitionTypeChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Тик пересечения
        /// </summary>
        [SaveLoad]
        public long IntersectionPoint
        {
            get => _intersectionPoint;
            set
            {
                if (_intersectionPoint == value)
                {
                    return;
                }

                _intersectionPoint = value;
                TotalTicksNotify();
            }
        }

        public override bool TryGetFrame(long tick, out ScenarioFrame frame, bool calculateDefaultSize,
            float riseOrFadeLevel)
        {
            frame = null;
            if (_first == null || _second == null)
            {
                return false;
            }
            
            float rel = tick / (float)TotalTicks;

            float totalRiseOrFadeLevel = riseOrFadeLevel * GetLevelByRiseAndFade(tick);

            if (_transitionType is TransitionType.Fading)
            {
                if (rel <= 0.5f)
                {
                    if (!_first.TryGetFrame(_intersectionPoint + tick, out frame, calculateDefaultSize,
                            totalRiseOrFadeLevel))
                    {
                        return false;
                    }

                    rel *= 2;
                    LerpToBlack(frame.Raster.Colors, rel);

                    if (calculateDefaultSize)
                    {
                        LerpToBlack(frame.Default.Colors, rel);
                    }
                }
                else
                {
                    if (!_second.TryGetFrame(tick, out frame, calculateDefaultSize, totalRiseOrFadeLevel))
                    {
                        return false;
                    }

                    rel = (rel - 0.5f) * 2.0f;
                    LerpFromBlack(frame.Raster.Colors, rel);
                    if (calculateDefaultSize)
                    {
                        LerpFromBlack(frame.Default.Colors, rel);
                    }
                }
            }
            else
            {
                if (!_first.TryGetFrame(_intersectionPoint + tick, out frame, calculateDefaultSize,
                        totalRiseOrFadeLevel) ||
                    !_second.TryGetFrame(tick, out ScenarioFrame frame2, calculateDefaultSize, totalRiseOrFadeLevel))
                    return false;

                LerpToMap1(frame.Raster.Colors, frame2.Raster.Colors, rel);

                if (calculateDefaultSize)
                {
                    if (frame.Default != null && frame2.Default != null)
                    {
                        LerpToMap1(frame.Default.Colors, frame2.Default.Colors, rel);
                    }
                    else
                    {
                    }
                }
            }

            return true;
        }


        /// <summary>
        /// Получить id первого эффекта
        /// </summary>
        /// <returns> id </returns>
        public int GetFirstItemId()
        {
            if (_first != null)
            {
                return _first.Id;
            }

            Debug.WriteLine("Transition: _first == null");
            return -1;
        }

        /// <summary>
        /// Получить id второго эффекта
        /// </summary>
        /// <returns> id </returns>
        public int GetSecondItemId()
        {
            if (_second != null)
            {
                return _second.Id;
            }

            Debug.WriteLine("Transition: _second == null");
            return -1;
        }

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
            IEnumerable<LCObject> lcObjects = primitives.OfType<LCObject>().ToList();
            LoadParent(lcObjects);

            _first = lcObjects.FirstOrDefault(a => a.Id == _saveFirstEntityId) as IPlayingEntity;
            _second = lcObjects.FirstOrDefault(a => a.Id == _saveSecondEntityId) as IPlayingEntity;

            Subscription();
        }

        protected override long GetTotalTicks()
        {
            if (_first == null)
            {
                return 0;
            }
            return _first.TotalTicks - _intersectionPoint;
        }

        private static void LerpToMap1(Dictionary<IntPoint, CompositeColor> map1,
            IReadOnlyDictionary<IntPoint, CompositeColor> map2, float rel)
        {
            foreach (var key in map1.Keys.ToArray())
            {
                map1[key] = CompositeColor.Lerp(map1[key], map2[key], rel);
            }
        }

        private static void LerpToBlack(Dictionary<IntPoint, CompositeColor> map1, float rel)
        {
            foreach (var key in map1.Keys.ToArray())
            {
                map1[key] = CompositeColor.Lerp(map1[key], CompositeColor.BlackColor, rel);
            }
        }

        private static void LerpFromBlack(Dictionary<IntPoint, CompositeColor> map1, float rel)
        {
            foreach (var key in map1.Keys.ToArray())
            {
                map1[key] = CompositeColor.Lerp(CompositeColor.BlackColor, map1[key], rel);
            }
        }

        private void OnEffect_TotalTicksChanged(object sender, EventArgs e)
        {
            TotalTicksNotify();
        }

        private void Subscription()
        {
            _first.TotalTicksChanged += OnEffect_TotalTicksChanged;
            _second.TotalTicksChanged += OnEffect_TotalTicksChanged;
        }

        private void UnSubscription()
        {
            _first.TotalTicksChanged -= OnEffect_TotalTicksChanged;
            _second.TotalTicksChanged -= OnEffect_TotalTicksChanged;
        }
    }