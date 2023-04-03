using System;
using Mochi.JS.Graphics.Canvas;

namespace Mochi.JS.Dom;

public class Image : ICanvasDrawable
{
    private readonly object _handle;

    public Image(object handle)
    {
        _handle = handle;
    }

    public Image()
    {
        _handle = Interop.GetNew(Window.Current.Handle, "Image", Array.Empty<object>());
    }
    
    public string CrossOrigin
    {
        get => Interop.AsString(Interop.Get(_handle, "crossOrigin"));
        set => Interop.Set(_handle, "crossOrigin", value);
    }
    
    public string Source
    {
        get => Interop.AsString(Interop.Get(_handle, "src"));
        set => Interop.Set(_handle, "src", value);
    }
    
    public int Width => Interop.AsInt32(Interop.Get(_handle, "width"));
    
    public int Height => Interop.AsInt32(Interop.Get(_handle, "height"));

    public object Handle => _handle;
}