using System;
using Mochi.Brigadier.Tree;

namespace Mochi.Brigadier.Context;

public class ParsedCommandNode<TS>
{
    private readonly CommandNode<TS> _node;

    private readonly StringRange _range;

    public ParsedCommandNode(CommandNode<TS> node, StringRange range)
    {
        _node = node;
        _range = range;
    }

    public CommandNode<TS> Node => _node;

    public StringRange Range => _range;

    public override string ToString()
    {
        return _node + "@" + _range;
    }

    public override bool Equals(object o)
    {
        if (this == o) return true;
        var that = o as ParsedCommandNode<TS>;
        if (that == null) return false;
        return _node == that._node && _range == that._range;
    }

    public override int GetHashCode() => HashCode.Combine(_node, _range);
}