using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LCSVersionControl.Converters;

public class BaseVcJsonConverter : JsonConverter<BaseVC>
{
    public override bool CanWrite => false;

    // protected BaseVC Create(Type objectType, JObject jObject)
    // {
    //     string type = jObject["Type"].Value<string>();
    //     int version = jObject["Version"].Value<int>();
    //     return BaseVC.CreateBaseVC(type, version);
    // }

    public override void WriteJson(JsonWriter writer, BaseVC value, JsonSerializer serializer)
    {
        throw new NotImplementedException("Unnecessary because CanWrite is false. The type will skip the converter.");
    }

    public override BaseVC ReadJson(JsonReader reader, Type objectType, BaseVC existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        // Load JObject from stream
        JObject jObject = JObject.Load(reader);

        // Create target object based on JObject
        string type = jObject["Type"]?.Value<string>();
        int version = jObject["Version"]?.Value<int>() ?? 0;
        BaseVC target = BaseVC.CreateBaseVC(type, version);
        //BaseVC target = Create(objectType, jObject);

        if (target == null)
        {
            return null;
        }

        // Populate the object properties
        serializer.Populate(jObject.CreateReader(), target);

        return target;
    }
}