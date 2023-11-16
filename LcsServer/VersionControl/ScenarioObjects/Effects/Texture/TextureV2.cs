using LCSVersionControl.Converters;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.ScenarioObjects.Effects.Texture;

[JsonConverter(typeof(BaseVcJsonConverter))]
    [VcClass(JsonName = ModelClassName, Version = 2)]
    public class TextureV2 : BaseVC
    {
        private const string ModelClassName = "Texture";

        public int Speed { get; set; }
        public int Angle { get; set; }
        public bool IsFloatingAngle { get; set; }
        public long TotalTicks { get; set; }
        public bool IsStretched { get; set; }
        public string FileName { get; set; }
        public long RiseTime { get; set; }
        public long FadeTime { get; set; }

        public override ISaveLoad ToConcreteObject()
        {
            throw new Exception("Cannot instantiate old class version");
        }

        public override BaseVC FromConcreteObject(ISaveLoad o)
        {
            if (o is not LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.Texture texture)
            {
                return null;
            }

            TextureV2 textureVc = new TextureV2
            {
                Id = texture.Id,
                Name = texture.Name,
                ParentId = texture.ParentId,
                Angle = texture.Angle,
                FileName = texture.FileName,
                TotalTicks = texture.TotalTicks,
                IsFloatingAngle = texture.IsFloatingAngle,
                IsStretched = texture.IsStretched,
                Speed = texture.Speed,
                RiseTime = texture.RiseTime,
                FadeTime = texture.FadeTime,
            };

            return textureVc;

        }

        public override void FromPrevious(BaseVC baseVC)
        {
            if (baseVC is not TextureV1 textureV1)
            {
                return;
            }

            base.FromPrevious(baseVC);
            Angle = textureV1.Angle;
            FileName = textureV1.FileName;
            TotalTicks = textureV1.TotalTicks;
            IsFloatingAngle = textureV1.IsFloatingAngle;
            IsStretched = textureV1.IsStretched;
            Speed = textureV1.Speed;
        }
    }