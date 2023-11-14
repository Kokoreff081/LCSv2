using System.Diagnostics;
using LcsServer.Models.LCProjectModels.GlobalBase;
using LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;
using LcsServer.Models.LCProjectModels.GlobalBase.Utils;
using LCSVersionControl.Interfaces;

namespace LcsServer.Models.LCProjectModels.Models.ScenarioObjects;

/// <summary>
    /// Базовый класс Последовательность объектов
    /// У каждого объекта есть свой индивидуальный старт
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Sequence<T> : PlayingEntityWithRaster, ISequence<T> where T : PlayingEntity
    {
        private readonly SequenceOfPlayingItems<T> _sequence;

        /// <summary>
        /// Коллекция изменена
        /// </summary>
        public event EventHandler<SequenceEventArgs> SequenceChanged;

        protected Sequence()
        {
            _sequence = new SequenceOfPlayingItems<T>();
            _sequence.TotalTicksChanged += OnTotalTicksChanged; 
        }

        protected Sequence(int id, string name, long riseTime, long fadeTime, float dimmingLevel) : base(id, name,
            riseTime, fadeTime, dimmingLevel)
        {
            _sequence = new SequenceOfPlayingItems<T>();
            _sequence.TotalTicksChanged += OnTotalTicksChanged; 
        }

        ~Sequence()
        {
            _sequence.TotalTicksChanged -= OnTotalTicksChanged; 
        }

        /// <summary>
        /// Получение объектов последовательности с их стартами
        /// </summary>
        public IReadOnlyDictionary<long, T> StartToEntityDictionary => _sequence.StartToTPlayItemMap;

        /// <summary>
        /// Получение переходов
        /// </summary>
        public IReadOnlyList<Transition> Transitions => _sequence.Transitions;

        /// <summary>
        /// Получение объектов последовательности
        /// </summary>
        public IReadOnlyList<T> Entities => _sequence.TPlayItems;

        /// <summary>
        /// Изменить коллекцию
        /// </summary>
        /// <param name="addEntities"> Добавить объекты </param>
        /// <param name="removeEntities"> Удалить объекты </param>
        /// <param name="addTransition"> Добавить переходы </param>
        /// <param name="removeTransition"> Удалить переходы </param>
        /// <param name="changeEntitiesStarts"> Изменить старты объектов </param>
        public void ChangeSequence(
            IEnumerable<KeyValuePair<T, long>> addEntities,
            IEnumerable<KeyValuePair<T, long>> removeEntities,
            IEnumerable<Transition> addTransition,
            IEnumerable<Transition> removeTransition,
            IEnumerable<KeyValuePair<T, long>> changeEntitiesStarts)
        {
            addEntities = addEntities ?? new KeyValuePair<T, long>[0];
            removeEntities = removeEntities ?? new KeyValuePair<T, long>[0];
            addTransition = addTransition ?? new Transition[0];
            removeTransition = removeTransition ?? new Transition[0];
            changeEntitiesStarts = changeEntitiesStarts ?? new KeyValuePair<T, long>[0];

            //UnSubscription();

            _sequence.Change(addEntities, removeEntities, addTransition, removeTransition, changeEntitiesStarts);

            addEntities.ForEach(pair => pair.Key.Parent = this);
            addTransition.ForEach(tr => tr.Parent = this);

            //Subscription();

            UpdateRasterInChildren();

            var args = new SequenceEventArgs(addEntities.Any() || addTransition.Any(),
                removeEntities.Any() || removeTransition.Any(), changeEntitiesStarts.Any());
            SequenceChanged?.Invoke(this, args);
        }

        public override bool TryGetFrame(long tick, out ScenarioFrame frame, bool needCalculateDefaultSize,
            float riseOrFadeLevel)
        {
            frame = null;

            if (_sequence.TPlayItems.Count == 0)
                return false;

            if (!IsEnabled)
            {
                return false;//GetEmptyFrame(tick, out frame);
            }

            var playItem = _sequence.GetItem(tick);

            float totalRiseOrFadeLevel = riseOrFadeLevel * GetLevelByRiseAndFade(tick);

            if (playItem != null)
                playItem.TryGetFrame(tick - _sequence.GetStartOfPlayItem(playItem.Id), out frame,
                    CanCalculateDefaultSize && needCalculateDefaultSize, totalRiseOrFadeLevel);

            return frame != null;
        }

        // public override bool GetEmptyFrame(long tick, out ScenarioFrame frame)
        // {
        //     frame = null;
        //
        //     var playItem = _sequence.GetItem(tick);
        //
        //     if (playItem != null)
        //         playItem.GetEmptyFrame(tick - _sequence.GetStartOfPlayItem(playItem.Id), out frame);
        //
        //     return frame != null;
        // }

        private void MergeToFrame1(ScenarioFrame frame1, ScenarioFrame frame2)
        {
            var map1 = frame1.GetDictionary();
            var map2 = frame2.GetDictionary();
            foreach (var pair in map2)
            {
                map1[pair.Key] = pair.Value;
            }
        }

        /// <summary>
        /// Получить последовательность объектов с переходами
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IPlayingEntity> GetSequence()
        {
            return _sequence.GetSequence();
        }

        /// <summary>
        /// Получить Старт объекта
        /// </summary>
        /// <param name="itemId"> Id объекта </param>
        /// <returns> Старт </returns>
        public long GetStartOfPlayItem(int itemId)
        {
            return _sequence.GetStartOfPlayItem(itemId);
        }

        /// <summary>
        /// Попытка получить проигрываемый объект и его Старт
        /// </summary>
        /// <param name="playItemId"> Id объекта </param>
        /// <param name="playItem"> Проигрываемый элемент </param>
        /// <param name="start"> Старт </param>
        /// <returns> Результат поаытки </returns>
        public bool TryGetPlayItem(int playItemId, out IPlayingEntity playItem, out long start)
        {
            return _sequence.TryGetSegment(playItemId, out playItem, out start);
        }

        /// <summary>
        /// Попытка получить Переход
        /// </summary>
        /// <param name="tPlayItemId1"> Id объекта </param>
        /// <param name="tPlayItemId2"> Id объекта  </param>
        /// <param name="transition"> Переход </param>
        /// <returns> Результат поиска перехода </returns>
        public bool TryGetTransition(int tPlayItemId1, int tPlayItemId2, out Transition transition)
        {
            return _sequence.TryGetTransition(tPlayItemId1, tPlayItemId2, out transition);
        }

        /// <summary>
        /// Загрузить коллекция из примитивов
        /// </summary>
        /// <param name="primitives"> Коллекция примитивов </param>
        public abstract void SetSequenceFromLoad(IEnumerable<PlayingEntity> primitives);

        protected abstract bool CanCalculateDefaultSize { get; }

        protected void SetSequenceFromLoad(IEnumerable<PlayingEntity> primitives, long[] starts, int[] entitiesIds)
        {
            if (starts != null && entitiesIds != null)
            {
                if (starts.Length != entitiesIds.Length)
                {
                    throw new LoadException(
                        $"Can not load Sequence (id='{Id}'): The length of 'starts' differs from the length of 'entitiesIds'.");
                }

                var playItems = primitives.Where(a => a.Parent == this);
                var transitions = playItems.OfType<Transition>();
                var ts = playItems.OfType<T>();

                Dictionary<T, long> addTs = new Dictionary<T, long>(starts.Length);

                for (int i = 0; i < starts.Length; i++)
                {
                    T t = ts.First(a => a.Id == entitiesIds[i]);
                    addTs[t] = starts[i];
                }

                ChangeSequence(addTs, null, transitions, null, null);
            }
        }

        protected void SetOwnerSequence(Sequence<T> sequenceItem2, BaseLCObjectsManager lcObjectsManager)
        {
            var ownerIdToClone = new Dictionary<int, T>();
            var addEntities = new Dictionary<T, long>();
            foreach (var pair in _sequence.StartToTPlayItemMap)
            {
                var entity = pair.Value;
                if (entity is not IClone clonable) continue;

                var clone = (T)clonable.Clone(lcObjectsManager, sequenceItem2);
                clone.Name = entity.Name;

                var start = pair.Key;

                addEntities.Add(clone, start);
                ownerIdToClone[entity.Id] = clone;
            }

            List<Transition> addTransitions = new List<Transition>();
            foreach (Transition transition in Transitions)
            {
                int firstId = transition.GetFirstItemId();
                int secondId = transition.GetSecondItemId();

                if (!ownerIdToClone.TryGetValue(firstId, out T ent1) || !ownerIdToClone.TryGetValue(secondId, out T ent2))
                {
                    Debug.WriteLine("SetOwnerSequence: wrong transition");
                    continue;
                }

                Transition clone = new Transition(ent1, ent2, transition.IntersectionPoint) {
                    Parent = ent1.Parent,
                    Id = lcObjectsManager.GetNewCurrentId(),
                    Name = lcObjectsManager.GetNewName("Transition")
                };

                addTransitions.Add(clone);
            }

            sequenceItem2.ChangeSequence(addEntities, null, addTransitions, null, null);
        }

        protected override long GetTotalTicks()
        {
            return _sequence.GetTotalTicks();
        }

        protected void GetSequenceForSave(out long[] starts, out int[] entitiesIds)
        {
            var startToEntity = _sequence.StartToTPlayItemMap;
            entitiesIds = startToEntity.Select(a => a.Value.Id).ToArray();
            starts = startToEntity.Keys.ToArray();
        }


        private void Subscription()
        {
            _sequence.TotalTicksChanged += OnTotalTicksChanged;
        }

        private void UnSubscription()
        {
            _sequence.TotalTicksChanged -= OnTotalTicksChanged;
        }

        private void OnTotalTicksChanged(object sender, EventArgs e)
        {
            TotalTicksNotify();
        }
    }
    
public class LoadException : ApplicationException
{
    public LoadException(string message) : base(message) { }
        
}