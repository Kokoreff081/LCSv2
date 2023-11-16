using LCSVersionControl.Converters;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.ScenarioObjects.Effects.Avi;

[JsonConverter(typeof(BaseVcJsonConverter))]
[VcClass(JsonName = ModelClassName, Version = 2)]
public class AviV2 : BaseVC
{
    private const string ModelClassName = "Avi";

    public string FileName { get; set; }
    public long TotalTicks { get; set; }
    public int FromFrame { get; set; }
    public int ToFrame { get; set; }
    public long RiseTime { get; set; }
    public long FadeTime { get; set; }
    public bool FitToRaster { get; set; } = true;

    public override ISaveLoad ToConcreteObject()
    {
        throw new Exception("Cannot instantiate old class version");
    }

    public override BaseVC FromConcreteObject(ISaveLoad o)
    {
        LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.Avi avi = (LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.Avi)o;

        AviV2 aviVc = new AviV2()
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
        };

        return aviVc;
    }

    public override void FromPrevious(BaseVC baseVC)
    {
        if (baseVC is not AviV1 aviV1)
        {
            return;
        }

        base.FromPrevious(baseVC);

        FileName = aviV1.FileName;
        TotalTicks = aviV1.TotalTicks;
        FromFrame = aviV1.FromFrame;
        ToFrame = aviV1.ToFrame;
    }
}