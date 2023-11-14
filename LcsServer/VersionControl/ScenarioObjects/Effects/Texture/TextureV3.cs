using LCSVersionControl.Converters;
using LCSVersionControl.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.ScenarioObjects.Effects.Texture;

[JsonConverter(typeof(BaseVcJsonConverter))]
[VcClass(JsonName = ModelClassName, Version = 3)]
public class TextureV3 : BaseVC
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
    public float DimmingLevel { get; set; } = 1.0f;

    public override ISaveLoad ToConcreteObject()
    {
        LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.Texture texture = new LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.Texture(Id, Name, ParentId,
            Angle, FileName, TotalTicks, IsFloatingAngle, IsStretched, Speed, RiseTime, FadeTime, DimmingLevel);
        return texture;
    }

    public override BaseVC FromConcreteObject(ISaveLoad o)
    {
        if (o is not LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.Texture texture)
        {
            return null;
        }

        TextureV3 textureVc = new TextureV3 {
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
            DimmingLevel = texture.DimmingLevel
        };

        return textureVc;
    }

    public override void FromPrevious(BaseVC baseVC)
    {
        if (baseVC is not TextureV2 textureV2)
        {
            return;
        }

        base.FromPrevious(baseVC);
        Angle = textureV2.Angle;
        FileName = textureV2.FileName;
        TotalTicks = textureV2.TotalTicks;
        IsFloatingAngle = textureV2.IsFloatingAngle;
        IsStretched = textureV2.IsStretched;
        Speed = textureV2.Speed;
        RiseTime = textureV2.RiseTime;
        FadeTime = textureV2.FadeTime;
    }
}