namespace Mochi.Brigadier.Exceptions;

public class DynamicNCommandExceptionType : ICommandExceptionType
{
    private readonly Function _function;

    public DynamicNCommandExceptionType(Function function)
    {
        _function = function;
    }

    public CommandSyntaxException Create(object a, params object[] args)
    {
        return new CommandSyntaxException(this, _function(args));
    }

    public CommandSyntaxException CreateWithContext(IMutableStringReader reader, params object[] args)
    {
        return new CommandSyntaxException(this, _function(args), reader.GetString(), reader.Cursor);
    }

    public delegate IBrigadierMessage Function(object[] args);
}