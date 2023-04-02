using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SmartFormat;
using SmartFormat.Core.Settings;

namespace Mochi.Localizations;

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

    private static void Initialize()
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
    
    public virtual string Of(string key, Dictionary<string, object> placeholders)
    {
        placeholders ??= new Dictionary<string, object>();
        WriteDefaultPlaceholders(placeholders);
        Smart.Default.Settings.Parser.ErrorAction = ParseErrorAction.Ignore;
        return Smart.Format(Get(key) ?? key, placeholders);
    }
    
    protected virtual void WriteDefaultPlaceholders(Dictionary<string, object> placeholders)
    {
    }
    
    public virtual string Of(string key, params IPlaceholder[] placeholders)
    {
        var dict = new Dictionary<string, object>();
        foreach (var p in placeholders)
        {
            p.WritePlaceholders(dict);
        }

        return Of(key, dict);
    }

    
    public virtual string Of<T>(string key, T placeholder)
    {
        if (placeholder == null) return key;
        if (placeholder is IPlaceholder p)
        {
            return Of(key, p);
        }
        
        if (placeholder is Dictionary<string, object> dict)
        {
            return Of(key, dict);
        }

        if (placeholder is IDictionary dictGeneric)
        {
            if (dictGeneric.Keys is not ICollection<string> keys)
            {
                throw new NotSupportedException();
            }

            var d = keys.ToDictionary(k => k, k => dictGeneric[k]);
            return Of(key, d);
        }

        var propDict = new Dictionary<string, object>();
        var type = placeholder.GetType();
        var props = type.GetProperties();
        foreach (var prop in props)
        {
            if (prop.CanRead)
            {
                var val = prop.GetMethod!.Invoke(placeholder, Array.Empty<object>());
                propDict.Add(prop.Name, val);
            }
        }

        return Of(key, propDict);
    }

    public virtual string Of(string key) => Of(key, new Dictionary<string, object>());
}