namespace Mochi.Texts;

internal abstract class AnsiColor
{
    public static AnsiColor FromTextColor(TextColor color)
    {
        var closest = color.ToNearestPredefinedColor();
        var code = closest.ToString().Substring(1)[0];
        return LegacyAnsiColor.Of(code);
    }
    
    public static AnsiColor CreateRgb(TextColor color)
    {
        return new RgbAnsiColor(color.Color);
    }

    public abstract string ToAnsiCode();
}