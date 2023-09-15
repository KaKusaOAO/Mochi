using System;
using Mochi.Brigadier.Tree;

namespace Mochi.Brigadier.Context;

public class ParsedCommandNode<T>
{
    public CommandNode<T> Node { get; }

    public StringRange Range { get; }
    
    public ParsedCommandNode(CommandNode<T> node, StringRange range)
    {
        Node = node;
        Range = range;
    }

    public override string ToString() => Node + "@" + Range;

    public override bool Equals(object? o)
    {
        if (this == o) return true;
        if (o is not ParsedCommandNode<T> that) return false;
        return Node == that.Node && Range == that.Range;
    }

    public override int GetHashCode() => HashCode.Combine(Node, Range);
}