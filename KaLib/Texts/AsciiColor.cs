using System;
using System.Collections.Generic;

namespace KaLib.Texts;

internal sealed class AsciiColor
{
    public char ColorCode { get; set; }
    public int Color { get; set; }
    public bool Bright { get; set; }

    private static Dictionary<char, AsciiColor> byCode = new();

    public static readonly AsciiColor Black = new('0', 30);
    public static readonly AsciiColor DarkBlue = new('1', 34);
    public static readonly AsciiColor DarkGreen = new('2', 32);
    public static readonly AsciiColor DarkAqua = new('3', 36);
    public static readonly AsciiColor DarkRed = new('4', 31);
    public static readonly AsciiColor DarkPurple = new('5', 35);
    public static readonly AsciiColor Gold = new('6', 33);
    public static readonly AsciiColor Gray = new('7', 37);
    public static readonly AsciiColor DarkGray = new('8', 30, true);
    public static readonly AsciiColor Blue = new('9', 34, true);
    public static readonly AsciiColor Green = new('a', 32, true);
    public static readonly AsciiColor Aqua = new('b', 36, true);
    public static readonly AsciiColor Red = new('c', 31, true);
    public static readonly AsciiColor Purple = new('d', 35, true);
    public static readonly AsciiColor Yellow = new('e', 33, true);
    public static readonly AsciiColor White = new('f', 37, true);
    public static readonly AsciiColor Reset = new('r', 0);

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
        return "0123456789abcdefr".ToCharArray();
    }
}