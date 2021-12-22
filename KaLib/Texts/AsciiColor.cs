using System;
using System.Collections.Generic;

namespace KaLib.Texts;

internal sealed class AsciiColor
{
    public char ColorCode { get; set; }
    public int Color { get; set; }
    public bool Bright { get; set; }

    private static Dictionary<char, AsciiColor> byCode = new Dictionary<char, AsciiColor>();

    public static readonly AsciiColor Black = new AsciiColor('0', 30);
    public static readonly AsciiColor DarkBlue = new AsciiColor('1', 34);
    public static readonly AsciiColor DarkGreen = new AsciiColor('2', 32);
    public static readonly AsciiColor DarkAqua = new AsciiColor('3', 36);
    public static readonly AsciiColor DarkRed = new AsciiColor('4', 31);
    public static readonly AsciiColor DarkPurple = new AsciiColor('5', 35);
    public static readonly AsciiColor Gold = new AsciiColor('6', 33);
    public static readonly AsciiColor Gray = new AsciiColor('7', 37);
    public static readonly AsciiColor DarkGray = new AsciiColor('8', 30, true);
    public static readonly AsciiColor Blue = new AsciiColor('9', 34, true);
    public static readonly AsciiColor Green = new AsciiColor('a', 32, true);
    public static readonly AsciiColor Aqua = new AsciiColor('b', 36, true);
    public static readonly AsciiColor Red = new AsciiColor('c', 31, true);
    public static readonly AsciiColor Purple = new AsciiColor('d', 35, true);
    public static readonly AsciiColor Yellow = new AsciiColor('e', 33, true);
    public static readonly AsciiColor White = new AsciiColor('f', 37, true);

    public const char ColorChar = '\u00a7';

    private AsciiColor(char code, int color, bool isBright = false)
    {
        ColorCode = code;
        Color = color;
        Bright = isBright;

        byCode.Add(code, this);
    }

    public static AsciiColor Of(char c)
    {
        try
        {
            return byCode[c];
        }
        catch (KeyNotFoundException)
        {
            throw new ArgumentException($"Color of '{c}' is not defined");
        }
    }

    public static AsciiColor FromTextColor(TextColor color)
    {
        var closest = color.ToNearestPredefinedColor();
        var code = closest.ToString().Substring(1)[0];
        return Of(code);
    }

    public string ToAsciiCode()
    {
        string brightPrefix = Bright ? "1;" : "0;";
        return $"\u001b[{brightPrefix}{Color}m";
    }

    public string ToMcCode()
    {
        return $"{ColorChar}{ColorCode.ToString().ToLower()}";
    }

    public static char[] McCodes()
    {
        return "0123456789abcdef".ToCharArray();
    }
}