using LCSVersionControl.Converters;
using LCSVersionControl.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.ScenarioObjects.Effects.Flame;

[JsonConverter(typeof(BaseVcJsonConverter))]
    [VcClass(JsonName = ModelClassName, Version = 2)]
    public class FlameV2 : BaseVC
    {
        private const string ModelClassName = "Flame";

        public long TotalTicks { get; set; }
        public string[] ColorStack { get; set; }
        public int Height { get; set; }
        public int Speed { get; set; }
        public int Stability { get; set; }
        public bool Floating { get; set; }
        public long RiseTime { get; set; }
        public long FadeTime { get; set; }

        public override ISaveLoad ToConcreteObject()
        {
            throw new Exception("It's old class version");
        }

        public override BaseVC FromConcreteObject(ISaveLoad o)
        {
            LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.Flame flame = (LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.Flame)o;

            var colors = flame.ColorStack.Get;
            string[] saveColorStack = new string[colors.Count];
            for (int i = 0; i < colors.Count; i++)
            {
                saveColorStack[i] = colors[i].ToString();
            }

            FlameV2 flameVc = new FlameV2
            {
                Id = flame.Id,
                Name = flame.Name,
                ParentId = flame.ParentId,
                ColorStack = saveColorStack,
                Floating = flame.Floating,
                Height = flame.Height,
                Speed = flame.Speed,
                Stability = flame.Stability,
                TotalTicks = flame.TotalTicks,
                RiseTime = flame.RiseTime,
                FadeTime = flame.FadeTime,
            };

            return flameVc;
        }

        public override void FromPrevious(BaseVC baseVC)
        {
            FlameV1 flameV1 = baseVC as FlameV1;
            if (flameV1 == null)
                return;

            base.FromPrevious(baseVC);

            TotalTicks = flameV1.TotalTicks;
            ColorStack = flameV1.ColorStack;
            Height = flameV1.Height;
            Speed = flameV1.Speed;
            Stability = flameV1.Stability;
            Floating = flameV1.Floating;
        }
    }