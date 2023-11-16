using LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects;
using LCSVersionControl.Converters;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.ScenarioObjects.Track;

[JsonConverter(typeof(BaseVcJsonConverter))]
[VcClass(JsonName = ModelClassName, Version = 1)]
public class TrackV1 : BaseVC
{
    private const string ModelClassName = "Track";

    public long[] Starts { get; set; }
    public int[] EffectIds { get; set; }
    public int RasterId { get; set; }
    public bool UseParentRaster { get; set; }

    public override ISaveLoad ToConcreteObject()
    {
        throw new Exception("It's old class version");
    }

    public override BaseVC FromConcreteObject(ISaveLoad o)
    {
        LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Track track = (LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Track) o;

        List<int> effects = new List<int>();
        List<long> starts = new List<long>();

        foreach (KeyValuePair<long, Effect> keyValuePair in track.StartToEntityDictionary)
        {
            effects.Add(keyValuePair.Value.Id);
            starts.Add(keyValuePair.Key);
        }

        TrackV1 trackVc = new TrackV1
        {
            Id = track.Id,
            Name =  track.Name,
            ParentId =  track.ParentId,
            Starts = starts.ToArray(),
            EffectIds = effects.ToArray(),
            RasterId = track.Raster?.Id ?? 0,
            UseParentRaster = track.UseParentRaster,
        };

        return trackVc;
    }
}