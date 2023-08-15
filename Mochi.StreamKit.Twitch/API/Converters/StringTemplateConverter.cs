using System.Text.Json;
using System.Text.Json.Serialization;

namespace Mochi.StreamKit.Twitch.API.Converters;

public class StringTemplateConverter : JsonConverter<StringTemplate>
{
    public override StringTemplate Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) => 
        StringTemplate.Parse(reader.GetString()!);

    public override void Write(Utf8JsonWriter writer, StringTemplate value, JsonSerializerOptions options) => 
        writer.WriteStringValue(value.ToString());
}