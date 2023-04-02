using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using KaLib.Nbt.Serializations.Converters;

namespace KaLib.Nbt.Serializations;

public static class NbtConvert
{
    public static byte[] SerializeToBuffer<T>(T obj)
    {
        var serializer = new BufferSerializer();
        var o = Reflections.ToSerializable(obj);
        var tag = ToNbt(o);
        serializer.Write(tag);
        return serializer.ToBuffer();
    }

    public static NbtTag ToNbt<T>(T obj)
    {
        if (obj is NbtTag n) return n;
        if (obj is bool m) return new NbtByte((byte)(m ? 1 : 0));
        if (obj is byte a) return new NbtByte(a);
        if (obj is short b) return new NbtShort(b);
        if (obj is int c) return new NbtInt(c);
        if (obj is long d) return new NbtLong(d);
        if (obj is float e) return new NbtFloat(e);
        if (obj is double f) return new NbtDouble(f);
        if (obj is byte[] g) return new NbtByteArray(g);
        if (obj is string h) return new NbtString(h);
        if (obj is int[] k) return new NbtIntArray(k);
        if (obj is long[] l) return new NbtLongArray(l);

        if (obj is Dictionary<string, object> j)
        {
            var result = new NbtCompound();
            foreach (var pair in j)
            {
                var o = pair.Value;
                result.Add(pair.Key, ToNbt(Reflections.ToSerializable(o)));
            }

            return result;
        }

        if (obj is ICollection i)
        {
            if (i.Count == 0) return new NbtList(NbtTag.TagType.Byte);

            var t = i.GetType();
            var temp = ToNbt(t.GetProperty("Item").GetValue(i, new object[] { 0 }));
            var result = new NbtList((NbtTag.TagType)temp.RawType);
            result.Add(temp);

            for (var z = 1; z < i.Count; z++)
            {
                var x = t.GetProperty("Item").GetValue(i, new object[] { z });
                if (x != null) result.Add(ToNbt(x));
            }

            return result;
        }

        if (Reflections.IsInstanceOfGenericType(typeof(Nullable<>), obj))
        {
            var t = obj.GetType();
            var hasValue = (bool)t.GetProperty("HasValue")!.GetValue(obj);
            if (hasValue) return ToNbt(t.GetProperty("Value")!.GetValue(obj));
        }

        return ToNbt(Reflections.ToSerializable(obj));
    }

    public static string SerializeToString<T>(T obj)
    {
        if (obj == null) return "";

        var serializer = new StringSerializer();
        var o = Reflections.ToSerializable(obj);
        var tag = ToNbt(o);
        serializer.Write(tag);
        return serializer.ToString();
    }

    internal static object InternalDeserialize(Type t, byte[] arr, bool named = false) =>
        InternalDeserialize(t, arr, 0, arr.Length, named);

    internal static object InternalDeserialize(Type t, NbtTag tag)
    {
        if (t == null) return null;

        if (typeof(NbtCompound) == t) return tag;

        if (typeof(byte) == t)
        {
            if (!(tag is NbtByte))
                throw new InvalidOperationException("Cannot cast " + (NbtTag.TagType)tag.RawType + " into byte");
            return ((NbtByte)tag).Value;
        }

        if (typeof(bool) == t)
        {
            if (!(tag is NbtByte))
                throw new InvalidOperationException("Cannot cast " + (NbtTag.TagType)tag.RawType + " into bool");
            return ((NbtByte)tag).Value == 1;
        }

        if (typeof(short) == t)
        {
            if (!(tag is NbtShort))
                throw new InvalidOperationException("Cannot cast " + (NbtTag.TagType)tag.RawType + " into short");
            return ((NbtShort)tag).Value;
        }

        if (typeof(int) == t)
        {
            if (!(tag is NbtInt))
                throw new InvalidOperationException("Cannot cast " + (NbtTag.TagType)tag.RawType + " into int");
            return ((NbtInt)tag).Value;
        }

        if (typeof(long) == t)
        {
            if (!(tag is NbtLong))
                throw new InvalidOperationException("Cannot cast " + (NbtTag.TagType)tag.RawType + " into long");
            return ((NbtLong)tag).Value;
        }

        if (typeof(float) == t)
        {
            if (!(tag is NbtFloat))
                throw new InvalidOperationException("Cannot cast " + (NbtTag.TagType)tag.RawType + " into float");
            return ((NbtFloat)tag).Value;
        }

