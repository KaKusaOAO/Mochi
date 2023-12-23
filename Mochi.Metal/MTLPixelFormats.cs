using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Mochi.Metal;

public static class MTLPixelFormats
{
    private static readonly Lazy<HashSet<MTLPixelFormat>> _supportedValues = new(() =>
    {
        return Enum.GetValues<MTLPixelFormat>()
            .Where(InternalIsOSSupported)
            .ToHashSet();
    });

    public static IReadOnlySet<MTLPixelFormat> SupportedFormats => _supportedValues.Value;
    
    private static readonly Lazy<HashSet<MTLPixelFormat>> _formats8Bit = new(() =>
    {
        return SupportedFormats
            .Where(e => e is >= MTLPixelFormat.A8Unorm and <= MTLPixelFormat.R8Sint)
            .ToHashSet();
    });

    private static readonly Lazy<HashSet<MTLPixelFormat>> _formats16Bit = new(() =>
    {
        return SupportedFormats
            .Where(e => e is >= MTLPixelFormat.R16Unorm and <= MTLPixelFormat.RG8Sint)
            .ToHashSet();
    });

    private static readonly Lazy<HashSet<MTLPixelFormat>> _formats16BitPacked = new(() =>
    {
        return SupportedFormats
            .Where(e => (int) e is >= 40 and <= 43)
            .ToHashSet();
    });

    private static readonly Lazy<HashSet<MTLPixelFormat>> _formats32Bit = new(() =>
    {
        return SupportedFormats
            .Where(e => e is >= MTLPixelFormat.R32Uint and <= MTLPixelFormat.BGRA8Unorm_sRGB)
            .ToHashSet();
    });

    private static readonly Lazy<HashSet<MTLPixelFormat>> _formats32BitPacked = new(() =>
    {
        return SupportedFormats
            .Where(e => (int)e is >= 90 and <= 94 or 554 or 555)
            .ToHashSet();
    });

    private static readonly Lazy<HashSet<MTLPixelFormat>> _formats64Bit = new(() =>
    {
        return SupportedFormats
            .Where(e => (int)e is >= 103 and <= 114 or 552 or 553)
            .ToHashSet();
    });

    private static readonly Lazy<HashSet<MTLPixelFormat>> _formats128Bit = new(() =>
    {
        return SupportedFormats
            .Where(e => e is >= MTLPixelFormat.RGBA32Uint and <= MTLPixelFormat.RGBA32Float)
            .ToHashSet();
    });

    private static readonly Lazy<HashSet<MTLPixelFormat>> _formatsCompDxt = new(() =>
    {
        return SupportedFormats
            .Where(e => (int)e is >= 130 and <= 135)
            .ToHashSet();
    });

    private static readonly Lazy<HashSet<MTLPixelFormat>> _formatsCompRgtc = new(() =>
    {
        return SupportedFormats
            .Where(e => (int)e is >= 140 and <= 143)
            .ToHashSet();
    });

    private static readonly Lazy<HashSet<MTLPixelFormat>> _formatsCompBptc = new(() =>
    {
        return SupportedFormats
            .Where(e => (int)e is >= 150 and <= 153)
            .ToHashSet();
    });

    private static readonly Lazy<HashSet<MTLPixelFormat>> _formatsCompPvrtc = new(() =>
    {
        return SupportedFormats
            .Where(e => (int)e is >= 160 and <= 167)
            .ToHashSet();
    });

    private static readonly Lazy<HashSet<MTLPixelFormat>> _formatsCompEtc2 = new(() =>
    {
        return SupportedFormats
            .Where(e => (int)e is >= 170 and <= 183)
            .ToHashSet();
    });

    private static readonly Lazy<HashSet<MTLPixelFormat>> _formatsCompAstc = new(() =>
    {
        return SupportedFormats
            .Where(e => (int)e is >= 186 and <= 218)
            .ToHashSet();
    });

