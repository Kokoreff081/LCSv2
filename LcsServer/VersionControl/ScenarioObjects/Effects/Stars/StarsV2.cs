using LCSVersionControl.Converters;
using LCSVersionControl.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.ScenarioObjects.Effects.Stars;

[JsonConverter(typeof(BaseVcJsonConverter))]
    [VcClass(JsonName = ModelClassName, Version = 2)]
    public class StarsV2 : BaseVC
    {
        private const string ModelClassName = "Stars";

        public long TotalTicks { get; set; }
        public string[] ColorStack { get; set; }
        public int Filling { get; set; }
        public int LifeTime { get; set; }
        public bool Modulation { get; set; }
        public bool IsColorMix { get; set; }
        public string Background { get; set; }
        public long RiseTime { get; set; }
        public long FadeTime { get; set; }

        public override ISaveLoad ToConcreteObject()
        {
            throw new Exception("Cannot instantiate old class version");
        }

        public override BaseVC FromConcreteObject(ISaveLoad o)
        {
            if (o is not LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.Stars stars)
            {
                return null;
            }

            string[] saveColorStack = new string[stars.ColorStack.Count];
            for (int i = 0; i < stars.ColorStack.Count; i++)
            {
                saveColorStack[i] = stars.ColorStack[i].ToString();
            }

            StarsV2 starsVc = new StarsV2 {
                Id = stars.Id,
                Name = stars.Name,
                ParentId = stars.ParentId,
                Background = stars.Background.ToString(),
                ColorStack = saveColorStack,
                Filling = stars.Filling,
                IsColorMix = stars.IsColorMix,
                LifeTime = stars.LifeTime,
                Modulation = stars.Modulation,
                TotalTicks = stars.TotalTicks,
                RiseTime = stars.RiseTime,
                FadeTime = stars.FadeTime,
            };

            return starsVc;
        }

        public override void FromPrevious(BaseVC baseVC)
        {
            if (baseVC is not StarsV1 starsV1)
            {
                return;
            }

            base.FromPrevious(baseVC);

            Background = starsV1.Background;
            ColorStack = starsV1.ColorStack;
            Filling = starsV1.Filling;
            IsColorMix = starsV1.IsColorMix;
            LifeTime = starsV1.LifeTime;
            Modulation = starsV1.Modulation;
            TotalTicks = starsV1.TotalTicks;
        }
    }