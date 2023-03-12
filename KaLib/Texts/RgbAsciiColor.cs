using KaLib.Structs;

namespace KaLib.Texts;

internal class RgbAsciiColor : AsciiColor
{
    public Color Color { get; }
    
    public RgbAsciiColor(Color color)
    {
        Color = color;
    }
    
    public override string ToAsciiCode()
    {
        return $"\u001b[38;2;{Color.R};{Color.G};{Color.B}m";
    }
}