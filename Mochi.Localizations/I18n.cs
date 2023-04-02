using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Mochi.Localizations
{
    public class I18n
    {
        private const string LocaleDir = "lang";
        private const string BaseName = "base";
        private static JToken _baseLocale;
        private static bool _initialized;
        private string _currentLocale;
        private JToken _locale;

        private I18n _parent;

        public I18n(string locale = "", I18n parent = null)
        {
            _parent = parent;
            if (_parent == null && !(string.IsNullOrEmpty(locale) || locale == BaseName))
            {
                _parent = new I18n();
            }
            
            if (!_initialized)
            {
                Initialize();
            }

            if (locale.Length > 0)
            {
                SetLocale(locale);
            }
        }

        public string CurrentLocale => string.IsNullOrEmpty(_currentLocale) ? BaseName : _currentLocale;

        public static void Initialize()
        {
            _initialized = true;
            
            var baseFile = $"{LocaleDir}/{BaseName}.json";
            if (!Directory.Exists(LocaleDir))
            {
                Directory.CreateDirectory(LocaleDir);
            }

            if (File.Exists(baseFile))
            {
                _baseLocale = JsonConvert.DeserializeObject<JToken>(File.ReadAllText(baseFile));
            }
        }

        public void SetLocale(string key)
        {
            _currentLocale = key;
            if (string.IsNullOrEmpty(key)) return;
            
            var baseFile = $"{LocaleDir}/{key}.json";
            if (!Directory.Exists(LocaleDir))
            {
                Directory.CreateDirectory(LocaleDir);
            }

            if (File.Exists(baseFile))
            {
                _locale = JsonConvert.DeserializeObject<JToken>(File.ReadAllText(baseFile));
            }
        }
        
        public virtual string Get(string key)
        {
            return _locale?[key]?.Value<string>() ??
                   _locale?.SelectToken($"$.{key}")?.Value<string>() ??
                   _parent?.Get(key) ??
                   _baseLocale?[key]?.Value<string>() ??
                   _baseLocale?.SelectToken($"$.{key}")?.Value<string>();
        }

        public string Format(string key, params object[] args)
        {
            try
            {
                return string.Format(Get(key) ?? key, args);
            }
            catch (FormatException)
            {
                return Get(key) ?? key;
            }
        }
    }
}