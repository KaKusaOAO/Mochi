using System;
using System.Collections.Generic;

namespace Mochi.Texts;

internal class LegacyAnsiColor : AnsiColor
{
    public char ColorCode { get; set; }
    public int Color { get; set; }
    public bool Bright { get; set; }

    private static Dictionary<char, LegacyAnsiColor> byCode = new();
    
    
    public static readonly LegacyAnsiColor Black = new('0', 30);
    public static readonly LegacyAnsiColor DarkBlue = new('1', 34);
    public static readonly LegacyAnsiColor DarkGreen = new('2', 32);
    public static readonly LegacyAnsiColor DarkAqua = new('3', 36);
    public static readonly LegacyAnsiColor DarkRed = new('4', 31);
    public static readonly LegacyAnsiColor DarkPurple = new('5', 35);
    public static readonly LegacyAnsiColor Gold = new('6', 33);
    public static readonly LegacyAnsiColor Gray = new('7', 37);
    public static readonly LegacyAnsiColor DarkGray = new('8', 30, true);
    public static readonly LegacyAnsiColor Blue = new('9', 34, true);
    public static readonly LegacyAnsiColor Green = new('a', 32, true);
    public static readonly LegacyAnsiColor Aqua = new('b', 36, true);
    public static readonly LegacyAnsiColor Red = new('c', 31, true);
    public static readonly LegacyAnsiColor Purple = new('d', 35, true);
    public static readonly LegacyAnsiColor Yellow = new('e', 33, true);
    public static readonly LegacyAnsiColor White = new('f', 37, true);
    public static readonly LegacyAnsiColor Reset = new('r', 0);
    
    private LegacyAnsiColor(char code, int color, bool isBright = false)
    {
        ColorCode = code;
        Color = color;
        Bright = isBright;

        byCode.Add(code, this);
    }
    
    public static LegacyAnsiColor Of(char c)
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
    
    public override string ToAnsiCode()
    {
        var brightPrefix = Bright ? "1;" : "0;";
        return $"\u001b[{brightPrefix}{Color}m";
    }
}