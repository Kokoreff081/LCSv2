using LcsServer.Models.LCProjectModels.Models.ScenarioObjects;
using LCSVersionControl.Converters;
using LCSVersionControl.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.ScenarioObjects.Task;

[JsonConverter(typeof(BaseVcJsonConverter))]
[VcClass(JsonName = ModelClassName, Version = 1)]
public class TaskV1 : BaseVC
{
    private const string ModelClassName = "ScenarioTask";

    public int RasterId { get; set; }
    public bool UseParentRaster { get; set; }
    public int[] Units { get; set; }
    public long[] Starts { get; set; }

    public override ISaveLoad ToConcreteObject()
    {
        throw new Exception("It's old class version");
    }

    public override BaseVC FromConcreteObject(ISaveLoad o)
    {
        ScenarioTask scenarioTask = (ScenarioTask) o;

        List<int> units = new List<int>();
        List<long> starts = new List<long>();

        foreach (KeyValuePair<long, LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Unit> keyValuePair in scenarioTask.StartToEntityDictionary)
        {
            units.Add(keyValuePair.Value.Id);
            starts.Add(keyValuePair.Key);
        }

        TaskV1 scenarioTaskVc = new TaskV1
        {
            Id = scenarioTask.Id,
            Name = scenarioTask.Name,
            ParentId = scenarioTask.ParentId,

            Units = units.ToArray(),
            Starts = starts.ToArray(),
            UseParentRaster = scenarioTask.UseParentRaster,
            RasterId = scenarioTask.Raster?.Id ?? 0,
        };

        return scenarioTaskVc;
    }
}