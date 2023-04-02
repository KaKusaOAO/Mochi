using System;
using Mochi.Brigadier.Tree;

namespace Mochi.Brigadier.Context;

public class ParsedCommandNode<TS>
{
    private readonly CommandNode<TS> _node;

    private readonly StringRange _range;

    public ParsedCommandNode(CommandNode<TS> node, StringRange range)
    {
        this._node = node;
        this._range = range;
    }

    public CommandNode<TS> GetNode()
    {
        return _node;
    }

    public StringRange GetRange()
    {
        return _range;
    }

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

    public override int GetHashCode()
    {
#if NETCOREAPP
            return HashCode.Combine(_node, _range);
#else
        return _node.GetHashCode() * 31 + _range.GetHashCode();
#endif
    }
}