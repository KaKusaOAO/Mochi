using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace KaLib.Nbt
{
    public abstract class NbtTag : INbtTag
    {
        public byte RawType { get; private set; }

        public enum TagType : byte
        {
            End,
            Byte,
            Short,
            Int,
            Long,
            Float,
            Double,
            ByteArray,
            String,
            List,
            Compound,
            IntArray,
            LongArray
        }

        public string Name { get; set; } = null;

        protected NbtTag(byte b) => RawType = b;

        public static NbtTag Deserialize(byte[] buffer, ref int index, bool named = false, TagType? type = null)
        {
            switch (type ?? (TagType)NbtIO.ReadByte(buffer, ref index))
            {
                case TagType.End:
                    return null;
                case TagType.Byte:
                    return NbtByte.Deserialize(buffer, ref index, named);
                case TagType.Short:
                    return NbtShort.Deserialize(buffer, ref index, named);
                case TagType.Int:
                    return NbtInt.Deserialize(buffer, ref index, named);
                case TagType.Long:
                    return NbtLong.Deserialize(buffer, ref index, named);
                case TagType.Float:
                    return NbtFloat.Deserialize(buffer, ref index, named);
                case TagType.Double:
                    return NbtDouble.Deserialize(buffer, ref index, named);
                case TagType.ByteArray:
                    return NbtByteArray.Deserialize(buffer, ref index, named);
                case TagType.String:
                    return NbtString.Deserialize(buffer, ref index, named);
                case TagType.List:
                    return NbtList.Deserialize(buffer, ref index, named);
                case TagType.Compound:
                    return NbtCompound.Deserialize(buffer, ref index, named);
                case TagType.IntArray:
                    return NbtIntArray.Deserialize(buffer, ref index, named);
                case TagType.LongArray:
                    return NbtLongArray.Deserialize(buffer, ref index, named);
            }

            throw new ArgumentException($"Unknown type {buffer[index]} at index {index}: {(char)buffer[index]}");
        }

        public T As<T>() where T : NbtTag => (T)this;

        public static NbtTag Deserialize(byte[] buffer, bool named = false, TagType? type = null)
        {
            var i = 0;
            return Deserialize(buffer, ref i, named, type);
        }

        protected static void InternalDeserializePhaseA(byte[] buffer, ref int index, bool named, TagType target,
            NbtTag instance)
        {
            if (named) instance.Name = NbtIO.ReadString(buffer, ref index);
        }

        public new abstract string ToString();

        public virtual string ToValue() => "";
    }

    public static class NbtExtension
    {
        public static T Copy<T>(this T self) where T : NbtTag
        {
            if (self is NbtByte b) return new NbtByte(b.Value) { Name = self.Name } as T;
            if (self is NbtByteArray bArr)
            {
                var arr = new byte[bArr.Value.Length];
                Array.Copy(bArr.Value, arr, arr.Length);
                return new NbtByteArray(arr) { Name = self.Name } as T;
            }

            if (self is NbtCompound c)
            {
                var result = new NbtCompound() { Name = self.Name };
                foreach (var entry in c) result.Add(entry.Key, entry.Value.Copy());

                return result as T;
            }

            if (self is NbtDouble d) return new NbtDouble(d.Value) { Name = self.Name } as T;
            if (self is NbtFloat f) return new NbtFloat(f.Value) { Name = self.Name } as T;
            if (self is NbtInt i) return new NbtInt(i.Value) { Name = self.Name } as T;
            if (self is NbtIntArray iArr)
            {
                var arr = new int[iArr.Value.Length];
                Array.Copy(iArr.Value, arr, arr.Length);
                return new NbtIntArray(arr) { Name = self.Name } as T;
            }

            if (self is NbtList list)
            {
                var result = new NbtList(list.ContentType) { Name = self.Name };
                foreach (var tag in list) result.Add(tag.Copy());

                return result as T;
            }

            if (self is NbtLong l) return new NbtLong(l.Value) { Name = self.Name } as T;
            if (self is NbtLongArray lArr)
            {
                var arr = new long[lArr.Value.Length];
                Array.Copy(lArr.Value, arr, arr.Length);
                return new NbtLongArray(arr) { Name = self.Name } as T;
            }

            if (self is NbtShort s) return new NbtShort(s.Value) { Name = self.Name } as T;
            if (self is NbtString str) return new NbtString(str.Value) { Name = self.Name } as T;

            throw new ArgumentException($"Unknown tag: {self}");
        }
    }
}