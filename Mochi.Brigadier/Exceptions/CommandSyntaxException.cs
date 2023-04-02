﻿using System;
using System.Text;

namespace Mochi.Brigadier.Exceptions;

public class CommandSyntaxException : Exception
{
    public const int ContextAmount = 10;
    public static bool EnableCommandStackTraces = true;
    public static IBuiltInExceptionProvider BuiltInExceptions = new BuiltInExceptions();

    private readonly ICommandExceptionType _type;
    private readonly IBrigadierMessage _message;
    private readonly string _input;
    private readonly int _cursor;

    public CommandSyntaxException(ICommandExceptionType type, IBrigadierMessage message, Exception inner = null) : base(
        message.GetString(), inner)
    {
        _type = type;
        _message = message;
        _input = null;
        _cursor = -1;
    }

    public CommandSyntaxException(ICommandExceptionType type, IBrigadierMessage message, string input, int cursor,
        Exception inner = null) : base(
        message.GetString(), inner)
    {
        _type = type;
        _message = message;
        _input = input;
        _cursor = cursor;
    }

    public string GetMessage()
    {
        var message = _message.GetString();
        var context = GetContext();
        if (context != null)
        {
            message += " at position " + _cursor + ": " + context;
        }

        return message;
    }

    public IBrigadierMessage GetRawMessage()
    {
        return _message;
    }

    public string GetContext()
    {
        if (_input == null || _cursor < 0)
        {
            return null;
        }

        var builder = new StringBuilder();
        var cursor = Math.Min(_input.Length, _cursor);

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