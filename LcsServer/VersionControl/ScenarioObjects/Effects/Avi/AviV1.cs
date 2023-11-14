using LCSVersionControl.Converters;
using LCSVersionControl.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.ScenarioObjects.Effects.Avi;

[JsonConverter(typeof(BaseVcJsonConverter))]
[VcClass(JsonName = ModelClassName, Version = 1)]
public class AviV1 : BaseVC
{
    private const string ModelClassName = "Avi";

    public string FileName { get; set; }
    public long TotalTicks { get; set; }
    public int FromFrame { get; set; }
    public int ToFrame { get; set; }

    public override ISaveLoad ToConcreteObject()
    {
        throw new Exception("It's old class version");
    }

    public override BaseVC FromConcreteObject(ISaveLoad o)
    {
        LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.Avi avi = (LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.Avi)o;

        AviV1 aviVc = new AviV1()
        {
            Id = avi.Id,
            Name = avi.Name,
            ParentId = avi.ParentId,
            FileName = avi.FileName,
            FromFrame = avi.FromFrame,
            ToFrame = avi.ToFrame,
            TotalTicks = avi.TotalTicks
        };

        return aviVc;
    }
}