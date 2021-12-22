using System;
using System.Threading.Tasks;
using KaLib.Utils;

namespace KaLib.Procon.Test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var controller = new Controller();
            controller.OpenFirstProcon();
            if (controller.Device == null)
            {
                await Logger.FlushAsync();
                await Task.Delay(100);
                return;
            }
            
            Logger.Info("Now we have a Procon!");
            controller.ButtonPressed += b =>
            {
                Logger.Info($"{b} pressed!");

                if (b == Button.B)
                {
                    Logger.Info($"Gyroscope: {controller.States.Gyroscope}");
                    Logger.Info($"Accelerometer: {controller.States.Accelerometer}");
                }
            };
            
            await Task.Delay(100);
            
            while (true)
            {
                var left = (float)Math.Max(controller.States.LeftStick.Y, 0) * 250;
                var right = (float)Math.Max(controller.States.RightStick.Y, 0) * 250;
                controller.SetVibration(left, right, Math.Max(left, right) / 250);
                await Task.Yield();
            }
        }
    }
}