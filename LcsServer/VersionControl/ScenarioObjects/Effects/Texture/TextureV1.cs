using LCSVersionControl.Converters;
using LcsServer.Models.LCProjectModels.GlobalBase.Interfaces;
using Newtonsoft.Json;

namespace LCSVersionControl.ScenarioObjects.Effects.Texture;

[JsonConverter(typeof(BaseVcJsonConverter))]
[VcClass(JsonName = ModelClassName, Version = 1)]
public class TextureV1 : BaseVC
{
    private const string ModelClassName = "Texture";

    public int Speed { get; set; }
    public int Angle { get; set; }
    public bool IsFloatingAngle { get; set; }
    public long TotalTicks { get; set; }
    public bool IsStretched { get; set; }
    public string FileName { get; set; }

    public override ISaveLoad ToConcreteObject()
    {
        throw new Exception("It's old class version");
    }

    public override BaseVC FromConcreteObject(ISaveLoad o)
    {
        LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.Texture texture = (LcsServer.Models.LCProjectModels.Models.ScenarioObjects.Effects.Texture) o;

        TextureV1 textureVc = new TextureV1
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
        };

        return textureVc;
    }
}