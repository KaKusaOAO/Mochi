using System.Collections.Generic;
using Newtonsoft.Json;

namespace KaLib.Localizations
{
    public static class I18n
    {
        private static Dictionary<string, string>? baseLocale = new();
        private static Dictionary<string, string>? locale = new();

        private const string localeDir = "lang";
        private const string baseName = "base";
        
        public static void Initialize()
        {
            var baseFile = $"{localeDir}/{baseName}.json";
            if (!Directory.Exists(localeDir))
            {
                Directory.CreateDirectory(localeDir);
            }

            if (File.Exists(baseFile))
            {
                baseLocale = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(baseFile))
                    ?? new Dictionary<string, string>();
            }
        }

        public static void SetLocale(string key)
        {
            var baseFile = $"{localeDir}/{key}.json";
            if (!Directory.Exists(localeDir))
            {
                Directory.CreateDirectory(localeDir);
            }

            locale?.Clear();
            if (File.Exists(baseFile))
            {
                locale = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(baseFile))
                    ?? new Dictionary<string, string>();
            }
        }

        public static string Get(string key)
        {
            if (locale.ContainsKey(key)) return locale[key];
            return baseLocale.ContainsKey(key) ? baseLocale[key] : key;
        }

        public static string Format(string key, params object[] args)
            => string.Format(Get(key), args);
    }
}