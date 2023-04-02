using System.Collections.Generic;
using Mochi.Brigadier.Context;
using Mochi.Brigadier.Exceptions;
using Mochi.Brigadier.Tree;

namespace Mochi.Brigadier;

public class ParseResults<TS>
{
    private readonly CommandContextBuilder<TS> _context;
    private readonly Dictionary<CommandNode<TS>, CommandSyntaxException> _exceptions;
    private readonly IMutableStringReader _reader;

    public ParseResults(CommandContextBuilder<TS> context, IMutableStringReader reader,
        Dictionary<CommandNode<TS>, CommandSyntaxException> exceptions)
    {
        _context = context;
        _reader = reader;
        _exceptions = exceptions;
    }

    public ParseResults(CommandContextBuilder<TS> context) : this(context, new StringReader(""),
        new Dictionary<CommandNode<TS>, CommandSyntaxException>())
    {
    }

    public CommandContextBuilder<TS> Context => _context;
    public IMutableStringReader Reader => _reader;
    public Dictionary<CommandNode<TS>, CommandSyntaxException> Exceptions => _exceptions;
}