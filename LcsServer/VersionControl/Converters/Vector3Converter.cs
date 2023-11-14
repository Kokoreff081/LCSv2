using System.Numerics;
using Newtonsoft.Json;

namespace LCSVersionControl.Converters;

public class Vector3Converter : JsonConverter<Vector3>
{
    public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        string transforms = reader.Value as string;
        return transforms.ToVector3();
    }

    public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
    {
        string data = LCMath.Vector3ToString(value);
        writer.WriteValue(data);
    }

    //public override bool CanConvert(Type objectType)
    //{
    //    return typeof(Vector3).IsAssignableFrom(objectType);
    //}

    //public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    //{
    //    string transforms = reader.Value as string;
    //    return transforms?.ToVector3();
    //}

    //public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    //{
    //    Vector3 vector = (Vector3)value;
    //    string data = LCMath.Vector3ToString(vector);
    //    serializer.Serialize(writer, data);
    //}

}