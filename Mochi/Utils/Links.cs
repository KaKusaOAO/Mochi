#if NET5_0_OR_GREATER || NETCOREAPP1_0_OR_GREATER
#define SUPPORTS_URLENCODE
#endif

using System.Text.Encodings.Web;

namespace Mochi.Utils
{
    public static class Links
    {
#if SUPPORTS_URLENCODE
        public static string TTS(string language, string content) =>
            $"https://translate.google.com/translate_tts?ie=UTF-8&tl={language}&client=tw-ob&q={UrlEncoder.Default.Encode(content)}";
#endif

        public static string Twitter(string username) => $"https://twitter.com/{username}";

        public static string YouTubeChannel(string channelId) => $"https://youtube.com/c/${channelId}";
    }
}