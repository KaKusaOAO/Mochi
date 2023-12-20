using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mochi.Brigadier.Context;

namespace Mochi.Brigadier.Suggests;

public class Suggestions
{
    private static readonly Suggestions _internalEmpty = new(StringRange.At(0), new List<Suggestion>());

    private readonly StringRange _range;
    private readonly List<Suggestion> _suggestions;

    public Suggestions(StringRange range, List<Suggestion> suggestions)
    {
        _range = range;
        _suggestions = suggestions;
    }

    public StringRange GetRange()
    {
        return _range;
    }

    public List<Suggestion> GetList()
    {
        return _suggestions;
    }

    public bool IsEmpty()
    {
        return _suggestions.Count == 0;
    }

    public override bool Equals(object? o)
    {
        if (this == o) return true;
        if (o is not Suggestions that) return false;
        return _range == that._range &&
               _suggestions == that._suggestions;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_range, _suggestions);
    }

    public override string ToString()
    {
        return "Suggestions{" +
               "range=" + _range +
               ", suggestions=" + _suggestions +
               '}';
    }

    public static async Task<Suggestions> Empty()
    {
        await Task.CompletedTask;
        return _internalEmpty;
    }

    public static Suggestions Merge(string command, ICollection<Suggestions> input)
    {
        if (input.Count == 0) return _internalEmpty;
        if (input.Count == 1) return input.First();

        var texts = new HashSet<Suggestion>();
        foreach (var suggestions in input)
        {
            foreach (var item in suggestions.GetList())
            {
                texts.Add(item);
            }
        }

        return Create(command, texts);
    }

    public static Suggestions Create(string command, ICollection<Suggestion> suggestions)
    {
        if (suggestions.Count == 0)
        {
            return _internalEmpty;
        }

        var start = int.MaxValue;
        var end = int.MinValue;
        foreach (var suggestion in suggestions)
        {
            start = Math.Min(suggestion.Range.Start, start);
            end = Math.Max(suggestion.Range.End, end);
        }

        var range = new StringRange(start, end);
        var texts = new HashSet<Suggestion>();
        foreach (var suggestion in suggestions)
        {
            texts.Add(suggestion.Expand(command, range));
        }

        var sorted = new List<Suggestion>(texts);
        sorted.Sort((a, b) => a.CompareToIgnoreCase(b));
        return new Suggestions(range, sorted);
    }
}