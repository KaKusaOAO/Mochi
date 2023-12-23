using System.Runtime.Versioning;

namespace Mochi.Metal;

public enum MTLPixelFormat : uint
{
    Invalid,
    
    // Normal 8-bit formats
    A8Unorm,
    R8Unorm = 10,
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios8.0")]
    R8Unorm_sRGB,
    R8Snorm,
    R8Uint,
    R8Sint,
    
    // Normal 16-bit formats
    R16Unorm = 20,
    R16Snorm = 22,
    R16Uint,
    R16Sint,
    R16Float,
    
    RG8Unorm = 30,
    
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios8.0")]
    RG8Unorm_sRGB,
    RG8Snorm,
    RG8Uint,
    RG8Sint,
    
    // Packed 16-bit formats
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios8.0")]
    B5G6R5Unorm = 40,
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios8.0")]
    A1BGR5Unorm,
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios8.0")]
    ABGR4Unorm, 
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios8.0")]
    BGR5A1Unorm,
    
    // Normal 32-bit formats
    R32Uint = 53,
    R32Sint,
    R32Float,
    
    RG16Unorm = 60,
    RG16Snorm,
    RG16Uint,
    RG16Sint,
    RG16Float,
    
    RGBA8Unorm = 70,     
    RGBA8Unorm_sRGB,
    RGBA8Snorm,     
    RGBA8Uint,      
    RGBA8Sint,      
    
    BGRA8Unorm = 80,
    BGRA8Unorm_sRGB,
    
    // Packed 32-bit formats
    RGB10A2Unorm = 90,
    RGB10A2Uint, 
    
    RG11B10Float,
    RGB9E5Float,
    
    [SupportedOSPlatform("macos10.13")]
    [SupportedOSPlatform("ios11.0")]
    BGR10A2Unorm, 
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios10.0")]
    BGR10_XR = 554,  
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios10.0")]   
    BGR10_XR_sRGB,
    
    // Normal 64-bit formats
    RG32Uint = 103,
    RG32Sint,
    RG32Float,
    
    RGBA16Unorm = 110,
    RGBA16Snorm,
    RGBA16Uint,
    RGBA16Sint, 
    RGBA16Float,
    
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios10.0")]
    BGRA10_XR = 552,
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios10.0")]
    BGRA10_XR_sRGB,
    
    // Normal 128-bit formats
    RGBA32Uint = 123,
    RGBA32Sint, 
    RGBA32Float,
    
    // -- Compressed formats
    
    // S3TC/DXT
    [SupportedOSPlatform("macos10.11")]
    [SupportedOSPlatform("maccatalyst13.0")]
    [SupportedOSPlatform("ios16.4")]
    BC1_RGBA = 130,
    [SupportedOSPlatform("macos10.11")]
    [SupportedOSPlatform("maccatalyst13.0")]
    [SupportedOSPlatform("ios16.4")]
    BC1_RGBA_sRGB,
    [SupportedOSPlatform("macos10.11")]
    [SupportedOSPlatform("maccatalyst13.0")]
    [SupportedOSPlatform("ios16.4")]
    BC2_RGBA,     
    [SupportedOSPlatform("macos10.11")]
    [SupportedOSPlatform("maccatalyst13.0")]
    [SupportedOSPlatform("ios16.4")]
    BC2_RGBA_sRGB,
    [SupportedOSPlatform("macos10.11")]
    [SupportedOSPlatform("maccatalyst13.0")]
    [SupportedOSPlatform("ios16.4")]
    BC3_RGBA,     
    [SupportedOSPlatform("macos10.11")]
    [SupportedOSPlatform("maccatalyst13.0")]
    [SupportedOSPlatform("ios16.4")]
    BC3_RGBA_sRGB,
    
    // RGTC
    
    [SupportedOSPlatform("macos10.11")]
    [SupportedOSPlatform("maccatalyst13.0")]
    [SupportedOSPlatform("ios16.4")]
    BC4_RUnorm = 140,
    [SupportedOSPlatform("macos10.11")]
    [SupportedOSPlatform("maccatalyst13.0")]
    [SupportedOSPlatform("ios16.4")]
    BC4_RSnorm, 
    [SupportedOSPlatform("macos10.11")]
    [SupportedOSPlatform("maccatalyst13.0")]
    [SupportedOSPlatform("ios16.4")]
    BC5_RGUnorm,
    [SupportedOSPlatform("macos10.11")]
    [SupportedOSPlatform("maccatalyst13.0")]
    [SupportedOSPlatform("ios16.4")]
    BC5_RGSnorm,
    
    // BPTC
    
    [SupportedOSPlatform("macos10.11")]
    [SupportedOSPlatform("maccatalyst13.0")]
    [SupportedOSPlatform("ios16.4")]
    BC6H_RGBFloat = 150,
    [SupportedOSPlatform("macos10.11")]
    [SupportedOSPlatform("maccatalyst13.0")]
    [SupportedOSPlatform("ios16.4")]
    BC6H_RGBUfloat,  
    [SupportedOSPlatform("macos10.11")]
    [SupportedOSPlatform("maccatalyst13.0")]
    [SupportedOSPlatform("ios16.4")]  
    BC7_RGBAUnorm,  
    [SupportedOSPlatform("macos10.11")]
    [SupportedOSPlatform("maccatalyst13.0")]
    [SupportedOSPlatform("ios16.4")]   
    BC7_RGBAUnorm_sRGB,
    
