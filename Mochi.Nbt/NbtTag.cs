using System;
using System.Collections.Generic;
using System.IO;
using Mochi.Nbt.Serializations;

namespace Mochi.Nbt;

public abstract class NbtTag
{
    public abstract TagTypeInfo TypeInfo { get; }

    public TagType Type => TypeInfo.Type;

    public virtual NbtTag? this[string key]
    {
        get => AsCompound()[key];
        set => AsCompound()[key] = value;
    }

    public static implicit operator NbtTag(bool flag) => Create(flag);
    public static implicit operator NbtTag(byte val) => Create(val);
    public static implicit operator NbtTag(short val) => Create(val);
    public static implicit operator NbtTag(int val) => Create(val);
    public static implicit operator NbtTag(long val) => Create(val);
    public static implicit operator NbtTag(float val) => Create(val);
    public static implicit operator NbtTag(double val) => Create(val);
    public static implicit operator NbtTag(string val) => Create(val);
    public static implicit operator NbtTag(byte[] val) => Create(val);
    public static implicit operator NbtTag(int[] val) => Create(val);
    public static implicit operator NbtTag(long[] val) => Create(val);
    
    public static NbtTag Create(bool flag) => NbtByte.CreateValue(flag);
    public static NbtTag Create(byte val) => NbtByte.CreateValue(val);
    public static NbtTag Create(short val) => NbtShort.CreateValue(val);
    public static NbtTag Create(int val) => NbtInt.CreateValue(val);
    public static NbtTag Create(long val) => NbtLong.CreateValue(val);
    public static NbtTag Create(float val) => NbtFloat.CreateValue(val);
    public static NbtTag Create(double val) => NbtDouble.CreateValue(val);
    public static NbtTag Create(string val) => NbtString.CreateValue(val);
    public static NbtTag Create(IEnumerable<byte> val) => new NbtByteArray(val);
    public static NbtTag Create(IEnumerable<int> val) => new NbtIntArray(val);
    public static NbtTag Create(IEnumerable<long> val) => new NbtLongArray(val);
    
    public virtual NbtValue AsValue() =>
        throw new NotSupportedException("Not a value");
    
    public virtual NbtCompound AsCompound() => 
        throw new NotSupportedException("Not a compound");

    public T As<T>() where T : NbtTag
    {
        if (this is T result) return result;
        if (typeof(T) == typeof(NbtValue)) return (T) (object) AsValue();
        
        throw new InvalidOperationException($"{GetType().Name} cannot be casted to type {typeof(T)}");
    }
    
    public static NbtTag Parse(Stream stream, bool hasRootName = false)
    {
        var reader = hasRootName
            ? NbtReader.CreateWithRootName(stream, long.MaxValue)
            : NbtReader.Create(stream, long.MaxValue);
        
        var type = reader.ReadTagType();
        if (type == TagType.End) return NbtEnd.Instance;

        reader.ReadRootName();
        return TagTypeInfo.GetTagType(type).Load(reader);
    }

    public void WriteTo(NbtWriter writer)
    {
        writer.WriteTagType(Type);
        if (Type == TagType.End) return;
        
        writer.WriteRootName();
        WriteContentTo(writer);
    }

    public string GetAsString() => new StringTagVisitor().Visit(this);

    public override string ToString() => GetAsString();

    public abstract void Accept(ITagVisitor visitor);

    public abstract void WriteContentTo(NbtWriter writer);

    public virtual T GetValue<T>() =>
        throw new InvalidOperationException($"{nameof(GetValue)}() called on wrong type");
}