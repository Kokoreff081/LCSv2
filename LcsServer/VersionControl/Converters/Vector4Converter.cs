using System.Numerics;
using Newtonsoft.Json;

namespace LCSVersionControl.Converters;

public class Vector4Converter : JsonConverter<Vector4>
{
    public override Vector4 ReadJson(JsonReader reader, Type objectType, Vector4 existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        string transforms = reader.Value as string;
        return transforms.ToVector4();
    }

    public override void WriteJson(JsonWriter writer, Vector4 value, JsonSerializer serializer)
    {
        string data = LCMath.Vector4ToString(value);
        writer.WriteValue(data);
    }

    //public override bool CanConvert(Type objectType)
    //{
    //    return typeof(Vector4).IsAssignableFrom(objectType);
    //}

    //public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    //{
    //    string transforms = reader.Value as string;

    //    return transforms?.ToVector4();
    //}

    //public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    //{
    //    Vector4 vector = (Vector4)value;
    //    string data = LCMath.Vector4ToString(vector);
    //    serializer.Serialize(writer, data);
    //}

}