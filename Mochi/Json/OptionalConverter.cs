using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Mochi.Utils;

namespace Mochi.Json;

public class OptionalConverter : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsGenericType && 
               typeToConvert.GetInterfaces().Any(x => x.GetGenericTypeDefinition() == typeof(IOptional<>));
    }

    public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        return (JsonConverter) Activator.CreateInstance(typeof(OptionalConverterInner<>)
                .MakeGenericType(typeToConvert.GetGenericArguments()[0]),
            BindingFlags.Instance | BindingFlags.Public, null, new object[] {options}, null);
    }

    private class OptionalConverterInner<T> : JsonConverter<IOptional<T>>
    {
        public override IOptional<T> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return Optional.Of<T>(JsonSerializer.Deserialize<T>(ref reader, options)!);
        }

        public override void Write(Utf8JsonWriter writer, IOptional<T> value, JsonSerializerOptions options)
        {
            if (value.IsEmpty)
            {
                writer.WriteNullValue();
                return;
            }

            JsonSerializer.Serialize(writer, value.Value, options);
        }
    }
}