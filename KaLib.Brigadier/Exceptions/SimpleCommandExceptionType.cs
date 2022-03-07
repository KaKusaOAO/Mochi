namespace KaLib.Brigadier.Exceptions
{
    public class SimpleCommandExceptionType : ICommandExceptionType {
        private readonly IMessage _message;

        public SimpleCommandExceptionType( IMessage message) {
            this._message = message;
        }

        public CommandSyntaxException Create() {
            return new CommandSyntaxException(this, _message);
        }

        public CommandSyntaxException CreateWithContext( IMmutableStringReader reader) {
            return new CommandSyntaxException(this, _message, reader.GetString(), reader.GetCursor());
        }

        public override string ToString() {
            return _message.GetString();
        }
    }
}