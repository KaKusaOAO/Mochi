using System.Text.Json.Serialization;
using Mochi.StreamKit.Twitch.API.Converters;

namespace Mochi.StreamKit.Twitch.API;

public class ChannelEmote : EmoteSetEntry
{
    [JsonPropertyName("tier")]
    [JsonConverter(typeof(SubscriptionTierConverter))]
    public SubscriptionTier Tier { get; set; }
}