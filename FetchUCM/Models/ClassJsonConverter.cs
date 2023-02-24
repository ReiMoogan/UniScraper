using System;
using Newtonsoft.Json;

namespace FetchUCM.Models;

public class ClassJsonConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
    {
        if (reader.Value == null)
            return (byte)0;
        if (objectType == typeof(byte))
            return Convert.ToByte(reader.Value);
        if (objectType == typeof(int))
            return Convert.ToInt32(reader.Value);
        if (objectType == typeof(short))
            return Convert.ToInt16(reader.Value);
        if (objectType == typeof(float))
            return Convert.ToSingle(reader.Value);
        throw new NotImplementedException();
    }

    public override bool CanConvert(Type objectType)
    {
        return objectType == typeof(int) || objectType == typeof(short) || objectType == typeof(float) || objectType == typeof(byte);
    }
}