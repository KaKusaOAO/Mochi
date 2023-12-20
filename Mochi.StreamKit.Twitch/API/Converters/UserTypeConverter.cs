using System.Text.Json;
using System.Text.Json.Serialization;
using Mochi.StreamKit.Twitch.Entities;

namespace Mochi.StreamKit.Twitch.API.Converters;

public class UserTypeConverter : JsonConverter<UserType>
{
    public override UserType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetString() switch
        {
            "admin" => UserType.Admin,
            "global_mod" => UserType.GlobalMod,
            "staff" => UserType.Staff,
            _ => UserType.Normal
        };
    }

    public override void Write(Utf8JsonWriter writer, UserType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value switch
        {
            UserType.Admin => "admin",
            UserType.GlobalMod => "global_mod",
            UserType.Staff => "staff",
            _ => ""
        });
    }
}