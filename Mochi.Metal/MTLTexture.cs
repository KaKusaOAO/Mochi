using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Mochi.ObjC;

namespace Mochi.Metal;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct MTLTexture : IObjCNativeHandle<MTLTexture>, IMTLResource
{
    public readonly IntPtr Handle;
    IntPtr INativeHandle<MTLTexture>.Handle => Handle;

    public MTLTexture(IntPtr handle) => Handle = handle;

    public NSString Label
    {
        get => this.AsMTLResource().Label;
        set
        {
            var r = this.AsMTLResource();
            r.Label = value;
        }
    }

    public MTLDevice Device => this.AsMTLResource().Device;

    public MTLPixelFormat PixelFormat
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetPixelFormatDelegate>()(Handle,
                _selPixelFormat);
        }
    }
    
    public MTLTextureType TextureType
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetTextureTypeDelegate>()(Handle,
                _selTextureType);
        }
    }

    public void ReplaceRegion(MTLRegion region, UIntPtr mipmapLevel, UIntPtr slice, IntPtr pixelBytes,
        UIntPtr bytesPerRow, UIntPtr bytesPerImage)
    {
        this.EnsureInstanceNotNull();
        ObjCRuntime.GetSendMessageFunction<ReplaceRegionDelegate>()(Handle, _selReplaceRegion,
            region, mipmapLevel, slice, (void*) pixelBytes, bytesPerRow, bytesPerImage);
    }

    private void ValidatePixelFormatForTextureView(MTLPixelFormat format)
    {
        // Validate pixel format
        // https://developer.apple.com/documentation/metal/mtltexture/1515598-newtextureviewwithpixelformat?language=objc

        
    }

    private void ValidateTextureTypeForTextureView(MTLTextureType textureType)
    {
        // Validate texture type
        // https://developer.apple.com/documentation/metal/mtltexture/1515409-newtextureviewwithpixelformat?language=objc
        switch (TextureType)
        {
            case MTLTextureType.Type1D:
                if (textureType == MTLTextureType.Type1D) break;
                throw new IncompatibleTextureViewTypeException(TextureType, textureType);
            case MTLTextureType.Type2D:
                if (textureType is MTLTextureType.Type2D or MTLTextureType.Type2DArray) break;
                throw new IncompatibleTextureViewTypeException(TextureType, textureType);
            case MTLTextureType.Type2DArray:
            case MTLTextureType.TypeCube:
#pragma warning disable CA1416
            case MTLTextureType.TypeCubeArray:
                if (textureType is MTLTextureType.Type2D or MTLTextureType.Type2DArray or 
                    MTLTextureType.TypeCube or MTLTextureType.TypeCubeArray) break;
#pragma warning restore CA1416
                throw new IncompatibleTextureViewTypeException(TextureType, textureType);
            case MTLTextureType.Type3D:
                if (textureType == MTLTextureType.Type3D) break;
                throw new IncompatibleTextureViewTypeException(TextureType, textureType);
        }
        
        if (!OperatingSystem.IsMacOSVersionAtLeast(10, 11) && !OperatingSystem.IsIOSVersionAtLeast(11))
        {
            if (textureType == MTLTextureType.TypeCubeArray)
                throw new PlatformNotSupportedException("Cube array is not supported on this platform.");
        }
    }
    
    public MTLTexture CreateTextureView(MTLPixelFormat pixelFormat)
    {
        ValidatePixelFormatForTextureView(pixelFormat);
        
        this.EnsureInstanceNotNull();
        return ObjCRuntime.GetSendMessageFunction<NewTextureViewDelegate>()(Handle, _selNewTextureView, pixelFormat);
    }

    public MTLTexture CreateTextureView(MTLPixelFormat pixelFormat, MTLTextureType textureType, 
        NSRange levelRange, NSRange sliceRange)
    {
        ValidatePixelFormatForTextureView(pixelFormat);
        ValidateTextureTypeForTextureView(textureType);
        
        var sliceLength = (int)sliceRange.Length;
        
        if (textureType == MTLTextureType.TypeCube && sliceLength != 6)
            throw new IncompatibleSliceRangeException(sliceLength, 6, textureType);
        
        if (textureType == MTLTextureType.TypeCubeArray && sliceLength % 6 != 0)
            throw new IncompatibleSliceRangeException(sliceLength, textureType);

        if (textureType is MTLTextureType.Type1D or MTLTextureType.Type2D or MTLTextureType.Type2DMultisample
                or MTLTextureType.Type3D && sliceLength != 1)
            throw new IncompatibleSliceRangeException(sliceLength, 1, textureType);
                
        if (sliceRange.Length == 0)
            throw new MetalException("sliceRange.Length must not be 0.");
        
        if (levelRange.Length == 0)
            throw new MetalException("levelRange.Length must not be 0.");
        
        this.EnsureInstanceNotNull();
        return ObjCRuntime.GetSendMessageFunction<NewTextureViewWithTypeDelegate>()(Handle, _selNewTextureViewWithType,
            pixelFormat, textureType, levelRange, sliceRange);
    }
    
    static MTLTexture INativeHandle<MTLTexture>.CreateWithHandle(IntPtr handle) => new(handle);

    private delegate void ReplaceRegionDelegate(IntPtr handle, Selector sel, 
        MTLRegion region, UIntPtr mipmapLevel, UIntPtr slice, void* pixelBytes, UIntPtr bytesPerRow, UIntPtr bytesPerImage);

    private delegate MTLTexture NewTextureViewDelegate(IntPtr handle, Selector sel, 
        MTLPixelFormat pixelFormat);
    private delegate MTLTexture NewTextureViewWithTypeDelegate(IntPtr handle, Selector sel, 
        MTLPixelFormat pixelFormat, MTLTextureType textureType, NSRange levelRange, NSRange sliceRange);

    private static readonly Selector _selPixelFormat = "pixelFormat";
    private static readonly Selector _selTextureType = "textureType";
    private static readonly Selector _selReplaceRegion = "replaceRegion:mipmapLevel:slice:withBytes:bytesPerRow:bytesPerImage:";
    private static readonly Selector _selNewTextureView = "newTextureViewWithPixelFormat:";
    private static readonly Selector _selNewTextureViewWithType = "newTextureViewWithPixelFormat:textureType:levels:slices:";
}