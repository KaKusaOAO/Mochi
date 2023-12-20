using System.Text.Json.Serialization;
using Mochi.StreamKit.Twitch.API.Converters;

namespace Mochi.StreamKit.Twitch.API;

public class Emote
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    
    [JsonPropertyName("name")]
    public string Name { get; set; }
    
    [JsonPropertyName("images")]
    public EmoteImages Images { get; set; }
    
    [JsonPropertyName("format")]
    [JsonConverter(typeof(SimpleEnumCollectionConverter<EmoteFormatCollection, EmoteFormat>))]
    public EmoteFormatCollection Formats { get; set; } = new();
    
    [JsonPropertyName("scale")]
    [JsonConverter(typeof(SimpleEnumCollectionConverter<EmoteScaleCollection, EmoteScale>))]
    public EmoteScaleCollection Scales { get; set; } = new();
    
    [JsonPropertyName("theme_mode")]
    [JsonConverter(typeof(SimpleEnumCollectionConverter<EmoteThemeModeCollection, EmoteThemeMode>))]
    public EmoteThemeModeCollection ThemeModes { get; set; } = new();
}