    private static readonly Lazy<HashSet<MTLPixelFormat>> _formatsCompAstcHdr = new(() =>
    {
        return SupportedFormats
            .Where(e => (int)e is >= 222 and <= 236)
            .ToHashSet();
    });

    public static bool IsOSSupported(MTLPixelFormat format) => SupportedFormats.Contains(format);

    private static bool InternalIsOSSupported(MTLPixelFormat format)
    {
        if (!MetalUtil.IsOSSupported) return false;

        if (OperatingSystem.IsMacOSVersionAtLeast(11) ||
            OperatingSystem.IsMacCatalystVersionAtLeast(14) ||
            (!OperatingSystem.IsMacCatalyst() && OperatingSystem.IsIOSVersionAtLeast(8)))
        {
            switch (format)
            {
                case MTLPixelFormat.R8Unorm_sRGB:
                case MTLPixelFormat.RG8Unorm_sRGB:
                case MTLPixelFormat.B5G6R5Unorm:
                case MTLPixelFormat.A1BGR5Unorm:
                case MTLPixelFormat.ABGR4Unorm:
                case MTLPixelFormat.BGR5A1Unorm:
                    return true;
            }
        }

        if (OperatingSystem.IsMacOSVersionAtLeast(10, 13) ||
            OperatingSystem.IsIOSVersionAtLeast(11))
        {
            if (format == MTLPixelFormat.BGR10A2Unorm) return true;
        }

        if (OperatingSystem.IsMacOSVersionAtLeast(11) ||
            OperatingSystem.IsMacCatalystVersionAtLeast(14) ||
            (!OperatingSystem.IsMacCatalyst() && OperatingSystem.IsIOSVersionAtLeast(10)))
        {
            switch (format)
            {
                case MTLPixelFormat.BGR10_XR:
                case MTLPixelFormat.BGR10_XR_sRGB:
                case MTLPixelFormat.BGRA10_XR:
                case MTLPixelFormat.BGRA10_XR_sRGB:
                    return true;
            }
        }

        if (OperatingSystem.IsMacOSVersionAtLeast(10, 11) ||
            OperatingSystem.IsMacCatalystVersionAtLeast(13) ||
            OperatingSystem.IsIOSVersionAtLeast(16, 4))
        {
            switch (format)
            {
                // For some reasons the compiler doesn't see the macOS support
#pragma warning disable CA1416
                case MTLPixelFormat.BC1_RGBA:
                case MTLPixelFormat.BC1_RGBA_sRGB:
                case MTLPixelFormat.BC2_RGBA:
                case MTLPixelFormat.BC2_RGBA_sRGB:
                case MTLPixelFormat.BC3_RGBA:
                case MTLPixelFormat.BC3_RGBA_sRGB:
                case MTLPixelFormat.BC4_RUnorm:
                case MTLPixelFormat.BC4_RSnorm:
                case MTLPixelFormat.BC5_RGUnorm:
                case MTLPixelFormat.BC5_RGSnorm:
                case MTLPixelFormat.BC6H_RGBFloat:
                case MTLPixelFormat.BC6H_RGBUfloat:
                case MTLPixelFormat.BC7_RGBAUnorm:
                case MTLPixelFormat.BC7_RGBAUnorm_sRGB:
#pragma warning restore CA1416
                    return true;
            }
        }

        if (OperatingSystem.IsMacOSVersionAtLeast(11) ||
            OperatingSystem.IsMacCatalystVersionAtLeast(14) ||
            (!OperatingSystem.IsMacCatalyst() && OperatingSystem.IsIOSVersionAtLeast(8)))
        {
            switch (format)
            {
                case MTLPixelFormat.PVRTC_RGB_2BPP:
                case MTLPixelFormat.PVRTC_RGB_2BPP_sRGB:
                case MTLPixelFormat.PVRTC_RGB_4BPP:
                case MTLPixelFormat.PVRTC_RGB_4BPP_sRGB:
                case MTLPixelFormat.PVRTC_RGBA_2BPP:
                case MTLPixelFormat.PVRTC_RGBA_2BPP_sRGB:
                case MTLPixelFormat.PVRTC_RGBA_4BPP:
                case MTLPixelFormat.PVRTC_RGBA_4BPP_sRGB:
                case MTLPixelFormat.EAC_R11Unorm:
                case MTLPixelFormat.EAC_R11Snorm:
                case MTLPixelFormat.EAC_RG11Unorm:
                case MTLPixelFormat.EAC_RG11Snorm:
                case MTLPixelFormat.EAC_RGBA8:
                case MTLPixelFormat.EAC_RGBA8_sRGB:
                case MTLPixelFormat.ETC2_RGB8:
                case MTLPixelFormat.ETC2_RGB8_sRGB:
                case MTLPixelFormat.ETC2_RGB8A1:
                case MTLPixelFormat.ETC2_RGB8A1_sRGB:
                case MTLPixelFormat.ASTC_4x4_sRGB:
                case MTLPixelFormat.ASTC_5x4_sRGB:
                case MTLPixelFormat.ASTC_5x5_sRGB:
                case MTLPixelFormat.ASTC_6x5_sRGB:
                case MTLPixelFormat.ASTC_6x6_sRGB:
                case MTLPixelFormat.ASTC_8x5_sRGB:
                case MTLPixelFormat.ASTC_8x6_sRGB:
                case MTLPixelFormat.ASTC_8x8_sRGB:
                case MTLPixelFormat.ASTC_10x5_sRGB:
                case MTLPixelFormat.ASTC_10x6_sRGB:
                case MTLPixelFormat.ASTC_10x8_sRGB:
                case MTLPixelFormat.ASTC_10x10_sRGB:
                case MTLPixelFormat.ASTC_12x10_sRGB:
                case MTLPixelFormat.ASTC_12x12_sRGB:
                case MTLPixelFormat.ASTC_4x4_LDR:
                case MTLPixelFormat.ASTC_5x4_LDR:
                case MTLPixelFormat.ASTC_5x5_LDR:
                case MTLPixelFormat.ASTC_6x5_LDR:
                case MTLPixelFormat.ASTC_6x6_LDR:
                case MTLPixelFormat.ASTC_8x5_LDR:
                case MTLPixelFormat.ASTC_8x6_LDR:
                case MTLPixelFormat.ASTC_8x8_LDR:
                case MTLPixelFormat.ASTC_10x5_LDR:
                case MTLPixelFormat.ASTC_10x6_LDR:
                case MTLPixelFormat.ASTC_10x8_LDR:
                case MTLPixelFormat.ASTC_10x10_LDR:
                case MTLPixelFormat.ASTC_12x10_LDR:
                case MTLPixelFormat.ASTC_12x12_LDR:
                    return true;
            }
        }

        if (OperatingSystem.IsMacOSVersionAtLeast(11) ||
            OperatingSystem.IsMacCatalystVersionAtLeast(14) ||
            (!OperatingSystem.IsMacCatalyst() && OperatingSystem.IsIOSVersionAtLeast(13)))
        {
            switch (format)
            {
                case MTLPixelFormat.ASTC_4x4_HDR:
                case MTLPixelFormat.ASTC_5x4_HDR:
                case MTLPixelFormat.ASTC_5x5_HDR:
                case MTLPixelFormat.ASTC_6x5_HDR:
                case MTLPixelFormat.ASTC_6x6_HDR:
                case MTLPixelFormat.ASTC_8x5_HDR:
                case MTLPixelFormat.ASTC_8x6_HDR:
                case MTLPixelFormat.ASTC_8x8_HDR:
                case MTLPixelFormat.ASTC_10x5_HDR:
                case MTLPixelFormat.ASTC_10x6_HDR:
                case MTLPixelFormat.ASTC_10x8_HDR:
                case MTLPixelFormat.ASTC_10x10_HDR:
                case MTLPixelFormat.ASTC_12x10_HDR:
                case MTLPixelFormat.ASTC_12x12_HDR:
                    return true;
            }
        }

        if (!OperatingSystem.IsIOS() && (OperatingSystem.IsMacOSVersionAtLeast(10, 11) ||
                                         OperatingSystem.IsMacCatalystVersionAtLeast(13)))
        {
            if (format == MTLPixelFormat.Depth24Unorm_Stencil8) 
                return true;
        }

        if (OperatingSystem.IsMacOSVersionAtLeast(10, 12) ||
            OperatingSystem.IsIOSVersionAtLeast(13))
        {
            if (format == MTLPixelFormat.Depth16Unorm)
                return true;
        }
        
        if (OperatingSystem.IsMacOSVersionAtLeast(10, 11) ||
            OperatingSystem.IsIOSVersionAtLeast(9))
        {
            if (format == MTLPixelFormat.Depth32Float_Stencil8)
                return true;
        }
        
        if (OperatingSystem.IsMacOSVersionAtLeast(10, 12) ||
            OperatingSystem.IsIOSVersionAtLeast(10))
        {
            if (format == MTLPixelFormat.X32_Stencil8)
                return true;
        }

        if (!OperatingSystem.IsIOS() && (OperatingSystem.IsMacOSVersionAtLeast(10, 12) ||
                                         OperatingSystem.IsMacCatalystVersionAtLeast(13)))
        {
            if (format == MTLPixelFormat.X24_Stencil8) 
                return true;
        }
        
        switch (format)
        {
            case MTLPixelFormat.Invalid:
            case MTLPixelFormat.A8Unorm:
            case MTLPixelFormat.R8Unorm:
            case MTLPixelFormat.R8Snorm:
            case MTLPixelFormat.R8Uint:
            case MTLPixelFormat.R8Sint:
            case MTLPixelFormat.R16Unorm:
            case MTLPixelFormat.R16Snorm:
            case MTLPixelFormat.R16Uint:
            case MTLPixelFormat.R16Sint:
            case MTLPixelFormat.R16Float:
            case MTLPixelFormat.RG8Unorm:
            case MTLPixelFormat.RG8Snorm:
            case MTLPixelFormat.RG8Uint:
            case MTLPixelFormat.RG8Sint:
            case MTLPixelFormat.R32Uint:
            case MTLPixelFormat.R32Sint:
            case MTLPixelFormat.R32Float:
            case MTLPixelFormat.RG16Unorm:
            case MTLPixelFormat.RG16Snorm:
            case MTLPixelFormat.RG16Uint:
            case MTLPixelFormat.RG16Sint:
            case MTLPixelFormat.RG16Float:
            case MTLPixelFormat.RGBA8Unorm:
            case MTLPixelFormat.RGBA8Unorm_sRGB:
            case MTLPixelFormat.RGBA8Snorm:
            case MTLPixelFormat.RGBA8Uint:
            case MTLPixelFormat.RGBA8Sint:
            case MTLPixelFormat.BGRA8Unorm:
            case MTLPixelFormat.BGRA8Unorm_sRGB:
            case MTLPixelFormat.RGB10A2Unorm:
            case MTLPixelFormat.RGB10A2Uint:
            case MTLPixelFormat.RG11B10Float:
            case MTLPixelFormat.RGB9E5Float:
            case MTLPixelFormat.RG32Uint:
            case MTLPixelFormat.RG32Sint:
            case MTLPixelFormat.RG32Float:
            case MTLPixelFormat.RGBA16Unorm:
            case MTLPixelFormat.RGBA16Snorm:
            case MTLPixelFormat.RGBA16Uint:
            case MTLPixelFormat.RGBA16Sint:
            case MTLPixelFormat.RGBA16Float:
            case MTLPixelFormat.RGBA32Uint:
            case MTLPixelFormat.RGBA32Sint:
            case MTLPixelFormat.RGBA32Float:
            case MTLPixelFormat.GBGR422:
            case MTLPixelFormat.BGRG422:
            case MTLPixelFormat.Depth32Float:
            case MTLPixelFormat.Stencil8:
                return true;
            
            default:
                return false;
        }
    }
}