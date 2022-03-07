using System;
using System.Text;

namespace KaLib.Brigadier.Exceptions
{
    public class CommandSyntaxException : Exception
    {
        public const int ContextAmount = 10;
        public static bool EnableCommandStackTraces = true;
        public static IBuiltInExceptionProvider BuiltInExceptions = new BuiltInExceptions();

        private readonly ICommandExceptionType _type;
        private readonly IMessage _message;
        private readonly string _input;
        private readonly int _cursor;

        public CommandSyntaxException(ICommandExceptionType type, IMessage message) : base(message.GetString())
        {
            this._type = type;
            this._message = message;
            this._input = null;
            this._cursor = -1;
        }

        public CommandSyntaxException(ICommandExceptionType type, IMessage message, string input, int cursor) : base(
            message.GetString())
        {
            this._type = type;
            this._message = message;
            this._input = input;
            this._cursor = cursor;
        }

        public string GetMessage()
        {
            var message = this._message.GetString();
            var context = GetContext();
            if (context != null)
            {
                message += " at position " + _cursor + ": " + context;
            }

            return message;
        }

        public IMessage GetRawMessage()
        {
            return _message;
        }

        public string GetContext()
        {
            if (_input == null || this._cursor < 0)
            {
                return null;
            }

            var builder = new StringBuilder();
            var cursor = Math.Min(_input.Length, this._cursor);

            if (cursor > ContextAmount)
            {
                builder.Append("...");
            }

            builder.Append(_input.Substring(Math.Max(0, cursor - ContextAmount), cursor));
            builder.Append("<--[HERE]");

            return builder.ToString();
        }

        public ICommandExceptionType GetExceptionType()
        {
            return _type;
        }

        public string GetInput()
        {
            return _input;
        }

        public int GetCursor()
        {
            return _cursor;
        }
    }
}