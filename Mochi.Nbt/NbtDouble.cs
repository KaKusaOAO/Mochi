using System;
using Mochi.Nbt.Serializations;

namespace Mochi.Nbt;

public class NbtDouble : NbtNumeric<double>
{
    public static NbtDouble Zero { get; } = new(0);
    
    public override TagTypeInfo TypeInfo => TagTypeInfo.Double;

    public override double Value { get; }

    private NbtDouble(double value)
    {
        Value = value;
    }

    public static NbtDouble CreateValue(double value) => 
        value == 0 ? Zero : new NbtDouble(value);

    public override void WriteContentTo(NbtWriter writer)
    {
        writer.WriteDouble(Value);
    }

    public override void Accept(ITagVisitor visitor) => visitor.VisitDouble(this);

#if !NET7_0_OR_GREATER
    public override byte AsByte() => (byte) Value;
    public override short AsInt16() => (short) Value;
    public override int AsInt32() => (int) Value;
    public override long AsInt64() => (long) Value;
    public override ushort AsUInt16() => (ushort) Value;
    public override uint AsUInt32() => (uint) Value;
    public override ulong AsUInt64() => (ulong) Value;
    public override float AsSingle() => (float) Value;
    public override double AsDouble() => Value;
#endif
    
    public sealed class DoubleTypeInfo : TagTypeInfo<NbtDouble>
    {
        internal static DoubleTypeInfo Instance { get; } = new();
        
        public override TagType Type => TagType.Double;

        public override string FriendlyName => "TAG_Double";

        private DoubleTypeInfo() {}

        protected override NbtDouble LoadValue(NbtReader reader)
        {
            var val = reader.ReadDouble();
            return new NbtDouble(val);
        }
    }
}