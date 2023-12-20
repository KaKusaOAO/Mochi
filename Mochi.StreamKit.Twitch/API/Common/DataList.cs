using System.Text.Json.Serialization;

namespace Mochi.StreamKit.Twitch.API;

public class DataList<T>
{
    [JsonPropertyName("data")]
    public List<T> Data { get; set; }
}