using System;
using Mochi.Nbt.Serializations;

namespace Mochi.Nbt;

public class NbtFloat : NbtNumeric<float>
{
    public static NbtFloat Zero { get; } = new(0);
    
    public override TagTypeInfo TypeInfo => TagTypeInfo.Float;

    public override float Value { get; }

    private NbtFloat(float value)
    {
        Value = value;
    }

    public static NbtFloat CreateValue(float value) => 
        value == 0 ? Zero : new NbtFloat(value);

    public override void WriteContentTo(NbtWriter writer)
    {
        writer.WriteSingle(Value);
    }

    public override void Accept(ITagVisitor visitor) => visitor.VisitFloat(this);

#if !NET7_0_OR_GREATER
    public override byte AsByte() => (byte) Value;
    public override short AsInt16() => (short) Value;
    public override int AsInt32() => (int) Value;
    public override long AsInt64() => (long) Value;
    public override ushort AsUInt16() => (ushort) Value;
    public override uint AsUInt32() => (uint) Value;
    public override ulong AsUInt64() => (ulong) Value;
    public override float AsSingle() => Value;
    public override double AsDouble() => Value;
#endif
    
    public sealed class FloatTypeInfo : TagTypeInfo<NbtFloat>
    {
        internal static FloatTypeInfo Instance { get; } = new();
        
        public override TagType Type => TagType.Float;

        public override string FriendlyName => "TAG_Float";

        private FloatTypeInfo() {}

        protected override NbtFloat LoadValue(NbtReader reader)
        {
            var val = reader.ReadSingle();
            return new NbtFloat(val);
        }
    }
}