using LcsServer.Models.LCProjectModels.GlobalBase.Utils;
using LcsServer.Models.LCProjectModels.Models.Rasters;
using LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects;

namespace LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;

public class ScenarioFrame
    {
        private Dictionary<int, CompositeColor[]> _dictionary;

        public ScenarioFrame(EffectFrame frame, EffectFrame defaultFrame = null)
        {
            Default = defaultFrame;
            Raster = frame;
        }

        /// <summary>
        /// Дефолтный кадр (200 х 100)
        /// </summary>
        public EffectFrame Default { get; }

        /// <summary>
        /// Растерный кадр 
        /// </summary>
        public EffectFrame Raster { get; }

        /// <summary>
        /// Получение словаря цветов, где ключ это id лампы
        /// </summary>
        /// <returns></returns>
        public Dictionary<int, CompositeColor[]> GetDictionary()
        {
            if (_dictionary == null)
            {
                _dictionary = new Dictionary<int, CompositeColor[]>();
                foreach (var pr in Raster.Projection)
                {
                    _dictionary[pr.LampId] = GetColors(pr.Points, Raster.Colors);
                }
            }

            return _dictionary;
        }

        /// <summary>
        /// Смешивание кадров
        /// </summary>
        /// <param name="frame2"> Кадр 2 </param>
        /// <param name="mix"> Тип смешивания </param>
        /// <param name="opacity">прозрачность трека</param>
        public void Blend(ScenarioFrame frame2, Mixing mix, float opacity)
        {
            var dict = GetDictionary();

            foreach (var p in frame2.Raster.Projection)
            {
                var colors = GetColors(p.Points, frame2.Raster.Colors);
                if (dict.TryGetValue(p.LampId, out var compositeColors))
                {
                    for (int i = 0; i < colors.Length; i++)
                    {
                        compositeColors[i] = ColorBlending.Blend(compositeColors[i], colors[i], mix, opacity);
                    }
                }
                else
                {
                    dict[p.LampId] = colors;
                }
            }
        }

        /// <summary>
        /// Получить цвета для растра
        /// </summary>
        /// <param name="projection"> Проекция </param>
        /// <returns> Словарь цветов </returns>
        public Dictionary<IntPoint, CompositeColor> GetColors(IReadOnlyList<LampProjection> projection)
        {
            var dict = new Dictionary<IntPoint, CompositeColor>();

            if (_dictionary == null)
            {
                dict = Raster.Colors;
            }
            else
            {
                foreach (var pr in Raster.Projection)
                {
                    var key = pr.LampId;

                    var colors = _dictionary[key];
                    var points = pr.Points;

                    for (int i = 0; i < points.Count; i++)
                    {
                        dict[points[i]] = colors[i];
                    }
                }
            }

            return dict;
        }

        private static CompositeColor[] GetColors(IReadOnlyList<IntPoint> points, IReadOnlyDictionary<IntPoint, CompositeColor> map)
        {
            var colors = new CompositeColor[points.Count];

            for (int i = 0; i < points.Count; i++)
            {
                //var point = ;
                if (map.TryGetValue(points[i], out var compositeColor))
                    colors[i] = compositeColor;
                //else
                //    throw new ArithmeticException("GetColors. Key not found");
                //    Debug.WriteLine("GetColors. Key not found");
            }

            return colors;
        }

    }