        if (typeof(double) == t)
        {
            if (!(tag is NbtDouble))
                throw new InvalidOperationException("Cannot cast " + (NbtTag.TagType)tag.RawType + " into double");
            return ((NbtDouble)tag).Value;
        }

        if (typeof(string) == t)
        {
            if (!(tag is NbtString))
                throw new InvalidOperationException("Cannot cast " + (NbtTag.TagType)tag.RawType + " into float");
            return ((NbtString)tag).Value;
        }

        if (typeof(int[]) == t)
        {
            if (!(tag is NbtIntArray))
                throw new InvalidOperationException("Cannot cast " + (NbtTag.TagType)tag.RawType + " into int[]");
            return ((NbtIntArray)tag).Value;
        }

        if (typeof(long[]) == t)
        {
            if (!(tag is NbtLongArray))
                throw new InvalidOperationException("Cannot cast " + (NbtTag.TagType)tag.RawType + " into long[]");
            return ((NbtLongArray)tag).Value;
        }

        if (typeof(byte[]) == t)
        {
            if (!(tag is NbtByteArray))
                throw new InvalidOperationException("Cannot cast " + (NbtTag.TagType)tag.RawType + " into byte[]");
            return ((NbtByteArray)tag).Value;
        }

        if (Reflections.IsTypeOfGenericType(typeof(Nullable<>), t))
            return InternalDeserialize(t.GetGenericArguments()[0], tag);

        if (Reflections.IsTypeOfGenericType(typeof(Array), t))
        {
            if (!(tag is NbtList))
                throw new InvalidOperationException("Cannot cast " + (NbtTag.TagType)tag.RawType + " into float");

            var list = new List<object>();
            foreach (var child in (NbtList)tag)
            {
                var related = t.GetGenericArguments()[0];
                list.Add(InternalDeserialize(related, child));
            }

            return Convert.ChangeType(list, t);
        }

        if (Reflections.IsTypeOfGenericType(typeof(List<>), t))
        {
            if (!(tag is NbtList))
                throw new InvalidOperationException("Cannot cast " + (NbtTag.TagType)tag.RawType + " into list");

            var nl = (NbtList)tag;
            var listType = TypeFromNbt(nl);

            var list = listType.GetConstructor(Type.EmptyTypes).Invoke(Array.Empty<object>());
            foreach (var child in nl)
                try
                {
                    var item = InternalDeserialize(listType.GenericTypeArguments[0], child);
                    listType.GetMethod("Add").Invoke(list, new[] { item });
                }
                catch (ArgumentException ex)
                {
                    throw new ArgumentException(
                        "Exception thrown when deserializing child: " + child.ToString() + ", listType=" + listType,
                        ex);
                }

            return list;
        }

        if (!(tag is NbtCompound)) return InternalDeserialize(TypeFromNbt(tag), tag);

        var result = new Dictionary<string, object>();
        var c = (NbtCompound)tag;
        foreach (var pair in c)
        {
            var related = TypeFromNbt(pair.Value);
            result.Add(pair.Key, InternalDeserialize(related, pair.Value));
        }

        return result;
    }

    [Obsolete]
    internal static Type FromNbtType(NbtTag.TagType type)
    {
        switch (type)
        {
            case NbtTag.TagType.Byte:
                return typeof(byte);
            case NbtTag.TagType.Short:
                return typeof(short);
            case NbtTag.TagType.Int:
                return typeof(int);
            case NbtTag.TagType.Long:
                return typeof(long);
            case NbtTag.TagType.Float:
                return typeof(float);
            case NbtTag.TagType.Double:
                return typeof(double);
            case NbtTag.TagType.ByteArray:
                return typeof(byte[]);
            case NbtTag.TagType.String:
                return typeof(string);
            case NbtTag.TagType.List:
                return typeof(List<object>);
            case NbtTag.TagType.Compound:
                return typeof(Dictionary<string, object>);
            case NbtTag.TagType.IntArray:
                return typeof(int[]);
            case NbtTag.TagType.LongArray:
                return typeof(long[]);
        }

        return null;
    }

