using System.Runtime.InteropServices;

namespace KaLib.IO.Hid.Platform.Windows.Natives;

[StructLayout(LayoutKind.Sequential)]
public struct HidpCaps
{
    public ushort Usage;
    public ushort UsagePage;
    public ushort InputReportByteLength;
    public ushort OutputReportByteLength;
    public ushort FeatureReportByteLength;
    private unsafe fixed ushort Reserved[17];
    
    public ushort NumberLinkCollectionNodes;
    
    public ushort NumberInputButtonCaps;
    public ushort NumberInputValueCaps;
    public ushort NumberInputDataIndices;
    
    public ushort NumberOutputButtonCaps;
    public ushort NumberOutputValueCaps;
    public ushort NumberOutputDataIndices;

    public ushort NumberFeatureButtonCaps;
    public ushort NumberFeatureValueCaps;
    public ushort NumberFeatureDataIndices;
}