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
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetTextureTypeDelegate>()(Handle, 
                _propTextureType.Getter);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetTextureTypeDelegate>()(Handle, 
                _propTextureType.Setter, value);
        }
    }
    
    public MTLPixelFormat PixelFormat
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetPixelFormatDelegate>()(Handle, 
                _propPixelFormat.Getter);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetPixelFormatDelegate>()(Handle, 
                _propPixelFormat.Setter, value);
        }
    }
    
    public uint Width
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetNSUIntDelegate>()(Handle, 
                _propWidth.Getter);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetNSUIntDelegate>()(Handle, 
                _propWidth.Setter, value);
        }
    }
    
    public uint Height
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetNSUIntDelegate>()(Handle, 
                _propHeight.Getter);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetNSUIntDelegate>()(Handle, 
                _propHeight.Setter, value);
        }
    }
    
    public uint Depth
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetNSUIntDelegate>()(Handle, 
                _propDepth.Getter);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetNSUIntDelegate>()(Handle, 
                _propDepth.Setter, value);
        }
    }
    
    public uint MipmapLevelCount
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetNSUIntDelegate>()(Handle,
                _propMipmapLevelCount.Getter);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetNSUIntDelegate>()(Handle, 
                _propMipmapLevelCount.Setter, value);
        }
    }
    
    public uint SampleCount
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetNSUIntDelegate>()(Handle, 
                _propSampleCount.Getter);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetNSUIntDelegate>()(Handle, 
                _propSampleCount.Setter, value);
        }
    }
    
    public uint ArrayLength
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetNSUIntDelegate>()(Handle, 
                _propArrayLength.Getter);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetNSUIntDelegate>()(Handle, 
                _propArrayLength.Setter, value);
        }
    }

    public static MTLTextureDescriptor CreateTexture2D(MTLPixelFormat format, uint width, uint height, bool mipmap)
    {
        RuntimeClass.EnsureInstanceNotNull();
        return ObjCRuntime.GetSendMessageFunction<Texture2DDescriptorWithPixelFormatDelegate>()(
            RuntimeClass.Handle, "texture2DDescriptorWithPixelFormat:width:height:mipmapped:", 
            format, width, height, mipmap);
    }
    
    public static MTLTextureDescriptor CreateTextureCube(MTLPixelFormat format, uint size, bool mipmap)
    {
        RuntimeClass.EnsureInstanceNotNull();
        return ObjCRuntime.GetSendMessageFunction<TextureCubeDescriptorWithPixelFormatDelegate>()(
            RuntimeClass.Handle, "textureCubeDescriptorWithPixelFormat:size:mipmapped:", 
            format, size, mipmap);
    }
    
    [SupportedOSPlatform("macos10.14")]
    [SupportedOSPlatform("ios12.0")]
    public static MTLTextureDescriptor CreateTextureBuffer(MTLPixelFormat format, uint width,
        MTLResourceOptions options, MTLTextureUsage usage)
    {
        RuntimeClass.EnsureInstanceNotNull();
        return ObjCRuntime.GetSendMessageFunction<TextureBufferDescriptorWithPixelFormatDelegate>()(
            RuntimeClass.Handle, "textureBufferDescriptorWithPixelFormat:width:resourceOptions:usage:", 
            format, width, options, usage);
    }

    private delegate MTLTextureDescriptor Texture2DDescriptorWithPixelFormatDelegate(
        IntPtr handle, Selector sel, MTLPixelFormat format, uint width, uint height, NativeTypes.Bool8 mipmap);
    
    private delegate MTLTextureDescriptor TextureCubeDescriptorWithPixelFormatDelegate(
        IntPtr handle, Selector sel, MTLPixelFormat format, uint size, NativeTypes.Bool8 mipmap);
    
    private delegate MTLTextureDescriptor TextureBufferDescriptorWithPixelFormatDelegate(
        IntPtr handle, Selector sel, MTLPixelFormat format, uint width, MTLResourceOptions options, MTLTextureUsage usage);

    static MTLTextureDescriptor INativeHandle<MTLTextureDescriptor>.CreateWithHandle(IntPtr handle) => new(handle);

    public static ObjCClass RuntimeClass { get; } = nameof(MTLTextureDescriptor);
    
    private static readonly Property _propTextureType = Property.Create("textureType");
    private static readonly Property _propPixelFormat = Property.Create("pixelFormat");
    private static readonly Property _propWidth = Property.Create("width");
    private static readonly Property _propHeight = Property.Create("height");
    private static readonly Property _propDepth = Property.Create("depth");
    private static readonly Property _propMipmapLevelCount = Property.Create("mipmapLevelCount");
    private static readonly Property _propSampleCount = Property.Create("sampleCount");
    private static readonly Property _propArrayLength = Property.Create("arrayLength");
}