using System.Text.Json.Serialization;
using Mochi.StreamKit.Twitch.API.Converters;

namespace Mochi.StreamKit.Twitch.API;

public class DataListWithTemplate<T> : DataList<T>
{
    [JsonPropertyName("template")]
    [JsonConverter(typeof(StringTemplateConverter))]
    public StringTemplate Template { get; set; }
}