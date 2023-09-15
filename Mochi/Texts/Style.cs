using System.Text.Json.Nodes;

namespace Mochi.Texts;

public class Style : IStyle<Style>, IColoredStyle<Style>
{
    public static Style Empty { get; } = new();

    public TextColor? Color { get; private set; }

    private Style()
    {
    }

    public Style WithColor(TextColor? color) => new Style
    {
        Color = color
    }.ApplyTo(this);

    public Style ApplyTo(Style other)
    {
        if (this == Empty) return other;
        if (other == Empty) return this;

        return new Style
        {
            Color = Color ?? other.Color
        };
    }

    public void SerializeInto(JsonObject obj)
    {
        if (Color == null) return;
        obj["color"] = "#" + Color.Color.RGB.ToString("x6");
    }

    public Style Clear() => Empty;
}