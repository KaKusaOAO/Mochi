using System.Collections.Generic;

namespace Mochi.Texts;

public interface IText
{
    ICollection<IText> Extra { get; }
    IText Parent { get; set; }
    TextColor Color { get; }
    bool Bold { get; }
    bool Italic { get; }
    bool Obfuscated { get; }
    bool Underline { get; }
    bool Strikethrough { get; }
    bool Reset { get; }

    TextColor ParentColor { get; }
    string ToPlainText();
    string ToAscii();
    IText Clone();
    IMutableText MutableCopy();
}
    
public interface IMutableText : IText
{
    new ICollection<IText> Extra { set; }
    new TextColor Color { set; }
    new bool Bold { set; }
    new bool Italic { set; }
    new bool Obfuscated { set; }
    new bool Underline { set; }
    new bool Strikethrough { set; }
    new bool Reset { set; }
}

public interface ITextGenericHelper<out T>
{
    T AddExtra(params IText[] texts);
    T SetColor(TextColor color);
}

public interface IText<out T> : IText, ITextGenericHelper<T> where T : IText<T>
{
    T Format(TextFormatFlag flags);
    new T Clone();
}