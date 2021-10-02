using System;
using System.Collections.Generic;

namespace KaLib.Utils
{
    public class URL
    {
        public Dictionary<string, string> SearchParams { get; } = new();
        
        public string PathName { get; }

        public string Host { get; }
        
        public string Protocol { get; }
        
        public URL(string url)
        {
            int colonIndex = url.IndexOf("://", StringComparison.Ordinal);
            if (colonIndex == -1)
            {
                throw new ArgumentException("URL must contain protocol (...://)");
            }
            Protocol = url[..colonIndex].ToLower();
            PathName = url[(colonIndex + 3)..];
            Host = PathName.Split('/')[0];
            
            int queryIndex = url.IndexOf('?');
            if (queryIndex != -1)
            {
                string[] queries = url[(queryIndex + 1)..].Split('&');
                foreach (var query in queries)
                {
                    string[] items = query.Split('=', 2);
                    string key = items[0];
                    string val = items.Length > 1 ? items[1] : null;
                    SearchParams.Add(key, val);
                }
            }
        }
    }
}