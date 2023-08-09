using System.Text.Json;
using System.Text.Json.Serialization;
using Mochi.StreamKit.Twitch.Entities;

namespace Mochi.StreamKit.Twitch.API.Converters;

public class BroadcasterTypeConverter : JsonConverter<BroadcasterType>
{
    public override BroadcasterType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetString() switch
        {
            "affiliate" => BroadcasterType.Affiliate,
            "partner" => BroadcasterType.Partner,
            _ => BroadcasterType.Normal
        };
    }

    public override void Write(Utf8JsonWriter writer, BroadcasterType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value switch
        {
            BroadcasterType.Affiliate => "affiliate",
            BroadcasterType.Partner => "partner",
            _ => ""
        });
    }
}