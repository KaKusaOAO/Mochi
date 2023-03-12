namespace KaLib.Brigadier.Exceptions
{
    public class Dynamic2CommandExceptionType : ICommandExceptionType {
        private readonly Function _function;

        public Dynamic2CommandExceptionType(Function function) {
            this._function = function;
        }

        public CommandSyntaxException Create(object a, object b) {
            return new CommandSyntaxException(this, _function(a, b));
        }

        public CommandSyntaxException CreateWithContext(IMutableStringReader reader, object a, object b) {
            return new CommandSyntaxException(this, _function(a, b), reader.GetString(), reader.GetCursor());
        }


        public delegate IMessage Function(object a, object b);
    }
}