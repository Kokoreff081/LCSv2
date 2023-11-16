using LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects;
using LCSVersionControl.Converters;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.ScenarioObjects.Track;

[JsonConverter(typeof(BaseVcJsonConverter))]
[VcClass(JsonName = ModelClassName, Version = 3)]
public class TrackV3 : BaseVC
{
    private const string ModelClassName = "Track";

    public long[] Starts { get; set; }
    public int[] EffectIds { get; set; }
    public int RasterId { get; set; }
    public bool UseParentRaster { get; set; }
    public long RiseTime { get; set; }
    public long FadeTime { get; set; }
    public bool IsEnabled { get; set; } = true;
    public float DimmingLevel { get; set; } = 1.0f;

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

        TrackV3 trackVc = new TrackV3 {
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
            DimmingLevel = track.DimmingLevel
        };

        return trackVc;

    }

    public override void FromPrevious(BaseVC baseVC)
    {
        if (baseVC is not TrackV2 trackV2)
        {
            return;
        }

        base.FromPrevious(baseVC);

        Starts = trackV2.Starts;
        EffectIds = trackV2.EffectIds;
        RasterId = trackV2.RasterId;
        UseParentRaster = trackV2.UseParentRaster;
    }
}