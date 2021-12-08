using System.Collections.Generic;
using Newtonsoft.Json;

namespace KaLib.Localizations
{
    public class I18n
    {
        private static Dictionary<string, string>? baseLocale = new();
        private Dictionary<string, string>? locale = new();

        private const string localeDir = "lang";
        private const string baseName = "base";
        private static bool initialized;

        public I18n(string locale = "")
        {
            if (!initialized)
            {
                Initialize();
            }

            if (locale.Length > 0)
            {
                SetLocale(locale);
            }
        }
        
        public static void Initialize()
        {
            initialized = true;
            
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

        public void SetLocale(string key)
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

        public string Get(string key)
        {
            if (locale.ContainsKey(key)) return locale[key];
            return baseLocale.ContainsKey(key) ? baseLocale[key] : key;
        }

        public string Format(string key, params object[] args)
        {
            try
            {
                return string.Format(Get(key), args);
            }
            catch (FormatException)
            {
                return Get(key);
            }
        }
    }
}