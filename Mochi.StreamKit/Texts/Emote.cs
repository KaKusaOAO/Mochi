namespace Mochi.StreamKit.Texts;

public class Emote
{
    public string Name { get; }
    public Uri ImageUrl { get; }

    public Emote(string name, Uri url)
    {
        Name = name;
        ImageUrl = url;
    }

    public static Emote CreateFromTwitch(string name, string hash) => 
        new(name, new Uri($"https://static-cdn.jtvnw.net/emoticons/v2/{hash}/default/light/3.0"));
}