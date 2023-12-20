using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mochi.StreamKit.Twitch.API.Converters;

public class SimpleEnumCollectionConverter<T, TValue> : JsonConverter<T> where T : SimpleEnumCollection<TValue>
{
    public override T Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            throw new JsonException("Invalid emote format collection");

        var collection = Activator.CreateInstance<T>();
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndArray) break;
            if (reader.TokenType != JsonTokenType.String)
                throw new JsonException("Invalid entry in emote format collection");
            
            collection.Add(collection.CreateFromString(reader.GetString()!));
        }

        return collection;
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        foreach (var format in value)
        {
            writer.WriteStringValue(value.CreateFromValue(format));
        }
        writer.WriteEndArray();
    }
}