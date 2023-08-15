using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mochi.StreamKit.Twitch.API.Converters;

public class EmoteTypeConverter : JsonConverter<EmoteType>
{
    public override EmoteType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetString() switch
        {
            "bitstier" => EmoteType.BitsTier,
            "follower" => EmoteType.Follower,
            "subscriptions" => EmoteType.Subscriptions,
            _ => EmoteType.Globals
        };
    }

    public override void Write(Utf8JsonWriter writer, EmoteType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value switch
        {
            EmoteType.BitsTier => "bitstier",
            EmoteType.Follower => "follower",
            EmoteType.Subscriptions => "subscriptions",
            _ => "globals"
        });
    }
}