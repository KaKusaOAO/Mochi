using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using Mochi.Utils;

namespace Mochi.IO;

public class BufferWriter
{
    public Stream Stream { get; }
    
    public BufferWriter(Stream stream)
    {
        Stream = stream;
    }

    public BufferWriter WriteByte(byte value)
    {
        Stream.WriteByte(value);
        return this;
    }
    
#if NET7_0_OR_GREATER
    private BufferWriter WriteVariableLengthValue<T>(T value, T byteMask, T segmentBits, T continueBit) where T : 
        IConvertible, IBinaryInteger<T>
    {
        while (true)
        {
            var segment = (value & byteMask).ToByte(null);
            if ((value & ~segmentBits) == T.Zero)
            {
                WriteByte(segment);
                return this;
            }
            
            WriteByte(((value & segmentBits) | continueBit).ToByte(null));
            value >>>= 7;
        }
    }
    
    public BufferWriter WriteVarInt(int value) => WriteVariableLengthValue(value, 0xff, 
        VariableLengthValues.SegmentBits, VariableLengthValues.ContinueBit);
    public BufferWriter WriteVarLong(long value) => WriteVariableLengthValue(value, 0xff, 
        VariableLengthValues.SegmentBits, VariableLengthValues.ContinueBit);
#else
    private BufferWriter WriteVariableLengthValue<T>(T value, Func<T, byte> getSegment, 
        Func<T, int, T> andOperator,
        Func<T, bool> isZero,
        Func<T, int, T> orOperator,
        Func<T, int, T> uRightShifter) where T : IConvertible
    {
        while (true)
        {
            var segment = getSegment(value);
            if (isZero(andOperator(value, ~VariableLengthValues.SegmentBits)))
            {
                WriteByte(segment);
                return this;
            }
            
            WriteByte(Convert.ToByte(orOperator(andOperator(value, VariableLengthValues.SegmentBits),
                VariableLengthValues.ContinueBit)));
            value = uRightShifter(value, 7);
        }
    }

    public BufferWriter WriteVarInt(int value) => 
        WriteVariableLengthValue(value, v => (byte) v,
            (v, i) => v & i,
            v => v == 0,
            (v, i) => v | i,
            (v, i) => v >>> i);

    public BufferWriter WriteVarLong(long value) => 
        WriteVariableLengthValue(value, v => (byte) v, 
            (v, i) => v & i,
            v => v == 0,
            (v, i) => v | (uint)i,
            (v, i) => v >>> i);
#endif
    
    public BufferWriter WriteList<T>(List<T> list, Action<BufferWriter, T> write)
    {
        WriteVarInt(list.Count);
        foreach (var item in list)
        {
            write(this, item);
        }

        return this;
    }
    
    public BufferWriter WriteRange<T>(IEnumerable<T> arr, Action<BufferWriter, T> write) => WriteList(arr.ToList(), write);
    
    public BufferWriter WriteByteArray(byte[] arr)
    {
        // We don't need to run through the generic list method
        WriteVarInt(arr.Length);
        Stream.Write(arr, 0, arr.Length);
        return this;
    }

    public BufferWriter WriteString(Encoding encoding, string value) => WriteByteArray(encoding.GetBytes(value));
    
    public BufferWriter WriteUtf8String(string value) => WriteString(Encoding.UTF8, value);
    
    public BufferWriter WriteBool(bool value) => WriteByte((byte) (value ? 1 : 0));
    
    public BufferWriter WriteShort(short value)
    {
        WriteByte((byte) (value >> 8));
        WriteByte((byte) value);
        return this;
    }
    
    public BufferWriter WriteInt32(int value)
    {
        WriteByte((byte) (value >> 24));
        WriteByte((byte) (value >> 16));
        WriteByte((byte) (value >> 8));
        WriteByte((byte) value);
        return this;
    }

    public BufferWriter WriteInt64(long value)
    {
        WriteInt32((int) (value >> 32));
        WriteInt32((int) (value & 0xffffffff));
        return this;
    }
    
    public BufferWriter WriteFixedByteArray(byte[] arr)
    {
        Stream.Write(arr, 0, arr.Length);
        return this;
    }

    public BufferWriter WriteFixedByteArray(int size, byte[] arr)
    {
        // Additional sanity check
        if (arr.Length != size)
            throw new ArgumentException($"Array length {arr.Length} does not match size {size}");
        
        return WriteFixedByteArray(arr);
    }
    
    public BufferWriter WriteFloat(float value) => WriteInt32(BitConverter.ToInt32(BitConverter.GetBytes(value), 0));
    
    public BufferWriter WriteDouble(double value) => WriteInt64(BitConverter.DoubleToInt64Bits(value));
    
    public BufferWriter WriteGuid(Guid value) => WriteFixedByteArray(16, value.ToByteArray());
    
    private BufferWriter WriteOptional<T>(Func<bool> isPresent, Action<BufferWriter, T> write, Func<T> value)
    {
        if (isPresent())
        {
            WriteBool(true);
            write(this, value());
        }
        else
        {
            WriteBool(false);
        }
        
        return this;
    }
    
    public BufferWriter WriteOptional<T>(T? value, Action<BufferWriter, T> write) where T : struct =>
        WriteOptional(() => value.HasValue, write, () => value!.Value);

    public BufferWriter WriteOptional<T>(T value, Action<BufferWriter, T> write) where T : class =>
        WriteOptional(() => value != null, write, () => value!);
    
    public BufferWriter WriteOptional<T>(IOptional<T> value, Action<BufferWriter, T> write) =>
        WriteOptional(() => value.IsPresent, write, () => value.Value);
    
    public BufferWriter WriteEnum<T>(T value) where T : struct => 
        WriteVarInt((int) Convert.ChangeType(value, TypeCode.Int32));
}