using System.Collections.Generic;
using Mochi.Brigadier.Context;
using Mochi.Brigadier.Exceptions;
using Mochi.Brigadier.Tree;

namespace Mochi.Brigadier;

public class ParseResults<TS>
{
    public CommandContextBuilder<TS> Context { get; }

    public IMutableStringReader Reader { get; }

    public Dictionary<ICommandNode<TS>, CommandSyntaxException> Exceptions { get; }
    
    public ParseResults(CommandContextBuilder<TS> context, IMutableStringReader reader,
        Dictionary<ICommandNode<TS>, CommandSyntaxException> exceptions)
    {
        Context = context;
        Reader = reader;
        Exceptions = exceptions;
    }

    public ParseResults(CommandContextBuilder<TS> context) : this(context, new StringReader(""),
        new Dictionary<ICommandNode<TS>, CommandSyntaxException>())
    {
    }
}