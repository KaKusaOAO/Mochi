using System.Text.Encodings.Web;

namespace Mochi.Utils;

public static class Links
{
    public static URL TTS(string language, string content) => 
        new($"https://translate.google.com/translate_tts?ie=UTF-8&tl={language}&client=tw-ob&q={UrlEncoder.Default.Encode(content)}");

    public static URL Twitter(string username) => 
        new($"https://twitter.com/{username}");

    public static URL YouTubeChannel(string channelId) => 
        new($"https://youtube.com/c/${channelId}");

    public static URL GitHub(string username, string repo = "") => 
        new($"https://github.com/{username}/{repo}");
}