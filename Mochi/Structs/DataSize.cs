using System;
using System.Numerics;
using Mochi.Utils;

namespace Mochi.Structs;

public readonly struct DataSize :
#if NET7_0_OR_GREATER
    IMinMaxValue<DataSize>,
#endif
    IComparable<DataSize>
{
    private readonly BigInteger _bytes;

    private const ulong UnitScale = 1024;
    private const ulong KilobyteInBytes = UnitScale;
    private const ulong MegabyteInBytes = KilobyteInBytes * UnitScale;
    private const ulong GigabyteInBytes = MegabyteInBytes * UnitScale;
    private const ulong TerabyteInBytes = GigabyteInBytes * UnitScale;
    private const ulong PetabyteInBytes = TerabyteInBytes * UnitScale;
    private const ulong ExabyteInBytes = PetabyteInBytes * UnitScale;
    private static readonly BigInteger ZettabyteInBytes = new BigInteger(ExabyteInBytes) * UnitScale;
    private static readonly BigInteger YottabyteInBytes = ZettabyteInBytes * UnitScale;

    public static DataSize Zero { get; } = new();
    public static DataSize MaxValue { get; } = new(new BigInteger(decimal.MaxValue));
    public static DataSize MinValue => Zero;

    private DataSize(BigInteger b = default, BigInteger kb = default, BigInteger mb = default, BigInteger gb = default,
        BigInteger tb = default, BigInteger pb = default, BigInteger eb = default, BigInteger zb = default,
        BigInteger yb = default)
    {
        _bytes = b;
        _bytes += kb * KilobyteInBytes;
        _bytes += mb * MegabyteInBytes;
        _bytes += gb * GigabyteInBytes;
        _bytes += tb * TerabyteInBytes;
        _bytes += pb * PetabyteInBytes;
        _bytes += eb * ExabyteInBytes;
        _bytes += zb * ZettabyteInBytes;
        _bytes += yb * YottabyteInBytes;

        if (_bytes.Sign < 0) throw new ArgumentException("Size cannot be negative");
        if (_bytes > new BigInteger(decimal.MaxValue)) throw TooLargeException();
    }

    private static ArgumentException TooLargeException()
    {
        var y = decimal.MaxValue / (decimal)YottabyteInBytes;
        return new ArgumentException($"DataSize exceeds maximum of {y:N2} YB");
    }

    private static DataSize FromUnit(decimal amount, BigInteger unit)
    {
        var n = amount;
        while (unit >= UnitScale)
        {
            if (n > decimal.MaxValue / UnitScale) throw TooLargeException();
            
            n *= UnitScale;
            unit /= UnitScale;
        }

        return FromBytes(new BigInteger(decimal.Round(n)));
    }
    
    public static DataSize FromBytes(BigInteger bytes) => bytes == 0 ? Zero : new DataSize(bytes);
    public static DataSize FromKilobytes(decimal kb) => FromUnit(kb, KilobyteInBytes);
    public static DataSize FromMegabytes(decimal mb) => FromUnit(mb, MegabyteInBytes);
    public static DataSize FromGigabytes(decimal gb) => FromUnit(gb, GigabyteInBytes);
    public static DataSize FromTerabytes(decimal tb) => FromUnit(tb, TerabyteInBytes);
    public static DataSize FromPetabytes(decimal pb) => FromUnit(pb, PetabyteInBytes);
    public static DataSize FromExabytes(decimal eb) => FromUnit(eb, ExabyteInBytes);
    public static DataSize FromZettabytes(decimal zb) => FromUnit(zb, ZettabyteInBytes);
    public static DataSize FromYottabytes(decimal yb) => FromUnit(yb, YottabyteInBytes);

    public static DataSize FromBytes(long bytes) => bytes == 0 ? Zero : new DataSize(bytes);
    public static DataSize FromKilobytes(double kb) => FromUnit((decimal)kb, KilobyteInBytes);
    public static DataSize FromMegabytes(double mb) => FromUnit((decimal)mb, MegabyteInBytes);
    public static DataSize FromGigabytes(double gb) => FromUnit((decimal)gb, GigabyteInBytes);
    public static DataSize FromTerabytes(double tb) => FromUnit((decimal)tb, TerabyteInBytes);
    public static DataSize FromPetabytes(double pb) => FromUnit((decimal)pb, PetabyteInBytes);
    public static DataSize FromExabytes(double eb) => FromUnit((decimal)eb, ExabyteInBytes);
    public static DataSize FromZettabytes(double zb) => FromUnit((decimal)zb, ZettabyteInBytes);
    public static DataSize FromYottabytes(double yb) => FromUnit((decimal)yb, YottabyteInBytes);

    public BigInteger TotalBits => _bytes * 8;
    public BigInteger TotalBytes => _bytes;
    public decimal TotalKilobytes => CreateSizeDecimalFromDivision(KilobyteInBytes);
    public decimal TotalMegabytes => CreateSizeDecimalFromDivision(MegabyteInBytes);
    public decimal TotalGigabytes => CreateSizeDecimalFromDivision(GigabyteInBytes);
    public decimal TotalTerabytes => CreateSizeDecimalFromDivision(TerabyteInBytes);
    public decimal TotalPetabytes => CreateSizeDecimalFromDivision(PetabyteInBytes);
    public decimal TotalExabytes => CreateSizeDecimalFromDivision(ExabyteInBytes);
    public decimal TotalZettabytes => CreateSizeDecimalFromDivision(ZettabyteInBytes);
    public decimal TotalYottabytes => CreateSizeDecimalFromDivision(YottabyteInBytes);

    private decimal CreateSizeDecimalFromDivision(ulong divisor)
    {
        // var (q, rem) = BigInteger.DivRem(_bytes, divider);
        var q = BigInteger.DivRem(_bytes, divisor, out var rem);
        var dec = (decimal)rem / divisor;
        return (decimal)q + dec;
    }
    
    private decimal CreateSizeDecimalFromDivision(BigInteger divisor)
    {
        // var (q, rem) = BigInteger.DivRem(_bytes, divisor);
        var q = BigInteger.DivRem(_bytes, divisor, out var rem);
        var dec = (decimal)rem / (decimal)divisor;
        return (decimal)q + dec;
    }
    
    public int Kilobytes => (int) Math.Floor(TotalKilobytes % UnitScale);
    public int Megabytes => (int) Math.Floor(TotalMegabytes % UnitScale);
    public int Gigabytes => (int) Math.Floor(TotalGigabytes % UnitScale);
    public int Terabytes => (int) Math.Floor(TotalTerabytes % UnitScale);
    public int Petabytes => (int) Math.Floor(TotalPetabytes % UnitScale);
    public int Exabytes => (int) Math.Floor(TotalExabytes % UnitScale);
    public int Zettabytes => (int) Math.Floor(TotalZettabytes % UnitScale);
    public int Yottabytes => (int) Math.Floor(TotalYottabytes % UnitScale);

    public override string ToString() => ToString(2);
        
    public string ToString(int precision)
    {
        var p = (decimal)Math.Pow(10, precision);
        var abs = BigInteger.Abs(_bytes);

        string FormattedSize(decimal value)
        {
            var val = Math.Floor(value * p) / p;
            return val.ToString($"N{precision}");
        }

        if (abs >= YottabyteInBytes)
            return $"{FormattedSize(TotalYottabytes)} YB";
        if (abs >= ZettabyteInBytes)
            return $"{FormattedSize(TotalZettabytes)} ZB";
        if (abs >= ExabyteInBytes)
            return $"{FormattedSize(TotalExabytes)} EB";
        if (abs >= PetabyteInBytes)
            return $"{FormattedSize(TotalPetabytes)} PB";
        if (abs >= TerabyteInBytes)
            return $"{FormattedSize(TotalTerabytes)} TB";
        if (abs >= GigabyteInBytes)
            return $"{FormattedSize(TotalGigabytes)} GB";
        if (abs >= MegabyteInBytes)
            return $"{FormattedSize(TotalMegabytes)} MB";
        if (abs >= KilobyteInBytes)
            return $"{FormattedSize(TotalKilobytes)} KB";
        
        return $"{_bytes:N0} B";
    }

    public int CompareTo(DataSize other) => _bytes.CompareTo(other._bytes);
}