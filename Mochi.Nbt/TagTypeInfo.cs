using System;
using System.Collections.Generic;
using Mochi.Nbt.Serializations;

namespace Mochi.Nbt;

public abstract class TagTypeInfo : ITagTypeInfo
{
    private static readonly Dictionary<TagType, TagTypeInfo> _lookup = new();

    public static TagTypeInfo<NbtEnd> End { get; } = NbtEnd.EndTypeInfo.Instance;
    public static TagTypeInfo<NbtByte> Byte { get; } = NbtByte.ByteTypeInfo.Instance;
    public static TagTypeInfo<NbtShort> Short { get; } = NbtShort.ShortTypeInfo.Instance;
    public static TagTypeInfo<NbtInt> Int { get; } = NbtInt.IntTypeInfo.Instance;
    public static TagTypeInfo<NbtLong> Long { get; } = NbtLong.LongTypeInfo.Instance;
    public static TagTypeInfo<NbtFloat> Float { get; } = NbtFloat.FloatTypeInfo.Instance;
    public static TagTypeInfo<NbtDouble> Double { get; } = NbtDouble.DoubleTypeInfo.Instance;
    public static TagTypeInfo<NbtByteArray> ByteArray { get; } = NbtByteArray.ByteArrTypeInfo.Instance;
    public static TagTypeInfo<NbtString> String { get; } = NbtString.StringTypeInfo.Instance;
    public static TagTypeInfo<NbtList> List { get; } = NbtList.ListTypeInfo.Instance;
    public static TagTypeInfo<NbtCompound> Compound { get; } = NbtCompound.CompoundTypeInfo.Instance;
    public static TagTypeInfo<NbtIntArray> IntArray { get; } = NbtIntArray.IntArrTypeInfo.Instance;
    public static TagTypeInfo<NbtLongArray> LongArray { get; } = NbtLongArray.LongArrTypeInfo.Instance;
    
    public abstract TagType Type { get; }
    public abstract string FriendlyName { get; }

    protected TagTypeInfo()
    {
        _lookup[Type] = this;
    }

    public abstract NbtTag Load(NbtReader reader);

    public static TagTypeInfo GetTagType(TagType type) => _lookup[type];
}

public abstract class TagTypeInfo<T> : TagTypeInfo, ITagTypeInfo<T> where T : NbtTag
{
    public sealed override NbtTag Load(NbtReader reader) => LoadValue(reader);

    protected abstract T LoadValue(NbtReader reader);
    
    NbtTag ITagTypeInfo.Load(NbtReader reader) => LoadValue(reader);
    T ITagTypeInfo<T>.Load(NbtReader reader) => LoadValue(reader);
}