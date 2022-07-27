using System.Collections.Generic;

namespace KaLib.Texts
{
    public interface IText
    {
        ICollection<IText> Extra { get; }
        IText Parent { get; }
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
    }

    public interface IText<out T> : IText where T : IText<T>
    {
        T AddExtra(params IText[] texts);
        T SetColor(TextColor color);
        T Format(TextFormatFlag flags);
    }
}
