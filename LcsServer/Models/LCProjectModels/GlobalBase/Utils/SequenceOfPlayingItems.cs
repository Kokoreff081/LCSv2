using LcsServer.Models.LCProjectModels.Models.ScenarioObjects;
using LCSVersionControl.Interfaces;

namespace LcsServer.Models.LCProjectModels.GlobalBase.Utils;

public class SequenceOfPlayingItems<TPlayItem> where TPlayItem : IPlayingEntity
    {
        private readonly Dictionary<TPlayItem, long> _tPlayItemToStartMap;
        private readonly Dictionary<Transition, TPlayItem[]> _transitionToTPlayItemMap;
        
        private long _totalTicks;

        public event EventHandler TotalTicksChanged;

        public SequenceOfPlayingItems()
        {
            _tPlayItemToStartMap = new Dictionary<TPlayItem, long>();
            _transitionToTPlayItemMap = new Dictionary<Transition, TPlayItem[]>();
        }
        
        public IReadOnlyDictionary<long, TPlayItem> StartToTPlayItemMap => _tPlayItemToStartMap.OrderBy(a => a.Value).ToDictionary(a => a.Value, a => a.Key);
        public IReadOnlyList<Transition> Transitions => _transitionToTPlayItemMap.Keys.ToList();
        public IReadOnlyList<TPlayItem> TPlayItems => _tPlayItemToStartMap.Keys.ToList();
        
        public IEnumerable<IPlayingEntity> GetSequence()
        {
            Dictionary<long, IPlayingEntity> map = new Dictionary<long, IPlayingEntity>();
            foreach ((TPlayItem key, long value) in _tPlayItemToStartMap)
            {
                map.Add(value, key);
            }

            foreach (Transition transition in _transitionToTPlayItemMap.Keys)
            {
                map.Add(GetStartOfTransition(transition) - 1, transition);
            }

            //_transitionToTPlayItemMap.ForEach(a => map.Add(GetStartOfTransition(a.Key) - 1, a.Key));
            return map.OrderBy(a => a.Key).Select(a => a.Value);
        }

        public bool TryGetTransition(int tPlayItemId1, int tPlayItemId2, out Transition transition)
        {
            transition = _transitionToTPlayItemMap
                .FirstOrDefault(a => a.Value[0].Id == tPlayItemId1 && a.Value[1].Id == tPlayItemId2).Key;
            return transition != null;
        }

        public void Change(
            IEnumerable<KeyValuePair<TPlayItem, long>> addTPlayItem,
            IEnumerable<KeyValuePair<TPlayItem, long>> removeTPlayItem,
            IEnumerable<Transition> addTransition,
            IEnumerable<Transition> removeTransition,
            IEnumerable<KeyValuePair<TPlayItem, long>> changeTPlayItemStarts)
        {
            addTPlayItem.ForEach(a => Add(a.Key, a.Value));
            addTransition.ForEach(a => AddTransition(a));
            removeTPlayItem.ForEach(a => Remove(a.Key));
            removeTransition.ForEach(a => _transitionToTPlayItemMap.Remove(a));
            changeTPlayItemStarts.ForEach(a => SetStart(a.Key, a.Value));

            UpdateDuration();
        }
        
        public bool TryGetSegment(int playItemId, out IPlayingEntity playItem, out long start)
        {
            var pair = _tPlayItemToStartMap.FirstOrDefault(a => a.Key.Id == playItemId);
            if(pair.Key != null)
            {
                start = pair.Value;
                playItem = pair.Key;
                return true;
            }

            var pair1 = _transitionToTPlayItemMap.FirstOrDefault(a => a.Key.Id == playItemId);
            if(pair1.Value != null)
            {
                start = GetStartOfTransition(pair1.Key);
                playItem = pair1.Key;
                return true;
            }

            start = -1;
            playItem = null;
            return false;
        }

        public long GetStartOfPlayItem(int id)
        {
            var startToT = _tPlayItemToStartMap.FirstOrDefault(a => a.Key.Id == id);
            if(startToT.Key != null)
            {
                return startToT.Value;
            }

            var transition = _transitionToTPlayItemMap.FirstOrDefault(a => a.Key.Id == id).Key;
            return transition == null
                ? -1
                : GetStartOfTransition(transition);
        }

        private long CalculateTotalTicks()
        {
            if (!_tPlayItemToStartMap.Any())
            {
                return 0;
            }

            (TPlayItem key, long value) = _tPlayItemToStartMap.OrderBy(a => a.Value).Last();
            return value + key.TotalTicks;
        }
        
        public long GetTotalTicks()
        {
            return _totalTicks;
        }

        public IPlayingEntity GetItem(long tick)
        {
            List<KeyValuePair<TPlayItem, long>> ts = _tPlayItemToStartMap.Where(a => a.Value <= tick && a.Value + a.Key.TotalTicks >= tick).ToList();
            switch (ts.Count)
            {
                case 0:
                    return null;
                case 1:
                    return ts[0].Key;
                default:
                    {
                        TPlayItem[] keys = ts.Select(a => a.Key).ToArray();
                        if (keys.Length != 2)
                        {
                            return null;
                        }
                        TPlayItem pi1 = keys[0];
                        TPlayItem pi2 = keys[1];

                        KeyValuePair<Transition, TPlayItem[]> pe = _transitionToTPlayItemMap
                            .FirstOrDefault(a => a.Value.Contains(pi1) && a.Value.Contains(pi2));
                        
                        return pe.Key;
                    }
            }
        }

        private void SetStart(TPlayItem item, long newStart)
        {
            var oldStart = GetStartOfPlayItem(item.Id);
            if(oldStart != newStart)
            {
                _tPlayItemToStartMap[item] = newStart;
            }
        }

        private void Add(TPlayItem item, long start)
        {
            _tPlayItemToStartMap[item] = start;
            item.TotalTicksChanged += Item_TotalTicksChanged;
        }

        private void AddTransition(Transition transition)
        {
            var t1 = _tPlayItemToStartMap.FirstOrDefault(a => a.Key.Id == transition.GetFirstItemId()).Key;
            var t2 = _tPlayItemToStartMap.FirstOrDefault(a => a.Key.Id == transition.GetSecondItemId()).Key;

            if(t1 == null || t2 == null)
            {
                throw new Exception();
            }

            _transitionToTPlayItemMap.Add(transition, new[] {t1, t2 });
        }

        private void Remove(TPlayItem item)
        {
            _tPlayItemToStartMap.Remove(item);
            item.TotalTicksChanged -= Item_TotalTicksChanged;
        }
        
        private void Item_TotalTicksChanged(object sender, EventArgs e)
        {
            UpdateDuration();
        }

        private void UpdateDuration()
        {
            long newDuration = CalculateTotalTicks();
            if(Math.Abs(newDuration - _totalTicks) > 0L)
            {
                _totalTicks = newDuration;
                TotalTicksChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private long GetStartOfTransition(Transition transition)
        {
            TPlayItem eff2 = _transitionToTPlayItemMap[transition][1];
            return _tPlayItemToStartMap.FirstOrDefault(a => a.Key.Id == eff2.Id).Value;
        }
    }