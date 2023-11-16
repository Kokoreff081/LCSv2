using LCSVersionControl.Converters;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.ScenarioObjects.Effects.Fill;

    [JsonConverter(typeof(BaseVcJsonConverter))]
    [VcClass(JsonName = ModelClassName, Version = 2)]

    public class FillV2 : BaseVC
    {
        private const string ModelClassName = "Fill";

        public byte FillStyle { get; set; }
        public byte Direction { get; set; }
        public byte Rotation { get; set; }
        public string CompColor { get; set; }
        public string Background { get; set; }
        public int Angle { get; set; }
        public bool IsRandomAngle { get; set; }
        public long TotalTicks { get; set; }
        public bool IsRandomColor { get; set; }
        public int Repeat { get; set; }
        public long RiseTime { get; set; }
        public long FadeTime { get; set; }
        public long CycleRiseTime { get; set; }
        public long CycleFadeTime { get; set; }

        public override ISaveLoad ToConcreteObject()
        {
            throw new Exception("Cannot instantiate old class version");
        }

        public override BaseVC FromConcreteObject(ISaveLoad o)
        {
            LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.Fill fill = (LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.Fill)o;

            FillV2 fillVc = new FillV2
            {
                Id = fill.Id,
                Name = fill.Name,
                ParentId = fill.ParentId,
                FillStyle = (byte)fill.FillStyle,
                Angle = fill.Angle,
                Background = fill.BackgroundColor.ToString(),
                CompColor = fill.CompColor.ToString(),
                Direction = (byte)fill.Direction,
                IsRandomAngle = fill.IsRandomAngle,
                Rotation = (byte)fill.Rotation,
                TotalTicks = fill.TotalTicks,
                IsRandomColor = fill.IsRandomColor,
                Repeat = fill.Repeat,
                RiseTime = fill.RiseTime,
                FadeTime =  fill.FadeTime,
                CycleRiseTime = fill.CyclicRiseTime,
                CycleFadeTime = fill.CyclicFadeTime,
            };

            return fillVc;
        }

        public override void FromPrevious(BaseVC baseVC)
        {
            FillV1 fillV1 = baseVC as FillV1;
            if (fillV1 == null)
                return;

            base.FromPrevious(baseVC);
            Background = fillV1.Background;
            CompColor = fillV1.CompColor;
            IsRandomColor = fillV1.IsRandomColor;
            Repeat = fillV1.Repeat;
            TotalTicks = fillV1.TotalTicks;
            FillStyle = fillV1.FillStyle;
            Direction = fillV1.Direction;
            Rotation = fillV1.Rotation;
            Angle = fillV1.Angle;
            IsRandomAngle = fillV1.IsRandomAngle;
        }
    }