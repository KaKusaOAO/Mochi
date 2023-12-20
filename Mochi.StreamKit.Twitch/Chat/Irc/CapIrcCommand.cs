using Mochi.Irc;

namespace Mochi.StreamKit.Twitch.Chat.Irc;

public class CapIrcCommand : IIrcCommand
{
    public CapabilityAction Action { get; }

    public CapIrcCommand(CapabilityAction action)
    {
        Action = action;
    }

    string IIrcCommand.Command => "CAP";
    List<string> IIrcCommand.Arguments => new() { Action.ToIrcArgument() };
}

public class CapIrcCommandParser : IIrcCommandParser<CapIrcCommand>
{
    public CapIrcCommand Parse(string args)
    {
        var action = CapabilityActions.FromIrcArgument(args.Trim());
        return new CapIrcCommand(action);
    }
}