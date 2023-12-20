using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using Mochi.ObjC;

namespace Mochi.Metal;

public unsafe struct MTLTexture : IObjCInterface<MTLTexture>, IMTLResource
{
    public readonly IntPtr Handle;
    IntPtr INativeHandle<MTLTexture>.Handle => Handle;

    public MTLTexture(IntPtr handle) => Handle = handle;

    public NSString Label
    {
        get => this.AsMTLResource().Label;
        set => this.AsMTLResource().Label = value;
    }

    public MTLDevice Device => this.AsMTLResource().Device;

    public void ReplaceRegion(MTLRegion region, UIntPtr mipmapLevel, UIntPtr slice, IntPtr pixelBytes,
        UIntPtr bytesPerRow, UIntPtr bytesPerImage)
    {
        this.EnsureInstanceNotNull();
        ObjCRuntime.GetSendMessageFunction<ReplaceRegionDelegate>()(Handle, _selReplaceRegion,
            region, mipmapLevel, slice, (void*) pixelBytes, bytesPerRow, bytesPerImage);
    }

    public MTLTexture CreateTextureView(MTLPixelFormat pixelFormat, MTLTextureType textureType, 
        NSRange levelRange, NSRange sliceRange)
    {
        this.EnsureInstanceNotNull();
        return ObjCRuntime.GetSendMessageFunction<NewTextureViewDelegate>()(Handle, _selNewTextureView,
            pixelFormat, textureType, levelRange, sliceRange);
    }
    
    public static ObjCClass RuntimeClass { get; } = nameof(MTLTexture);
    static MTLTexture INativeHandle<MTLTexture>.CreateWithHandle(IntPtr handle) => new(handle);

    private delegate void ReplaceRegionDelegate(IntPtr handle, Selector sel, 
        MTLRegion region, UIntPtr mipmapLevel, UIntPtr slice, void* pixelBytes, UIntPtr bytesPerRow, UIntPtr bytesPerImage);

    private delegate MTLTexture NewTextureViewDelegate(IntPtr handle, Selector sel, 
        MTLPixelFormat pixelFormat, MTLTextureType textureType, NSRange levelRange, NSRange sliceRange);
    
    private static readonly Selector _selReplaceRegion = "replaceRegion:mipmapLevel:slice:withBytes:bytesPerRow:bytesPerImage:";
    private static readonly Selector _selNewTextureView = "newTextureViewWithPixelFormat:textureType:levels:slices:";
}