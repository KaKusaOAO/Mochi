using System.Text.Json;
using System.Text.Json.Serialization;
using Mochi.StreamKit.Twitch.Entities;

namespace Mochi.StreamKit.Twitch.API.Converters;

public class SubscriptionTierConverter : JsonConverter<SubscriptionTier>
{
    public override SubscriptionTier Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetString() switch
        {
            "1000" => SubscriptionTier.Tier1,
            "2000" => SubscriptionTier.Tier2,
            "3000" => SubscriptionTier.Tier3,
            _ => SubscriptionTier.None
        };
    }

    public override void Write(Utf8JsonWriter writer, SubscriptionTier value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value switch
        {
            SubscriptionTier.Tier1 => "1000",
            SubscriptionTier.Tier2 => "2000",
            SubscriptionTier.Tier3 => "3000",
            _ => ""
        });
    }
}