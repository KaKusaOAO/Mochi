using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mochi.Brigadier.Builder;
using Mochi.Brigadier.Context;
using Mochi.Brigadier.Suggests;

namespace Mochi.Brigadier.Tree;
#pragma warning disable CS0659
public class RootCommandNode<TS> : CommandNode<TS>
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

    public override void Parse(StringReader reader, CommandContextBuilder<TS> contextBuilder)
    {
    }

    public override Task<Suggestions> ListSuggestionsAsync(CommandContext<TS> context, SuggestionsBuilder builder)
    {
        return Suggestions.Empty();
    }

    protected override bool IsValidInput(string input)
    {
        return false;
    }

    public override bool Equals(object o)
    {
        if (this == o) return true;
        if (!(o is RootCommandNode<TS>)) return false;
        return Equals(o);
    }

    public override ArgumentBuilder<TS> CreateBuilder()
    {
        throw new Exception("Cannot convert root into a builder");
    }

    protected override string GetSortedKey()
    {
        return "";
    }

    public override IEnumerable<string> GetExamples()
    {
        return Array.Empty<string>();
    }

    public override string ToString()
    {
        return "<root>";
    }
}