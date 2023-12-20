using System;
using System.Linq;
using Mochi.Nbt.Serializations;

namespace Mochi.Nbt;

public class NbtByte : NbtNumeric<byte>
{
    private static readonly NbtByte[] _cache = Enumerable.Range(0, 0x100)
        .Select(i => new NbtByte((byte) i)).ToArray(); 
    
    public static NbtByte Zero { get; } = _cache[0];
    public static NbtByte One { get; } = _cache[1];
    
    public override TagTypeInfo TypeInfo => TagTypeInfo.Byte;

    public override byte Value { get; }

    private NbtByte(byte value)
    {
        if (_cache != null!)
        {
            throw new InvalidOperationException(
                $"Use {nameof(NbtTag)}.{nameof(Create)}({Value}), instead of creating a new instance.");
        }

        Value = value;
    }

    public static NbtByte CreateValue(byte value) => _cache[value];
    public static NbtByte CreateValue(bool bl) => bl ? One : Zero;

    public override void WriteContentTo(NbtWriter writer)
    {
        writer.WriteByte(Value);
    }

    public bool AsBool() => Value != 0;

    public override void Accept(ITagVisitor visitor) => visitor.VisitByte(this);

#if !NET7_0_OR_GREATER
    public override byte AsByte() => Value;
    public override short AsInt16() => Value;
    public override int AsInt32() => Value;
    public override long AsInt64() => Value;
    public override ushort AsUInt16() => Value;
    public override uint AsUInt32() => Value;
    public override ulong AsUInt64() => Value;
    public override float AsSingle() => Value;
    public override double AsDouble() => Value;
#endif
    
    public sealed class ByteTypeInfo : TagTypeInfo<NbtByte>
    {
        internal static ByteTypeInfo Instance { get; } = new();
        
        public override TagType Type => TagType.Byte;

        public override string FriendlyName => "TAG_Byte";

        private ByteTypeInfo() {}

        protected override NbtByte LoadValue(NbtReader reader)
        {
            var val = reader.ReadByte();
            return CreateValue(val);
        }
    }
}