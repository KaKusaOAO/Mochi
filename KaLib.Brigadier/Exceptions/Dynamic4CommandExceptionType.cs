namespace KaLib.Brigadier.Exceptions
{
    public class Dynamic4CommandExceptionType : ICommandExceptionType {
        private readonly Function _function;

        public Dynamic4CommandExceptionType(Function function) {
            this._function = function;
        }

        public CommandSyntaxException Create(object a, object b, object c, object d) {
            return new CommandSyntaxException(this, _function(a, b, c, d));
        }

        public CommandSyntaxException CreateWithContext(IMmutableStringReader reader, object a, object b, object c, object d) {
            return new CommandSyntaxException(this, _function(a, b, c, d), reader.GetString(), reader.GetCursor());
        }

        public delegate IMessage Function(object a, object b, object c, object d);
    }
}
