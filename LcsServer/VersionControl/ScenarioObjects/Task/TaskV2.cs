using LcsServer.Models.LCProjectModels.Models.ScenarioObjects;
using LCSVersionControl.Converters;
using LCSVersionControl.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.ScenarioObjects.Task;

[JsonConverter(typeof(BaseVcJsonConverter))]
    [VcClass(JsonName = ModelClassName, Version = 2)]
    public class TaskV2 : BaseVC
    {
        private const string ModelClassName = "ScenarioTask";

        public int RasterId { get; set; }
        public bool UseParentRaster { get; set; }
        public int[] Units { get; set; }
        public long[] Starts { get; set; }
        public long RiseTime { get; set; }
        public long FadeTime { get; set; }
        public bool IsEnabled { get; set; } = true;

        public override ISaveLoad ToConcreteObject()
        {
            throw new Exception("Cannot instantiate old class version");
        }

        public override BaseVC FromConcreteObject(ISaveLoad o)
        {
            if (o is not ScenarioTask scenarioTask)
            {
                return null;
            }

            List<int> units = new List<int>();
            List<long> starts = new List<long>();

            foreach (KeyValuePair<long, LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Unit> keyValuePair in scenarioTask
                         .StartToEntityDictionary)
            {
                units.Add(keyValuePair.Value.Id);
                starts.Add(keyValuePair.Key);
            }

            TaskV2 scenarioTaskVc = new TaskV2 {
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
            };

            return scenarioTaskVc;

        }

        public override void FromPrevious(BaseVC baseVC)
        {
            if (baseVC is not TaskV1 taskV1)
            {
                return;
            }

            base.FromPrevious(baseVC);

            Units = taskV1.Units;
            Starts = taskV1.Starts;
            UseParentRaster = taskV1.UseParentRaster;
            RasterId = taskV1.RasterId;
        }
    }