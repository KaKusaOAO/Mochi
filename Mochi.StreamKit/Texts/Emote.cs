namespace Mochi.StreamKit.Texts;

public class Emote
{
    public string Name { get; }
    public string ImageUrl { get; }

    public Emote(string name, string hash)
    {
        Name = name;
        ImageUrl = $"https://static-cdn.jtvnw.net/emoticons/v2/{hash}/default/light/3.0";
    }
}