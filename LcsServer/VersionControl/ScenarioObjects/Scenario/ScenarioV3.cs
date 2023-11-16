using LCSVersionControl.Converters;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.ScenarioObjects.Scenario;

[JsonConverter(typeof(BaseVcJsonConverter))]
[VcClass(JsonName = ModelClassName, Version = 3)]
public class ScenarioV3 : BaseVC
{
    private const string ModelClassName = "Scenario";

    public int[] TasksIds { get; set; }
    public int RasterId { get; set; }
    public long RiseTime { get; set; }
    public long FadeTime { get; set; }
    public bool IsEnabled{ get; set; } = true;
    public float DimmingLevel { get; set; } = 1.0f;

    public override ISaveLoad ToConcreteObject()
    {
        LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Scenario scenario =
            new LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Scenario(Id, Name, TasksIds, RasterId, RiseTime, FadeTime, IsEnabled, DimmingLevel);

        return scenario;
    }

    public override BaseVC FromConcreteObject(ISaveLoad o)
    {
        if (o is not LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Scenario scenario)
        {
            return null;
        }

        ScenarioV3 scenarioVc = new ScenarioV3 {
            Id = scenario.Id,
            Name = scenario.Name,
            TasksIds = scenario.ScenarioTasks.Select(x => x.Id).ToArray(),
            RasterId = scenario.Raster?.Id ?? 0,
            RiseTime = scenario.RiseTime,
            FadeTime = scenario.FadeTime,
            IsEnabled = scenario.IsEnabled,
            DimmingLevel = scenario.DimmingLevel
        };

        return scenarioVc;
    }

    public override void FromPrevious(BaseVC baseVC)
    {
        if (baseVC is not ScenarioV2 scenarioV2)
        {
            return;
        }

        base.FromPrevious(baseVC);
        TasksIds = scenarioV2.TasksIds;
        RasterId = scenarioV2.RasterId;
        RiseTime = scenarioV2.RiseTime;
        FadeTime = scenarioV2.FadeTime;
    }
}