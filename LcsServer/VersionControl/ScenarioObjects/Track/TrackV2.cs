using LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects;
using LCSVersionControl.Converters;
using LCSVersionControl.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.ScenarioObjects.Track;

[JsonConverter(typeof(BaseVcJsonConverter))]
    [VcClass(JsonName = ModelClassName, Version = 2)]
    public class TrackV2 : BaseVC
    {
        private const string ModelClassName = "Track";

        public long[] Starts { get; set; }
        public int[] EffectIds { get; set; }
        public int RasterId { get; set; }
        public bool UseParentRaster { get; set; }
        public long RiseTime { get; set; }
        public long FadeTime { get; set; }
        public bool IsEnabled { get; set; } = true;

        public override ISaveLoad ToConcreteObject()
        {
            throw new Exception("Cannot instantiate old class version");
        }

        public override BaseVC FromConcreteObject(ISaveLoad o)
        {
            if (o is not LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Track track)
            {
                return null;
            }

            List<int> effects = new List<int>();
            List<long> starts = new List<long>();
                
            foreach ((long key, Effect value) in track.StartToEntityDictionary)
            {
                effects.Add(value.Id);
                starts.Add(key);
            }

            TrackV2 trackVc = new TrackV2 {
                Id = track.Id,
                Name = track.Name,
                ParentId = track.ParentId,
                Starts = starts.ToArray(),
                EffectIds = effects.ToArray(),
                RasterId = track.Raster?.Id ?? 0,
                UseParentRaster = track.UseParentRaster,
                RiseTime = track.RiseTime,
                FadeTime = track.FadeTime,
                IsEnabled = track.IsEnabled,
            };

            return trackVc;

        }

        public override void FromPrevious(BaseVC baseVC)
        {
            if (baseVC is not TrackV1 trackV1)
            {
                return;
            }

            base.FromPrevious(baseVC);

            Starts = trackV1.Starts;
            EffectIds = trackV1.EffectIds;
            RasterId = trackV1.RasterId;
            UseParentRaster = trackV1.UseParentRaster;
        }
    }