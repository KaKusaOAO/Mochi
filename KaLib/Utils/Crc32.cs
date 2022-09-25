using System;
using System.Collections.Generic;
using System.Linq;

namespace KaLib.Utils;

/// <summary>
/// Performs 32-bit reversed cyclic redundancy checks.
/// </summary>
public class Crc32
{
    uint[] table;

    public static readonly Crc32 Shared = new();

    public uint Get(byte[] bytes, uint crc = 0xffffffff)
    {
        // var crc = 0xffffffff;
        foreach (var t in bytes)
        {
            var index = (byte)((crc & 0xff) ^ t);
            crc = (crc >> 8) ^ table[index];
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
        table = new uint[256];
        
        uint temp;
        for (uint i = 0; i < table.Length; ++i)
        {
            temp = i;
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

            table[i] = temp;
        }
    }
}