using System;
using System.Text;
using Mochi.Nbt.Serializations;

namespace Mochi.Nbt;

public class NbtString : NbtValue<string>
{
    public static NbtString Empty { get; } = new(string.Empty);
    
    public override TagTypeInfo TypeInfo => TagTypeInfo.String;

    public override string Value { get; }

    private NbtString(string value)
    {
        Value = value;
    }

    public static NbtString CreateValue(string value)
    {
        if (string.IsNullOrEmpty(value)) return Empty;
        return new NbtString(value);
    }

    public static string QuoteAndEscape(string str)
    {
        var sb = new StringBuilder(" ");
        var c = '\u0000';
        
        for (var i = 0; i < str.Length; i++)
        {
            var d = str[i];

            if (d == '\\')
            {
                sb.Append('\\');
            }
            else if (d is '"' or '\'')
            {
                if (c == 0)
                {
                    c = d == '"' ? (char)39 : (char)34;
                }

                if (c == d)
                {
                    sb.Append('\\');
                }
            }

            sb.Append(d);
        }

        if (c == 0) c = (char) 34;

        sb[0] = c;
        sb.Append(c);
        return sb.ToString();
    }

    public override void WriteContentTo(NbtWriter writer)
    {
        writer.WriteUtf(Value);
    }

    public override void Accept(ITagVisitor visitor) => visitor.VisitString(this);

    public sealed class StringTypeInfo : TagTypeInfo<NbtString>
    {
        internal static StringTypeInfo Instance { get; } = new();
        
        public override TagType Type => TagType.String;

        public override string FriendlyName => "TAG_String";
        
        private StringTypeInfo() {}

        protected override NbtString LoadValue(NbtReader reader)
        {
            var val = reader.ReadUtf();
            return CreateValue(val);
        }
    }
}