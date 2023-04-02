using System;
using System.Collections.Generic;

namespace Mochi.Texts;

internal class LegacyAsciiColor : AsciiColor
{
    public char ColorCode { get; set; }
    public int Color { get; set; }
    public bool Bright { get; set; }

    private static Dictionary<char, LegacyAsciiColor> byCode = new();
    
    
    public static readonly LegacyAsciiColor Black = new('0', 30);
    public static readonly LegacyAsciiColor DarkBlue = new('1', 34);
    public static readonly LegacyAsciiColor DarkGreen = new('2', 32);
    public static readonly LegacyAsciiColor DarkAqua = new('3', 36);
    public static readonly LegacyAsciiColor DarkRed = new('4', 31);
    public static readonly LegacyAsciiColor DarkPurple = new('5', 35);
    public static readonly LegacyAsciiColor Gold = new('6', 33);
    public static readonly LegacyAsciiColor Gray = new('7', 37);
    public static readonly LegacyAsciiColor DarkGray = new('8', 30, true);
    public static readonly LegacyAsciiColor Blue = new('9', 34, true);
    public static readonly LegacyAsciiColor Green = new('a', 32, true);
    public static readonly LegacyAsciiColor Aqua = new('b', 36, true);
    public static readonly LegacyAsciiColor Red = new('c', 31, true);
    public static readonly LegacyAsciiColor Purple = new('d', 35, true);
    public static readonly LegacyAsciiColor Yellow = new('e', 33, true);
    public static readonly LegacyAsciiColor White = new('f', 37, true);
    public static readonly LegacyAsciiColor Reset = new('r', 0);
    
    private LegacyAsciiColor(char code, int color, bool isBright = false)
    {
        ColorCode = code;
        Color = color;
        Bright = isBright;

        byCode.Add(code, this);
    }
    
    public static LegacyAsciiColor Of(char c)
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
    
    public override string ToAsciiCode()
    {
        var brightPrefix = Bright ? "1;" : "0;";
        return $"\u001b[{brightPrefix}{Color}m";
    }
}