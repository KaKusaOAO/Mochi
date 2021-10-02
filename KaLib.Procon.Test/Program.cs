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
            if (controller.Device == null) return;
            
            Logger.Info("Now we have a Procon!");
            controller.ButtonPressed += b =>
            {
                Logger.Info($"{b} pressed!");
            };
            
            await Task.Delay(100);
            while (true)
            {
                controller.PollInput();
                // controller.UpdateStatus();
                Console.CursorLeft = 0;
                await Task.Yield();
            }
        }
    }
}