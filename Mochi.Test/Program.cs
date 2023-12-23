// See https://aka.ms/new-console-template for more information

using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using System.Text;
using Mochi.Metal;
using Mochi.MetalFX;
using Mochi.ObjC;
using Mochi.Structs;
using Mochi.Utils;

Logger.Level = LogLevel.Verbose;
Logger.Logged += Logger.LogToEmulatedTerminalAsync;
Logger.RunThreaded();

if (OperatingSystem.IsMacOSVersionAtLeast(13) || OperatingSystem.IsIOSVersionAtLeast(16))
{
    MTLPixelFormats.IsOSSupported(MTLPixelFormat.Stencil8);
    var device = MTLDevice.CreateSystemDefaultDevice();
    var desc = MTLTextureDescriptor.CreateTexture2D(MTLPixelFormat.RGBA8Unorm, 256, 256, false);
    var texture = device.CreateTexture(desc);
    texture.Label = "My Texture #1";
    var view = texture.CreateTextureView(MTLPixelFormat.RGBA8Unorm, MTLTextureType.Type2D, new NSRange(0, 1), new NSRange(0, 0));
    Logger.Info(device.Name);
}