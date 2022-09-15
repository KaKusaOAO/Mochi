using System;
using System.Runtime.InteropServices;

namespace KaLib.IO.Hid.Platform.MacOs.Natives;

internal static unsafe class CoreFoundation
{
    private const string Framework = "/System/Library/Frameworks/CoreFoundation.framework/CoreFoundation";

    [DllImport(Framework)]
    public static extern CFRunLoop* CFRunLoopGetCurrent();
}