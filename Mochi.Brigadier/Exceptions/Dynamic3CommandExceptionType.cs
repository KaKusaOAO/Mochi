﻿namespace Mochi.Brigadier.Exceptions;

public class Dynamic3CommandExceptionType : ICommandExceptionType
{
    private readonly Function _function;

    public Dynamic3CommandExceptionType(Function function)
    {
        _function = function;
    }

    public CommandSyntaxException Create(object a, object b, object c)
    {
        return new CommandSyntaxException(this, _function(a, b, c));
    }

    public CommandSyntaxException CreateWithContext(IMutableStringReader reader, object a, object b, object c)
    {
        return new CommandSyntaxException(this, _function(a, b, c), reader.GetString(), reader.Cursor);
    }

    public delegate IBrigadierMessage Function(object a, object b, object c);
}