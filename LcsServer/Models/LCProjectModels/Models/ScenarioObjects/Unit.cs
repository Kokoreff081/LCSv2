using LcsServer.Models.LCProjectModels.GlobalBase;
using LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;
using LCSVersionControl.Interfaces;

namespace LcsServer.Models.LCProjectModels.Models.ScenarioObjects;

 /// <summary>
    /// Класс, описывающий параллельную коллекция треков
    /// </summary>
    public class Unit : PlayingEntityWithRaster, ISaveLoad, IClone
    {
        private List<TrackMixing> _trackMixingColl;

        private long _localTotalTicks;
        private bool _autoSize;

        [SaveLoad] private readonly int[] _trackIds;
        [SaveLoad] private readonly byte[] _mixingColl;

        protected override IEnumerable<PlayingEntityWithRaster> ChildrenWithRaster =>
            _trackMixingColl.Select(a => a.Track);

        /// <summary>
        /// Событие изменения коллекции треков
        /// </summary>
        public event EventHandler TracksChanged;

        /// <summary>
        /// Событие изменение Авто-размера 
        /// </summary>
        //public event EventHandler AutoSizeChanged;

        public Unit()
        {
            Init();
        }

        public Unit(int id, string name, int parentId, int[] tracks, byte[] mixing, bool autoSize, long localTotalTicks,
            int rasterId, bool useParentRaster, long riseTime, long fadeTime, bool isEnabled, float dimmingLevel) :
            base(id, name, riseTime, fadeTime, dimmingLevel)
        {
            Init();

            _parentId = parentId;
            _trackIds = tracks;
            _mixingColl = mixing;
            AutoSize = autoSize;
            LocalTotalTicks = localTotalTicks;
            _saveRasterId = rasterId;
            UseParentRaster = useParentRaster;
            IsEnabled = isEnabled;
        }

        private void Init()
        {
            _trackMixingColl = new List<TrackMixing>();
            _localTotalTicks = Helper.DefaultUnitDuration;
            _autoSize = true;
        }

        ~Unit()
        {
            _trackMixingColl.ForEach(a => UnSubscription(a.Track));
        }

        /// <summary>
        /// Список треков и их типы смешивания
        /// </summary>
        public Dictionary<Track, Mixing> TrackToMixingDictionary =>
            _trackMixingColl.ToDictionary(a => a.Track, b => b.Mixing);

        /// <summary>
        /// Треки
        /// </summary>
        public IReadOnlyList<Track> Tracks => _trackMixingColl.ConvertAll(a => a.Track);

        /// <summary>
        /// Авто размер 
        /// </summary>
        [SaveLoad]
        public bool AutoSize
        {
            get => _autoSize;
            set => _autoSize = true;
        }

        /// <summary>
        /// Длительность при отключенном авторазмере
        /// </summary>
        [SaveLoad]
        public long LocalTotalTicks
        {
            get => _localTotalTicks;
            set
            {
                if (_localTotalTicks == value) return;

                _localTotalTicks = value;
                TotalTicksNotify();
            }
        }

        /// <summary>
        /// Добавление треков
        /// </summary>
        /// <param name="trackMixingInfos"> Коллекция треков с учетом смешивания и индексом добавления </param>
        public void Add(params TrackMixingInfo[] trackMixingInfos)
        {
            foreach (var t in trackMixingInfos.OrderBy(a => a.Index))
            {
                Add(t.Track, t.Mixing, t.Index);
            }

            trackMixingInfos?.ForEach(data => data.Track.Parent = this);

            TracksChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Удаление треков
        /// </summary>
        /// <param name="tracks"></param>
        public void Remove(params Track[] tracks)
        {
            foreach (var track in tracks)
            {
                UnSubscription(track);

                _trackMixingColl.RemoveAll(a => a.Track == track);
            }

            TracksChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Изменение типов смешивания у треков
        /// </summary>
        /// <param name="newMixingValues"> коллекция Трек - тип смешивания </param>
        public void Change(Dictionary<Track, Mixing> newMixingValues)
        {
            foreach ((Track track, Mixing mixing) in newMixingValues)
            {
                TrackMixing trInf = _trackMixingColl.Find(a => a.Track == track);
                if (trInf != null)
                {
                    trInf.Mixing = mixing;
                }
            }

            TracksChanged?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Получение индекса трека по id-трека
        /// </summary>
        /// <param name="id"> id-трека </param>
        /// <returns> индекс трека </returns>
        public int GetCollectionIndex(int id)
        {
            return _trackMixingColl.FindIndex(a => a.Track.Id == id);
        }

        public override bool TryGetFrame(long tick, out ScenarioFrame frame, bool calculateDefaultSize,
            float riseOrFadeLevel)
        {
            frame = null;

            float totalRiseOrFadeLevel = riseOrFadeLevel * GetLevelByRiseAndFade(tick);

            if (!IsEnabled)
            {
                return false;//GetEmptyFrame(tick, out frame);
            }

            var frames = new Dictionary<ScenarioFrame, (Mixing, Track)>(_trackMixingColl.Count);
            foreach (var trackInfo in _trackMixingColl)
            {
                if (trackInfo.Track.TryGetFrame(tick, out ScenarioFrame f, false, totalRiseOrFadeLevel))
                {
                    frames[f] = (trackInfo.Mixing, trackInfo.Track);
                }
            }

            if (frames.Count == 0) return false;

            var pair = frames.ElementAt(0);
            frame = pair.Key;

            var mo = pair.Value;
            for (int i = 1; i < frames.Count; i++)
            {
                var pair2 = frames.ElementAt(i);
                var frame2 = pair2.Key;
                frame.Blend(frame2, mo.Item1, pair2.Value.Item2.Opacity);
                mo = pair2.Value;
            }

            return true;
        }

        // public override bool GetEmptyFrame(long tick, out ScenarioFrame frame)
        // {
        //     frame = null;
        //
        //     var frames = new Dictionary<ScenarioFrame, Mixing>(_trackMixingColl.Count);
        //     foreach (var trackInfo in _trackMixingColl)
        //     {
        //         if (trackInfo.Track.GetEmptyFrame(tick, out ScenarioFrame f))
        //         {
        //             frames[f] = trackInfo.Mixing;
        //         }
        //     }
        //
        //     if (frames.Count == 0)
        //         return false;
        //
        //     var pair = frames.ElementAt(0);
        //     frame = pair.Key;
        //
        //     Mixing mix = pair.Value;
        //     for (int i = 1; i < frames.Count; i++)
        //     {
        //         var pair2 = frames.ElementAt(i);
        //         var frame2 = pair2.Key;
        //         frame.Blend(frame2, mix);
        //         mix = pair2.Value;
        //     }
        //
        //     return true;
        // }

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
            if (_trackIds == null && _mixingColl == null)
            {
                return;
            }

            IEnumerable<LCObject> lcObjects = primitives.OfType<LCObject>();
            LoadParent(lcObjects);

            if ((_trackIds == null || _mixingColl == null) || _mixingColl.Length != _trackIds.Length)
            {
                throw new LoadException(
                    $"Can not load Unit (id='{Id}'): The length of 'mixingColl' differs from the length of 'trackIds'.");
            }

            for (int i = 0; i < _mixingColl.Length; i++)
            {
                if (lcObjects.FirstOrDefault(a => a.Id == _trackIds[i]) is Track track)
                {
                    _trackMixingColl.Add(new TrackMixing(track, (Mixing)_mixingColl[i]));
                }
            }

            _trackMixingColl.ForEach(a => Subscription(a.Track));
        }

        /// <summary>
        /// Клонирование
        /// </summary>
        /// <param name="lcObjectsManager">Ссылка на менеджер объектов LCObjectsManager (для присвоения ID)</param>
        /// <param name="parent">Родитель</param>
        public LCObject Clone(BaseLCObjectsManager lcObjectsManager, LCObject parent)
        {
            var unitClone = new Unit {
                Parent = parent as IPlayingEntity,
                Id = lcObjectsManager.GetNewCurrentId(),
            };

            SetOwnerRastersData(unitClone);
            unitClone.AutoSize = AutoSize;
            unitClone.LocalTotalTicks = LocalTotalTicks;

            foreach (var item in _trackMixingColl)
            {
                var trackClone = (Track)item.Track.Clone(lcObjectsManager, unitClone);
                trackClone.Name = item.Track.Name;
                unitClone.Add(trackClone, item.Mixing);
            }

            unitClone.RiseTime = RiseTime;

            unitClone.FadeTime = FadeTime;

            return unitClone;
        }


        private void Add(Track track, Mixing mixing, int index = -1)
        {
            if (index < 0 || index > _trackMixingColl.Count)
            {
                _trackMixingColl.Add(new TrackMixing(track, mixing));
            }
            else
            {
                _trackMixingColl.Insert(index, new TrackMixing(track, mixing));
            }

            Subscription(track);

            if (track.UseParentRaster)
            {
                track.Raster = Raster;
            }
        }

        protected override long GetTotalTicks()
        {
            long totalTicks = 0;
            foreach (TrackMixing item in _trackMixingColl)
            {
                totalTicks = Math.Max(totalTicks, item.Track.TotalTicks);
            }

            return totalTicks;
        }

        private void Subscription(Track track)
        {
            track.TotalTicksChanged += OnTrackTotalTicksChanged;
            track.SequenceChanged += OnTracksChanged;
        }

        private void UnSubscription(Track track)
        {
            track.TotalTicksChanged -= OnTrackTotalTicksChanged;
            track.SequenceChanged -= OnTracksChanged;
        }

        private void OnTracksChanged(object sender, EventArgs e)
        {
            TracksChanged?.Invoke(this, EventArgs.Empty);
        }

        private void OnTrackTotalTicksChanged(object sender, EventArgs e)
        {
            TotalTicksNotify();
        }

        private class TrackMixing
        {
            public Track Track { get; }
            public Mixing Mixing { get; set; }

            public TrackMixing(Track track, Mixing mixing)
            {
                Track = track;
                Mixing = mixing;
            }
        }
    }