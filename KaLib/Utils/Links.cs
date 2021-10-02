using System.Text.Encodings.Web;

namespace KaLib.Utils
{
    public static class Links
    {
        public static string TTS(string language, string content) =>
            $"https://translate.google.com/translate_tts?ie=UTF-8&tl={language}&client=tw-ob&q={UrlEncoder.Default.Encode(content)}";
    }
}