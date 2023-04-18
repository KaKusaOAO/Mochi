using System;
using System.Diagnostics;
using Mochi.JS;
using Mochi.JS.Dom;
using Mochi.JS.Graphics.Canvas;

var canvas = Document.Current.GetElementById<HtmlCanvasElement>("canvas");
var ctx = canvas.GetContext<CanvasRenderingContext2D>("2d");

// Make it DPI aware
var ratio = Window.Current.DevicePixelRatio;
var width = canvas.Width;
var height = canvas.Height;

canvas.Width = (int)(width * ratio);
canvas.Height = (int)(height * ratio);
canvas.SetStyle("width", $"{width}px");
canvas.SetStyle("height", $"{height}px");

var stopwatch = new Stopwatch();
stopwatch.Start();

var img = new Image
{
    Source = "https://i.imgur.com/67qrzRo.jpeg",
    CrossOrigin = ""
};

void Update()
{
    Window.Current.RequestAnimationFrame(Update);

    ctx.FillStyle = "black";
    ctx.ClearRect(0, 0, canvas.Width, canvas.Height);
    ctx.FillRect(0, 0, canvas.Width, canvas.Height);
    
    ctx.FillStyle = "white";
    ctx.Font = "48px serif";
    ctx.FillText("Hello from C#!", Math.Sin(stopwatch.Elapsed.TotalMilliseconds / 1000) * 30, 50);
    
    ctx.DrawImage(img, 0, 100);
}

Update();