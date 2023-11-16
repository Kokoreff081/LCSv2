using LCSVersionControl.Converters;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.ScenarioObjects.Transition;

[JsonConverter(typeof(BaseVcJsonConverter))]
[VcClass(JsonName = ModelClassName, Version = 1)]
public class TransitionV1 : BaseVC
{
    private const string ModelClassName = "Transition";

    public int FirstEntityId { get; set; }
    public int SecondEntityId { get; set; }
    public byte TransitionType { get; set; }
    public long IntersectionPoint { get; set; }

    public override ISaveLoad ToConcreteObject()
    {
        throw new Exception("Cannot instantiate old class version");
    }

    public override BaseVC FromConcreteObject(ISaveLoad o)
    {
        if (o is not LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Transition transition)
        {
            return null;
        }

        TransitionV1 transitionVc = new TransitionV1
        {
            Id = transition.Id,
            Name = transition.Name,
            ParentId = transition.ParentId,
            IntersectionPoint = transition.IntersectionPoint,
            FirstEntityId = transition.GetFirstItemId(),
            SecondEntityId = transition.GetSecondItemId(),
            TransitionType = (byte)transition.TransitionType,
        };

        return transitionVc;

    }
}