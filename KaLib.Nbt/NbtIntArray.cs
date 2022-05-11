namespace KaLib.Nbt
{
    public class NbtIntArray : NbtTag, INbtValue<int[]>
    {
        public NbtIntArray() : base(11)
        {
        }

        public NbtIntArray(int[] arr) : base(11) => Value = arr;

        public static NbtIntArray FromGuid(Guid g)
        {
            var raw = g.ToByteArray();
            Array.Reverse(raw, 0, 4);
            Array.Reverse(raw, 4, 2);
            Array.Reverse(raw, 6, 2);

            return new NbtIntArray(new[]
            {
                BitConverter.ToInt32(raw, 0),
                BitConverter.ToInt32(raw, 4),
                BitConverter.ToInt32(raw, 8),
                BitConverter.ToInt32(raw, 12)
            });
        }

        public int[] Value { get; set; }

        public Guid ToGuid()
        {
            var array = Value;

            // Perform my magic here
            var raw = new byte[16];
            for (var i = 0; i < 4; i++)
            {
                var val = array[i];
                var bs = BitConverter.GetBytes(val);
                Array.Copy(bs, 0, raw, i * 4, 4);
            }

            Array.Reverse(raw, 0, 4);
            Array.Reverse(raw, 4, 2);
            Array.Reverse(raw, 6, 2);

            var uuid = "";
            for (var i = 0; i < 16; i++)
            {
                uuid += $"{raw[i]:x2}";
                if (i == 3 || i == 5 || i == 7 || i == 9) uuid += "-";
            }

            return Guid.Parse(uuid);
        }

        public static NbtIntArray Deserialize(byte[] buffer, ref int index, bool named = false)
        {
            var result = new NbtIntArray();
            InternalDeserializePhaseA(buffer, ref index, named, TagType.IntArray, result);
            var count = NbtIO.ReadInt(buffer, ref index);

            result.Value = new int[count];
            for (var i = 0; i < count; i++) result.Value[i] = NbtIO.ReadInt(buffer, ref index);

            return result;
        }

        public override string ToString()
        {
            var name = Name == null ? "None" : $"'{Name}'";
            return $"TAG_Int_Array({name}): [{Value.Length} items]";
        }
    }
}