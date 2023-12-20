using System.Runtime.Versioning;
using Mochi.ObjC;

namespace Mochi.Metal;

public struct MTLTextureDescriptor : IObjCInterface<MTLTextureDescriptor>
{
    private readonly IntPtr Handle;
    IntPtr INativeHandle<MTLTextureDescriptor>.Handle => Handle;

    public MTLTextureDescriptor(IntPtr handle) => Handle = handle;

    public static MTLTextureDescriptor AllocInit() => RuntimeClass.AllocInit<MTLTextureDescriptor>();

    public MTLTextureType TextureType
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetTextureTypeDelegate>()(Handle, _selGetTextureType);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetTextureTypeDelegate>()(Handle, _selSetTextureType, value);
        }
    }
    
    public MTLPixelFormat PixelFormat
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetPixelFormatDelegate>()(Handle, _selGetPixelFormat);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetPixelFormatDelegate>()(Handle, _selSetPixelFormat, value);
        }
    }
    
    public uint Width
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetUInt32Delegate>()(Handle, _selGetWidth);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetUInt32Delegate>()(Handle, _selSetWidth, value);
        }
    }
    
    public uint Height
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetUInt32Delegate>()(Handle, _selGetHeight);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetUInt32Delegate>()(Handle, _selSetHeight, value);
        }
    }
    
    public uint Depth
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetUInt32Delegate>()(Handle, _selGetDepth);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetUInt32Delegate>()(Handle, _selSetDepth, value);
        }
    }
    
    public uint MipmapLevelCount
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetUInt32Delegate>()(Handle, _selGetMipmapLevelCount);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetUInt32Delegate>()(Handle, _selSetMipmapLevelCount, value);
        }
    }
    
    public uint SampleCount
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetUInt32Delegate>()(Handle, _selGetSampleCount);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetUInt32Delegate>()(Handle, _selSetSampleCount, value);
        }
    }
    
    public uint ArrayLength
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetUInt32Delegate>()(Handle, _selGetArrayLength);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetUInt32Delegate>()(Handle, _selSetArrayLength, value);
        }
    }

    public static MTLTextureDescriptor CreateTexture2D(MTLPixelFormat format, uint width, uint height, bool mipmap)
    {
        RuntimeClass.EnsureInstanceNotNull();
        return ObjCRuntime.GetSendMessageFunction<Texture2DDescriptorWithPixelFormatDelegate>()(
            RuntimeClass.Handle, _selTexture2DDescriptorWithPixelFormat, format, width, height, mipmap);
    }
    
    public static MTLTextureDescriptor CreateTextureCube(MTLPixelFormat format, uint size, bool mipmap)
    {
        RuntimeClass.EnsureInstanceNotNull();
        return ObjCRuntime.GetSendMessageFunction<TextureCubeDescriptorWithPixelFormatDelegate>()(
            RuntimeClass.Handle, _selTexture2DDescriptorWithPixelFormat, format, size, mipmap);
    }
    
    [SupportedOSPlatform("macos10.14")]
    [SupportedOSPlatform("ios12.0")]
    public static MTLTextureDescriptor CreateTextureBuffer(MTLPixelFormat format, uint width,
        MTLResourceOptions options, MTLTextureUsage usage)
    {
        RuntimeClass.EnsureInstanceNotNull();
        return ObjCRuntime.GetSendMessageFunction<TextureBufferDescriptorWithPixelFormatDelegate>()(
            RuntimeClass.Handle, _selTexture2DDescriptorWithPixelFormat, format, width, options, usage);
    }

    private delegate MTLTextureDescriptor Texture2DDescriptorWithPixelFormatDelegate(
        IntPtr handle, Selector sel, MTLPixelFormat format, uint width, uint height, NativeTypes.Bool8 mipmap);
    
    private delegate MTLTextureDescriptor TextureCubeDescriptorWithPixelFormatDelegate(
        IntPtr handle, Selector sel, MTLPixelFormat format, uint size, NativeTypes.Bool8 mipmap);
    
    private delegate MTLTextureDescriptor TextureBufferDescriptorWithPixelFormatDelegate(
        IntPtr handle, Selector sel, MTLPixelFormat format, uint width, MTLResourceOptions options, MTLTextureUsage usage);

    static MTLTextureDescriptor INativeHandle<MTLTextureDescriptor>.CreateWithHandle(IntPtr handle) => new(handle);

    public static ObjCClass RuntimeClass { get; } = nameof(MTLTextureDescriptor);

    private static readonly Selector _selTexture2DDescriptorWithPixelFormat =
        "texture2DDescriptorWithPixelFormat:width:height:mipmapped:";

    private static readonly Selector _selTextureCubeDescriptorWithPixelFormat = 
        "textureCubeDescriptorWithPixelFormat:size:mipmapped:";

    private static readonly Selector _selTextureBufferDescriptorWithPixelFormat =
        "textureBufferDescriptorWithPixelFormat:width:resourceOptions:usage:";
    
    private static readonly Selector _selGetTextureType = "textureType";
    private static readonly Selector _selSetTextureType = "setTextureType:";
    private static readonly Selector _selGetPixelFormat = "pixelFormat";
    private static readonly Selector _selSetPixelFormat = "setPixelFormat:";
    private static readonly Selector _selGetWidth = "width";
    private static readonly Selector _selSetWidth = "setWidth:";
    private static readonly Selector _selGetHeight = "height";
    private static readonly Selector _selSetHeight = "setHeight:";
    private static readonly Selector _selGetDepth = "depth";
    private static readonly Selector _selSetDepth = "setDepth:";
    private static readonly Selector _selGetMipmapLevelCount = "mipmapLevelCount";
    private static readonly Selector _selSetMipmapLevelCount = "setMipmapLevelCount:";
    private static readonly Selector _selGetSampleCount = "sampleCount";
    private static readonly Selector _selSetSampleCount = "setSampleCount:";
    private static readonly Selector _selGetArrayLength = "arrayLength";
    private static readonly Selector _selSetArrayLength = "setArrayLength:";
}