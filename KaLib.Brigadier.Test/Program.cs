using System.Text;
using KaLib;
using KaLib.Brigadier;
using KaLib.Brigadier.Arguments;
using KaLib.Brigadier.Builder;
using KaLib.Brigadier.TerminalHelper;
using KaLib.Texts;
using KaLib.Utils;
using StringReader = KaLib.Brigadier.StringReader;

public class SpaceSeperatedArgument : IArgumentType<string>
{
    private SpaceSeperatedArgument() {}
    
    public static SpaceSeperatedArgument String() => new();
    
    public string Parse(StringReader reader)
    {
        var ch = reader.Peek();
        if (!StringReader.IsQuotedStringStart(ch))
        {
            var sb = new StringBuilder();
            while (reader.CanRead() && reader.Peek() == ' ') reader.Skip();
            while (reader.CanRead() && reader.Peek() != ' ') sb.Append(reader.Read());

            if (reader.CanRead())
            {
                while (reader.CanRead() && reader.Peek() == ' ') reader.Skip();
                reader.SetCursor(reader.GetCursor() - 1);
            }
            
            return sb.ToString();
        }
        reader.Skip();
        return reader.ReadStringUntil(ch);
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
        Logger.Logged += Logger.LogToEmulatedTerminalAsync;

        Task.Run(async () =>
        {
            var rand = new Random();
            var i = 0;
            while (true)
            {
                await Task.Delay(1000);
                Logger.Verbose($"Rand: {rand.Next(100)} (iteration: {i++})");
            }
        });
        
        var dispatcher = new CommandDispatcher<CommandSource>();
        
        // Register some commands
        var rootNode = dispatcher.Register(Literal("execute"));
        dispatcher.Register(Literal("execute")
            .Then(Literal("run").Redirect(dispatcher.GetRoot()))
            .Then(Literal("at")
                .Then(Argument("target", SpaceSeperatedArgument.String()).Fork(rootNode, context => new[] { context.GetSource() })))
            .Then(Literal("as")
                .Then(Argument("target", SpaceSeperatedArgument.String()).Fork(rootNode, context => new[] { context.GetSource() })))
        );
        
        dispatcher.Register(Literal("foo")
            .Then(Literal("bar").Executes(context =>
            {
                Logger.Info("FooBar", "/foo");
                return 1;
            }))
            .Executes(context =>
            {
                Logger.Info("Foo without Bar", "/foo");
                return 1;
            })
        );

        dispatcher.Register(Literal("data")
            .Then(Literal("modify")
                .Then(Literal("entity")
                    .Then(Argument("target", StringArgumentType.Word())
                        .Then(Argument("targetPath", StringArgumentType.Word())
                            .Then(Literal("set")
                                .Then(Literal("from")
                                    .Then(Literal("entity")
                                        .Then(Argument("source", StringArgumentType.Word())
                                            .Then(Argument("sourcePath", StringArgumentType.Word())
                                                .Executes(_ => 1)))))))))));
        
        while (true)
        {
            try
            {
                var source = new CommandSource();
                var line = Terminal.ReadLine("> ",
                    (input, index) => BrigadierTerminal.AutoComplete(input, index, dispatcher, source),
                    (input, suggestion, index) => BrigadierTerminal.Render(input, suggestion, index, dispatcher, source));

                var text = LiteralText.Of("Console issued: ")
                    .AddExtra(LiteralText.Of(line).SetColor(TextColor.Aqua));
                Logger.Info(text);
                dispatcher.Execute(line, new CommandSource());
            }
            catch (Exception ex)
            {
                Logger.Error("Exception occurred!");
                Logger.Error(ex.ToString());
            }
        }
    }
}