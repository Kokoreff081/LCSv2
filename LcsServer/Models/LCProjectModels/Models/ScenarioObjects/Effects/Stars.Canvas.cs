using LcsServer.Models.LCProjectModels.GlobalBase;
using LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;
using LcsServer.Models.LCProjectModels.Models.Rasters;
using LCSVersionControl;

namespace LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects;

public partial class Stars
    {
        private class Canvas
        {
            private int _activestarCount;
            private bool _isColorMix;
            private bool _modulation;
            private List<CompositeColor> _stack;
            //private CompositeColor _baseColor;

            //private Dictionary<int, List<IntPoint>> _groups;
            //private readonly Dictionary<int, Star> _stars;
            private List<Star> _starsnew;
            private readonly List<IntPoint> _starpoints;
            private readonly List<CompositeColor> _starcolors;
            private int _curstarindex;

            private int _filing;

            public Canvas() : this(new List<LampProjection>()) { }

            public Canvas(IReadOnlyList<LampProjection> projection)
            {
                Projection = projection;
                
                //_groups = new Dictionary<int, List<IntPoint>>();
                //_stars = new Dictionary<int, Star>();
                _starpoints = new List<IntPoint>();
                _starsnew = new List<Star>();
                _starcolors = new List<CompositeColor>();
                ResetStars = true;
            }
            
            public IReadOnlyList<LampProjection> Projection { get; private set; }
            public bool ResetStars { get; set; }
            
            public void Reset(int filing, bool modulation, bool isColorMix, List<CompositeColor> colors)
            {
                //_stars.Clear();
                _starsnew.Clear();

                _isColorMix = isColorMix;
                _stack = colors;
                _modulation = modulation;
                _filing = filing;
                
                InitStarPoints();
                ResetStars = false;
            }

            public void Tick(int frames)
            {
                //_baseColor = _stack[LCRandom.RandomNext(_stack.Count+1)];

                while (_starsnew.Count < _activestarCount)
                {
                    var lifeTime = GetRandomTime(frames);
                    var color = GetColor(_curstarindex);
                    var position = _starpoints[_curstarindex];
                    _curstarindex++;
                    if (_curstarindex >= _starpoints.Count)
                        _curstarindex = 0;
                    _starsnew.Add(new Star(color, _modulation, lifeTime, position));
                }

                var stars = new List<Star>();
                foreach(var star in _starsnew)
                {
                    bool isDead = star.Quant();

                    if(!isDead)
                    {
                        stars.Add(star);
                    }
                }

                _starsnew = stars;
            }

            public void FillFrameByStars(EffectFrame frame, float level)
            {
                foreach(var star in _starsnew)
                {
                    frame.Colors[star.Position] = star.GetColor() * level;
                }
            }

            private void InitStarPoints()
            {
                //_groups.Clear();
                _starpoints.Clear();
                _starsnew.Clear();
                _starcolors.Clear();
                var sorted = Projection.SelectMany(a => a.Points).ToList();
                while (sorted.Count>1)
                {
                    var index = LCRandom.RandomNext(sorted.Count + 1);
                    _starpoints.Add(sorted[index]);
                    sorted.RemoveAt(index);
                }
                _starpoints.AddRange(sorted);
                Enumerable.Range(0, _starpoints.Count).ForEach(_ => _starcolors.Add(_stack[LCRandom.RandomNext(_stack.Count+1)]));

                _activestarCount = (int)Math.Ceiling(_starpoints.Count * _filing * 0.01f);
                _curstarindex = 0;
            }


            private int GetRandomTime(int frames)
            {
                var time = frames / 10 + LCRandom.RandomNext(frames * 9 / 10); //т.е. от 10 до 100%
                return time < 1 ? 1 : time;
            }

            private CompositeColor GetColor(int startindex)
            {
                if (startindex + 1 > _starcolors.Count)
                {
                    //Массив цветов еще пуст
                    return _stack[LCRandom.RandomNext(_stack.Count + 1)];
                }

                return _isColorMix ? _stack[LCRandom.RandomNext(_stack.Count + 1)] : _starcolors[startindex];
            }
        }
    }