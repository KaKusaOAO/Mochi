using System.Text.Json.Nodes;

namespace Mochi.Texts;

public interface IContent
{
    public IContentType Type { get; }
    public IContent Clone();
    public void InsertPayload(JsonObject target) => Type.InsertPayload(target, this);
    public void Visit(IContentVisitor visitor, IStyle style);
}

public interface IContent<T> : IContent where T : IContent<T>
{
    public new IContentType<T> Type { get; }
    IContentType IContent.Type => Type;

    public new T Clone();
    IContent IContent.Clone() => Clone();
}