using LCSVersionControl.Converters;
using LCSVersionControl.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.ScenarioObjects.Effects.Avi;

[JsonConverter(typeof(BaseVcJsonConverter))]
[VcClass(JsonName = ModelClassName, Version = 3)]
public class AviV3 : BaseVC
{
    private const string ModelClassName = "Avi";

    public string FileName { get; set; }
    public long TotalTicks { get; set; }
    public int FromFrame { get; set; }
    public int ToFrame { get; set; }
    public long RiseTime { get; set; }
    public long FadeTime { get; set; }
    public bool FitToRaster { get; set; } = true;
    public float DimmingLevel { get; set; } = 1.0f;

    public override ISaveLoad ToConcreteObject()
    {
        LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.Avi avi = new LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.Avi(Id, Name, ParentId, FileName,
            FromFrame, ToFrame, TotalTicks, RiseTime, FadeTime, FitToRaster, DimmingLevel);

        return avi;
    }

    public override BaseVC FromConcreteObject(ISaveLoad o)
    {
        if (o is not LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.Avi avi)
        {
            return null;
        }

        AviV3 aviVc = new AviV3
        {
            Id = avi.Id,
            Name = avi.Name,
            ParentId = avi.ParentId,
            FileName = avi.FileName,
            FromFrame = avi.FromFrame,
            ToFrame = avi.ToFrame,
            TotalTicks = avi.TotalTicks,
            RiseTime = avi.RiseTime,
            FadeTime = avi.FadeTime,
            FitToRaster = avi.FitToRaster,
            DimmingLevel = avi.DimmingLevel
        };

        return aviVc;

    }

    public override void FromPrevious(BaseVC baseVC)
    {
        if (baseVC is not AviV2 aviV2)
        {
            return;
        }

        base.FromPrevious(baseVC);

        FileName = aviV2.FileName;
        TotalTicks = aviV2.TotalTicks;
        FromFrame = aviV2.FromFrame;
        ToFrame = aviV2.ToFrame;
        RiseTime = aviV2.RiseTime;
        FadeTime = aviV2.FadeTime;
    }
}