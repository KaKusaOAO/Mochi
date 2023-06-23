using System.Diagnostics;
using Mochi.Brigadier.Context;
using Mochi.Brigadier.Tree;
using Mochi.Texts;
using Mochi.Utils;

namespace Mochi.Brigadier.Bridge;

public static class BrigadierTerminal
{
    public static List<string> AutoComplete<T>(string input, int index, CommandDispatcher<T> dispatcher, T source)
    {
        var result = dispatcher.Parse(input, source);
        return dispatcher.GetCompletionSuggestions(result).Result.GetList().Select(s => s.Apply(input)).ToList();
    }
    
    public static void Render<T>(string input, string suggestion, int index, CommandDispatcher<T> dispatcher, T source)
    {       
        Console.CursorTop--;
        var c = Console.CursorLeft;
        Console.CursorLeft = 0;
        Terminal.ClearLine();
        Console.CursorLeft = c;
        Console.CursorTop++;

        void WriteWithSuggestion(IText text, int? co = null)
        {
            co ??= Console.CursorLeft;
            Terminal.Write(text);
            if (Console.CursorLeft - co < suggestion.Length && suggestion.StartsWith(input))
            {
                Terminal.Write(LiteralText.Of(suggestion[(Console.CursorLeft - co.Value)..]).SetColor(TextColor.DarkGray));
            }
        }

        void WriteError(string msg)
        {
            if (Console.BufferWidth - Console.CursorLeft <= msg.Length)
                msg = msg[..(Console.BufferWidth - Console.CursorLeft - 1)];
            Terminal.Write(LiteralText.Of(msg).SetColor(TextColor.Red));
        }
        
        void WriteUsage(string msg)
        {
            if (Console.BufferWidth - Console.CursorLeft <= msg.Length)
                msg = msg[..(Console.BufferWidth - Console.CursorLeft - 1)];
            Terminal.Write(LiteralText.Of(msg).SetColor(TextColor.DarkGray));
        }
        
        var result = dispatcher.Parse(input, source);
        var reader = result.Reader;
        var context = result.Context;
        
        if (reader.CanRead())
        {
            if (context.Range.IsEmpty)
            {
                WriteWithSuggestion(LiteralText.Of(input).SetColor(TextColor.Red));
                var c2 = Console.CursorLeft;
                Terminal.ClearRemaining();
                
                Console.CursorLeft = c2;
                WriteError(" <- Unknown command");
                Terminal.ClearRemaining();
                Console.CursorLeft = c2;
                return;
            }
        }
        
        var started = false;
        var startFrom = 0;

        var colors = new[]
        {
            TextColor.Aqua, TextColor.Yellow, TextColor.Green, TextColor.LightPurple
        };
        var colorIndex = 0;

        var coLeft = Console.CursorLeft;
        ParsedCommandNode<T> lastProcessedNode = null;
        while (context != null)
        {
            foreach (var node in context.Nodes)
            {
                if (node == null)
                {
                    Terminal.WriteLineStdOut("node is null??");
                    continue;
                }
            
                try
                {
                    if (started) Terminal.Write(" ");
                    startFrom = node.Range.End;

                    var useColor = node.Node is not LiteralCommandNode<T>;
                    var range = node.Range;
                    var text = LiteralText.Of(range.Get(reader)).SetColor(useColor ? colors[colorIndex++] : null);
                    Terminal.Write(text);

                    colorIndex %= colors.Length;
                    started = true;
                    lastProcessedNode = node;
                }
                catch (Exception ex)
                {
                    var range = node.Range;
                    WriteWithSuggestion(LiteralText.Of(input[range.Start..range.End])
                        .SetColor(TextColor.Red));

                    var c2 = c + reader.Cursor;
                    Console.CursorLeft = c2;
                    WriteError($" <- " + ex.Message);
                    Terminal.ClearRemaining();
                    return;
                }
            }

            var child = context.Child;
            if (child == null && reader.CanRead())
            {
                var last = context.Nodes.LastOrDefault();
                var nextNode = last?.Node?.Children?.FirstOrDefault();
                var usage = nextNode is ArgumentCommandNode<T> ? nextNode.GetUsageText() : null;
            
                WriteWithSuggestion(LiteralText.Of(reader.GetString()[startFrom..]).SetColor(TextColor.Red), coLeft);
                if (usage != null) WriteUsage($" :{usage}");
                var c1 = Console.CursorLeft;
                Terminal.ClearRemaining();

                Console.CursorLeft = c1;
                var errMsg = "Incorrect argument";
                var err = result.Exceptions;
                if (err.Any())
                {
                    errMsg = err.First().Value.Message;
                }

                WriteError($" <- {errMsg}");
                Terminal.ClearRemaining();
                return;
            }

            context = child;
        }
        
        WriteWithSuggestion(LiteralText.Of(""), coLeft);

        if (lastProcessedNode != null && lastProcessedNode.Node.Command == null)
        {
            WriteError(" <- Incomplete Command");
        }
        
        Terminal.ClearRemaining();
    }
}