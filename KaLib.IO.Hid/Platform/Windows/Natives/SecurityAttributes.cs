using System;
using System.Runtime.InteropServices;

namespace KaLib.IO.Hid.Platform.Windows.Natives;

[StructLayout(LayoutKind.Sequential)]
public struct SecurityAttributes
{
    public uint Length;
    public IntPtr SecurityDescriptor;
    public bool InheritHandle;
}