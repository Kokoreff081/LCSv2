using LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;
using LCSVersionControl.Converters;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.ScenarioObjects.Transition;

[JsonConverter(typeof(BaseVcJsonConverter))]
[VcClass(JsonName = ModelClassName, Version = 2)]
public class TransitionV2 : BaseVC
{
    private const string ModelClassName = "Transition";

    public int FirstEntityId { get; set; }
    public int SecondEntityId { get; set; }
    public byte TransitionType { get; set; }
    public long IntersectionPoint { get; set; }
    public float DimmingLevel { get; set; } = 1.0f;

    public override ISaveLoad ToConcreteObject()
    {
        TransitionType transitionType = (TransitionType)Enum.Parse(typeof(TransitionType), TransitionType.ToString());

        LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Transition transition = new LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Transition(Id, Name, ParentId, IntersectionPoint,
            FirstEntityId, SecondEntityId, transitionType, DimmingLevel);

        return transition;
    }

    public override BaseVC FromConcreteObject(ISaveLoad o)
    {
        if (o is not LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Transition transition)
        {
            return null;
        }

        TransitionV2 transitionVc = new TransitionV2
        {
            Id = transition.Id,
            Name = transition.Name,
            ParentId = transition.ParentId,
            IntersectionPoint = transition.IntersectionPoint,
            FirstEntityId = transition.GetFirstItemId(),
            SecondEntityId = transition.GetSecondItemId(),
            TransitionType = (byte)transition.TransitionType,
            DimmingLevel = transition.DimmingLevel
        };

        return transitionVc;

    }

    public override void FromPrevious(BaseVC baseVC)
    {
        if (baseVC is not TransitionV1 transitionV1)
        {
            return;
        }
        
        base.FromPrevious(baseVC);

        IntersectionPoint = transitionV1.IntersectionPoint;
        FirstEntityId = transitionV1.FirstEntityId;
        SecondEntityId = transitionV1.SecondEntityId;
        TransitionType = transitionV1.TransitionType;

    }
}