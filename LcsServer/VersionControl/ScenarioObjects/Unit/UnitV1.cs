using LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;
using LCSVersionControl.Converters;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.ScenarioObjects.Unit;

[JsonConverter(typeof(BaseVcJsonConverter))]
[VcClass(JsonName = ModelClassName, Version = 1)]
public class UnitV1 : BaseVC
{
    private const string ModelClassName = "Unit";

    public int[] TrackIds { get; set; }
    public byte[] Mixing { get; set; }
    public int RasterId { get; set; }
    public bool UseParentRaster { get; set; }
    public bool AutoSize { get; set; }
    public long LocalTotalTicks { get; set; }

    public override ISaveLoad ToConcreteObject()
    {
        throw new Exception("It's old class version");
    }

    public override BaseVC FromConcreteObject(ISaveLoad o)
    {
        LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Unit unit = (LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Unit) o;

        List<int> tracks = new List<int>();
        List<byte> mixing = new List<byte>();

        foreach (KeyValuePair<LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Track, Mixing> keyValuePair in unit.TrackToMixingDictionary)
        {
            tracks.Add(keyValuePair.Key.Id);
            mixing.Add((byte)keyValuePair.Value);
        }

        UnitV1 unitVc = new UnitV1
        {
            Id = unit.Id,
            Name = unit.Name,
            ParentId = unit.ParentId,
            TrackIds = tracks.ToArray(),
            Mixing = mixing.ToArray(),
            AutoSize = unit.AutoSize,
            LocalTotalTicks = unit.LocalTotalTicks,
            RasterId = unit.Raster?.Id ?? 0,
            UseParentRaster = unit.UseParentRaster
        };

        return unitVc;
    }
}