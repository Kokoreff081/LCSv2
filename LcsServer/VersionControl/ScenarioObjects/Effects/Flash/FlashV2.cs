using LCSVersionControl.Converters;
using LCSVersionControl.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.ScenarioObjects.Effects.Flash;

    [JsonConverter(typeof(BaseVcJsonConverter))]
    [VcClass(JsonName = ModelClassName, Version = 2)]
    public class FlashV2 : BaseVC
    {
        private const string ModelClassName = "Flash";

        public long TotalTicks { get; set; }
        public long RiseTime { get; set; }
        public long FadeTime { get; set; }
        public long CycleRiseTime { get; set; }
        public long CycleFadeTime { get; set; }
        public int Repeat { get; set; }
        public bool IsRandomColor { get; set; }
        public string CompColor { get; set; }
        public string Background { get; set; }

        public override ISaveLoad ToConcreteObject()
        {
            throw new Exception("Cannot instantiate old class version");
        }

        public override BaseVC FromConcreteObject(ISaveLoad o)
        {
            LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.Flash flash = (LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.Flash)o;

            FlashV2 flashVc = new FlashV2
            {
                Id = flash.Id,
                Name = flash.Name,
                ParentId = flash.ParentId,
                Background = flash.BackgroundColor.ToString(),
                CompColor = flash.CompColor.ToString(),
                FadeTime = flash.FadeTime,
                IsRandomColor = flash.IsRandomColor,
                Repeat = flash.Repeat,
                RiseTime = flash.RiseTime,
                TotalTicks = flash.TotalTicks,
                CycleRiseTime = flash.CyclicRiseTime,
                CycleFadeTime = flash.CyclicFadeTime,
            };

            return flashVc;
        }

        public override void FromPrevious(BaseVC baseVC)
        {
            FlashV1 flashV1 = baseVC as FlashV1;
            if (flashV1 == null)
                return;

            base.FromPrevious(baseVC);
            Background = flashV1.Background;
            CompColor = flashV1.CompColor;
            IsRandomColor = flashV1.IsRandomColor;
            Repeat = flashV1.Repeat;
            TotalTicks = flashV1.TotalTicks;
            CycleRiseTime = (long)(flashV1.RisePercent * 0.01 * flashV1.TotalTicks / flashV1.Repeat); // переводим % в мс.
            CycleFadeTime = (long)(flashV1.FadePercent * 0.01 * flashV1.TotalTicks / flashV1.Repeat); // переводим % в мс.
        }
    }