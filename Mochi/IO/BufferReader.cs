using System;
using System.Collections.Generic;
using System.IO;
#if NET7_0_OR_GREATER
using System.Numerics;
#endif
using System.Text;
using Mochi.Utils;

namespace Mochi.IO;

public class BufferReader
{
    public Stream Stream { get; }

    public BufferReader(Stream stream)
    {
        Stream = stream;
    }

    public byte ReadByte()
    {
        var read = Stream.ReadByte();
        if (read == -1) throw new EndOfStreamException();
        return (byte) read;
    }

#if !NET7_0_OR_GREATER
    private delegate T VariableLengthByteReadHandler<T>(T currentValue, byte read, int pos);
    
    private T ReadVariableLengthValue<T>(int maxLen, VariableLengthByteReadHandler<T> handleByteRead)
#else
    private T ReadVariableLengthValue<T>(int maxLen) where T : IConvertible, IBinaryInteger<T>
#endif
    {
#if NET7_0_OR_GREATER
        var value = T.Zero;
        var segmentBits = T.CreateChecked(VariableLengthValues.SegmentBits);
#else
        var value = default(T);
#endif

        var pos = 0;

        while (true)
        {
            var b = ReadByte();
#if NET7_0_OR_GREATER
            value |= (T.CreateChecked(b) & segmentBits) << pos;
#else
            value = handleByteRead(value!, b, pos);
#endif
            if ((b & VariableLengthValues.ContinueBit) == 0) break;

            pos += 7;
            if (pos >= maxLen)
            {
                throw new Exception($"Variable length value ({typeof(T)} is too big (maxLen: {maxLen})");
            }
        }

        return value;
    }

    public int ReadVarInt() => ReadVariableLengthValue<int>(32
#if !NET7_0_OR_GREATER
        ,
        (value, read, pos) => value | (read & VariableLengthValues.SegmentBits) << pos
#endif
    );

    public long ReadVarLong() => ReadVariableLengthValue<long>(64
#if !NET7_0_OR_GREATER
        ,
        (value, read, pos) => value | (long) (read & VariableLengthValues.SegmentBits) << pos
#endif
    );

    public List<T> ReadList<T>(Func<BufferReader, T> readFunc)
    {
        var count = ReadVarInt();
        var list = new List<T>(count);
        for (var i = 0; i < count; i++)
        {
            list.Add(readFunc(this));
        }

        return list;
    }

    public T[] ReadArray<T>(Func<BufferReader, T> readFunc) => ReadList(readFunc).ToArray();

    public byte[] ReadByteArray()
    {
        // We don't need to run through the generic list method
        var count = ReadVarInt();
        var bytes = new byte[count];
        var read = Stream.Read(bytes, 0, count);
        if (read != count) throw new EndOfStreamException("Could not read all bytes");
        return bytes;
    }

    public string ReadString(Encoding encoding) => encoding.GetString(ReadByteArray());

    public string ReadUtf8String() => ReadString(Encoding.UTF8);

    public byte[] ReadFixedByteArray(int length)
    {
        var bytes = new byte[length];
        var read = Stream.Read(bytes, 0, length);
        if (read != length) throw new EndOfStreamException();
        return bytes;
    }

    public Guid ReadGuid() => new(ReadFixedByteArray(16));

    public bool ReadBool() => ReadByte() > 0;

    public short ReadInt16() => (short) ((ReadByte() << 8) | ReadByte());

    public int ReadInt32() => (ReadByte() << 24) | (ReadByte() << 16) | (ReadByte() << 8) | ReadByte();

    public long ReadInt64() => (long) ReadInt32() << 32 | (uint) ReadInt32();

    public ushort ReadUInt16() => (ushort) ((ReadByte() << 8) | ReadByte());

    public uint ReadUInt32() => (uint) ((ReadByte() << 24) | (ReadByte() << 16) | (ReadByte() << 8) | ReadByte());

    public ulong ReadUInt64() => (ulong) ReadUInt32() << 32 | ReadUInt32();

    public float ReadSingle() => BitConverter.ToSingle(ReadFixedByteArray(4), 0);

    public double ReadDouble() => BitConverter.Int64BitsToDouble(ReadInt64());

    public T ReadEnum<T>() where T : struct
    {
        var type = Enum.GetUnderlyingType(typeof(T));
        if (type == typeof(int)) return (T) Enum.ToObject(typeof(T), ReadVarInt());
        throw new NotSupportedException();
    }

    private delegate T NullableFactory<out T, in TValue>(TValue value);

    private delegate T EmptyNullableFactory<out T>();

    private T ReadGenericNullable<T, TValue>(NullableFactory<T, TValue> factory, EmptyNullableFactory<T> emptyFactory,
        Func<BufferReader, TValue> readFunc)
    {
        var isPresent = ReadBool();
        return isPresent ? factory(readFunc(this)) : emptyFactory();
    }

    public T? ReadNullable<T>(Func<BufferReader, T> readFunc) where T : struct =>
        ReadGenericNullable<T?, T>(v => v, () => null, readFunc);

    public IOptional<T> ReadOptional<T>(Func<BufferReader, T> readFunc) =>
        ReadGenericNullable(Optional.Of, Optional.Empty<T>, readFunc);
}