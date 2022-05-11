using System.Text;

namespace KaLib.Nbt
{
    internal static class NbtIO
    {
        internal static byte ReadByte(byte[] buffer, ref int index) => buffer[index++];

        internal static void WriteByte(byte b, List<byte> output)
        {
            output.Add(b);
        }

        internal static short ReadShort(byte[] buffer, ref int index)
        {
            short result = 0;
            for (var i = 0; i < 2; i++) result = (short)((result << 8) | buffer[index++]);
            return result;
        }

        internal static void WriteShort(short s, List<byte> output)
        {
            for (var i = 0; i < 2; i++)
            {
                var b = (byte)(s & 0xff);
                output.Add(b);
                s = (short)(s >> 8);
            }
        }

        internal static int ReadInt(byte[] buffer, ref int index)
        {
            var result = 0;
            for (var i = 0; i < 4; i++) result = (result << 8) | buffer[index++];
            return result;
        }

        internal static void WriteInt(int n, List<byte> output)
        {
            for (var i = 0; i < 4; i++)
            {
                var b = (byte)(n & 0xff);
                output.Add(b);
                n = n >> 8;
            }
        }

        internal static ushort ReadUShort(byte[] buffer, ref int index)
        {
            var a = ReadByte(buffer, ref index);
            var b = ReadByte(buffer, ref index);
            return (ushort)((a << 8) | b);
        }

        internal static uint ReadUInt(byte[] buffer, ref int index)
        {
            uint result = 0;
            for (var i = 0; i < 4; i++) result = (result << 8) | buffer[index++];
            return result;
        }

        internal static long ReadLong(byte[] buffer, ref int index)
        {
            long result = 0;
            for (var i = 0; i < 8; i++) result = (result << 8) | buffer[index++];
            return result;
        }

        internal static void WriteLong(long n, List<byte> output)
        {
            for (var i = 0; i < 8; i++)
            {
                var b = (byte)(n & 0xff);
                output.Add(b);
                n = n >> 8;
            }
        }

        internal static float ReadFloat(byte[] buffer, ref int index)
#if NETCOREAPP
            => BitConverter.Int32BitsToSingle(ReadInt(buffer, ref index));
#else
        {
            var arr = BitConverter.GetBytes(ReadInt(buffer, ref index));
            return BitConverter.ToSingle(arr, 0);
        }
#endif

        internal static double ReadDouble(byte[] buffer, ref int index) =>
            BitConverter.Int64BitsToDouble(ReadLong(buffer, ref index));

        internal static byte[] ReadByteArray(byte[] buffer, ref int index)
        {
            var length = ReadInt(buffer, ref index);
            var result = new byte[length];
            Array.Copy(buffer, index, result, 0, length);
            index += (int)length;
            return result;
        }

        internal static string ReadString(byte[] buffer, ref int index)
        {
            var length = ReadUShort(buffer, ref index);
            var result = new byte[length];
            Array.Copy(buffer, index, result, 0, length);
            index += length;
            return Encoding.UTF8.GetString(result);
        }

        internal static byte Peek(byte[] buffer, ref int index) => buffer[index];

        internal static NbtTag.TagType PeekType(byte[] buffer, ref int index) => (NbtTag.TagType)Peek(buffer, ref index);
    }
}