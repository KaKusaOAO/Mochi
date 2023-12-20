namespace Mochi.StreamKit;

public class ChatServiceType
{
    private static Dictionary<string, ChatServiceType> _map = new();

    public static readonly ChatServiceType Twitch = CreateAndRegister("twitch");
    
    public string Name { get; }
    
    private ChatServiceType(string name)
    {
        Name = name;
    }

    public static ChatServiceType CreateAndRegister(string name)
    {
        var type = new ChatServiceType(name);
        _map[name] = type;
        return type;
    }
}