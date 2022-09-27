// See https://aka.ms/new-console-template for more information

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading;
using ImGuiNET;
using KaLib.IO.Controllers.DualSense;
using KaLib.IO.Controllers.Test;
using KaLib.Structs;
using KaLib.Utils;
using ManagedBass;
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
var controllerLock = new SemaphoreSlim(1, 1);

var fixVectorsBound = true;
var applyDeadzone = true;

var stopwatch = new Stopwatch();
stopwatch.Start();

var pollStopwatch = new Stopwatch();
pollStopwatch.Start();

var pollRate = 0.0;
var controllerException = null as Exception;
var controllerSnapshot = Optional.Empty<DualSenseSnapshot>();

var leftChanArr = new float[16380];
var rightChanArr = new float[16380];

var waveformType = WaveformType.Sine;
var rumbleEmulator = new RumbleEmulator
{
    WaveGenerator = x =>
    {
        switch (waveformType)
        {
            case WaveformType.Sine:
                return MathF.Sin(x);
            case WaveformType.Saw:
                return x / MathF.PI - 1;
            case WaveformType.Triangle:
                return Waveform.Triangle(x);
            case WaveformType.AbsSine:
                return MathF.Abs(MathF.Sin(x / 2) * 2) - 1;
            case WaveformType.Square:
                return x >= MathF.PI ? -1 : 1;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
};

Bass.Init();
var rumblePreview = Bass.CreateStream(48000, 2, BassFlags.Default, StreamProcedureType.Push);
Bass.ChannelPlay(rumblePreview);

var previewEnabled = false;
var lastAudioUpdate = DateTime.Now;

var controllerThread = new Thread(() =>
{
    while (window.Exists)
    {
        try
        {
            if (controller == null)
            {
                var device = DualSenseController.FindFirstDualSense();
                if (device != null)
                {
                    controller = new DualSenseController(device);
                    Logger.Info($"Connection type: {controller.ConnectionType}");

                    controller.Disconnected += () =>
                    {
                        controller.Dispose();
                        controllerLock.Wait();
                        controller = null;
                        controllerLock.Release();
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
                    
                    controller.AudioHapticsDataRequested += (left, right) =>
                    {
                        var leftFreq = (controller.LeftTrigger.Value + 1) * 110 / 2;
                        var rightFreq = (controller.RightTrigger.Value + 1) * 55 / 2;
                        
                        rumbleEmulator.Emulate(left, right, 
                            leftFreq, controller.LeftTrigger.Value,
                            rightFreq, controller.RightTrigger.Value * 0.8f);
                            
                        void AppendArray(float[] original, float[] append)
                        {
                            var amount = append.Length;
                            if (amount >= original.Length)
                            {
                                Array.Copy(append, original, original.Length);
                                return;
                            }

                            var tmp = new float[original.Length - amount];
                            Array.Copy(original, original.Length - tmp.Length, tmp, 0, tmp.Length);
                            Array.Copy(tmp, original, tmp.Length);
                            Array.Copy(append, 0, original, tmp.Length, amount);
                        }
                        
                        AppendArray(leftChanArr, left);
                        AppendArray(rightChanArr, right);

                        if (previewEnabled)
                        {
                            var buf = new short[left.Length * 2];
                            for (var i = 0; i < left.Length; i++)
                            {
                                buf[i * 2 + 0] = (short) (short.MaxValue * left[i]);
                                buf[i * 2 + 1] = (short) (short.MaxValue * right[i]);
                            }

                            Bass.StreamPutData(rumblePreview, buf, buf.Length * 2);
                        }
                        lastAudioUpdate = DateTime.Now;
                    };

                    lastAudioUpdate = DateTime.Now;
                    controller.UpdateStates();
                }
                else
                {
                    continue;
                }
            }

            if (controller == null) continue;
            pollStopwatch.Restart();
            controllerSnapshot = Optional.Of(controller.PollInput());
            controller.UpdateStates();
            pollRate = pollStopwatch.Elapsed.TotalMilliseconds;
            controllerException = null;
        }
        catch (Exception ex)
        {
            controllerException = ex;
        }
    }
})
{
    Name = "ControllerIOThread"
};
controllerThread.Start();

while (window.Exists)
{
    var snapshot = window.PumpEvents();
    if (!window.Exists) break;
    SpinWait.SpinUntil(() => stopwatch.Elapsed.TotalSeconds >= 1f / 144);
    renderer.Update((float) stopwatch.Elapsed.TotalSeconds, snapshot);
    stopwatch.Restart();

    SubmitUi();
    // ImGui.ShowDemoWindow();
    
    cl.Begin();
    cl.SetFramebuffer(gd.SwapchainFramebuffer);
    cl.ClearColorTarget(0, RgbaFloat.Black);
    renderer.Render(gd, cl);
    cl.End();
    gd.SubmitCommands(cl);
    gd.SwapBuffers(gd.MainSwapchain);
}

controller?.Dispose();

void SubmitUi()
{
    ImGui.Begin("Controller");
    if (controller == null)
    {
        ImGui.Text("No DualSense connected");
    }
    else
    {
        SubmitControllerUi();
    }
    
    if (controllerException != null) 
    {
        ImGui.TextColored(new RgbaFloat(1, 0.33f, 0, 1).ToVector4(), $"Error! {controllerException}");
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
    if (controller == null) return;
    if (controllerException != null) return;
    if (controllerSnapshot.IsEmpty) return;

    Common.AcquireSemaphore(controllerLock, () =>
    {
        var snapshot = controllerSnapshot.Value;
        ImGui.Text($"Polling input via {controller.ConnectionType} took {pollRate}ms");
        
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

        if (ImGui.CollapsingHeader("Sticks (L3 / R3)", ImGuiTreeNodeFlags.DefaultOpen))
        {
            ImGui.Checkbox("Fix stick vector bounds", ref fixVectorsBound);
            ImGui.Checkbox("Apply deadzone? (5%)", ref applyDeadzone);
            ControllerGui.DrawStick(0, lsVector, snapshot.LeftStick.Pressed);
            ImGui.SameLine();
            ControllerGui.DrawStick(1, rsVector, snapshot.RightStick.Pressed);
        }

        if (ImGui.CollapsingHeader("Touch Pad", ImGuiTreeNodeFlags.DefaultOpen))
        {
            ControllerGui.DrawTouchPad(snapshot.TouchPad, color);
        }

        if (ImGui.CollapsingHeader("Triggers (L2 / R2)", ImGuiTreeNodeFlags.DefaultOpen))
        {
            ControllerGui.DrawPlotFloat(0, snapshot.LeftTrigger, 1);
            ImGui.SameLine();
            ControllerGui.DrawPlotFloat(1, snapshot.RightTrigger, 1);
        }

        if (ImGui.CollapsingHeader("Audio Haptics / Rumble", ImGuiTreeNodeFlags.DefaultOpen))
        {
            ImGui.Checkbox("Monitor Audio Haptics", ref previewEnabled);

            var curr = (int) waveformType;
            var names = Enum.GetNames<WaveformType>();
            ImGui.Combo("Waveform", ref curr, names, names.Length);
            waveformType = (WaveformType) curr;
            
            ControllerGui.DrawPlotFloatNormalized(leftChanArr);
            ImGui.SameLine();
            ControllerGui.DrawPlotFloatNormalized(rightChanArr);
            
            var leftFreq = (controller.LeftTrigger.Value + 1) * 110 / 2;
            var rightFreq = (controller.RightTrigger.Value + 1) * 55 / 2;
            ImGui.Text($"Frequencies: {leftFreq:F2} Hz / {rightFreq:F2} Hz");

            if (DateTime.Now - lastAudioUpdate > TimeSpan.FromSeconds(1))
            {
                ImGui.TextColored(new RgbaFloat(1, 0.25f, 0, 1).ToVector4(), "The audio engine is not responding!");
            }
        }
    });
}