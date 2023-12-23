using System.Runtime.Versioning;
using Mochi.Metal;
using Mochi.ObjC;

namespace Mochi.MetalFX;

public struct MTLFXTemporalScalerDescriptor : IObjCInterface<MTLFXTemporalScalerDescriptor>
{
    private readonly IntPtr Handle;
    IntPtr INativeHandle<MTLFXTemporalScalerDescriptor>.Handle => Handle;

    public MTLFXTemporalScalerDescriptor(IntPtr handle) => Handle = handle;

    public static MTLFXTemporalScalerDescriptor AllocInit()
    {
        if (!IsOnSupportedOS)
            throw new PlatformNotSupportedException();
        
        return RuntimeClass.AllocInit<MTLFXTemporalScalerDescriptor>();
    }

    public MTLPixelFormat ColorTextureFormat
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetPixelFormatDelegate>()(Handle,
                _propColorTextureFormat.Getter);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetPixelFormatDelegate>()(Handle,
                _propColorTextureFormat.Setter, value);
        }
    }

    public MTLPixelFormat DepthTextureFormat
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetPixelFormatDelegate>()(Handle,
                _propDepthTextureFormat.Getter);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetPixelFormatDelegate>()(Handle,
                _propDepthTextureFormat.Setter, value);
        }
    }

    public MTLPixelFormat MotionTextureFormat
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetPixelFormatDelegate>()(Handle,
                _propMotionTextureFormat.Getter);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetPixelFormatDelegate>()(Handle,
                _propMotionTextureFormat.Setter, value);
        }
    }

    public MTLPixelFormat OutputTextureFormat
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetPixelFormatDelegate>()(Handle,
                _propOutputTextureFormat.Getter);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetPixelFormatDelegate>()(Handle,
                _propOutputTextureFormat.Setter, value);
        }
    }

    public uint InputWidth
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetNSUIntDelegate>()(Handle,
                _propInputWidth.Getter);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetNSUIntDelegate>()(Handle,
                _propInputWidth.Setter, value);
        }
    }

    public uint InputHeight
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetNSUIntDelegate>()(Handle,
                _propInputHeight.Getter);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetNSUIntDelegate>()(Handle, 
                _propInputHeight.Setter, value);
        }
    }

    public uint OutputWidth
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetNSUIntDelegate>()(Handle,
                _propOutputWidth.Getter);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetNSUIntDelegate>()(Handle, 
                _propOutputWidth.Setter, value);
        }
    }

    public uint OutputHeight
    {
        get
        {
            this.EnsureInstanceNotNull();
            return ObjCRuntime.GetSendMessageFunction<PropertyDelegates.GetNSUIntDelegate>()(Handle,
                _propOutputHeight.Getter);
        }
        set
        {
            this.EnsureInstanceNotNull();
            ObjCRuntime.GetSendMessageFunction<PropertyDelegates.SetNSUIntDelegate>()(Handle, 
                _propOutputHeight.Setter, value);
        }
    }

    public MTLFXTemporalScaler CreateTemporalScaler(MTLDevice device)
    {
        this.EnsureInstanceNotNull();
        
        if (OutputWidth == 0)
            throw new MetalException("The output width for the temporal scaler cannot be 0.");
        
        if (OutputHeight == 0)
            throw new MetalException("The output height for the temporal scaler cannot be 0.");
        
        // Input size with zeros cause the result value to be null,
        // but it is not validated by the Metal validation layer itself
        
        return ObjCRuntime.GetSendMessageFunction<NewTemporalScalerDelegate>()(Handle,
            _selNewTemporalScalerWithDevice, 
            device);
    }

    [SupportedOSPlatform("macos14.0")]
    [SupportedOSPlatform("ios17.0")]
    public static float GetSupportedInputContentMinScale(MTLDevice device)
    {
        if (!OperatingSystem.IsMacOSVersionAtLeast(14) && !OperatingSystem.IsIOSVersionAtLeast(17))
            throw new PlatformNotSupportedException();
        
        return ObjCRuntime.GetSendMessageFunction<SupportsInputContentScaleForDeviceDelegate>()(RuntimeClass.Handle,
            _selSupportedInputContentMinScaleForDevice, device);
    }

    [SupportedOSPlatform("macos14.0")]
    [SupportedOSPlatform("ios17.0")]
    public static float GetSupportedInputContentMaxScale(MTLDevice device)
    {
        if (!OperatingSystem.IsMacOSVersionAtLeast(14) && !OperatingSystem.IsIOSVersionAtLeast(17))
            throw new PlatformNotSupportedException();
        
        return ObjCRuntime.GetSendMessageFunction<SupportsInputContentScaleForDeviceDelegate>()(RuntimeClass.Handle,
            _selSupportedInputContentMaxScaleForDevice, device);
    }

    [SupportedOSPlatformGuard("macos13.0")]
    [SupportedOSPlatformGuard("ios16.0")]
    private static bool IsOnSupportedOS =>
        OperatingSystem.IsMacOSVersionAtLeast(13) || OperatingSystem.IsIOSVersionAtLeast(16);

    public static bool IsDeviceSupported(MTLDevice device)
    {
        if (!IsOnSupportedOS) return false;
        return ObjCRuntime.GetSendMessageFunction<SupportsDeviceDelegate>()(RuntimeClass.Handle, _selSupportsDevice,
            device);
    }

    private delegate MTLFXTemporalScaler NewTemporalScalerDelegate(IntPtr handle, Selector sel, MTLDevice device);
    private delegate NativeTypes.Bool8 SupportsDeviceDelegate(IntPtr handle, Selector sel, MTLDevice device);
    private delegate float SupportsInputContentScaleForDeviceDelegate(IntPtr handle, Selector sel, MTLDevice device);
    
    public static ObjCClass RuntimeClass { get; } = nameof(MTLFXTemporalScalerDescriptor);
    static MTLFXTemporalScalerDescriptor INativeHandle<MTLFXTemporalScalerDescriptor>.CreateWithHandle(IntPtr handle) =>
        new(handle);

    private static readonly Property _propColorTextureFormat = Property.Create("colorTextureFormat");
    private static readonly Property _propDepthTextureFormat = Property.Create("depthTextureFormat");
    private static readonly Property _propMotionTextureFormat = Property.Create("motionTextureFormat");
    private static readonly Property _propOutputTextureFormat = Property.Create("outputTextureFormat");

    private static readonly Property _propInputWidth = Property.Create("inputWidth");
    private static readonly Property _propInputHeight = Property.Create("inputHeight");
    private static readonly Property _propOutputWidth = Property.Create("outputWidth");
    private static readonly Property _propOutputHeight = Property.Create("outputHeight");
    
    private static readonly Selector _selNewTemporalScalerWithDevice = "newTemporalScalerWithDevice:";
    private static readonly Selector _selSupportedInputContentMinScaleForDevice = "supportedInputContentMinScaleForDevice:";
    private static readonly Selector _selSupportedInputContentMaxScaleForDevice = "supportedInputContentMaxScaleForDevice:";
    private static readonly Selector _selSupportsDevice = "supportsDevice:";
}