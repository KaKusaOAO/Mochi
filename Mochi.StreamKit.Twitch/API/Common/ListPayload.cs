using System.Text.Json.Serialization;

namespace Mochi.StreamKit.Twitch.API;

public class ListPayload<T>
{
    [JsonPropertyName("data")]
    public List<T> Data { get; set; }
}