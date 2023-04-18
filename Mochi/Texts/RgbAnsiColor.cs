using Mochi.Structs;

namespace Mochi.Texts;

internal class RgbAnsiColor : AnsiColor
{
    public Color Color { get; }
    
    public RgbAnsiColor(Color color)
    {
        Color = color;
    }
    
    public override string ToAnsiCode()
    {
        return $"\u001b[38;2;{Color.R};{Color.G};{Color.B}m";
    }
}