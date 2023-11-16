using LcsServer.Models.LCProjectModels.Models.ScenarioObjects;
using LCSVersionControl.Converters;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.ScenarioObjects.Task;

[JsonConverter(typeof(BaseVcJsonConverter))]
[VcClass(JsonName = ModelClassName, Version = 3)]
public class TaskV3 : BaseVC
{
    private const string ModelClassName = "ScenarioTask";

    public int RasterId { get; set; }
    public bool UseParentRaster { get; set; }
    public int[] Units { get; set; }
    public long[] Starts { get; set; }
    public long RiseTime { get; set; }
    public long FadeTime { get; set; }
    public bool IsEnabled { get; set; } = true;
    public float DimmingLevel { get; set; } = 1.0f;

    public override ISaveLoad ToConcreteObject()
    {
        ScenarioTask scenarioTask = new ScenarioTask(Id, Name, ParentId, Units, Starts, UseParentRaster, RasterId,
            RiseTime, FadeTime, IsEnabled, DimmingLevel);

        return scenarioTask;
    }

    public override BaseVC FromConcreteObject(ISaveLoad o)
    {
        if (o is not ScenarioTask scenarioTask)
        {
            return null;
        }

        List<int> units = new List<int>();
        List<long> starts = new List<long>();

        foreach ((long key, LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Unit value) in scenarioTask.StartToEntityDictionary)
        {
            units.Add(value.Id);
            starts.Add(key);
        }

        TaskV3 scenarioTaskVc = new TaskV3 {
            Id = scenarioTask.Id,
            Name = scenarioTask.Name,
            ParentId = scenarioTask.ParentId,

            Units = units.ToArray(),
            Starts = starts.ToArray(),
            UseParentRaster = scenarioTask.UseParentRaster,
            RasterId = scenarioTask.Raster?.Id ?? 0,

            RiseTime = scenarioTask.RiseTime,
            FadeTime = scenarioTask.FadeTime,

            IsEnabled = scenarioTask.IsEnabled,
            DimmingLevel = scenarioTask.DimmingLevel
        };

        return scenarioTaskVc;
    }

    public override void FromPrevious(BaseVC baseVC)
    {
        if (baseVC is not TaskV2 taskV2)
        {
            return;
        }

        base.FromPrevious(baseVC);

        Units = taskV2.Units;
        Starts = taskV2.Starts;
        UseParentRaster = taskV2.UseParentRaster;
        RasterId = taskV2.RasterId;
        RiseTime = taskV2.RiseTime;
        FadeTime = taskV2.FadeTime;
    }
}