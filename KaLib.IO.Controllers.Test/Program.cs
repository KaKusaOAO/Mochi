// See https://aka.ms/new-console-template for more information

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using ImGuiNET;
using KaLib.IO.Controllers.DualSense;
using KaLib.IO.Controllers.Test;
using KaLib.Structs;
using KaLib.Utils;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

Logger.Level = LogLevel.Verbose;
Logger.Logged += Logger.LogToEmulatedTerminalAsync;

var window = new Sdl2Window("ControllerTest", 100, 100, 1280, 720, 
    SDL_WindowFlags.Shown | SDL_WindowFlags.Resizable | SDL_WindowFlags.OpenGL, false);
var gd = VeldridStartup.CreateGraphicsDevice(window, GraphicsBackend.OpenGL);

var renderer = new ImGuiRenderer(gd, gd.SwapchainFramebuffer.OutputDescription, window.Width, window.Height);
window.Resized += () =>
{
    gd.MainSwapchain.Resize((uint) window.Width, (uint) window.Height);
    renderer.WindowResized(window.Width, window.Height);
};

var cl = gd.ResourceFactory.CreateCommandList();
var controller = null as DualSenseController;

var touch1History = new List<TouchState>();
var touch2History = new List<TouchState>();

var fixVectorsBound = true;
var applyDeadzone = true;

var stopwatch = new Stopwatch();
stopwatch.Start();

while (window.Exists)
{
    var snapshot = window.PumpEvents();
    if (!window.Exists) break;
    renderer.Update((float) stopwatch.Elapsed.TotalSeconds, snapshot);
    stopwatch.Restart();

    SubmitUi();
    ImGui.ShowDemoWindow();
    
    cl.Begin();
    cl.SetFramebuffer(gd.SwapchainFramebuffer);
    cl.ClearColorTarget(0, RgbaFloat.Black);
    renderer.Render(gd, cl);
    cl.End();
    gd.SubmitCommands(cl);
    gd.SwapBuffers(gd.MainSwapchain);
}

void SubmitUi()
{
    ImGui.Begin("Controller");
    if (controller == null)
    {
        ImGui.Text("No DualSense connected");
        
        var device = DualSenseController.FindFirstDualSense();
        if (device != null)
        {
            controller = new DualSenseController(device);
            Logger.Info($"Connection type: {controller.ConnectionType}");

            controller.Disconnected += () =>
            {
                controller.Dispose();
                controller = null;
            };

            controller.ButtonPressed += b =>
            {
                if (b == controller.PlayStationLogo)
                {
                    Logger.Info("Back to PlayStation main screen!");
                }

                if (b == controller.Mic)
                {
                    controller.Mic.IsLedEnabled = !controller.Mic.IsLedEnabled;
                }

                if (b == controller.TouchPad)
                {
                    var rand = new Random();
                    controller.TouchPad.LightBar = new(rand.Next(255), rand.Next(255), rand.Next(255));
                }

                if (b == controller.ButtonCircle)
                {
                    controller.TouchPad.LightBar = new(0);
                    controller.TouchPad.PlayerLed = PlayerLedMode.None;
                }
            };
            
            controller.UpdateStates();
        }
    }
    else
    {
        SubmitControllerUi();
    }
    ImGui.End();
}

Vector2 ApplyDeadzone(Vector2 raw, float threshold)
{
    var len = raw.Length();
    if (len <= threshold) return Vector2.Zero;

    var newLen = len - threshold;
    newLen *= 1 / (1 - threshold);
    return raw / len * newLen;
}

void SubmitControllerUi()
{
    DualSenseSnapshot snapshot;
    try
    {
        snapshot = controller.PollInput();
    }
    catch (Exception ex)
    {
        ImGui.TextColored(new RgbaFloat(1, 0.33f, 0, 1).ToVector4(), $"Error! {ex}");
        return;
    }
    
    if (controller == null) return;

    ImGui.Checkbox("Fix stick vector bounds", ref fixVectorsBound);
    ImGui.Checkbox("Apply deadzone?", ref applyDeadzone);
    var lsVector = fixVectorsBound ? snapshot.LeftStick.GetFixedVector() : snapshot.LeftStick.Vector;
    var rsVector = fixVectorsBound ? snapshot.RightStick.GetFixedVector() : snapshot.RightStick.Vector;

    if (applyDeadzone)
    {
        lsVector = ApplyDeadzone(lsVector, 0.05f);
        rsVector = ApplyDeadzone(rsVector, 0.05f);
    }

    var vector = lsVector;
    var hue = Math.Atan2(vector.Y, vector.X) + Math.PI / 2;
    var color = Color.FromHsv(hue, vector.Length(), 1 - snapshot.RightTrigger);
    controller.TouchPad.LightBar = color;
    controller.UpdateStates();

    var colorVec = new Vector3(color.R / 255f, color.G / 255f, color.B / 255f);
    ImGui.ColorEdit3("LightBar", ref colorVec);
    
    ControllerGui.DrawStick(0, lsVector, snapshot.LeftStick.Pressed);
    ImGui.SameLine();
    ControllerGui.DrawStick(1, rsVector, snapshot.RightStick.Pressed);
    
    ControllerGui.DrawTouchPad(snapshot.TouchPad);
    
    ControllerGui.DrawPlotFloat(0, snapshot.LeftTrigger, 1);
    ControllerGui.DrawPlotFloat(1, snapshot.RightTrigger, 1);
}