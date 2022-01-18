namespace KaLib.Brigadier.Exceptions;

public class DynamicCommandExceptionType : ICommandExceptionType {
    private readonly Func<object, IMessage> _function;

    public DynamicCommandExceptionType(Func<object, IMessage> function) {
        this._function = function;
    }

    public CommandSyntaxException Create(object arg) {
        return new CommandSyntaxException(this, _function(arg));
    }

    public CommandSyntaxException CreateWithContext(IMmutableStringReader reader, object arg) {
        return new CommandSyntaxException(this, _function(arg), reader.GetString(), reader.GetCursor());
    }
}
