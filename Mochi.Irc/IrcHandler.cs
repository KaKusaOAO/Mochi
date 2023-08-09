using Mochi.Texts;
using Mochi.Utils;

namespace Mochi.Irc;

public class IrcHandler
{
    private readonly StreamReader _reader;
    private readonly StreamWriter _writer;
    private readonly IrcMessageSerializer _serializer;

    public IrcHandler(Stream inputStream, Stream outputStream, IrcMessageSerializer? parser = null)
    {
        _reader = new StreamReader(inputStream);
        _writer = new StreamWriter(outputStream);
        _serializer = parser ?? new IrcMessageSerializer();
    }
    
    public IrcMessage? ReadMessage()
    {
        var line = _reader.ReadLine();
        Logger.Info(TranslateText.Of("Read: %s")
            .AddWith(LiteralText.Of(line).SetColor(TextColor.Gold))
        );
        return line == null ? null : _serializer.Parse(line);
    }

    public async Task<IrcMessage?> ReadMessageAsync() => await Task.Run(ReadMessage);

    public void WriteMessage(IrcMessage message)
    {
        var line = _serializer.Serialize(message);
        Logger.Info(TranslateText.Of("Sent: %s")
            .AddWith(LiteralText.Of(line).SetColor(TextColor.Aqua))
        );
        _writer.WriteLine(line);
        _writer.Flush();
    }

    public async Task WriteMessageAsync(IrcMessage message) => await Task.Run(() => WriteMessage(message));
}