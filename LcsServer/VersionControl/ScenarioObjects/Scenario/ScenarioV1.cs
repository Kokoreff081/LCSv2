using LCSVersionControl.Converters;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.ScenarioObjects.Scenario;

[JsonConverter(typeof(BaseVcJsonConverter))]
[VcClass(JsonName = ModelClassName, Version = 1)]
public class ScenarioV1 : BaseVC
{
    private const string ModelClassName = "Scenario";

    public int[] TasksIds { get; set; }
    public int RasterId { get; set; }

    public override ISaveLoad ToConcreteObject()
    {
        throw new Exception("It's old class version");
    }

    public override BaseVC FromConcreteObject(ISaveLoad o)
    {
        LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Scenario scenario = (LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Scenario)o;

        ScenarioV1 scenarioVc = new ScenarioV1
        {
            Id = scenario.Id,
            Name = scenario.Name,
            TasksIds = scenario.ScenarioTasks.Select(x => x.Id).ToArray(),
            RasterId = scenario.Raster == null ? 0 : scenario.Raster.Id,
        };

        return scenarioVc;
    }
}