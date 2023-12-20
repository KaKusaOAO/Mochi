namespace Mochi.Irc;

public class GenericCommand : IIrcCommand
{
    public string Command { get; }
    public List<string> Arguments { get; }

    public GenericCommand(string command, params string[] args)
    {
        Command = command;
        Arguments = args.ToList();
    }
}