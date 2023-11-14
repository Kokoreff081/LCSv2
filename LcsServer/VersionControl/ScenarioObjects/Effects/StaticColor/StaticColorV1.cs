using LCSVersionControl.Converters;
using LCSVersionControl.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.ScenarioObjects.Effects.StaticColor;

[JsonConverter(typeof(BaseVcJsonConverter))]
[VcClass(JsonName = ModelClassName, Version = 1)]
public class StaticColorV1 : BaseVC
{
    private const string ModelClassName = "StaticColor";

    public string Background { get; set; }
    public long TotalTicks { get; set; }

    public override ISaveLoad ToConcreteObject()
    {
        throw new Exception("It's old class version");
    }

    public override BaseVC FromConcreteObject(ISaveLoad o)
    {
        LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.StaticColor staticColor = (LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.StaticColor) o;

        StaticColorV1 staticColorVc = new StaticColorV1
        {
            Id = staticColor.Id,
            Name = staticColor.Name,
            ParentId =  staticColor.ParentId,
            Background = staticColor.BackgroundColor.ToString(),
            TotalTicks = staticColor.TotalTicks,
        };

        return staticColorVc;
    }
}