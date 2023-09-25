using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Mochi.Nbt.Serializations;

public partial class StringTagVisitor : ITagVisitor
{
    private static readonly Regex _simpleValueRegex = CreateSimpleValueRegex(); 
    private readonly StringBuilder _sb = new();
    
    public string Visit(NbtTag tag)
    {
        tag.Accept(this);
        return _sb.ToString();
    }
    
    public void VisitString(NbtString tag) => _sb.Append(NbtString.QuoteAndEscape(tag.Value));

    public void VisitByte(NbtByte tag) => _sb.Append(tag.Value).Append('b');

    public void VisitShort(NbtShort tag) => _sb.Append(tag.Value).Append('s');

    public void VisitInt(NbtInt tag) => _sb.Append(tag.Value);

    public void VisitLong(NbtLong tag) => _sb.Append(tag.Value).Append('L');

    public void VisitFloat(NbtFloat tag) => _sb.Append(tag.Value).Append('f');

    public void VisitDouble(NbtDouble tag) => _sb.Append(tag.Value).Append('d');

    public void VisitByteArray(NbtByteArray tag)
    {
        _sb.Append("[B;");

        var arr = tag.Value;
        for (var i = 0; i < arr.Length; i++)
        {
            if (i != 0) _sb.Append(',');
            _sb.Append(arr[i]).Append('B');
        }

        _sb.Append(']');
    }

    public void VisitIntArray(NbtIntArray tag)
    {
        _sb.Append("[I;");

        var arr = tag.Value;
        for (var i = 0; i < arr.Length; i++)
        {
            if (i != 0) _sb.Append(',');
            _sb.Append(arr[i]);
        }

        _sb.Append(']');
    }

    public void VisitLongArray(NbtLongArray tag)
    {
        _sb.Append("[L;");

        var arr = tag.Value;
        for (var i = 0; i < arr.Length; i++)
        {
            if (i != 0) _sb.Append(',');
            _sb.Append(arr[i]).Append('L');
        }

        _sb.Append(']');
    }

    public void VisitList(NbtList tag)
    {
        _sb.Append('[');

        for (var i = 0; i < tag.Count; i++)
        {
            if (i != 0) _sb.Append(',');
            _sb.Append(new StringTagVisitor().Visit(tag[i]));
        }

        _sb.Append(']');
    }

    public void VisitCompound(NbtCompound tag)
    {
        _sb.Append('{');

        var keys = tag.Keys.OrderBy(t => t).ToList();
        for (var i = 0; i < keys.Count; i++)
        {
            if (i != 0) _sb.Append(',');

            var key = keys[i];
            _sb.Append(HandleEscape(key)).Append(':');
            _sb.Append(new StringTagVisitor().Visit(tag[key]));
        }

        _sb.Append('}');
    }

    private static string HandleEscape(string key) => 
        _simpleValueRegex.IsMatch(key) ? key : NbtString.QuoteAndEscape(key);

    public void VisitEnd(NbtEnd tag) => _sb.Append("END");

#if NET7_0_OR_GREATER
    [GeneratedRegex("^[A-Za-z0-9._+-]+$")]
    private static partial Regex CreateSimpleValueRegex();
#else
    private static Regex CreateSimpleValueRegex() => new("^[A-Za-z0-9._+-]+$");
#endif
}