using Mochi.JS.Graphics.Canvas;

namespace Mochi.JS.Dom;

public class HtmlImageElement : HtmlElement, ICanvasDrawable
{
    public HtmlImageElement(object handle) : base(handle)
    {
    }
    
    public static HtmlImageElement Create() => 
        HandlePool<HtmlImageElement>.Shared
            .Get(Interop.GetInvoke(Document.Current.Handle, "createElement", new object[] { "img" }));
    
    public string CrossOrigin
    {
        get => Interop.AsString(Interop.Get(Handle, "crossOrigin"));
        set => Interop.Set(Handle, "crossOrigin", value);
    }
    
    public string Source
    {
        get => Interop.AsString(Interop.Get(Handle, "src"));
        set => Interop.Set(Handle, "src", value);
    }
    
    public int Width => Interop.AsInt32(Interop.Get(Handle, "width"));
    
    public int Height => Interop.AsInt32(Interop.Get(Handle, "height"));
}