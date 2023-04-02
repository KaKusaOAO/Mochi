using System;
using Mochi.Utils;

namespace Mochi.Structs;

public struct Color
{
    public byte R { get; set; }
    public byte G { get; set; }
    public byte B { get; set; }

    public Color(byte r, byte g, byte b)
    {
        R = r;
        G = g;
        B = b;
    }

    public Color(double r, double g, double b)
    {
        Preconditions.IsPositive(r, nameof(r));
        Preconditions.IsPositive(g, nameof(g));
        Preconditions.IsPositive(b, nameof(b));

        R = (byte)(Math.Min(1, r) * 255);
        G = (byte)(Math.Min(1, g) * 255);
        B = (byte)(Math.Min(1, b) * 255);
    }

    public Color(int hex)
    {
        R = (byte)((hex >> 16) & 0xff);
        G = (byte)((hex >> 8) & 0xff);
        B = (byte)(hex & 0xff);
    }

    // ReSharper disable once InconsistentNaming
    public int RGB
    {
        get
        {
            int result = R;
            result = result << 8 | G;
            result = result << 8 | B;
            return result;
        }
    }

    public (double NormalizedR, double NormalizedG, double NormalizedB) Normalized => (R / 255.0, G / 255.0, B / 255.0);

    public (double Hue, double Saturation, double Value) ToHsv()
    {
        var (r, g, b) = Normalized;

        var max = Mth.Max(r, g, b);
        var min = Mth.Min(r, g, b);
        var delta = max - min;

        double hue, saturation, value;
        var d60 = Mth.DegToRad * 60;

        // Calculate the hue
        if (delta == 0)
        {
            hue = 0;
        }
        else if (Math.Abs(max - r) < double.Epsilon * 2)
        {
            hue = d60 * ((g - b) / delta % 6);
        }
        else if (Math.Abs(max - g) < double.Epsilon * 2)
        {
            hue = d60 * ((b - r) / delta + 2);
        }
        else if (Math.Abs(max - b) < double.Epsilon * 2)
        {
            hue = d60 * ((r - g) / delta + 4);
        }
        else
        {
            throw new Exception("Shouldn't reach here");
        }

        // Calculate the saturation
        if(max == 0)
        {
            saturation = 0;
        } else
        {
            saturation = delta / max;
        }

        value = max;
        return (hue, saturation, value);
    }

    public double Hue => ToHsv().Hue;

    public static readonly Color White = new(0xffffff);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="hue">radians</param>
    /// <param name="saturation">0-1</param>
    /// <param name="value">0-1</param>
    /// <returns></returns>
    public static Color FromHsv(double hue, double saturation, double value)
    {
        Preconditions.IsPositive(saturation, nameof(saturation));
        Preconditions.IsPositive(value, nameof(value));
            
        // Normalize the hue input.
        if (hue < 0)
        {
            hue *= -1;
            hue %= Math.PI * 2;
            hue *= -1;
            hue += Math.PI * 2;
        }
        else
        {
            hue %= Math.PI * 2;
        }
            
        saturation = Math.Min(1, saturation);
        value = Math.Min(1, value);

        var d60 = Mth.DegToRad * 60;
        var d120 = d60 * 2;
        var d180 = d60 * 3;
        var d240 = d60 * 4;
        var d300 = d60 * 5;

        var c = value * saturation;
        var x = c * (1 - Math.Abs(hue / d60 % 2 - 1));
        var m = value - c;

        double r = 0, g = 0, b = 0;

        // Calculate the colors.
        if (0 <= hue && hue < d60)
        {
            r = c;
            g = x;
        } else if (d60 <= hue && hue < d120)
        {
            r = x;
            g = c;
        } else if (d120 <= hue && hue < d180)
        {
            g = c;
            b = x;
        } else if (d180 <= hue && hue < d240)
        {
            g = x;
            b = c;
        } else if (d240 <= hue && hue < d300)
        {
            b = c;
            r = x;
        } else
        {
            b = x;
            r = c;
        }

        return new Color(r + m, g + m, b + m);
    }
}