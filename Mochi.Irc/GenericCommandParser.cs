namespace Mochi.Irc;

public class GenericCommandParser : IIrcCommandParser
{
    public string CommandName { get; }

    public GenericCommandParser(string commandName)
    {
        CommandName = commandName;
    } 
    
    public IIrcCommand Parse(string args) => new GenericCommand(CommandName, args.Split(' '));
}