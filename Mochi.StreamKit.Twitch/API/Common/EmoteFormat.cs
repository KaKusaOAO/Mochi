using System.Collections;

namespace Mochi.StreamKit.Twitch.API;

public enum EmoteFormat
{
    Default, Static, Animated
}

public class EmoteFormatCollection : SimpleEnumCollection<EmoteFormat>
{
    public override EmoteFormat CreateFromString(string value)
    {
        return value switch
        {
            "static" => EmoteFormat.Static,
            "animated" => EmoteFormat.Animated,
            _ => EmoteFormat.Default
        };
    }

    public override string CreateFromValue(EmoteFormat value)
    {
        return value switch
        {
            EmoteFormat.Animated => "animated",
            _ => "static"
        };
    }
}