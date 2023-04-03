using System;
using Mochi.JS.Graphics;
using Mochi.JS.Graphics.Canvas;

namespace Mochi.JS.Dom;

public class HtmlCanvasElement : HtmlElement, ICanvasDrawable
{
    public HtmlCanvasElement(object handle) : base(handle)
    {
    }

    private object InternalGetContext(string type) => 
        Interop.GetInvoke(Handle, "getContext", new object[] { type });

    public T GetContext<T>(string type) where T : IRenderingContext =>
        HandlePool<T>.Shared.Get(InternalGetContext(type));
    
    public CanvasRenderingContext2D GetContext2D() => GetContext<CanvasRenderingContext2D>("2d");
    
    public int Width
    {
        get => Interop.AsInt32(Interop.Get(Handle, "width"));
        set => Interop.Set(Handle, "width", value);
    }
    
    public int Height
    {
        get => Interop.AsInt32(Interop.Get(Handle, "height"));
        set => Interop.Set(Handle, "height", value);
    }

    public string ToDataURL() => 
        Interop.AsString(Interop.GetInvoke(Handle, "toDataURL", Array.Empty<object>()));
}