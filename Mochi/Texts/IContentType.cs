using System.Text.Json.Nodes;

namespace Mochi.Texts;

public interface IContentType
{
    public IContent CreateContent(JsonObject payload);
    public void InsertPayload(JsonObject target, IContent content);
}

public interface IContentType<T> : IContentType where T : IContent<T>
{
    public new T CreateContent(JsonObject payload);
    IContent IContentType.CreateContent(JsonObject payload) => CreateContent(payload);
    
    public void InsertPayload(JsonObject target, T content);
    void IContentType.InsertPayload(JsonObject target, IContent content) => InsertPayload(target, (T)content);
}