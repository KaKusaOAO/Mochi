namespace Mochi.JS.Dom;

public class Document : IJSObject
{
    private object _handle;
    private static Document _current;
    
    private static Document CreateDocument() => new(Interop.Get(Window.Current.Handle, "document"));

    private Document(object handle)
    {
        _handle = handle;
    }
    
    public static Document Current => _current ??= CreateDocument();
    
    public object Handle => _handle;
    
    public HtmlElement GetElementById(string id) =>
        HandlePool<HtmlElement>.Shared.Get(Interop.GetInvoke(_handle, "getElementById", new object[] { id }));
    
    public T GetElementById<T>(string id) where T : HtmlElement =>
#pragma warning disable IL2087
        HandlePool<T>.Shared.Get(GetElementById(id).Handle);
#pragma warning restore IL2087
}