    // PVRTC
    
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios8.0")] 
    PVRTC_RGB_2BPP = 160,    
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios8.0")]
    PVRTC_RGB_2BPP_sRGB, 
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios8.0")]
    PVRTC_RGB_4BPP,      
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios8.0")]
    PVRTC_RGB_4BPP_sRGB, 
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios8.0")]
    PVRTC_RGBA_2BPP,     
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios8.0")]
    PVRTC_RGBA_2BPP_sRGB,
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios8.0")]
    PVRTC_RGBA_4BPP,     
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios8.0")]
    PVRTC_RGBA_4BPP_sRGB,
    
    // ETC2
    
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios8.0")]
    EAC_R11Unorm = 170,    
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios8.0")]
    EAC_R11Snorm,   
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios8.0")] 
    EAC_RG11Unorm,   
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios8.0")]
    EAC_RG11Snorm,   
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios8.0")]
    EAC_RGBA8,       
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios8.0")]
    EAC_RGBA8_sRGB, 
    
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios8.0")]
    ETC2_RGB8 = 180,
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios8.0")]       
    ETC2_RGB8_sRGB, 
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios8.0")] 
    ETC2_RGB8A1,    
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios8.0")] 
    ETC2_RGB8A1_sRGB,
    
    // ASTC
    
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios8.0")] 
    ASTC_4x4_sRGB = 186,        
    ASTC_5x4_sRGB,    
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios8.0")]     
    ASTC_5x5_sRGB,     
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios8.0")]    
    ASTC_6x5_sRGB,      
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios8.0")]   
    ASTC_6x6_sRGB,      
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios8.0")]   
    ASTC_8x5_sRGB = 192,
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios8.0")]         
    ASTC_8x6_sRGB,    
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios8.0")]     
    ASTC_8x8_sRGB,   
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios8.0")]      
    ASTC_10x5_sRGB, 
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios8.0")]       
    ASTC_10x6_sRGB, 
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios8.0")]       
    ASTC_10x8_sRGB, 
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios8.0")]       
    ASTC_10x10_sRGB, 
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios8.0")]      
    ASTC_12x10_sRGB,  
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios8.0")]     
    ASTC_12x12_sRGB,     
    
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios8.0")] 
    ASTC_4x4_LDR = 204, 
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios8.0")]         
    ASTC_5x4_LDR,   
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios8.0")]       
    ASTC_5x5_LDR,   
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios8.0")]       
    ASTC_6x5_LDR,    
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios8.0")]      
    ASTC_6x6_LDR,      
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios8.0")]    
    ASTC_8x5_LDR = 210,  
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios8.0")]        
    ASTC_8x6_LDR,   
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios8.0")]       
    ASTC_8x8_LDR,   
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios8.0")]       
    ASTC_10x5_LDR,  
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios8.0")]       
    ASTC_10x6_LDR,  
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios8.0")]       
    ASTC_10x8_LDR,   
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios8.0")]      
    ASTC_10x10_LDR,  
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios8.0")]      
    ASTC_12x10_LDR,    
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios8.0")]    
    ASTC_12x12_LDR,     
    
    // ASTC HDR (High Dynamic Range) Formats
    
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios13.0")] 
    ASTC_4x4_HDR = 222,  
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios13.0")] 
    ASTC_5x4_HDR,  
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios13.0")] 
    ASTC_5x5_HDR,  
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios13.0")] 
    ASTC_6x5_HDR,  
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios13.0")] 
    ASTC_6x6_HDR,  
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios13.0")] 
    ASTC_8x5_HDR = 228,
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios13.0")]   
    ASTC_8x6_HDR,  
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios13.0")] 
    ASTC_8x8_HDR,  
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios13.0")] 
    ASTC_10x5_HDR, 
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios13.0")] 
    ASTC_10x6_HDR,
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios13.0")] 
    ASTC_10x8_HDR, 
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios13.0")] 
    ASTC_10x10_HDR,
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios13.0")] 
    ASTC_12x10_HDR,
    [SupportedOSPlatform("macos11.0")]
    [SupportedOSPlatform("maccatalyst14.0")]
    [SupportedOSPlatform("ios13.0")] 
    ASTC_12x12_HDR,
    
    /// <summary>
    /// A pixel format where the red and green channels are subsampled horizontally.
    /// Two pixels are stored in 32 bits, with shared red and blue values, and unique green values.
    /// </summary>
    GBGR422 = 240,
    BGRG422,
    
    // Depth
    
    [SupportedOSPlatform("macos10.12")]
    [SupportedOSPlatform("ios13.0")] 
    Depth16Unorm = 250,
    Depth32Float = 252,
    
    // Stencil
    Stencil8 = 253,
    
    // Depth Stencil
    
    [SupportedOSPlatform("macos10.11")]
    [SupportedOSPlatform("maccatalyst13.0")]
    [UnsupportedOSPlatform("ios")]
    Depth24Unorm_Stencil8 = 255,
    [SupportedOSPlatform("macos10.11")]
    [SupportedOSPlatform("ios9.0")] 
    Depth32Float_Stencil8 = 260,
    
    [SupportedOSPlatform("macos10.12")]
    [SupportedOSPlatform("ios10.0")] 
    X32_Stencil8 = 261,
    [SupportedOSPlatform("macos10.12")]
    [SupportedOSPlatform("maccatalyst13.0")]
    [UnsupportedOSPlatform("ios")]
    X24_Stencil8,
}