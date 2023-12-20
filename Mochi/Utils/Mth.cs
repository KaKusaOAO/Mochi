#if NET7_0_OR_GREATER
#define HAS_GENERIC_NUMERICS
#endif

using System;
using System.Collections.Generic;
#if HAS_GENERIC_NUMERICS
using System.Numerics;
#endif
using System.Linq;

namespace Mochi.Utils;

public static class Mth
{
    public const double DegToRad = Math.PI / 180.0;
    public const double RadToDeg = 180.0 / Math.PI;
    
    public static T Max<T>(T val, params T[] values) where T : IComparable<T> => values.Prepend(val).Max();

    public static T Min<T>(T val, params T[] values) where T : IComparable<T> => values.Prepend(val).Min();

    public static T Max<T>(this IEnumerable<T> source) where T : IComparable<T> =>
        Compare(source, (a, b) => a > b);
    
    public static T Min<T>(this IEnumerable<T> source) where T : IComparable<T> =>
        Compare(source, (a, b) => a < b);

    private static T Compare<T>(IEnumerable<T> source, Func<int, int, bool> compare) where T : IComparable<T>
    {
        using var enumerator = source.GetEnumerator();
        var result = default(T);
        var hasFirst = false;
        
        while (enumerator.MoveNext())
        {
            if (!hasFirst) 
            {
                result = enumerator.Current;
                if (result is IComparable<T>) hasFirst = true;
            }
            else
            {
                var val = enumerator.Current;
                if (val is IComparable<T> && result is IComparable<T> && compare(result.CompareTo(val), 0))
                {
                    result = val;
                }
            }
        }

        if (!hasFirst)
            throw new InvalidOperationException("Sequence is empty");

        return result!;
    }

    /*
    public static double Max(double val, params double[] values) => values.Prepend(val).Max();
    public static double Min(double val, params double[] values) => values.Prepend(val).Min();
    public static ushort Max(ushort val, params ushort[] values) => values.Prepend(val).Max();
    public static ushort Min(ushort val, params ushort[] values) => values.Prepend(val).Min();
    public static sbyte Max(sbyte val, params sbyte[] values) => values.Prepend(val).Max();
    public static sbyte Min(sbyte val, params sbyte[] values) => values.Prepend(val).Min();
    public static float Max(float val, params float[] values) => values.Prepend(val).Max();
    public static float Min(float val, params float[] values) => values.Prepend(val).Min();
    public static short Max(short val, params short[] values) => values.Prepend(val).Max();
    public static short Min(short val, params short[] values) => values.Prepend(val).Min();
    public static ulong Max(ulong val, params ulong[] values) => values.Prepend(val).Max();
    public static ulong Min(ulong val, params ulong[] values) => values.Prepend(val).Min();
    public static byte Max(byte val, params byte[] values) => values.Prepend(val).Max();
    public static byte Min(byte val, params byte[] values) => values.Prepend(val).Min();
    public static long Max(long val, params long[] values) => values.Prepend(val).Max();
    public static long Min(long val, params long[] values) => values.Prepend(val).Min();
    public static uint Max(uint val, params uint[] values) => values.Prepend(val).Max();
    public static uint Min(uint val, params uint[] values) => values.Prepend(val).Min();
    public static int Max(int val, params int[] values) => values.Prepend(val).Max();
    public static int Min(int val, params int[] values) => values.Prepend(val).Min(); 
    */
    
#if HAS_GENERIC_NUMERICS
    private static T LerpFloating<T>(T a, T b, T t) where T : IFloatingPoint<T> => a + (b - a) * t;
    
    public static T Lerp<T>(IFloatingPoint<T> a, IFloatingPoint<T> b, IFloatingPoint<T> t) where T : IFloatingPoint<T> => 
        LerpFloating((T) a, (T) b, (T) t);

    public static TFloat Lerp<T, TFloat>(INumberBase<T> a, INumberBase<T> b, IFloatingPoint<TFloat> t) 
        where T : INumberBase<T> where TFloat : IFloatingPoint<TFloat>
    {
        var da = TFloat.CreateChecked((T) a);
        var db = TFloat.CreateChecked((T) b);
        return LerpFloating(da, db, (TFloat) t);
    }
#else
    public static float Lerp(float a, float b, float t) => a + (b - a) * t;
    public static double Lerp(double a, double b, double t) => a + (b - a) * t;
    public static decimal Lerp(decimal a, decimal b, decimal t) => a + (b - a) * t;
#endif
}