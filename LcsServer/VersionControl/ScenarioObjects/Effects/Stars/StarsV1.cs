using LCSVersionControl.Converters;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.ScenarioObjects.Effects.Stars;

[JsonConverter(typeof(BaseVcJsonConverter))]
[VcClass(JsonName = ModelClassName, Version = 1)]
public class StarsV1 : BaseVC
{
    private const string ModelClassName = "Stars";

    public long TotalTicks { get; set; }
    public string[] ColorStack { get; set; }
    public int Filling { get; set; }
    public int LifeTime { get; set; }
    public bool Modulation { get; set; }
    public bool IsColorMix { get; set; }
    public string Background { get; set; }

    public override ISaveLoad ToConcreteObject()
    {
        throw new Exception("It's old class version");
    }

    public override BaseVC FromConcreteObject(ISaveLoad o)
    {
        LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.Stars stars = (LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.Stars)o;

        string[] saveColorStack = new string[stars.ColorStack.Count];
        for (int i = 0; i < stars.ColorStack.Count; i++)
        {
            saveColorStack[i] = stars.ColorStack[i].ToString();
        }

        StarsV1 starsVc = new StarsV1
        {
            Id = stars.Id,
            Name = stars.Name,
            ParentId = stars.ParentId,
            Background = stars.Background.ToString(),
            ColorStack = saveColorStack,
            Filling = stars.Filling,
            IsColorMix = stars.IsColorMix,
            LifeTime = stars.LifeTime,
            Modulation = stars.Modulation,
            TotalTicks = stars.TotalTicks,
        };

        return starsVc;
    }
}