﻿using System;
using System.Collections.Generic;
using System.Linq;
using Mochi.Utils;

namespace Mochi.Extensions;

public static class Extension
{
    public static string Hexdump(this IEnumerable<byte> data) => 
        string.Join(' ', data.Select(t => $"{t:x2}")).Trim();
    
    public static TValue ComputeIfAbsent<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key,
        Func<TKey, TValue> resolver)
    {
        if (dict.TryGetValue(key, out var value)) return value;
        var item = resolver(key);
        dict.Add(key, item);
        return item;
    }

    public static TValue? AddOrSet<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, TValue value)
    {
        if (dict.ContainsKey(key))
        {
            var old = dict[key];
            dict[key] = value;
            return old;
        }
        
        dict.Add(key, value);
        return default;
    }
    
    public static IOptional<T> FindFirst<T>(this IEnumerable<T> source) => 
        source
            .Select(Optional.Of)
            .Append(Optional.Empty<T>())
            .First();
}