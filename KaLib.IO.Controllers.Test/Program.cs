// See https://aka.ms/new-console-template for more information

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading;
using ImGuiNET;
using KaLib.IO.Controllers.DualSense;
using KaLib.IO.Controllers.Switch;
using KaLib.IO.Controllers.XInput;
using KaLib.IO.Hid;
using KaLib.Structs;
using KaLib.Utils;
using ManagedBass;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace KaLib.IO.Controllers.Test;

public static class Program
{
    public static void Main(string[] args)
    {
        Logger.Level = LogLevel.Verbose;
        Logger.Logged += Logger.LogToEmulatedTerminalAsync;
        RunGeneric();
    }
    
    private static void RunGeneric()
    {
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

        var fixVectorsBound = true;
        var applyDeadzone = true;

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var pollStopwatch = new Stopwatch();
        pollStopwatch.Start();

        var deviceSet = new HashSet<IController>();
        var controllerLock = new SemaphoreSlim(1, 1);

        var controllerThread = new Thread(() =>
        {
            while (window.Exists)
            {
                try
                {
                    Common.AcquireSemaphore(controllerLock, () =>
                    {
                        foreach (var device in Controller.FindAllControllers())
                        {
                            if (deviceSet.Contains(device)) continue;
                            deviceSet.Add(device);
                            
                            device.Disconnected += () =>
                            {
                                device.Dispose();
                                deviceSet.Remove(device);
                            };

                            device.Initialize();
                            device.StartEventLoopThread();
                        }
                    });
                }
                catch (Exception ex)
                {
                    
                }
                
                Thread.Sleep(2);
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

        foreach (var controller in deviceSet)
        {
            controller.Dispose();
        }

        void SubmitUi()
        {
            var i = 0;
            var devices = new List<IController>();
            devices.AddRange(deviceSet);

            foreach (var device in devices)
            {
                ImGui.Begin($"Controller #{i++}");
                try
                {
                    SubmitControllerUi(device);
                }
                catch (Exception ex)
                {
                    ImGui.TextColored(new RgbaFloat(1, 0.25f, 0, 1).ToVector4(), $"{ex}");
                }
                ImGui.End();
            }
        }

        void SubmitControllerUi(IController device)
        {
            var h = device.GetHashCode();
            var os = device.LastSnapshot;
            if (os.IsEmpty) return;
            var s = os.Value;
            
            ImGui.Text($"Snapshot type: {s.GetType().Name}");
            ImGui.Separator();
            if (s is not IBasicControllerSnapshot snapshot) return;

            var lsVector = fixVectorsBound ? snapshot.LeftStick.GetFixedVector() : snapshot.LeftStick.Vector;
            var rsVector = fixVectorsBound ? snapshot.RightStick.GetFixedVector() : snapshot.RightStick.Vector;

            if (applyDeadzone)
            {
                lsVector = ApplyDeadzone(lsVector, 0.05f);
                rsVector = ApplyDeadzone(rsVector, 0.05f);
            }

            if (ImGui.CollapsingHeader("Sticks (L3 / R3)", ImGuiTreeNodeFlags.DefaultOpen))
            {
                ImGui.Checkbox("Fix stick vector bounds", ref fixVectorsBound);
                ImGui.Checkbox("Apply deadzone? (5%)", ref applyDeadzone);
                ControllerGui.DrawStick(h + 0, lsVector, snapshot.LeftStick.Pressed);
                ImGui.SameLine();
                ControllerGui.DrawStick(h + 1, rsVector, snapshot.RightStick.Pressed);
            }

            if (snapshot is DualSenseSnapshot ds)
            {
                if (ImGui.CollapsingHeader("Touch Pad", ImGuiTreeNodeFlags.DefaultOpen))
                {
                    ControllerGui.DrawTouchPad(ds.TouchPad);
                }
            }

            if (ImGui.CollapsingHeader("Triggers (L2 / R2)", ImGuiTreeNodeFlags.DefaultOpen))
            {
                ControllerGui.DrawPlotFloat(h + 0, snapshot.LeftTrigger, 1);
                ImGui.SameLine();
                ControllerGui.DrawPlotFloat(h + 1, snapshot.RightTrigger, 1);
            }
            
            if (device is ITwoSideRumbleController rc)
            {
                rc.LeftRumble.Amplitude = snapshot.LeftTrigger;
                rc.RightRumble.Amplitude = snapshot.RightTrigger;

                if (rc.LeftRumble is IControllerExtendedRumble rl)
                {
                    rl.Frequency = (snapshot.LeftTrigger + 1) * 55 / 2;
                }
                
                if (rc.RightRumble is IControllerExtendedRumble rr)
                {
                    rr.Frequency = (snapshot.RightTrigger + 1) * 110 / 2;
                }

                var dsc = device as DualSenseController;
                if (ImGui.CollapsingHeader(dsc != null ? "Audio Haptics / Rumbles" : "Rumbles", ImGuiTreeNodeFlags.DefaultOpen))
                {
                    if (dsc == null)
                    {
                        ControllerGui.DrawPlotFloat(h + 10, rc.LeftRumble.Amplitude, 1);
                    } else
                    {
                        ControllerGui.DrawPlotFloatNormalized(dsc.GetLastLeftChannelSamples());
                    }

                    ImGui.SameLine();
                    ImGui.BeginGroup();
                    ImGui.Text($"Amplitude: {rc.LeftRumble.Amplitude * 100:F2}%%");
                    if (rc.LeftRumble is IControllerExtendedRumble rl2)
                    {
                        ImGui.Text($"Frequency: {rl2.Frequency:F2} Hz");
                    }
                    ImGui.EndGroup();

                    if (dsc == null)
                    {
                        ControllerGui.DrawPlotFloat(h + 11, rc.RightRumble.Amplitude, 1);
                    }
                    else
                    {
                        ControllerGui.DrawPlotFloatNormalized(dsc.GetLastRightChannelSamples());
                    }

                    ImGui.SameLine();
                    ImGui.BeginGroup();
                    ImGui.Text($"Amplitude: {rc.RightRumble.Amplitude * 100:F2}%%");
                    if (rc.RightRumble is IControllerExtendedRumble rr2)
                    {
                        ImGui.Text($"Frequency: {rr2.Frequency:F2} Hz");
                    }
                    ImGui.EndGroup();
                }
            }
        }
    }
    
    private static void RunXInput()
    {
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
        var controller = null as XInputController;
        var controllerLock = new SemaphoreSlim(1, 1);

        var fixVectorsBound = true;
        var applyDeadzone = true;

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var pollStopwatch = new Stopwatch();
        pollStopwatch.Start();

        var pollRate = 0.0;
        var controllerException = null as Exception;
        var controllerSnapshot = Optional.Empty<XInputSnapshot>();

        var controllerThread = new Thread(() =>
        {
            while (window.Exists)
            {
                try
                {
                    if (controller == null)
                    {
                        controller = XInputController.FindAll().FirstOrDefault();
                        
                        if (controller != null)
                        {
                            controller.Initialize();
                            
                            controller.Disconnected += () =>
                            {
                                controllerSnapshot = Optional.Empty<XInputSnapshot>();
                                controller.Dispose();
                                controllerLock.Wait();
                                controller = null;
                                controllerLock.Release();
                            };
                        }
                    }

                    if (controller == null)
                    {
                        Thread.Sleep(100);
                        continue;
                    }
                    
                    pollStopwatch.Restart();
                    controllerSnapshot = Optional.Of(controller.PollInput());
                    controller.UpdateStates();
                    pollRate = pollStopwatch.Elapsed.TotalMilliseconds;
                    controllerException = null;
                    Thread.Sleep(2);
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
                ImGui.Text("No XInput controller connected");
            }
    
            ImGui.BeginDisabled(controller == null);
            SubmitControllerUi();
            ImGui.EndDisabled();
    
            if (controllerException != null) 
            {
                ImGui.TextColored(new RgbaFloat(1, 0.33f, 0, 1).ToVector4(), $"Error! {controllerException}");
            }
            ImGui.End();
        }

        void SubmitControllerUi()
        {
            if (controllerSnapshot.IsEmpty)
            {
                controllerSnapshot = Optional.Of(new XInputSnapshot());
            }

            Common.AcquireSemaphore(controllerLock, () =>
            {
                var snapshot = controllerSnapshot.Value;
                if (controller != null)
                {
                    ImGui.Text($"Polling input took {pollRate}ms");
                }

                ImGui.Separator();

                var lsVector = fixVectorsBound ? snapshot.LeftStick.GetFixedVector() : snapshot.LeftStick.Vector;
                var rsVector = fixVectorsBound ? snapshot.RightStick.GetFixedVector() : snapshot.RightStick.Vector;

                if (applyDeadzone)
                {
                    lsVector = ApplyDeadzone(lsVector, 0.05f);
                    rsVector = ApplyDeadzone(rsVector, 0.05f);
                }

                if (ImGui.CollapsingHeader("Sticks (L3 / R3)", ImGuiTreeNodeFlags.DefaultOpen))
                {
                    ImGui.Checkbox("Fix stick vector bounds", ref fixVectorsBound);
                    ImGui.Checkbox("Apply deadzone? (5%)", ref applyDeadzone);
                    ControllerGui.DrawStick(0, lsVector, snapshot.LeftStick.Pressed);
                    ImGui.SameLine();
                    ControllerGui.DrawStick(1, rsVector, snapshot.RightStick.Pressed);
                }

                if (ImGui.CollapsingHeader("Triggers (L2 / R2)", ImGuiTreeNodeFlags.DefaultOpen))
                {
                    ControllerGui.DrawPlotFloat(0, snapshot.LeftTrigger, 1);
                    ImGui.SameLine();
                    ControllerGui.DrawPlotFloat(1, snapshot.RightTrigger, 1);
                }

                if (ImGui.CollapsingHeader("Rumble", ImGuiTreeNodeFlags.DefaultOpen))
                {
                    
                }
            });
        }
    }
    
    private static void RunDualSense()
    {
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
                    case WaveformType.Noise:
                        return new Random().NextSingle() * 2 - 1;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        };

        Bass.Init();
        var rumblePreview = Bass.CreateStream(48000, 2, BassFlags.Default, StreamProcedureType.Push);
        Bass.ChannelPlay(rumblePreview);

        var previewEnabled = false;

        var controllerThread = new Thread(() =>
        {
            while (window.Exists)
            {
                try
                {
                    if (controller == null)
                    {
                        var device = DualSenseController.FindAllDualSense().FirstOrDefault();
                        if (device != null)
                        {
                            controller = new DualSenseController(device);
                            controller.Initialize();
                            Logger.Info($"Connection type: {controller.ConnectionType}");

                            controller.Disconnected += () =>
                            {
                                controllerSnapshot = Optional.Empty<DualSenseSnapshot>();
                                controller.Dispose();
                                controllerLock.Wait();
                                controller = null;
                                controllerLock.Release();
                            };

                            controller.SnapshotUpdated += (_, snapshot) =>
                            {
                                var lsVector = fixVectorsBound ? snapshot.LeftStick.GetFixedVector() : snapshot.LeftStick.Vector;
                                var rsVector = fixVectorsBound ? snapshot.RightStick.GetFixedVector() : snapshot.RightStick.Vector;

                                if (applyDeadzone)
                                {
                                    lsVector = ApplyDeadzone(lsVector, 0.05f);
                                }

                                var vector = lsVector;
                                var hue = Math.Atan2(vector.Y, vector.X) + Math.PI / 2;
                                var color = Color.FromHsv(hue, vector.Length(), 1 - snapshot.RightTrigger);
                                if (controller != null)
                                {
                                    controller.TouchPad.LightBar = color;
                                }
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
                                    var modes = Enum.GetValues<PlayerLedMode>().ToList();
                                    var idx = modes.IndexOf(controller.TouchPad.PlayerLed) + 1;
                                    controller.TouchPad.PlayerLed = modes[idx % (modes.Count - 1)];
                                }
                            };

                            controller.EnableAudioHaptics = true;
                    
                            controller.AudioHapticsDataRequested += (left, right) =>
                            {
                                var leftFreq = (controller.LeftTrigger.Value + 1) * 55 / 2;
                                var rightFreq = (controller.RightTrigger.Value + 1) * 110 / 2;
                        
                                rumbleEmulator.Emulate(left, right, 
                                    leftFreq, controller.LeftTrigger.Value,
                                    rightFreq, controller.RightTrigger.Value * 0.8f);

                                if (previewEnabled)
                                {
                                    var buf = new short[left.Length * 2];
                                    for (var i = 0; i < left.Length; i++)
                                    {
                                        buf[i * 2 + 0] = (short) (short.MaxValue * Math.Clamp(left[i], -1, 1));
                                        buf[i * 2 + 1] = (short) (short.MaxValue * Math.Clamp(right[i], -1, 1));
                                    }

                                    Bass.StreamPutData(rumblePreview, buf, buf.Length * 2);
                                }
                            };

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
    
            ImGui.BeginDisabled(controller == null);
            SubmitControllerUi();
            ImGui.EndDisabled();
    
            if (controllerException != null) 
            {
                ImGui.TextColored(new RgbaFloat(1, 0.33f, 0, 1).ToVector4(), $"Error! {controllerException}");
            }
            ImGui.End();
        }

        void SubmitControllerUi()
        {
            if (controllerSnapshot.IsEmpty)
            {
                controllerSnapshot = Optional.Of(new DualSenseSnapshot());
            }

            Common.AcquireSemaphore(controllerLock, () =>
            {
                var snapshot = controllerSnapshot.Value;
                if (controller != null)
                {
                    ImGui.Text($"Polling input via {controller.ConnectionType} took {pollRate}ms");
                }

                ImGui.Separator();

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
                    ImGui.BeginDisabled(!controller?.IsAudioHapticsAvailable ?? true);
                    ImGui.Checkbox("Monitor Audio Haptics", ref previewEnabled);

                    var curr = (int) waveformType;
                    var names = Enum.GetNames<WaveformType>();
                    ImGui.Combo("Waveform", ref curr, names, names.Length);
                    waveformType = (WaveformType) curr;
            
                    ControllerGui.DrawPlotFloatNormalized(controller?.GetLastLeftChannelSamples() ?? new float[1]);
                    ImGui.SameLine();
                    ControllerGui.DrawPlotFloatNormalized(controller?.GetLastRightChannelSamples() ?? new float[1]);
            
                    var leftFreq = (snapshot.LeftTrigger + 1) * 55 / 2;
                    var rightFreq = (snapshot.RightTrigger + 1) * 110 / 2;
                    ImGui.Text($"Frequencies: {leftFreq:F2} Hz / {rightFreq:F2} Hz");
                    ImGui.EndDisabled();
                }
            });
        }
    }

    private static Vector2 ApplyDeadzone(Vector2 raw, float threshold)
    {
        var len = raw.Length();
        if (len <= threshold) return Vector2.Zero;

        var newLen = len - threshold;
        newLen *= 1 / (1 - threshold);
        return raw / len * newLen;
    }
}