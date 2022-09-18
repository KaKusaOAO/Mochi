// See https://aka.ms/new-console-template for more information

using System;
using System.Diagnostics;
using System.Threading;
using KaLib.IO.Controllers.DualSense;
using KaLib.Structs;
using KaLib.Utils;

Logger.Level = LogLevel.Verbose;
Logger.Logged += Logger.LogToEmulatedTerminalAsync;

var device = DualSenseController.FindFirstDualSense();
if (device == null)
{
    Logger.Error("No DualSense connected.");
    return;
}

var connected = true;
var controller = new DualSenseController(device);

controller.Disconnected += () =>
{
    connected = false;
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
        controller.TouchPad.LedColor = new(rand.Next(255), rand.Next(255), rand.Next(255));
    }

    if (b == controller.ButtonCircle)
    {
        connected = false;
        controller.TouchPad.LedColor = new(0);
        controller.TouchPad.PlayerLed = PlayerLedMode.None;
    }
};

var stopwatch = new Stopwatch();
stopwatch.Start();

while (connected)
{
    var hue = stopwatch.ElapsedMilliseconds / 1000f;
    var color = Color.FromHsv(hue, 1, 1);
    controller.TouchPad.LedColor = color;
    controller.Update();
    SpinWait.SpinUntil(() => false, 16);
}

Logger.Info("Disconnected");
