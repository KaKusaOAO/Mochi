using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace KaLib.Localizations
{
    public class I18n
    {
        private const string localeDir = "lang";
        private const string baseName = "base";
        private static Dictionary<string, string> baseLocale = new Dictionary<string, string>();
        private static bool initialized;
        private string currentLocale;
        private Dictionary<string, string> locale = new Dictionary<string, string>();

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

        public string CurrentLocale => string.IsNullOrEmpty(currentLocale) ? baseName : currentLocale;

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
            locale?.Clear();
            currentLocale = key;
            if (string.IsNullOrEmpty(key)) return;
            
            var baseFile = $"{localeDir}/{key}.json";
            if (!Directory.Exists(localeDir))
            {
                Directory.CreateDirectory(localeDir);
            }

            if (File.Exists(baseFile))
            {
                locale = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(baseFile))
                    ?? new Dictionary<string, string>();
            }
        }

        public virtual string Get(string key)
        {
            if (locale?.ContainsKey(key) ?? false) return locale[key];
            return baseLocale?.ContainsKey(key) ?? false ? baseLocale[key] : key;
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