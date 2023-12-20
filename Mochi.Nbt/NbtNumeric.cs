using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Numerics;
using System.Reflection;

namespace Mochi.Nbt;

public abstract class NbtNumeric<T> : NbtValue<T>, INbtNumeric
#if NET7_0_OR_GREATER
    where T : INumberBase<T>
#endif
{
#if NET7_0_OR_GREATER
    public virtual byte AsByte() => byte.CreateTruncating(Value);
    public virtual short AsInt16() => short.CreateTruncating(Value);
    public virtual int AsInt32() => int.CreateTruncating(Value);
    public virtual long AsInt64() => long.CreateTruncating(Value);
    public virtual ushort AsUInt16() => ushort.CreateTruncating(Value);
    public virtual uint AsUInt32() => uint.CreateTruncating(Value);
    public virtual ulong AsUInt64() => ulong.CreateTruncating(Value);
    public virtual float AsSingle() => float.CreateTruncating(Value);
    public virtual double AsDouble() => double.CreateTruncating(Value);
    
#else
    public abstract byte AsByte();
    public abstract short AsInt16();
    public abstract int AsInt32();
    public abstract long AsInt64();
    public abstract ushort AsUInt16();
    public abstract uint AsUInt32();
    public abstract ulong AsUInt64();
    public abstract float AsSingle();
    public abstract double AsDouble();
#endif

#if NET7_0_OR_GREATER
    private TValue GetNumberValue<TValue>() where TValue : INumberBase<TValue>
    {
        return TValue.CreateTruncating(Value);
    }
#endif
    
    public override TValue GetValue<
#if NET7_0_OR_GREATER && NATIVEAOT
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.Interfaces |
                                    DynamicallyAccessedMemberTypes.PublicMethods)]
#endif
        TValue>()
    {
        if (Value is TValue val) return val;
        
#if NET7_0_OR_GREATER

        var itf = typeof(TValue).GetInterfaces()
            .FirstOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(INumberBase<>));
        
        if (itf != null)
        {
#if NATIVEAOT
            throw new InvalidOperationException(
                $"Using generic numeric types on AOT build is not available");
#else
            var method = typeof(TValue).GetMethod("CreateTruncating", BindingFlags.Public | BindingFlags.Static);
            if (method != null)
                return (TValue) method.MakeGenericMethod(typeof(TValue))
                    .Invoke(null, new object?[] {Value})!;

            throw new Exception("CreateTruncating not found?");
#endif
        }
        
#endif

        if (typeof(TValue) == typeof(byte))
            return (TValue) (object) AsByte();
        
        if (typeof(TValue) == typeof(short))
            return (TValue) (object) AsInt16();
        
        if (typeof(TValue) == typeof(int))
            return (TValue) (object) AsInt32();
        
        if (typeof(TValue) == typeof(long))
            return (TValue) (object) AsInt64();
        
        if (typeof(TValue) == typeof(ushort))
            return (TValue) (object) AsUInt16();
        
        if (typeof(TValue) == typeof(uint))
            return (TValue) (object) AsUInt32();
        
        if (typeof(TValue) == typeof(ulong))
            return (TValue) (object) AsUInt64();
        
        if (typeof(TValue) == typeof(float))
            return (TValue) (object) AsSingle();

        if (typeof(TValue) == typeof(ulong))
            return (TValue) (object) AsDouble();
                
        if (typeof(TValue) == typeof(bool))
            return (TValue) (object) (AsByte() != 0);

        throw new InvalidOperationException(
            $"Wrong type for {nameof(GetValue)}(), expected numeric type or bool, found {typeof(TValue).Name}");
    }
}