using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Mochi.Texts;

namespace Mochi.Json;

public class ComponentConverter : JsonConverter<IComponent?>
{
    public override IComponent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var node = JsonNode.Parse(ref reader);
        return node == null ? null : Component.FromJson(node);
    }

    public override void Write(Utf8JsonWriter writer, IComponent? value, JsonSerializerOptions options)
    {
        var node = value?.ToJson();
        if (node == null)
        {
            writer.WriteNullValue();    
        }
        else
        {
            node.WriteTo(writer);
        }
    }
}