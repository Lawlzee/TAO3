﻿using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace TAO3.Converters.Xml;

public class ValueToList<T> : JsonConverter<List<T>>
{
    public override List<T> ReadJson(JsonReader reader, Type objectType, [AllowNull] List<T> existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.StartArray)
        {
            List<T>? value = existingValue ?? (List<T>)serializer.ContractResolver.ResolveContract(typeof(List<T>)).DefaultCreator!();
            serializer.Populate(reader, value);
            return value;
        }

        return new List<T>
        {
            serializer.Deserialize<T>(reader)!
        };
    }

    public override bool CanWrite => false;

    public override void WriteJson(JsonWriter writer, [AllowNull] List<T> value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }
}
