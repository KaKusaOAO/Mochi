using Mochi.StreamKit.Twitch.API;

namespace Mochi.StreamKit.Twitch.Entities;

public class Badge : IUserBadge
{
    public string Name { get; }
    
    string IUserBadge.ImageUrl => Version.ImageUrlLarge;
    
    public BadgeVersion Version { get; }
    
    public Badge(string name, BadgeVersion version)
    {
        Name = name;
        Version = version;
    }
}