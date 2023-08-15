using System.Collections;

namespace Mochi.StreamKit.Twitch.API;

public enum EmoteThemeMode
{
    Dark, Light
}

public class EmoteThemeModeCollection : SimpleEnumCollection<EmoteThemeMode>
{
    public override EmoteThemeMode CreateFromString(string value)
    {
        return value switch
        {
            "dark" => EmoteThemeMode.Dark,
            _ => EmoteThemeMode.Light
        };
    }

    public override string CreateFromValue(EmoteThemeMode value)
    {
        return value switch
        {
            EmoteThemeMode.Dark => "dark",
            _ => "light"
        };
    }
}