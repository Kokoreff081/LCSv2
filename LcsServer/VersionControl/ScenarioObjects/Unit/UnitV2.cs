using LcsServer.Models.LCProjectModels.GlobalBase.Scenarios;
using LCSVersionControl.Converters;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.ScenarioObjects.Unit;

[JsonConverter(typeof(BaseVcJsonConverter))]
    [VcClass(JsonName = ModelClassName, Version = 2)]
    public class UnitV2 : BaseVC
    {
        private const string ModelClassName = "Unit";

        public int[] TrackIds { get; set; }
        public byte[] Mixing { get; set; }
        public int RasterId { get; set; }
        public bool UseParentRaster { get; set; }
        public bool AutoSize { get; set; }
        public long LocalTotalTicks { get; set; }
        public long RiseTime { get; set; }
        public long FadeTime { get; set; }
        public bool IsEnabled { get; set; } = true;


        public override ISaveLoad ToConcreteObject()
        {
            throw new Exception("Cannot instantiate old class version");
        }

        public override BaseVC FromConcreteObject(ISaveLoad o)
        {
            LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Unit unit = (LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Unit) o;

            List<int> tracks = new List<int>();
            List<byte> mixing = new List<byte>();

            foreach (KeyValuePair<LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Track, Mixing> keyValuePair in unit.TrackToMixingDictionary)
            {
                tracks.Add(keyValuePair.Key.Id);
                mixing.Add((byte)keyValuePair.Value);
            }

            UnitV2 unitVc = new UnitV2
            {
                Id = unit.Id,
                Name = unit.Name,
                ParentId = unit.ParentId,
                TrackIds = tracks.ToArray(),
                Mixing = mixing.ToArray(),
                AutoSize = unit.AutoSize,
                LocalTotalTicks = unit.LocalTotalTicks,
                RasterId = unit.Raster?.Id ?? 0,
                UseParentRaster = unit.UseParentRaster,
                RiseTime = unit.RiseTime,
                FadeTime = unit.FadeTime,
                IsEnabled = unit.IsEnabled,
            };

            return unitVc;
        }

        public override void FromPrevious(BaseVC baseVC)
        {
            UnitV1 unitV1 = baseVC as UnitV1;
            if (unitV1 == null)
                return;

            base.FromPrevious(baseVC);

            TrackIds = unitV1.TrackIds;
            Mixing = unitV1.Mixing;
            AutoSize = unitV1.AutoSize;
            LocalTotalTicks = unitV1.LocalTotalTicks;
            RasterId = unitV1.RasterId;
            UseParentRaster = unitV1.UseParentRaster;
        }
    }