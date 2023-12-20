using System;
using System.Collections.Generic;
using System.IO;

namespace Mochi.Nbt.Serializations;

public class NbtWriter
{
    private readonly Stream _stream;
    private bool _writeRootName;

    public NbtWriter(Stream stream, bool writeRootName = false)
    {
        _stream = stream;
        _writeRootName = writeRootName;
    }

    public void WriteByte(byte b)
    {
        _stream.WriteByte(b);
    }

    public void WriteTagType(TagType type)
    {
        _stream.WriteByte((byte) type);
    }

    public void WriteBytes(byte[] arr)
    {
        _stream.Write(arr, 0, arr.Length);
    }

    private void WriteEndiannessSensitive(byte[] arr)
    {
        // Call this only right after getting bytes from BitConverter.
        if (BitConverter.IsLittleEndian) Array.Reverse(arr);
        _stream.Write(arr, 0, arr.Length);
    }

    public void WriteInt16(short val) => WriteEndiannessSensitive(BitConverter.GetBytes(val));
    public void WriteInt32(int val) => WriteEndiannessSensitive(BitConverter.GetBytes(val));
    public void WriteInt64(long val) => WriteEndiannessSensitive(BitConverter.GetBytes(val));
    
    public void WriteUInt16(ushort val) => WriteEndiannessSensitive(BitConverter.GetBytes(val));
    public void WriteUInt32(uint val) => WriteEndiannessSensitive(BitConverter.GetBytes(val));
    public void WriteUInt64(ulong val) => WriteEndiannessSensitive(BitConverter.GetBytes(val));

    public void WriteSingle(float val) => WriteInt32(BitConverter.SingleToInt32Bits(val));
    public void WriteDouble(double val) => WriteInt64(BitConverter.DoubleToInt64Bits(val));

    public void WriteUtf(string s)
    {
        var bytes = new List<byte>();

        foreach (var ch in s)
        {
            if (ch is >= '\u0001' and <= '\u007f')
            {
                bytes.Add((byte) ch);
                continue;
            }

            if (ch is '\u0000' or >= '\u0080' and <= '\u07ff')
            {
                bytes.Add((byte) (0xc0 | (0x1f & (ch >> 6))));
                bytes.Add((byte) (0x80 | (0x3f & ch)));
                continue;
            }

            if (ch >= '\u0800')
            {
                bytes.Add((byte) (0xe0 | (0x0f & (ch >> 12))));
                bytes.Add((byte) (0x80 | (0x3f & (ch >> 6))));
                bytes.Add((byte) (0x80 | (0x3f & ch)));
                continue;
            }

            var ich = (int) ch;
            throw new InvalidDataException($"Don't know how to encode character '\\u{ich:x4}' ({ch})");
        }
        
        if (bytes.Count > ushort.MaxValue)
            throw new InvalidDataException($"Encoded string exceeds {ushort.MaxValue} bytes (found {bytes.Count})");

        var arr = bytes.ToArray();
        WriteUInt16((ushort) bytes.Count);
        WriteBytes(arr);
    }

    public void WriteRootName()
    {
        if (!_writeRootName) return;
        _writeRootName = false;
        WriteUtf("");
    }
}