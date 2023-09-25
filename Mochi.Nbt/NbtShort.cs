using System.Linq;
using Mochi.Nbt.Serializations;

namespace Mochi.Nbt;

public class NbtShort : NbtNumeric<short>
{
    private const short MinCachedValue = -128;
    private const short MaxCachedValue = 1024;
    
    private static readonly NbtShort[] _cache = Enumerable
        .Range(MinCachedValue, MaxCachedValue - MinCachedValue + 1)
        .Select(i => new NbtShort((short) i)).ToArray(); 
    
    public static NbtShort Zero { get; } = CreateValue(0);
    
    public override TagTypeInfo TypeInfo => TagTypeInfo.Short;

    public override short Value { get; }

    private NbtShort(short value)
    {
        Value = value;
    }

    public static NbtShort CreateValue(short value)
    {
        var index = value - MinCachedValue;
        if (index < 0 || index >= _cache.Length) return new NbtShort(value);
        return _cache[index];
    }

    public override void WriteContentTo(NbtWriter writer)
    {
        writer.WriteInt16(Value);
    }

    public override void Accept(ITagVisitor visitor) => visitor.VisitShort(this);

#if !NET7_0_OR_GREATER
    public override byte AsByte() => (byte) (Value & byte.MaxValue);
    public override short AsInt16() => Value;
    public override int AsInt32() => Value;
    public override long AsInt64() => Value;
    public override ushort AsUInt16() => (ushort) Value;
    public override uint AsUInt32() => (uint) Value;
    public override ulong AsUInt64() => (ulong) Value;
    public override float AsSingle() => Value;
    public override double AsDouble() => Value;
#endif
    
    public sealed class ShortTypeInfo : TagTypeInfo<NbtShort>
    {
        internal static ShortTypeInfo Instance { get; } = new();
        
        public override TagType Type => TagType.Short;

        public override string FriendlyName => "TAG_Short";

        private ShortTypeInfo() {}

        protected override NbtShort LoadValue(NbtReader reader)
        {
            var val = reader.ReadInt16();
            return CreateValue(val);
        }
    }
}