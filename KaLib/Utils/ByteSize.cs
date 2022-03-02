using System;

namespace KaLib.Utils;

public struct ByteSize
{
    private long bytes;

    private const long UnitScale = 1024;
    private const long KilobyteInBytes = UnitScale;
    private const long MegabyteInBytes = KilobyteInBytes * UnitScale;
    private const long GigabyteInBytes = MegabyteInBytes * UnitScale;
    private const long TerabyteInBytes = GigabyteInBytes * UnitScale;

    public ByteSize(long b = 0, long kb = 0, long mb = 0, long gb = 0, long tb = 0)
    {
        bytes = b;
        bytes += kb * KilobyteInBytes;
        bytes += mb * MegabyteInBytes;
        bytes += gb * GigabyteInBytes;
        bytes += tb * TerabyteInBytes;
    }

    public long TotalBits => bytes * 8;
    public long TotalBytes => bytes;
    public double TotalKilobytes => bytes / (double)KilobyteInBytes;
    public double TotalMegabytes => bytes / (double)MegabyteInBytes;
    public double TotalGigabytes => bytes / (double)GigabyteInBytes;
    public double TotalTerabytes => bytes / (double)TerabyteInBytes;

    public int Kilobytes => (int) Math.Floor(TotalKilobytes % UnitScale);
    public int Megabytes => (int) Math.Floor(TotalMegabytes % UnitScale);
    public int Gigabytes => (int) Math.Floor(TotalGigabytes % UnitScale);
    public int Terabytes => (int) Math.Floor(TotalTerabytes % UnitScale);

    public override string ToString() => ToString(2);
        
    public string ToString(int precision)
    {
        var p = Math.Pow(10, precision);
        return bytes switch
        {
            >= TerabyteInBytes => $"{Math.Round(TotalTerabytes * p) / p} TB",
            >= GigabyteInBytes => $"{Math.Round(TotalGigabytes * p) / p} GB",
            >= MegabyteInBytes => $"{Math.Round(TotalMegabytes * p) / p} MB",
            >= KilobyteInBytes => $"{Math.Round(TotalKilobytes * p) / p} KB",
            _ => $"{bytes} B"
        };
    }
}