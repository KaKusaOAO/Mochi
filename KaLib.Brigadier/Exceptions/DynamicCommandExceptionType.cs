using System;

namespace KaLib.Brigadier.Exceptions
{
    public class DynamicCommandExceptionType : ICommandExceptionType {
        private readonly Func<object, IMessage> _function;

        public DynamicCommandExceptionType(Func<object, IMessage> function) {
            this._function = function;
        }

        public CommandSyntaxException Create(object arg, Exception inner = null) {
            return new CommandSyntaxException(this, _function(arg), inner);
        }

        public CommandSyntaxException CreateWithContext(IMutableStringReader reader, object arg, Exception inner = null) {
            return new CommandSyntaxException(this, _function(arg), reader.GetString(), reader.GetCursor(), inner);
        }
    }
}
