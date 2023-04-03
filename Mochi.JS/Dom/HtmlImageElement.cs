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
}