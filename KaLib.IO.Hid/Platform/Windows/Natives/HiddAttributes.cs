using System.Runtime.InteropServices;

namespace KaLib.IO.Hid.Platform.Windows.Natives;

[StructLayout(LayoutKind.Sequential)]
public struct HiddAttributes
{
    public ulong Size;
    public ushort VendorId;
    public ushort ProductId;
    public ushort VersionNumber;
}