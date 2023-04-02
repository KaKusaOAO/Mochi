namespace Mochi.Texts;

internal abstract class AsciiColor
{
    public static AsciiColor FromTextColor(TextColor color)
    {
        var closest = color.ToNearestPredefinedColor();
        var code = closest.ToString().Substring(1)[0];
        return LegacyAsciiColor.Of(code);
    }
    
    public static AsciiColor CreateRgb(TextColor color)
    {
        return new RgbAsciiColor(color.Color);
    }

    public abstract string ToAsciiCode();
}