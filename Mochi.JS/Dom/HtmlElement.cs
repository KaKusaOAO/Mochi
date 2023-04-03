namespace Mochi.JS.Dom;

public class HtmlElement : IJSObject
{
    private object _handle;
    
    public HtmlElement(object handle)
    {
        _handle = handle;
    }
    
    public object Handle => _handle;
    
    public static HtmlElement Create(string tagName) => 
        HandlePool<HtmlElement>.Shared
            .Get(Interop.GetInvoke(Document.Current.Handle, "createElement", new object[] { tagName }));
    
    public static T Create<T>(string tagName) where T : HtmlElement =>
        HandlePool<T>.Shared.Get(Create(tagName).Handle);
    
    public void AppendChild(HtmlElement child) =>
        Interop.GetInvoke(_handle, "appendChild", new object[] { child.Handle });
    
    public void RemoveChild(HtmlElement child) =>
        Interop.GetInvoke(_handle, "removeChild", new object[] { child.Handle });
    
    public void SetAttribute(string name, string value) =>
        Interop.GetInvoke(_handle, "setAttribute", new object[] { name, value });
    
    public string GetAttribute(string name) =>
        Interop.AsString(Interop.GetInvoke(_handle, "getAttribute", new object[] { name }));
    
    public void RemoveAttribute(string name) =>
        Interop.GetInvoke(_handle, "removeAttribute", new object[] { name });

    public void SetStyle(string name, string value)
    {
        var style = Interop.Get(_handle, "style");
        Interop.Set(style, name, value);
    }
    
    public string GetStyle(string name)
    {
        var style = Interop.Get(_handle, "style");
        return Interop.AsString(Interop.Get(style, name));
    }
}