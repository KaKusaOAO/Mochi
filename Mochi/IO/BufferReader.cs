using System;
using System.Collections.Generic;
using System.IO;
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
        return (byte)read;
    }

    private delegate T VariableLengthByteReadHandler<T>(T currentValue, byte read, int pos);

    private T ReadVariableLengthValue<T>(int maxLen, VariableLengthByteReadHandler<T> handleByteRead)
    {
        var value = default(T);
        var pos = 0;
        
        while (true)
        {
            var b = ReadByte();
            value = handleByteRead(value, b, pos);
            if ((b & VariableLengthValues.ContinueBit) == 0) break;

            pos += 7;
            if (pos >= maxLen)
            {
                throw new Exception($"Variable length value ({typeof(T)} is too big (maxLen: {maxLen})");
            }
        }

        return value;
    }

    public int ReadVarInt() => ReadVariableLengthValue<int>(32, 
        (value, read, pos) => value | (read & VariableLengthValues.SegmentBits) << pos);
    
    public long ReadVarLong() => ReadVariableLengthValue<long>(64, 
        (value, read, pos) => value | (long) (read & VariableLengthValues.SegmentBits) << pos);
    
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
        if (read != count) throw new EndOfStreamException();
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
        if (type == typeof(int)) return (T)Enum.ToObject(typeof(T), ReadVarInt());
        throw new NotSupportedException();
    }

    public T? ReadNullable<T>(Func<BufferReader, T> readFunc) where T : struct
    {
        var isPresent = ReadBool();
        return isPresent ? readFunc(this) : null;
    }
    
    public IOptional<T> ReadOptional<T>(Func<BufferReader, T> readFunc)
    {
        var isPresent = ReadBool();
        return isPresent ? Optional.Of(readFunc(this)) : Optional.Empty<T>();
    }
}