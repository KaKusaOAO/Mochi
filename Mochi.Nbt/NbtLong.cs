using System;
using System.Linq;
using Mochi.Nbt.Serializations;

namespace Mochi.Nbt;

public class NbtLong : NbtNumeric<long>
{
    private const short MinCachedValue = -128;
    private const short MaxCachedValue = 1024;
    
    private static readonly NbtLong[] _cache = Enumerable
        .Range(MinCachedValue, MaxCachedValue - MinCachedValue + 1)
        .Select(i => new NbtLong((short) i)).ToArray(); 
    
    public static NbtLong Zero { get; } = CreateValue(0);
    
    public override TagTypeInfo TypeInfo => TagTypeInfo.Long;

    public override long Value { get; }

    private NbtLong(long value)
    {
        Value = value;
    }

    public static NbtLong CreateValue(long value)
    {
        var index = value - MinCachedValue;
        if (index < 0 || index >= _cache.Length) return new NbtLong(value);
        return _cache[index];
    }

    public override void WriteContentTo(NbtWriter writer)
    {
        writer.WriteInt64(Value);
    }

    public override void Accept(ITagVisitor visitor) => visitor.VisitLong(this);

#if !NET7_0_OR_GREATER
    public override byte AsByte() => (byte) (Value & byte.MaxValue);
    public override short AsInt16() => (short) (Value & short.MaxValue);
    public override int AsInt32() => (int) (Value & int.MaxValue);
    public override long AsInt64() => Value;
    public override ushort AsUInt16() => (ushort) (Value & ushort.MaxValue);
    public override uint AsUInt32() => (uint) (Value & uint.MaxValue);
    public override ulong AsUInt64() => (ulong) Value;
    public override float AsSingle() => Value;
    public override double AsDouble() => Value;
#endif
    
    public sealed class LongTypeInfo : TagTypeInfo<NbtLong>
    {
        internal static LongTypeInfo Instance { get; } = new();
        
        public override TagType Type => TagType.Long;

        public override string FriendlyName => "TAG_Long";

        private LongTypeInfo() {}

        protected override NbtLong LoadValue(NbtReader reader)
        {
            var val = reader.ReadInt64();
            return new NbtLong(val);
        }
    }
}