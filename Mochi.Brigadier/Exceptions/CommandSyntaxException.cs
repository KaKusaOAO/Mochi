using System;
using System.Text;

namespace Mochi.Brigadier.Exceptions;

public class CommandSyntaxException : Exception
{
    public const int ContextAmount = 10;
    
    public static bool EnableCommandStackTraces = true;
    
    public IBrigadierMessage RawMessage { get; }
    public ICommandExceptionType ExceptionType { get; }
    public string? Input { get; }
    public int Cursor { get; }

    public static IBuiltInExceptionProvider BuiltInExceptions { get; set; } = new BuiltInExceptions();

    public CommandSyntaxException(ICommandExceptionType type, IBrigadierMessage message, Exception? inner = null) : base(
        message.GetString(), inner)
    {
        ExceptionType = type;
        RawMessage = message;
        Input = null;
        Cursor = -1;
    }

    public CommandSyntaxException(ICommandExceptionType type, IBrigadierMessage message, string? input, int cursor,
        Exception? inner = null) : base(
        message.GetString(), inner)
    {
        ExceptionType = type;
        RawMessage = message;
        Input = input;
        Cursor = cursor;
    }

    public override string Message
    {
        get
        {
            var message = RawMessage.GetString();
            var context = Context;
            if (context != null)
            {
                message += " at position " + Cursor + ": " + context;
            }

            return message;
        }
    }

    public string? Context
    {
        get
        {
            if (Input == null || Cursor < 0)
            {
                return null;
            }

            var builder = new StringBuilder();
            var cursor = Math.Min(Input.Length, Cursor);

            if (cursor > ContextAmount)
            {
                builder.Append("...");
            }

            builder.Append(Input.Substring(Math.Max(0, cursor - ContextAmount), cursor));
            builder.Append("<--[HERE]");

            return builder.ToString();
        }
    }
}