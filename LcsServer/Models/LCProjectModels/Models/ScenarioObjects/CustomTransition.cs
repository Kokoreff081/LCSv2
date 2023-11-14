using LcsServer.Models.LCProjectModels.GlobalBase;
using LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;
using LCSVersionControl.Interfaces;

namespace LcsServer.Models.LCProjectModels.Models.ScenarioObjects;

/// <summary>
    /// Класс, описывающий наложение наложение 2-х эффектов
    /// </summary>
    public class CustomTransition : PlayingEntity
    {
        private TransitionType _transitionType;
        private long _intersectionPoint;

        private IPlayingEntity _first;
        private IPlayingEntity _second;

        private readonly long _maxDuration;

        [SaveLoad]
        private int _saveFirstEntityId;
        [SaveLoad]
        private int _saveSecondEntityId;

        /// <summary>
        /// Изменение типа перехода
        /// </summary>
        public event EventHandler TransitionTypeChanged;

        public CustomTransition() { }

        public CustomTransition(IPlayingEntity first, IPlayingEntity second, long intersectionPoint, long duration)
        {
            _first = first;
            _second = second;
            _intersectionPoint = intersectionPoint;
            _maxDuration = duration;
        }

        public void ChangeSecondPlayingEntity(IPlayingEntity second)
        {
            _second = second;
            TotalTicksNotify();
        }

        /// <summary>
        /// Тип перехода
        /// </summary>
        public TransitionType TransitionType
        {
            get => _transitionType;
            set
            {
                if (_transitionType == value) return;

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
                if (_intersectionPoint == value) return;

                _intersectionPoint = value;
                TotalTicksNotify();
            }
        }

        public override bool TryGetFrame(long tick, out ScenarioFrame frame, bool calculateDefaultSize, float riseOrFadeLevel)
        {
            CheckPlayItems();

            frame = null;
            var rel = tick / (float)TotalTicks;

            if (_transitionType is TransitionType.Fading)
            {
                if (rel <= 0.5f)
                {
                    if (!_first.TryGetFrame(_intersectionPoint + tick, out frame, calculateDefaultSize, 1))
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
                    if (!_second.TryGetFrame(tick, out frame, calculateDefaultSize, 1))
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
                if (!_first.TryGetFrame(_intersectionPoint + tick, out frame, calculateDefaultSize, 1) ||
                  !_second.TryGetFrame(tick, out ScenarioFrame frame2, calculateDefaultSize, 1)) return false;

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


        // public override bool GetEmptyFrame(long tick, out ScenarioFrame frame)
        // {
        //     if (!_first.GetEmptyFrame(_intersectionPoint + tick, out frame) || !_second.GetEmptyFrame(tick, out ScenarioFrame frame2)) 
        //         return false;
        //
        //     float rel = tick / (float)TotalTicks;
        //     LerpToMap1(frame.Raster.Colors, frame2.Raster.Colors, rel);
        //
        //     return true;
        // }

        protected override long GetTotalTicks()
        {
            if (_first == null)
            {
                return 0;
            }
            return _first.TotalTicks - _intersectionPoint < _maxDuration ? _first.TotalTicks - _intersectionPoint : _maxDuration;
        }

        private static void LerpToMap1(Dictionary<IntPoint, CompositeColor> map1, IReadOnlyDictionary<IntPoint, CompositeColor> map2, float rel)
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

        private void CheckPlayItems()
        {
            CheckFirstPlayItem();
            CheckSecondPlayItem();
        }

        private void CheckFirstPlayItem()
        {
            if (_first == null) throw new NullPlayableItemsInTransitionException(Id);
        }

        private void CheckSecondPlayItem()
        {
            if (_second == null) throw new NullPlayableItemsInTransitionException(Id);
        }

    }
    
public class NullPlayableItemsInTransitionException : ApplicationException
{
    public NullPlayableItemsInTransitionException(int objectId) : base($"Play items for Transition (Id='{objectId}') is not found.")
    { }

}