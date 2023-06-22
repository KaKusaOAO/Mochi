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
    public static ushort Max(params ushort[] values) => values.Prepend(ushort.MinValue).Max();
    public static ushort Min(params ushort[] values) => values.Prepend(ushort.MaxValue).Min();
    public static sbyte Max(params sbyte[] values) => values.Prepend(sbyte.MinValue).Max();
    public static sbyte Min(params sbyte[] values) => values.Prepend(sbyte.MaxValue).Min();
    public static float Max(params float[] values) => values.Prepend(float.MinValue).Max();
    public static float Min(params float[] values) => values.Prepend(float.MaxValue).Min();
    public static short Max(params short[] values) => values.Prepend(short.MinValue).Max();
    public static short Min(params short[] values) => values.Prepend(short.MaxValue).Min();
    public static ulong Max(params ulong[] values) => values.Prepend(ulong.MinValue).Max();
    public static ulong Min(params ulong[] values) => values.Prepend(ulong.MaxValue).Min();
    public static byte Max(params byte[] values) => values.Prepend(byte.MinValue).Max();
    public static byte Min(params byte[] values) => values.Prepend(byte.MaxValue).Min();
    public static long Max(params long[] values) => values.Prepend(long.MinValue).Max();
    public static long Min(params long[] values) => values.Prepend(long.MaxValue).Min();
    public static uint Max(params uint[] values) => values.Prepend(uint.MinValue).Max();
    public static uint Min(params uint[] values) => values.Prepend(uint.MaxValue).Min();
    public static int Max(params int[] values) => values.Prepend(int.MinValue).Max();
    public static int Min(params int[] values) => values.Prepend(int.MaxValue).Min();
#endif

    public const double DegToRad = Math.PI / 180.0;
    
    public static float Lerp(float a, float b, float t) => a + (b - a) * t;
        
    public static double Lerp(double a, double b, double t) => a + (b - a) * t;
}