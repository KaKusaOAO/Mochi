#if NET7_0_OR_GREATER
#define HAS_GENERIC_NUMERICS
#endif

using System;
#if HAS_GENERIC_NUMERICS
using System.Numerics;
#else
using System.Linq;
#endif

namespace Mochi.Utils;

public static class Mth
{
#if HAS_GENERIC_NUMERICS
    public static T Max<T>(params T[] values) where T : IMinMaxValue<T>, IComparable<T>
    {
        var result = T.MinValue;
        foreach (var v in values)
        {
            var c = result.CompareTo(v);
            if (c > 0) result = v;
        }

        return result;
    }
    
    public static T Min<T>(params T[] values) where T : IMinMaxValue<T>, IComparable<T>
    {
        var result = T.MaxValue;
        foreach (var v in values)
        {
            var c = result.CompareTo(v);
            if (c < 0) result = v;
        }

        return result;
    }
#else
    public static double Max(params double[] values) => values.Prepend(double.MinValue).Max();
    public static double Min(params double[] values) => values.Prepend(double.MaxValue).Min();
#endif

    public const double DegToRad = Math.PI / 180.0;
    
    public static float Lerp(float a, float b, float t) => a + (b - a) * t;
        
    public static double Lerp(double a, double b, double t) => a + (b - a) * t;
}