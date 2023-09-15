using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mochi.Brigadier.Builder;
using Mochi.Brigadier.Context;
using Mochi.Brigadier.Suggests;

namespace Mochi.Brigadier.Tree;

public class RootCommandNode<T> : CommandNode<T>
{
    public RootCommandNode() : 
        base(null, _ => true, null, s => new[] { s.Source }, false)
    {
    }

    public override string Name => "";

    public override string GetUsageText()
    {
        return "";
    }

    public override void Parse(StringReader reader, CommandContextBuilder<T> contextBuilder)
    {
    }

    public override Task<Suggestions> ListSuggestionsAsync(CommandContext<T> context, SuggestionsBuilder builder) => 
        Suggestions.Empty();

    protected override bool IsValidInput(string input) => false;

    public override bool Equals(object? o)
    {
        if (this == o) return true;
        return o is RootCommandNode<T> && Equals(o);
    }

    public override int GetHashCode() => HashCode.Combine(this);

    public override IArgumentBuilder<T> CreateBuilder() => 
        throw new Exception("Cannot convert root into a builder");

    protected override string GetSortedKey() => "";

    public override ICollection<string> Examples => new List<string>();

    public override string ToString() => "<root>";
}