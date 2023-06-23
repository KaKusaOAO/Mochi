using System.Collections.Generic;

namespace Mochi.Texts;

public interface IText
{
    IContent Content { get; }
    ICollection<IText> Extra { get; }
    IText? Parent { get; set; }
    TextColor? Color { get; }
    bool Bold { get; }
    bool Italic { get; }
    bool Obfuscated { get; }
    bool Underline { get; }
    bool Strikethrough { get; }
    bool Reset { get; }

    TextColor? ParentColor { get; }
    string ToPlainText();
    string ToAnsi();
    IMutableText Clone();
}
    
public interface IMutableText : IText
{
    new IContent Content { get; set; }
    IContent IText.Content => Content;
    
    new ICollection<IText> Extra { get; set; }
    ICollection<IText> IText.Extra => Extra;
    
    new TextColor? Color { get; set; }
    TextColor? IText.Color => Color;
    
    new bool Bold { get; set; }
    bool IText.Bold => Bold;
    
    new bool Italic { get; set; }
    bool IText.Italic => Italic;
    
    new bool Obfuscated { get; set; }
    bool IText.Obfuscated => Obfuscated;
    
    new bool Underline { get; set; }
    bool IText.Underline => Underline;
    
    new bool Strikethrough { get; set; }
    bool IText.Strikethrough => Strikethrough;
    
    new bool Reset { get; set; }
    bool IText.Reset => Reset;
}

public interface ITextGenericHelper<out T>
{
    T AddExtra(params IText[] texts);
    T SetColor(TextColor? color);
    T Clone();
}