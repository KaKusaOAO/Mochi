using System;
using Mochi.JS.Dom;

namespace Mochi.JS.Graphics.Canvas;

// GitHub Copilot helps a lot with this :|
public class CanvasRenderingContext2D : IRenderingContext
{
    private object _handle;
    
    public CanvasRenderingContext2D(object handle)
    {
        _handle = handle;
    }
    
    public object Handle => _handle;
    
    public void FillRect(double x, double y, double width, double height) => 
        Interop.GetInvoke(Handle, "fillRect", new object[] { x, y, width, height });

    public void ClearRect(double x, double y, double width, double height) => 
        Interop.GetInvoke(Handle, "clearRect", new object[] { x, y, width, height });

    public void StrokeRect(double x, double y, double width, double height) => 
        Interop.GetInvoke(Handle, "strokeRect", new object[] { x, y, width, height });

    public void FillText(string text, double x, double y) => 
        Interop.GetInvoke(Handle, "fillText", new object[] { text, x, y });

    public void StrokeText(string text, double x, double y) => 
        Interop.GetInvoke(Handle, "strokeText", new object[] { text, x, y });
    
    public string FillStyle
    {
        get => Interop.AsString(Interop.Get(Handle, "fillStyle"));
        set => Interop.Set(Handle, "fillStyle", value);
    }
    
    public string StrokeStyle
    {
        get => Interop.AsString(Interop.Get(Handle, "strokeStyle"));
        set => Interop.Set(Handle, "strokeStyle", value);
    }
    
    public string Font
    {
        get => Interop.AsString(Interop.Get(Handle, "font"));
        set => Interop.Set(Handle, "font", value);
    }
    
    public string TextAlign
    {
        get => Interop.AsString(Interop.Get(Handle, "textAlign"));
        set => Interop.Set(Handle, "textAlign", value);
    }
    
    public string TextBaseline
    {
        get => Interop.AsString(Interop.Get(Handle, "textBaseline"));
        set => Interop.Set(Handle, "textBaseline", value);
    }
    
    public string Filter
    {
        get => Interop.AsString(Interop.Get(Handle, "filter"));
        set => Interop.Set(Handle, "filter", value);
    }
    
    public double GlobalAlpha
    {
        get => Interop.AsDouble(Interop.Get(Handle, "globalAlpha"));
        set => Interop.Set(Handle, "globalAlpha", value);
    }
    
    public string GlobalCompositeOperation
    {
        get => Interop.AsString(Interop.Get(Handle, "globalCompositeOperation"));
        set => Interop.Set(Handle, "globalCompositeOperation", value);
    }
    
    public double LineWidth
    {
        get => Interop.AsDouble(Interop.Get(Handle, "lineWidth"));
        set => Interop.Set(Handle, "lineWidth", value);
    }
    
    public LineCapType LineCap
    {
        get => Enum.Parse<LineCapType>(Interop.AsString(Interop.Get(Handle, "lineCap")), true);
        set => Interop.Set(Handle, "lineCap", Enum.GetName(value)!.ToLowerInvariant());
    }
    
    public LineJoinType LineJoin
    {
        get => Enum.Parse<LineJoinType>(Interop.AsString(Interop.Get(Handle, "lineJoin")), true);
        set => Interop.Set(Handle, "lineJoin", Enum.GetName(value)!.ToLowerInvariant());
    }
    
    public double MiterLimit
    {
        get => Interop.AsDouble(Interop.Get(Handle, "miterLimit"));
        set => Interop.Set(Handle, "miterLimit", value);
    }
    
    public double ShadowBlur
    {
        get => Interop.AsDouble(Interop.Get(Handle, "shadowBlur"));
        set => Interop.Set(Handle, "shadowBlur", value);
    }
    
    public string ShadowColor
    {
        get => Interop.AsString(Interop.Get(Handle, "shadowColor"));
        set => Interop.Set(Handle, "shadowColor", value);
    }
    
    public double ShadowOffsetX
    {
        get => Interop.AsDouble(Interop.Get(Handle, "shadowOffsetX"));
        set => Interop.Set(Handle, "shadowOffsetX", value);
    }
    
    public double ShadowOffsetY
    {
        get => Interop.AsDouble(Interop.Get(Handle, "shadowOffsetY"));
        set => Interop.Set(Handle, "shadowOffsetY", value);
    }
    
    public void BeginPath() => Interop.GetInvoke(Handle, "beginPath", Array.Empty<object>());
    
    public void ClosePath() => Interop.GetInvoke(Handle, "closePath", Array.Empty<object>());
    
    public void MoveTo(double x, double y) => Interop.GetInvoke(Handle, "moveTo", new object[] { x, y });
    
    public void LineTo(double x, double y) => Interop.GetInvoke(Handle, "lineTo", new object[] { x, y });
    
    public void QuadraticCurveTo(double cpx, double cpy, double x, double y) => 
        Interop.GetInvoke(Handle, "quadraticCurveTo", new object[] { cpx, cpy, x, y });
    
    public void BezierCurveTo(double cp1X, double cp1Y, double cp2X, double cp2Y, double x, double y) =>
        Interop.GetInvoke(Handle, "bezierCurveTo", new object[] { cp1X, cp1Y, cp2X, cp2Y, x, y });
    
    public void ArcTo(double x1, double y1, double x2, double y2, double radius) =>
        Interop.GetInvoke(Handle, "arcTo", new object[] { x1, y1, x2, y2, radius });
    
    public void Rect(double x, double y, double width, double height) =>
        Interop.GetInvoke(Handle, "rect", new object[] { x, y, width, height });
    
    public void Arc(double x, double y, double radius, double startAngle, double endAngle, bool anticlockwise) =>
        Interop.GetInvoke(Handle, "arc", new object[] { x, y, radius, startAngle, endAngle, anticlockwise });
    
    public void Fill() => Interop.GetInvoke(Handle, "fill", Array.Empty<object>());
    
    public void Stroke() => Interop.GetInvoke(Handle, "stroke", Array.Empty<object>());
    
    public void Clip() => Interop.GetInvoke(Handle, "clip", Array.Empty<object>());
    
    public bool IsPointInPath(double x, double y) => 
        Interop.AsBool(Interop.GetInvoke(Handle, "isPointInPath", new object[] { x, y }));
    
    public void DrawImage(ICanvasDrawable image, double x, double y) =>
        Interop.GetInvoke(Handle, "drawImage", new object[] { image.Handle, x, y });
    
    public void DrawImage(ICanvasDrawable image, double x, double y, double width, double height) =>
        Interop.GetInvoke(Handle, "drawImage", new object[] { image.Handle, x, y, width, height });
    
    public void DrawImage(ICanvasDrawable image, double sx, double sy, double sw, double sh, double dx, double dy, double dw, double dh) =>
        Interop.GetInvoke(Handle, "drawImage", new object[] { image.Handle, sx, sy, sw, sh, dx, dy, dw, dh });

    public HtmlCanvasElement Canvas => HandlePool<HtmlCanvasElement>.Shared.Get(Interop.Get(_handle, "canvas"));
}