using LCSVersionControl.Converters;
using LCSVersionControl.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.ScenarioObjects.Scenario;

[JsonConverter(typeof(BaseVcJsonConverter))]
[VcClass(JsonName = ModelClassName, Version = 2)]
public class ScenarioV2 : BaseVC
{
    private const string ModelClassName = "Scenario";

    public int[] TasksIds { get; set; }
    public int RasterId { get; set; }
    public long RiseTime { get; set; }
    public long FadeTime { get; set; }
    public bool IsEnabled { get; set; } = true;

    public override ISaveLoad ToConcreteObject()
    {
        throw new Exception("Cannot instantiate old class version");
    }

    public override BaseVC FromConcreteObject(ISaveLoad o)
    {
        if (o is not LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Scenario scenario)
        {
            return null;
        }

        ScenarioV2 scenarioVc = new ScenarioV2 {
            Id = scenario.Id,
            Name = scenario.Name,
            TasksIds = scenario.ScenarioTasks.Select(x => x.Id).ToArray(),
            RasterId = scenario.Raster?.Id ?? 0,
            RiseTime = scenario.RiseTime,
            FadeTime = scenario.FadeTime,
            IsEnabled = scenario.IsEnabled,
        };

        return scenarioVc;

    }

    public override void FromPrevious(BaseVC baseVC)
    {
        if (baseVC is not ScenarioV1 scenarioV1)
        {
            return;
        }

        base.FromPrevious(baseVC);
        TasksIds = scenarioV1.TasksIds;
        RasterId = scenarioV1.RasterId;
    }
}