using System;

namespace Mochi.Utils;

/// <summary>
/// Performs 32-bit reversed cyclic redundancy checks.
/// </summary>
public class Crc32
{
    private readonly uint[] _table;

    public static readonly Crc32 Shared = new();

    public uint Get(byte[] bytes, uint crc = 0xffffffff)
    {
        foreach (var t in bytes)
        {
            var index = (byte)((crc & 0xff) ^ t);
            crc = (crc >> 8) ^ _table[index];
        }

        return crc;
    }

    public byte[] GetBytes(byte[] bytes, uint crc = 0xffffffff)
    {
        return BitConverter.GetBytes(Get(bytes, crc));
    }

    public Crc32()
    {
        var poly = 0xedb88320;
        _table = new uint[256];

        for (uint i = 0; i < _table.Length; ++i)
        {
            var temp = i;
            for (var j = 8; j > 0; --j)
            {
                if ((temp & 1) == 1)
                {
                    temp = (temp >> 1) ^ poly;
                }
                else
                {
                    temp >>= 1;
                }
            }

            _table[i] = temp;
        }
    }
}