    internal static Type TypeFromNbt(NbtTag tag)
    {
        switch ((NbtTag.TagType)tag.RawType)
        {
            case NbtTag.TagType.Byte:
                return typeof(byte);
            case NbtTag.TagType.Short:
                return typeof(short);
            case NbtTag.TagType.Int:
                return typeof(int);
            case NbtTag.TagType.Long:
                return typeof(long);
            case NbtTag.TagType.Float:
                return typeof(float);
            case NbtTag.TagType.Double:
                return typeof(double);
            case NbtTag.TagType.ByteArray:
                return typeof(byte[]);
            case NbtTag.TagType.String:
                return typeof(string);
            case NbtTag.TagType.List:
                return typeof(List<object>);
            case NbtTag.TagType.Compound:
                return typeof(Dictionary<string, object>);
            case NbtTag.TagType.IntArray:
                return typeof(int[]);
            case NbtTag.TagType.LongArray:
                return typeof(long[]);
        }

        return null;
    }

    internal static object InternalDeserialize(Type t, byte[] arr, int index, int length, bool named = false)
    {
        var _i = index;
        var tag = NbtTag.Deserialize(arr, ref _i, named);

        if (typeof(NbtTag).IsAssignableFrom(t)) return tag;

        return InternalDeserialize(t, tag);
    }

    public static T Deserialize<T>(byte[] buffer, bool named = false)
    {
        var result = InternalDeserialize(typeof(T), buffer, named);
        if (result.GetType() == typeof(T)) return (T)result;

        if (result is Dictionary<string, object>)
            return (T)CastFromDictionary(typeof(T), (Dictionary<string, object>)result);

        return default;
    }

    public static T Deserialize<T>(NbtTag tag)
    {
        if (typeof(NbtTag).IsAssignableFrom(typeof(T))) return (T)(object)tag;

        var result = InternalDeserialize(typeof(T), tag);

        if (result.GetType() == typeof(T)) return (T)result;

        if (result is Dictionary<string, object>)
            return (T)CastFromDictionary(typeof(T), (Dictionary<string, object>)result);

        return default;
    }

    private static NbtTag FromObject(object obj)
    {
        if (obj is NbtTag n) return n;

        var index = 0;
        return NbtTag.Deserialize(SerializeToBuffer(obj), ref index);
    }

    // Called from deserializer
    private static object CastFromDictionary(Type t, Dictionary<string, object> dict)
    {
        var result = Activator.CreateInstance(t);

        if (result is NbtCompound c)
        {
            foreach (var pair in dict)
                if (pair.Value != null)
                    c.Add(pair.Key, FromObject(pair.Value));
            return result;
        }

        var props = t.GetProperties();
        foreach (var prop in props)
        {
            var attr = prop.GetCustomAttribute<NbtConverterAttribute>();
            if (attr != null)
            {
                var converter = (NbtConverter)Activator.CreateInstance(attr.ConverterType);
                var val = dict[prop.Name];
                prop.SetValue(result, converter.FromNbt(FromObject(val)));
            }
            else
            {
                var dictName = prop.Name;
                if (prop.GetCustomAttribute<NbtPropertyAttribute>() != null)
                {
                    var attr2 = prop.GetCustomAttribute<NbtPropertyAttribute>();
                    dictName = attr2.Name;
                }

                if (dict == null) return null;

                if (dict.ContainsKey(dictName))
                {
                    var val = dict[dictName];

                    if (val is Dictionary<string, object> dictionary)
                        val = CastFromDictionary(prop.PropertyType, dictionary);

                    if (val is byte b && prop.PropertyType == typeof(bool)) val = b == 1;

                    if (val is ICollection && !(val is Array) ||
                        Reflections.IsTypeOfGenericType(typeof(ICollection<>), prop.PropertyType))
                    {
                        var related = prop.PropertyType.GetGenericArguments()[0];
                        var listType = typeof(List<>).MakeGenericType(related);

                        var list = listType.GetConstructor(Type.EmptyTypes).Invoke(Array.Empty<object>());
                        var count = (int)val.GetType().GetProperty("Count").GetValue(val);

                        for (var i = 0; i < count; i++)
                        {
                            var child = val.GetType().GetProperty("Item").GetValue(val, new object[] { i });
                            listType.GetMethod("Add").Invoke(list,
                                new object[] { CastFromDictionary(related, child as Dictionary<string, object>) });
                        }

                        val = list;
                    }

                    try
                    {
                        prop.SetValue(result, val);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }
        }

        return result;
    }
}