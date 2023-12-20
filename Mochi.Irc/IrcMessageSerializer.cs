using System.Text;
using Mochi.Strings;

namespace Mochi.Irc;

public class IrcMessageSerializer
{
    private readonly Dictionary<string, IIrcCommandParser> _commandParsers = new();
    private readonly Dictionary<string, IIrcTagParser> _tagParsers = new();

    public IrcMessage Parse(string input)
    {
        string? rawTagsComponent = null;
        string? rawSourceComponent = null;
        string? rawParamsComponent = null;
        
        var reader = new StringParser(input);
        if (reader.Peek() == '@')
        {
            reader.Skip();
            var builder = new StringBuilder();
            while (reader.Peek() != ' ')
            {
                builder.Append(reader.Read());
            }

            rawTagsComponent = builder.ToString().Trim();
            reader.Skip();
        }

        if (reader.Peek() == ':')
        {
            reader.Skip();
            var builder = new StringBuilder();
            while (reader.Peek() != ' ')
            {
                builder.Append(reader.Read());
            }

            rawSourceComponent = builder.ToString().Trim();
            reader.Skip();
        }

        var commandBuilder = new StringBuilder();
        while (reader.CanRead())
        {
            if (reader.Peek() == ':') break;
            commandBuilder.Append(reader.Read());
        }
        var rawCommandComponent = commandBuilder.ToString().Trim();

        if (reader.CanRead())
        {
            var paramsReader = new StringBuilder();
            reader.Expect(':');
            while (reader.CanRead())
            {
                if (reader.Peek() is '\n' or '\r') break;
                paramsReader.Append(reader.Read());
            }

            rawParamsComponent = paramsReader.ToString().Trim();
        }

        var tags = new TagCollection();
        if (rawTagsComponent != null) tags = ParseTagsComponent(rawTagsComponent);

        MessageSource? source = null;
        if (rawSourceComponent != null) source = ParseSourceComponent(rawSourceComponent);

        var command = ParseCommandComponent(rawCommandComponent);
        return new IrcMessage(command)
        {
            Tags = tags,
            Source = source,
            Parameters = rawParamsComponent
        };
    }

    public bool TryParse(string input, out IrcMessage? result)
    {
        try
        {
            result = Parse(input);
            return true;
        }
        catch (Exception)
        {
            result = null;
            return false;
        }
    }

    public string Serialize(IrcMessage message)
    {
        var builder = new StringBuilder();
        if (message.Tags.Any())
        {
            builder.Append('@');

            var appendSemiColon = false;
            foreach (var (key, value) in message.Tags)
            {
                if (!appendSemiColon)
                {
                    appendSemiColon = true;
                }
                else
                {
                    builder.Append(';');
                }
                
                builder.Append(key);
                builder.Append('=');
                builder.Append(value.RawValue);
            }

            builder.Append(' ');
        }

        var source = message.Source;
        if (source != null)
        {
            builder.Append(':');
            if (source.Nick != null)
            {
                builder.Append(source.Nick);
                builder.Append('!');
            }

            builder.Append(source.Host);
            builder.Append(' ');
        }

        var command = message.Command;
        builder.Append(command.Command);
        builder.Append(' ');
        
        if (command.Arguments.Any())
        {
            builder.Append(string.Join(' ', command.Arguments));
            builder.Append(' ');
        }

        if (message.Parameters == null) return builder.ToString();
        
        builder.Append(':');
        builder.Append(message.Parameters);
        return builder.ToString();
    }

    private TagCollection ParseTagsComponent(string tags)
    {
        var result = new TagCollection();
        var pairs = tags.Split(';');

        foreach (var pair in pairs)
        {
            var arr = pair.Split('=');
            var parser = _tagParsers.GetValueOrDefault(arr[0]) ?? new GenericTagParser();
            result[arr[0]] = parser.Parse(arr[1]);
        }

        return result;
    }

    private MessageSource ParseSourceComponent(string source)
    {
        var parts = source.Split('!');
        var nick = parts.Length == 2 ? parts[0] : null;
        var host = parts.Length == 2 ? parts[1] : parts[0];
        return new MessageSource(host, nick);
    }

    private IIrcCommand ParseCommandComponent(string command)
    {
        var parts = command.Split(' ', 2);
        var parser = _commandParsers.GetValueOrDefault(parts[0]) ?? new GenericCommandParser(parts[0]);
        return parser.Parse(parts.Length == 2 ? parts[1] : "");
    }

    public IrcMessageSerializer RegisterCommandParser(string type, IIrcCommandParser parser)
    {
        _commandParsers[type] = parser;
        return this;
    }
    
    public IrcMessageSerializer RegisterTagParser(string type, IIrcTagParser parser)
    {
        _tagParsers[type] = parser;
        return this;
    }
}