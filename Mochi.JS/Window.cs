using System;

namespace Mochi.JS;

public class Window
{
    private object _handle;
    private static Window _current;

    private static Window CreateWindow()
    {
        return new Window(Interop.GetWindow());
    }

    private Window(object handle)
    {
        _handle = handle;
    }
    
    public static Window Current => _current ??= CreateWindow();

    public object Handle => _handle;

    public int SetTimeout(Action action, int millis)
    {
        return Interop.AsInt32(Interop.GetInvoke(_handle, "setTimeout", new[]
        {
            Interop.ConvertToAny(action), 
            millis
        }));
    }

    public void RequestAnimationFrame(Action action)
    {
        Interop.GetInvoke(_handle, "requestAnimationFrame", new[]
        {
            Interop.ConvertToAny(action)
        });
    }
    
    public void ClearTimeout(int id)
    {
        Interop.GetInvoke(_handle, "clearTimeout", new object[] { id });
    }
    
    public void SetInterval(Action action, int millis)
    {
        Interop.GetInvoke(_handle, "setInterval", new[]
        {
            Interop.ConvertToAny(action), 
            millis
        });
    }
    
    public void ClearInterval(int id)
    {
        Interop.GetInvoke(_handle, "clearInterval", new object[] { id });
    }
    
    public double DevicePixelRatio => Interop.AsDouble(Interop.Get(_handle, "devicePixelRatio"));
    
    public int InnerWidth => Interop.AsInt32(Interop.Get(_handle, "innerWidth"));
    
    public int InnerHeight => Interop.AsInt32(Interop.Get(_handle, "innerHeight"));
    
    public int OuterWidth => Interop.AsInt32(Interop.Get(_handle, "outerWidth"));
    
    public int OuterHeight => Interop.AsInt32(Interop.Get(_handle, "outerHeight"));
    
    public void Alert(string message)
    {
        Interop.GetInvoke(_handle, "alert", new object[] { message });
    }
}