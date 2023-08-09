using System.Text.Json;
using System.Text.Json.Serialization;
using Mochi.StreamKit.Twitch.Entities;

namespace Mochi.StreamKit.Twitch.API.Converters;

public class DateTimeOffsetConverter : JsonConverter<DateTimeOffset>
{
    public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateTimeOffset.Parse(reader.GetString()!);
    }

    public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
    {
        throw new NotSupportedException();
        // writer.WriteStringValue(value.ToString());
    }
}