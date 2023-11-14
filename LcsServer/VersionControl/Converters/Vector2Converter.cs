using System.Numerics;
using Newtonsoft.Json;

namespace LCSVersionControl.Converters;

public class Vector2Converter : JsonConverter<Vector2>
{
    public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue, bool hasExistingValue,
        JsonSerializer serializer)
    {
        string transforms = reader.Value as string;
        return transforms.ToVector2();
    }

    public override void WriteJson(JsonWriter writer, Vector2 value, JsonSerializer serializer)
    {
        string data = LCMath.Vector2ToString(value);
        writer.WriteValue(data);
    }

    //public override bool CanConvert(Type objectType)
    //{
    //    return typeof(Vector2).IsAssignableFrom(objectType);
    //}


    //public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    //{
    //    string transforms = reader.Value as string;
    //    return transforms?.ToVector2();
    //}

    //public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    //{
    //    Vector2 vector = (Vector2)value;
    //    string data = LCMath.Vector2ToString(vector);
    //    serializer.Serialize(writer, data);
    //}

}