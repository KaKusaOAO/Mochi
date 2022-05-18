using KaLib;
using KaLib.Brigadier;
using KaLib.Brigadier.Arguments;
using KaLib.Brigadier.Builder;
using KaLib.Brigadier.Tree;
using KaLib.Texts;
using KaLib.Utils;

public static class Helper
{
    public static List<string> AutoComplete(string input, int index, CommandDispatcher<CommandSource> dispatcher)
    {
        return new List<string>();
    }
    
    public static void Render(string input, string suggestion, int suggestionCursor, int index, CommandDispatcher<CommandSource> dispatcher)
    {       
        Console.CursorTop--;
        var c = Console.CursorLeft;
        Console.CursorLeft = 0;
        Terminal.ClearLine();
        Console.CursorLeft = c;
        Console.CursorTop++;
        
        var result = dispatcher.Parse(input, new CommandSource());
        var err = result.GetExceptions();
        if (err.Any())
        {
            Terminal.Write(LiteralText.Of(input).SetColor(TextColor.Red));
            var c1 = Console.CursorLeft;
            Terminal.ClearRemaining();
            
            Console.CursorLeft = c1 + 1;
            Terminal.Write(LiteralText.Of("<- " + err.First().Value.Message).SetColor(TextColor.Red));
            Terminal.ClearRemaining();
            Console.CursorLeft = c1;
            return;
        }

        var reader = result.GetReader();

        var context = result.GetContext();
        if (reader.CanRead())
        {
            if (context.GetRange().IsEmpty())
            {
                Terminal.Write(LiteralText.Of(input).SetColor(TextColor.Red));
                Terminal.ClearRemaining();
                return;
            }
            
            Terminal.Write(LiteralText.Of(reader.GetRead()));
            Terminal.Write(LiteralText.Of(reader.GetRemaining()).SetColor(TextColor.Red));
            var c1 = Console.CursorLeft;
            Terminal.ClearRemaining();

            Console.CursorLeft = c1 + 1;
            Terminal.Write(LiteralText.Of(" <- Incorrect argument").SetColor(TextColor.Red));
            Terminal.Write(" ".PadRight(Console.BufferWidth - 1 - Console.CursorLeft));
            Console.CursorLeft = c1;
            return;
        }
        
        var started = false;
        var startFrom = 0;

        var colors = new TextColor[]
        {
            TextColor.Aqua, TextColor.Yellow, TextColor.Green, TextColor.Purple
        };
        var colorIndex = 0;
        
        foreach (var node in context.GetNodes())
        {
            if (node == null)
            {
                Terminal.WriteLineStdOut("node is null??");
                continue;
            }
            
            try
            {
                if (started) Terminal.Write(" ");
                startFrom = node.GetRange().GetEnd();

                var useColor = node.GetNode() is not LiteralCommandNode<CommandSource>;
                Terminal.Write(LiteralText.Of(node.GetRange().Get(reader)).SetColor(useColor ? colors[colorIndex++] : null));
                colorIndex %= colors.Length;
                started = true;
            }
            catch (Exception ex)
            {
                var range = node.GetRange();
                Terminal.Write(LiteralText.Of(input[range.GetStart()..range.GetEnd()])
                    .SetColor(TextColor.Red));

                var c1 = c + reader.GetCursor();
                Console.CursorLeft = c1 + 1;
                Terminal.Write(LiteralText.Of("<- " + ex.Message).SetColor(TextColor.Red));
                Terminal.ClearRemaining();
                Console.CursorLeft = c1;
            }
        }

        Terminal.Write(input[startFrom..]);
        Terminal.ClearRemaining();
    }
}

public static class Program
{
    private static LiteralArgumentBuilder<CommandSource> Literal(string name) =>
        LiteralArgumentBuilder<CommandSource>.Literal(name);
    
    private static RequiredArgumentBuilder<CommandSource, T> Argument<T>(string name, IArgumentType<T> type)
        => RequiredArgumentBuilder<CommandSource, T>.Argument(name, type);

    public static void Main(string[] args)
    {
        var dispatcher = new CommandDispatcher<CommandSource>();
        
        // Register some commands
        var rootNode = dispatcher.Register(Literal("execute"));
        dispatcher.Register(Literal("execute")
            .Then(Literal("run").Redirect(dispatcher.GetRoot()))
            .Then(Literal("at")
                .Then(Argument("target", StringArgumentType.Word()).Fork(rootNode, context => new[] { context.GetSource() })))
            .Then(Literal("as")
                .Then(Argument("target", StringArgumentType.Word()).Fork(rootNode, context => new[] { context.GetSource() })))
        );
        
        dispatcher.Register(Literal("foo")
            .Then(Literal("bar").Executes(context =>
            {
                Terminal.WriteLineStdOut("FooBar");
                return 1;
            })
            .Executes(context =>
            {
                Terminal.WriteLineStdOut("Foo without Bar");
                return 1;
            }))
        );
        
        while (true)
        {
            try
            {
                var line = Terminal.ReadLine("> ",
                    (input, index) => Helper.AutoComplete(input, index, dispatcher),
                    (input, suggestion, suggestionCursor, index) => Helper.Render(input, suggestion, suggestionCursor, index, dispatcher));

                dispatcher.Execute(line, new CommandSource());
            }
            catch (Exception ex)
            {
                Terminal.WriteLineStdOut("Exception occurred!");
                Terminal.WriteLineStdOut(ex.ToString());
                Terminal.WriteLineStdOut(ex.StackTrace);
            }
        }
    }
}