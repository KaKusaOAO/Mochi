using System;

namespace Mochi.JS;

public class Performance : IJSObject
{
    private object _handle;

    public Performance(object handle)
    {
        _handle = handle;
    }
    
    public static Performance Current => 
        HandlePool<Performance>.Shared.Get(Interop.Get(Window.Current.Handle, "performance"));

    public double Now => Interop.AsDouble(Interop.GetInvoke(_handle, "now", Array.Empty<object>()));
    public object Handle => _handle;
}