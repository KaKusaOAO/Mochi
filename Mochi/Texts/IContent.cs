using System.Text.Json.Nodes;

namespace Mochi.Texts;

public interface IContent
{
    public IText? Parent { get; }
    public IContentType Type { get; }
    public IContent Clone();
    public string ToAnsi();
    public string ToPlainText();
    public void BindParent(IText? parent) {}
    public void InsertPayload(JsonObject target) => Type.InsertPayload(target, this);
}

public interface IContent<T> : IContent where T : IContent<T>
{
    public new IContentType<T> Type { get; }
    IContentType IContent.Type => Type;

    public new T Clone();
    IContent IContent.Clone() => Clone();
}