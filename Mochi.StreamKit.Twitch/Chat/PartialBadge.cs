namespace Mochi.StreamKit.Twitch.Chat;

public class PartialBadge
{
    public string Name { get; }
    public string Version { get; }

    public PartialBadge(string name, string version)
    {
        Name = name;
        Version = version;
    }
}