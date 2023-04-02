namespace Mochi.Brigadier.Exceptions;

public class SimpleCommandExceptionType : ICommandExceptionType
{
    private readonly IBrigadierMessage _message;

    public SimpleCommandExceptionType(IBrigadierMessage message)
    {
        _message = message;
    }

    public CommandSyntaxException Create()
    {
        return new CommandSyntaxException(this, _message);
    }

    public CommandSyntaxException CreateWithContext(IMutableStringReader reader)
    {
        return new CommandSyntaxException(this, _message, reader.GetString(), reader.Cursor);
    }

    public override string ToString()
    {
        return _message.GetString();
    }
}