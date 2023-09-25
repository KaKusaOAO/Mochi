using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Mochi.Nbt.Serializations;

public class NbtReader
{
    private const int MaxStackDepth = 512;
    
    private readonly Stream _stream;
    private readonly long _quota;
    private readonly int _maxDepth;
    private long _usage;
    private int _depth;
    private bool _readRootName;

    private NbtReader(Stream stream, long quota, int maxDepth = MaxStackDepth, bool readRootName = false)
    {
        _stream = stream;
        _quota = quota;
        _maxDepth = maxDepth;
        _readRootName = readRootName;
    }
    
    public static NbtReader Create(Stream stream, long quota, int maxDepth = MaxStackDepth)
        => new(stream, quota, maxDepth);

    public static NbtReader CreateWithRootName(Stream stream, long quota, int maxDepth = MaxStackDepth)
        => new(stream, quota, maxDepth, true);
    

    #region => Essential decoding functions
    public byte ReadByte()
    {
        AccountBytes(1);
        
        var r = _stream.ReadByte();
        if (r == -1)
            throw new EndOfStreamException();

        _usage++;
        return (byte) (r & 0xff);
    }

    public byte[] ReadFixedLength(int length)
    {
        AccountBytes(length);
        var buffer = new byte[length];
        var totalRead = 0;

        while (totalRead < length)
        {
            var read = _stream.Read(buffer, totalRead, length - totalRead);
            if (read == 0)
                throw new EndOfStreamException();

            totalRead += read;
        }

        return buffer;
    }

    public TagType ReadTagType()
    {
        return (TagType) ReadByte();
    }

    private byte[] ReadEndiannessSensitive(int length)
    {
        // Call this only right before passing into BitConverter
        var arr = ReadFixedLength(length);
        if (BitConverter.IsLittleEndian) Array.Reverse(arr);
        return arr;
    }

    public short ReadInt16() => BitConverter.ToInt16(ReadEndiannessSensitive(sizeof(short)));
    public int ReadInt32() => BitConverter.ToInt32(ReadEndiannessSensitive(sizeof(int)));
    public long ReadInt64() => BitConverter.ToInt64(ReadEndiannessSensitive(sizeof(long)));
    
    public ushort ReadUInt16() => BitConverter.ToUInt16(ReadEndiannessSensitive(sizeof(ushort)));
    public uint ReadUInt32() => BitConverter.ToUInt32(ReadEndiannessSensitive(sizeof(uint)));
    public ulong ReadUInt64() => BitConverter.ToUInt64(ReadEndiannessSensitive(sizeof(ulong)));
    
    public float ReadSingle() => BitConverter.Int32BitsToSingle(ReadInt32());
    public double ReadDouble() => BitConverter.Int64BitsToDouble(ReadInt64());

    public string ReadUtf()
    {
        // Read the string represented with modified UTF-8.
        var len = ReadUInt16();
        var chars = new List<char>();
        
        for (var i = 0; i < len; i++)
        {
            var a = ReadByte();

            if ((a & 0b10000000) == 0)
            {
                chars.Add((char) a);
                continue;
            }

            if (a >> 5 == 0b110)
            {
                // Character is 0 or in the range \u0080 - \u07ff.
                if (i + 1 == len) 
                    throw new InvalidDataException($"Not enough data for group of 2! Identifier: 0x{a:x2}");
                
                var b = ReadByte();
                chars.Add((char) (((a & 0b00011111) << 6) | (b & 0b00111111)));

                i += 1;
                continue;
            }

            if (a >> 4 == 0b1110)
            {
                // Character is in the range \u0800 to \uffff.
                if (i + 2 >= len) 
                    throw new InvalidDataException($"Not enough data for group of 3! Identifier: 0x{a:x2}");

                var b = ReadByte();
                var c = ReadByte();
                chars.Add((char) (((a & 0x0f) << 12) | ((b & 0x3f) << 6) | (c & 0x3f)));

                i += 2;
                continue;
            }

            if (a >> 6 == 0b10 || a >> 4 == 0b1111)
                throw new InvalidDataException($"Unexpected data: 0x{a:x2}");
        }

        return new string(chars.ToArray());
    }
    #endregion

    private void AccountBytes(long size)
    {
        if (_usage + size > _quota)
            throw new NbtTooLargeException($"Tried to read NBT tag that is too large, size > {_quota} bytes");

        _usage += size;
    }

    public void PushDepth()
    {
        if (_depth >= _maxDepth)
            throw new NbtTooComplexException($"Tried to read NBT tag with too high complexity, depth > {_maxDepth}");

        _depth++;
    }

    public void PushDepth(Action action)
    {
        PushDepth();
        try
        {
            action();
        }
        finally
        {
            PopDepth();
        }
    }
    
    public void PopDepth()
    {
        if (_depth <= 0)
            throw new InvalidOperationException("Attempted to pop stack-depth at top-level");

        _depth--;
    }

    public void ReadRootName()
    {
        if (!_readRootName) return;
        _readRootName = false;

        var skipped = ReadUInt16();
        ReadFixedLength(skipped);
    }
}