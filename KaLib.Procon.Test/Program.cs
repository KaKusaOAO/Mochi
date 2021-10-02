using System;
using System.Threading.Tasks;

namespace KaLib.Procon.Test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var controller = new Controller();
            controller.OpenFirstProcon();
            if (controller.Device == null) return;
            
            Console.WriteLine("Now we have a Procon!");
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