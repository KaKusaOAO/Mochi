using System.Text;
using Mochi.Strings;

namespace Mochi.StreamKit.Twitch.API;

public class StringTemplate
{
    public List<string> Fields { get; } = new();
    public List<string> Components { get; } = new();

    public static StringTemplate Parse(string template)
    {
        var reader = new StringParser(template);
        var componentBuilder = new StringBuilder();
        var fieldBuilder = new StringBuilder();

        var result = new StringTemplate();
        var isField = false;
        while (reader.CanRead())
        {
            var c = reader.Read();
            if (!isField)
            {
                if (c == '{' && reader.CanRead() && reader.Peek() == '{')
                {
                    reader.Skip();
                    result.Components.Add(componentBuilder.ToString());
                    componentBuilder.Clear();
                    isField = true;
                    continue;
                }

                componentBuilder.Append(c);
            } else if (isField)
            {
                if (c == '}' && reader.CanRead() && reader.Peek() == '}')
                {
                    reader.Skip();
                    result.Fields.Add(fieldBuilder.ToString());
                    fieldBuilder.Clear();
                    isField = false;
                    continue;
                }

                fieldBuilder.Append(c);
                if (!reader.CanRead())
                    throw new ArgumentException("Unclosed field name");
            }
        }
        
        result.Components.Add(componentBuilder.ToString());
        return result;
    }

    public string Resolve(IDictionary<string, string> values)
    {
        var resolvableFields = values.Keys.ToHashSet();
        resolvableFields.IntersectWith(Fields);

        var sb = new StringBuilder();
        for (var i = 0; i < Fields.Count; i++)
        {
            sb.Append(Components[i]);

            var field = Fields[i];
            if (resolvableFields.Contains(field))
            {
                sb.Append(values[field]);
            }
            else
            {
                sb.Append("{{" + field + "}}");
            }
        }

        sb.Append(Components.Last());
        return sb.ToString();
    }

    public override string ToString() => Resolve(new Dictionary<string, string>());
}