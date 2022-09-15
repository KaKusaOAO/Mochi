using System;
using System.Runtime.InteropServices;

namespace KaLib.IO.Hid.Platform.MacOs.Natives;

internal enum HidOptionsType
{
    None
}

internal static unsafe class IOKit
{
    private const string Framework = "/System/Library/Frameworks/IOKit.framework/IOKit";
    
    [DllImport(Framework)]
    public static extern IOHIDManager* IOHIDManagerCreate(IntPtr allocator, HidOptionsType type);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="manager">Reference to an <see cref="IOHIDManager"/>.</param>
    /// <param name="matching"><c>CFDictionaryRef</c> containg device matching criteria.</param>
    /// <remarks>Available on Mac OS X 10.5 and later.</remarks>
    [DllImport(Framework)]
    public static extern void IOHIDManagerSetDeviceMatching(IOHIDManager* manager, IntPtr matching);

    [DllImport(Framework)]
    public static extern void IOHIDManagerScheduleWithRunLoop(IOHIDManager* manager, CFRunLoop* runLoop,
        IntPtr runLoopMode);
}