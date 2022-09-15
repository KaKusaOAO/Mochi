using System;
using System.Threading.Tasks;
using KaLib.IO.Hid;
using KaLib.Utils;

namespace KaLib.Procon.Test
{
    class Program
    {
        static async Task Main(string[] args)
        {
            HidProviders.Shared.GetDevicesInfo();
        }
    }
}