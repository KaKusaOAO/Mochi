using System;
using System.Linq;
using Mochi.Nbt.Serializations;

namespace Mochi.Nbt;

public class NbtInt : NbtNumeric<int>
{
    private const short MinCachedValue = -128;
    private const short MaxCachedValue = 1024;
    
    private static readonly NbtInt[] _cache = Enumerable
        .Range(MinCachedValue, MaxCachedValue - MinCachedValue + 1)
        .Select(i => new NbtInt((short) i)).ToArray(); 
    
    public static NbtInt Zero { get; } = CreateValue(0);
    
    public override TagTypeInfo TypeInfo => TagTypeInfo.Int;

    public override int Value { get; }

    private NbtInt(int value)
    {
        Value = value;
    }

    public static NbtInt CreateValue(int value)
    {
        var index = value - MinCachedValue;
        if (index < 0 || index >= _cache.Length) return new NbtInt(value);
        return _cache[index];
    }

    public override void WriteContentTo(NbtWriter writer)
    {
        writer.WriteInt32(Value);
    }

    public override void Accept(ITagVisitor visitor) => visitor.VisitInt(this);

#if !NET7_0_OR_GREATER
    public override byte AsByte() => (byte) (Value & byte.MaxValue);
    public override short AsInt16() => (short) (Value & short.MaxValue);
    public override int AsInt32() => Value;
    public override long AsInt64() => Value;
    public override ushort AsUInt16() => (ushort) (Value & ushort.MaxValue);
    public override uint AsUInt32() => (uint) Value;
    public override ulong AsUInt64() => (ulong) Value;
    public override float AsSingle() => Value;
    public override double AsDouble() => Value;
#endif
    
    public sealed class IntTypeInfo : TagTypeInfo<NbtInt>
    {
        internal static IntTypeInfo Instance { get; } = new();
        
        public override TagType Type => TagType.Int;

        public override string FriendlyName => "TAG_Int";

        private IntTypeInfo() {}

        protected override NbtInt LoadValue(NbtReader reader)
        {
            var val = reader.ReadInt32();
            return new NbtInt(val);
        }
    }
}