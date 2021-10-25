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
            Protocol = url.Substring(0, colonIndex).ToLower();
            PathName = url.Substring(colonIndex + 3);
            Host = PathName.Split('/')[0];
            
            int queryIndex = url.IndexOf('?');
            if (queryIndex != -1)
            {
                string[] queries = url.Substring(queryIndex + 1).Split('&');
                foreach (var query in queries)
                {
                    string[] items = query.Split(new[] { '=' }, 2);
                    string key = items[0];
                    string val = items.Length > 1 ? items[1] : null;
                    SearchParams.Add(key, val);
                }
            }
        }
    }
}