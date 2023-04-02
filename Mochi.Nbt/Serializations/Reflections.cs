using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Mochi.Nbt.Serializations.Converters;

namespace Mochi.Nbt.Serializations;

internal static class Reflections
{
    internal static string[] GetProperties<T>() => GetProperties(typeof(T));

    internal static string[] GetProperties(Type t)
    {
        var results = new List<string>();
        var props = t.GetProperties();
        foreach (var prop in props) results.Add(prop.Name);
        return results.ToArray();
    }

    internal static Dictionary<string, object> ToDictionary(object obj)
    {
        var result = new Dictionary<string, object>();
        var t = obj.GetType();
        foreach (var prop in GetProperties(t))
        {
            var p = t.GetProperty(prop);
            var op = p.GetValue(obj);
            if (op != null)
            {
                var attr = p.GetCustomAttribute<NbtConverterAttribute>();
                if (attr != null)
                {
                    var c = (NbtConverter)attr.ConverterType.GetConstructor(new Type[0]).Invoke(Array.Empty<object>());
                    op = c.ToNbt(op);
                }
                else
                {
                    op = ToSerializable(op);
                }

                if (p.GetCustomAttribute<NbtIgnoreAttribute>() == null)
                {
                    var pAttr = p.GetCustomAttribute<NbtPropertyAttribute>();
                    if (pAttr != null)
                        result.Add(pAttr.Name, op);
                    else
                        result.Add(prop, op);
                }
            }
        }

        return result;
    }

    internal static object ToSerializable(object o)
    {
        if (o is Dictionary<string, object>) return o;

        if (o is NbtTag) return o;

        var t = o.GetType();
        if (t.IsPrimitive) return o;

        if (o is string s) return s;

        if (IsInstanceOfGenericType(typeof(IDictionary<,>), o))
        {
            var d = o as IDictionary<object, object>;
            var args = t.GenericTypeArguments;
            if (args[0] != typeof(string))
            {
                var inner = new Dictionary<string, object>();
                foreach (var pair in d) inner.Add(pair.Key.ToString(), ToSerializable(pair.Value));
                return inner;
            }

            return o;
        }

        if (o is ICollection || IsInstanceOfGenericType(typeof(ICollection<>), o)) return o;

        return ToDictionary(o);
    }

    internal static bool IsInstanceOfGenericType(Type genericType, object instance) =>
        IsTypeOfGenericType(genericType, instance.GetType());

    internal static bool IsTypeOfGenericType(Type genericType, Type type)
    {
        while (type != null)
        {
            if (type.IsGenericType &&
                type.GetGenericTypeDefinition() == genericType)
                return true;
            type = type.BaseType;
        }

        return false;
    }
}