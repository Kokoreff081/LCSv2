using LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;
using LCSVersionControl.Converters;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.ScenarioObjects.Unit;

[JsonConverter(typeof(BaseVcJsonConverter))]
[VcClass(JsonName = ModelClassName, Version = 3)]
public class UnitV3 : BaseVC
{
    private const string ModelClassName = "Unit";

    public int[] TrackIds { get; set; }
    public byte[] Mixing { get; set; }
    public int RasterId { get; set; }
    public bool UseParentRaster { get; set; }
    public bool AutoSize { get; set; }
    public long LocalTotalTicks { get; set; }
    public long RiseTime { get; set; }
    public long FadeTime { get; set; }
    public bool IsEnabled { get; set; } = true;
    public float DimmingLevel { get; set; } = 1.0f;


    public override ISaveLoad ToConcreteObject()
    {
        LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Unit unit = new LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Unit(Id, Name, ParentId, TrackIds, Mixing,
            AutoSize, LocalTotalTicks, RasterId, UseParentRaster, RiseTime, FadeTime, IsEnabled, DimmingLevel);

        return unit;
    }

    public override BaseVC FromConcreteObject(ISaveLoad o)
    {
        if (o is not LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Unit unit)
        {
            return null;
        }

        List<int> tracks = new List<int>();
        List<byte> mixing = new List<byte>();
        foreach ((LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Track key, Mixing value) in unit.TrackToMixingDictionary)
        {
            tracks.Add(key.Id);
            mixing.Add((byte)value);
        }

        UnitV3 unitVc = new UnitV3 {
            Id = unit.Id,
            Name = unit.Name,
            ParentId = unit.ParentId,
            TrackIds = tracks.ToArray(),
            Mixing = mixing.ToArray(),
            AutoSize = unit.AutoSize,
            LocalTotalTicks = unit.LocalTotalTicks,
            RasterId = unit.Raster?.Id ?? 0,
            UseParentRaster = unit.UseParentRaster,
            RiseTime = unit.RiseTime,
            FadeTime = unit.FadeTime,
            IsEnabled = unit.IsEnabled,
            DimmingLevel = unit.DimmingLevel
        };

        return unitVc;

    }

    public override void FromPrevious(BaseVC baseVC)
    {
        if (baseVC is not UnitV2 unitV2)
        {
            return;
        }

        base.FromPrevious(baseVC);

        TrackIds = unitV2.TrackIds;
        Mixing = unitV2.Mixing;
        AutoSize = unitV2.AutoSize;
        LocalTotalTicks = unitV2.LocalTotalTicks;
        RasterId = unitV2.RasterId;
        UseParentRaster = unitV2.UseParentRaster;
    }
}