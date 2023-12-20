using Mochi.ObjC;

namespace Mochi.Metal;

public static class PropertyDelegates
{
    public delegate NativeTypes.Bool8 GetBoolDelegate(IntPtr handle, Selector sel);
    public delegate void SetBoolDelegate(IntPtr handle, Selector sel, NativeTypes.Bool8 val);
    
    public delegate uint GetUInt32Delegate(IntPtr handle, Selector sel);
    public delegate void SetUInt32Delegate(IntPtr handle, Selector sel, uint val);
    
    public delegate NSString GetNSStringDelegate(IntPtr handle, Selector sel);
    public delegate void SetNSStringDelegate(IntPtr handle, Selector sel, NSString value);
    
    public delegate NSArray GetNSArrayDelegate(IntPtr handle, Selector sel);
    public delegate void SetNSArrayDelegate(IntPtr handle, Selector sel, NSArray arr);
    
    public delegate MTLBlendOperation GetBlendOperationDelegate(IntPtr handle, Selector sel);
    public delegate void SetBlendOperationDelegate(IntPtr handle, Selector sel, MTLBlendOperation op);
    
    public delegate MTLBlendFactor GetBlendFactorDelegate(IntPtr handle, Selector sel);
    public delegate void SetBlendFactorDelegate(IntPtr handle, Selector sel, MTLBlendFactor factor);
    
    public delegate MTLColorWriteMask GetWriteMaskDelegate(IntPtr handle, Selector sel);
    public delegate void SetWriteMaskDelegate(IntPtr handle, Selector sel, MTLColorWriteMask mask);

    public delegate MTLPixelFormat GetPixelFormatDelegate(IntPtr handle, Selector sel);
    public delegate void SetPixelFormatDelegate(IntPtr handle, Selector sel, MTLPixelFormat format);
        
    public delegate MTLArchitecture GetArchitectureDelegate(IntPtr handle, Selector sel);
    public delegate void SetArchitectureDelegate(IntPtr handle, Selector sel, MTLArchitecture architecture);
    
    public delegate MTLDevice GetDeviceDelegate(IntPtr handle, Selector sel);
    public delegate void SetDeviceDelegate(IntPtr handle, Selector sel, MTLDevice device);
    
    public delegate MTLDeviceLocation GetDeviceLocationDelegate(IntPtr handle, Selector sel);
    public delegate void SetDeviceLocationDelegate(IntPtr handle, Selector sel, MTLDeviceLocation location);
    
    public delegate MTLLanguageVersion GetLanguageVersionDelegate(IntPtr handle, Selector sel);
    public delegate void SetLanguageVersionDelegate(IntPtr handle, Selector sel, MTLLanguageVersion version);
    
    public delegate MTLLibraryType GetLibraryTypeDelegate(IntPtr handle, Selector sel);
    public delegate void SetLibraryTypeDelegate(IntPtr handle, Selector sel, MTLLibraryType type);
    
    public delegate MTLLibraryOptimizationLevel GetOptimizationLevelDelegate(IntPtr handle, Selector sel);
    public delegate void SetOptimizationLevelDelegate(IntPtr handle, Selector sel, MTLLibraryOptimizationLevel level);
    
    public delegate MTLCompileSymbolVisibility GetCompileSymbolVisibilityDelegate(IntPtr handle, Selector sel);
    public delegate void SetCompileSymbolVisibilityDelegate(IntPtr handle, Selector sel, MTLCompileSymbolVisibility visibility);
    
    public delegate MTLTextureType GetTextureTypeDelegate(IntPtr handle, Selector sel);
    public delegate void SetTextureTypeDelegate(IntPtr handle, Selector sel, MTLTextureType type);
}