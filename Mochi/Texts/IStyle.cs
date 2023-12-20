using System.Text.Json.Nodes;

namespace Mochi.Texts;

public interface IStyle
{
    public void SerializeInto(JsonObject obj);
    public IStyle ApplyTo(IStyle other);
    public IStyle Clear();
}

public interface IStyle<T> : IStyle where T : IStyle<T>
{
    public T ApplyTo(T other);
    IStyle IStyle.ApplyTo(IStyle other) => ApplyTo((T) other);

    public new T Clear();
    IStyle IStyle.Clear() => Clear();
}

public interface IColoredStyle : IStyle
{
    public TextColor? Color { get; }
    public IStyle WithColor(TextColor? color);
}

public interface IColoredStyle<out T> : IColoredStyle where T : IColoredStyle<T>
{
    public new T WithColor(TextColor? color);
    IStyle IColoredStyle.WithColor(TextColor? color) => WithColor(color);
}