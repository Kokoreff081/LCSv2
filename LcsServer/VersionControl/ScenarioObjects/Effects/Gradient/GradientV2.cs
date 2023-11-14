using LCSVersionControl.Converters;
using LCSVersionControl.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.ScenarioObjects.Effects.Gradient;

[JsonConverter(typeof(BaseVcJsonConverter))]
    [VcClass(JsonName = ModelClassName, Version = 2)]
    public class GradientV2 : BaseVC
    {
        private const string ModelClassName = "Gradient";

        public byte FillStyle { get; set; }
        public byte Direction { get; set; }
        public byte Rotation { get; set; }
        public string[] ColorStack { get; set; }
        public int Angle { get; set; }
        public bool IsRandomAngle { get; set; }
        public long TotalTicks { get; set; }
        public int Speed { get; set; }
        public float Scale { get; set; } = 1;
        public long RiseTime { get; set; }
        public long FadeTime { get; set; }

        public override ISaveLoad ToConcreteObject()
        {
            throw new Exception("Cannot instantiate old class version");
        }

        public override BaseVC FromConcreteObject(ISaveLoad o)
        {
            LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.Gradient gradient = (LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.Gradient)o;

            string[] saveColorStack = new string[gradient.ColorsPalette.Count];
            for (int i = 0; i < gradient.ColorsPalette.Count; i++)
            {
                saveColorStack[i] = gradient.ColorsPalette[i].ToString();
            }

            GradientV2 gradientVc = new GradientV2
            {
                Id = gradient.Id,
                Name = gradient.Name,
                ParentId = gradient.ParentId,
                Angle = gradient.Angle,
                ColorStack = saveColorStack,
                Direction = (byte)gradient.Direction,
                TotalTicks = gradient.TotalTicks,
                FillStyle = (byte)gradient.FillStyle,
                IsRandomAngle = gradient.IsRandomAngle,
                Rotation = (byte)gradient.Rotation,
                Speed = gradient.Speed,
                Scale = gradient.Scale,
                RiseTime = gradient.RiseTime,
                FadeTime = gradient.FadeTime,
            };

            return gradientVc;
        }

        public override void FromPrevious(BaseVC baseVC)
        {
            if (baseVC is not GradientV1 gradientV1)
            {
                return;
            }

            base.FromPrevious(baseVC);

            FillStyle = gradientV1.FillStyle;
            Direction = gradientV1.Direction;
            Rotation = gradientV1.Rotation;
            ColorStack = gradientV1.ColorStack;
            Angle = gradientV1.Angle;
            IsRandomAngle = gradientV1.IsRandomAngle;
            TotalTicks = gradientV1.TotalTicks;
            Speed = gradientV1.Speed;
            Scale = gradientV1.Scale;
        }
    }