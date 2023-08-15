using System.Collections;

namespace Mochi.StreamKit.Twitch.API;

public enum EmoteScale
{
    Small, Medium, Large
}

public class EmoteScaleCollection : SimpleEnumCollection<EmoteScale>
{
    public override EmoteScale CreateFromString(string value)
    {
        return value switch
        {
            "1.0" => EmoteScale.Small,
            "2.0" => EmoteScale.Medium,
            _ => EmoteScale.Large
        };
    }

    public override string CreateFromValue(EmoteScale value)
    {
        return value switch
        {
            EmoteScale.Medium => "2.0",
            EmoteScale.Large => "3.0",
            _ => "1.0"
        };
    }
}